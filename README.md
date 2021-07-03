# OFX Loader

Simple utility to read a bunch of [OFX files][ofx] and output a single CSV file with the transactions in tabular form.

## Pre-requisites

* Windows or Linux
* [DotNet Core 5.0+][dotnetcore]
* [OFX files][ofx]

## Build

```
dotnet build
```

Alternatively, to build a single file application, run one of the following depending on you Operating System.

```
# Windows 64bit
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true

# Linux 64bit
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true
```

See [Publish a single file app - CLI][singlefileapps] for more information.

## Usage

Create a directory structure as follows and place your OFX files into the `ofx` directory.

```
<ofx-dir>/
 - ofx/                 <--- OFX's will be read from here
   - OFXfile1.ofx
   - OFXfile2.ofx
   - ...
   - OFXfileN.ofx
 - csv/                 <--- CSV output will be written here
```

Copy the output `OFXLoader.dll` and `OFXLoader.runtimeconfig.json` files to the `<ofx-dir>` directory.

Run the following command with the `<ofx-dir>` directory as current.

```
dotnet OFXLoader.dll
```

The program will find each OFX file in the `ofx` directory and create a single CSV file in the `csv` directory, and load the unique set of transaction data into it. Duplicate transactions will be ignored.

Open the resultant CSV file in your favourite spreadsheet program to see the transactions in tabular form.

## Output Format

The following table shows an example of the tabular content produced.

| Bank | Date       | Description       | Currency | Amount | FitNum | CheckNum |
|------|------------|-------------------|----------|--------|--------|----------|
| ABC  | YYYY-MM-DD | Transaction Ref 1 | GBP      |  10.99 |  98765 |    56473 |
| ABC  | YYYY-MM-DD | Transaction Ref 2 | GBP      |  11.99 |  98766 |    56474 |
| ABC  | YYYY-MM-DD | Transaction Ref 3 | GBP      |  12.99 |  98767 |    56475 |
| ABC  | YYYY-MM-DD | Transaction Ref 4 | GBP      |  13.99 |  98768 |    56476 |
| ...  | ...        | ...               | ...      |  ...   | ...    | ...      |

## License

MIT License. Copyright (c) 2020 Chris Stefano. See [LICENSE](LICENSE) for details.

<!-- Links -->

[dotnetcore]: https://dotnet.microsoft.com/
[ofx]: https://en.wikipedia.org/wiki/Open_Financial_Exchange
[singlefileapps]: https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#publish-a-single-file-app---cli
