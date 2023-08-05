MongoDB C# Analyzer Sample
==========================

This sample demonstrates the usage of MongoDB Roslyn analyzer.
MongoDB.Analyzer allows to preview in compile time the underlying MQL, generated for LINQ and Builders queries, and the corresponding JSON for POCOs(Plain Old CLR/Class Objects).
It also provides an early warning for unsupported POCOs, LINQ and Builder expressions, and invalid queries.

Getting Started
---------------
* Open MongoDB.Analyzer.Samples.sln solution in VS2019, VS2022 or Rider 2021.2 (and higher)
* Build the solution.
* For the initial run, allow a few minutes for VS to run the analyzer.
* Inspect the warnings and information messages in the output panel or in the code browser.
* Edit LINQ and Builder expressions and POCOs and inspect the live updates.

After successful install, the analyzer should appear under Dependencies/Analyzers folder:
![Kiku](images/analyzer_dep.jpg)

Inspecting the MQL and JSON
---------------------------
* Hover over 3 grey dots under the LINQ or Builders expression or the POCO. An information tooltip containing corresponding MQL query or JSON will pop up.
* Not supported LINQ or Builders expressions or POCOs will be highlighted in IDE as a warning.

Samples
-------
* [Builders samples](BasicSample/BuildersSample.cs)
* [LINQ samples](BasicSample/LinqSample.cs)
* [LINQ3 samples](BasicSample/Linq3Sample.cs)
* [POCO samples](BasicSample/PocoSample.cs)

Examples:
Builders filter expression:
![Kiku](images/builders_1.jpg)

LINQ expression:
![Kiku](images/linq2_1.jpg)

LINQ expression:
![Kiku](images/linq2_2.jpg)

POCO:
![Kiku](images/poco_1.jpg)

Builders unsupported expression:
![Kiku](images/builders_warning.jpg)

LINQ unsupported expression:
![Kiku](images/linq_warning.jpg)

POCO unsupported:
![Kiku](images/poco_warning.jpg)

All Analyzer diagnostics in the output panel:
![Kiku](images/output_panel.jpg)


LINQ 3
------
MongoDB Driver 2.14 and higher, introduces new LINQ3 provider, while LINQ2 provider is still used by default. The analyzer will use the default LINQ provider (LINQ2).
For the expressions that are not supported in LINQ2 but are supported in LINQ3, appropriate warning will be generated containing LINQ3 MQL information.

LINQ3 only expression:
![Kiku](images/linq3_1.jpg)

To switch the default Analyzer LINQ provider to LINQ3, please set ```"DefaultLinqVersion": "V3"``` in ```mongodb.analyzer.json``` settings file.

POCO to JSON Analysis Settings
------------------------------
Two settings are available within the analyzer for POCO to JSON Analysis. The first, ```PocoAnalysisVerbosity``` sets POCO analysis verbosity.
The default setting is "Medium" which only provides diagnostics for POCOs with class-level BSON attributes, property/field level BSON attributes, or if the POCO type has been used with a Builders/LINQ Query.
The other settings are ```"None"```, which processes no POCOs, or ```"All"``` which processes all POCOs within the file. The other setting, PocoLimit, sets an upper limit on the number of POCOs to be analyzed. Default value of ```PocoLimit``` is 500.

To change the default POCO analysis verbosity, please set either ```"PocoAnalysisVerbosity": "None"``` or ```"PocoAnalysisVerbosity": "All"``` in ```mongodb.analyzer.json``` settings file.
To change the default Analyzer POCO processing limit, please set ```PocoLimit: limit```.

Not supported
-------------
* Custom serializers are not supported in the current version. Therefore the MQL generated in runtime could be differ from the MQL presented by the Analyzer.

Troubleshooting
---------------
* Please make sure that VS2019, VS2022, Rider 2021.2 (and higher) have the most recent updates. Analysis for some expressions might not be supported in older VS2019 versions.
* LINQ or Builders expressions or POCOs for which MQL or JSON is not generated, might be not supported in the current Analyzer version.
* Some expressions are not being analyzed due to internal VS errors. In such cases please set ```"OutputInternalExceptions": "true"``` in ```mongodb.analyzer.json``` settings file. If the expression fails to be analyzed due to an internal error, the error will be reported as a warning. Please report this error, along with the VS version and OS platform.
* For further troubleshooting please enable the logs output by setting ```"OutputInternalLogsToFile": "true"``` and ```"LogFileName": "full_existing_path\\logs_filename"```.

Error example:
![Kiku](images/error.jpg)
