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

      var assemblyFileName = new FileInfo(Assembly.GetEntryAssembly().Location);
      var ofxPath = Path.Combine(assemblyFileName.Directory.FullName, "ofx");
      var csvPath = Path.Combine(assemblyFileName.Directory.FullName, "csv");

      var transform = new XslCompiledTransform();

      // load XSLT from assembly manifest resource
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(OFXLoader.Program), "Extractor.xslt"))
      {
        transform.Load(XmlReader.Create(stream));
      }

      // create CSV output file
      using (var ext = new Ext(Path.Combine(csvPath, String.Format("output_{0:yyyyMMdd}.csv", DateTime.Today))))
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
              // then load the content as XML

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
  class Ext : IDisposable
  {
    private StreamWriter _writer;

    private Dictionary<String, Boolean> _duplicateCheck;

    public Ext(string fileName)
    {
      _writer = File.CreateText(fileName);
      _writer.WriteLine("Account,Type,Date,Description,Amount,FitNum,CheckNum");
      _duplicateCheck = new Dictionary<string, bool>();
    }

    public void SaveTransaction(string account, string tranType, string date, string description, string transactionAmount, string fitId, string checkNum)
    {
      // combine fitID and amount to make a unique key
      var key = String.Format("{0}:{1}", fitId, transactionAmount);

      // duplicate entry?
      if (_duplicateCheck.ContainsKey(key))
      {
        Console.WriteLine("Skipping duplicate. [{0}]", key);
        return;
      }

      var dateparts = new string[3];

      dateparts[0] = date.Substring(0, 4); // year
      dateparts[1] = date.Substring(4, 2); // month
      dateparts[2] = date.Substring(6, 2); // day

      _writer.WriteLine("{0},\"{1}\",{2:yyyy-MM-dd},\"{3}\",{4}, {5}, {6}", account, tranType, DateTime.Parse(String.Join("-", dateparts)), description, transactionAmount, fitId, checkNum);

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
