# OFX Loader

Simple utility to read a bunch of [OFX files][ofx] and output a single CSV file with the transactions in tabular form.

## Pre-requisites

* Windows, Linux or MacOS
* [DotNet Core][dotnetcore] `>= 2.2`
* [OFX files][ofx]

## Build

```
dotnet build
```

## Usage

Create a directory structure as follows and place your OFX files into the `ofx` directory.

```
<root>/
 - ofx/                 <--- OFX's will be read from here
   - OFXfile1.ofx
   - OFXfile2.ofx
   - ...
   - OFXfileN.ofx
 - csv/                 <--- CSV output will be written here
```

Copy the output `OFXLoader.dll` and `OFXLoader.runtimeconfig.json` files to the `<root>` directory.

Run the following command with the `<root>` directory as current.

```
dotnet OFXLoader.dll
```

The program will find each OFX file in the `ofx` directory and create a single CSV file in the `csv` directory, and load the unique set of transaction data into it. Duplicate transactions will be ignored.

Open the resultant CSV file in your favourite spreadsheet program to see the transactions in tabular form.

## Output Format

The following table shows an example of the tabular content produced.

| Date | Description | Amount | FitNum | CheckNum |
|------|-------------|--------|--------|----------|
| YYYY-MM-DD | Transaction Ref 1 |  10.99 | 98765 | 56473 |
| YYYY-MM-DD | Transaction Ref 2 |  11.99 | 98766 | 56474 |
| YYYY-MM-DD | Transaction Ref 3 |  12.99 | 98767 | 56475 |
| YYYY-MM-DD | Transaction Ref 4 |  13.99 | 98768 | 56476 |
| ... | ... |  ... | ... | ... |

## License

MIT License. Copyright (c) 2020 Chris Stefano. See [LICENSE](LICENSE) for details.

[ofx]: https://en.wikipedia.org/wiki/Open_Financial_Exchange
[dotnetcore]: https://dotnet.microsoft.com/
