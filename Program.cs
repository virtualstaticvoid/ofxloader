using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using System.Collections.Generic;

namespace OFXLoader
{
  class Program
  {
    static void Main()
    {

      //
      // assumes directory structure:
      //
      // ofx/                 <--- OFX's read from
      //   - ofxfile1.ofx
      //   - ofxfile2.ofx
      //   - ...
      //   - ofxfileN.ofx
      // csv/                 <--- CSV outputs to
      //   - csvfile1.csv
      //   - csvfile2.csv
      //   - ...
      //   - csvfileN.csv
      //

      var baseDirectory = Directory.GetCurrentDirectory();
      var ofxPath = Path.Combine(baseDirectory, "ofx");
      var csvPath = Path.Combine(baseDirectory, "csv");

      var transform = new XslCompiledTransform();

      // load XSLT from assembly manifest resource
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(OFXLoader.Program), "Extractor.xslt"))
      {
        transform.Load(XmlReader.Create(stream));
      }

      // create CSV output file
      using (var ext = new XsltExt(Path.Combine(csvPath, $"output_{DateTime.Today:yyyyMMdd}.csv")))
      {

        var args = new XsltArgumentList();
        args.AddExtensionObject("uri:extension", ext);

        var srcDir = new DirectoryInfo(ofxPath);

        using (var results = new XmlTextWriter(new MemoryStream(), Encoding.UTF8))
        {

          // read in OFX files
          foreach (var file in srcDir.GetFiles("*.ofx"))
          {
            Console.WriteLine(file.Name);
            var sb = new StringBuilder();

            // load the file; may start with OFX header, so read to the start of the XML
            using (var stream = File.OpenText(file.FullName))
            {

              // some OFX file has non-XML content at the top of the file
              // ignore lines until line starting with "<OFX>"
              // then load the remaining content as XML

              var inContent = false;
              var line = stream.ReadLine();

              while (line != null)
              {
                if (inContent)
                {
                  sb.Append(line);
                }
                else if (line.StartsWith("<OFX>"))
                {
                  inContent = true;
                  sb.Append(line);
                }
                line = stream.ReadLine();
              }
            }
            var dataFile = new XPathDocument(new StringReader(sb.ToString()));
            transform.Transform(dataFile.CreateNavigator(), args, results);
          }
        }
      }
    }
  }

  // XSLT extension
  class XsltExt : IDisposable
  {
    private StreamWriter _writer;
    private readonly Dictionary<String, Boolean> _duplicateCheck;

    public XsltExt(string fileName)
    {
      _writer = File.CreateText(fileName);
      _writer.WriteLine("BankId,Account,Type,Date,Description,Currency,Amount,FitNum,CheckNum");
      _duplicateCheck = new Dictionary<string, bool>();
    }

    public void SaveTransaction(
      string bankId,
      string account,
      string tranType,
      string date,
      string description,
      string currency,
      string transactionAmount,
      string fitId,
      string checkNum)
    {
      // combine account, fitID and amount to make a unique key
      var key = $"{account}:{fitId}:{transactionAmount}";

      // duplicate entry?
      if (_duplicateCheck.ContainsKey(key))
      {
        Console.WriteLine("Skipping duplicate. [{0}]", key);
        return;
      }

      var dateParts = new string[3];

      dateParts[0] = date.Substring(0, 4); // year
      dateParts[1] = date.Substring(4, 2); // month
      dateParts[2] = date.Substring(6, 2); // day

      _writer.WriteLine("{0},{1},\"{2}\",{3:yyyy-MM-dd},\"{4}\",{5},{6},{7},{8}", bankId, account, tranType, DateTime.Parse(string.Join("-", dateParts)), description, currency, transactionAmount, fitId, checkNum);

      _duplicateCheck.Add(key, true);
    }

    #region IDisposable Members

    public void Dispose()
    {
      if (_writer != null)
      {
        _writer.Flush();
        _writer.Dispose();
        _writer = null;
        GC.SuppressFinalize(this);
      }
    }

    #endregion

  }
}
