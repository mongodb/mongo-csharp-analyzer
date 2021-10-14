; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
MABuilders1001 | MongoDB.Analyzer.Builders | Info | Builders expression to MQL [Documentation](http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-builders-2-mql-v-1-0)
MABuilders2001 | MongoDB.Analyzer.Builders | Warning | Unsupported builders expression [Documentation](http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-not-supported-builders-expression-v-1-0)
MALinq1001 | MongoDB.Analyzer.LINQ | Info | LINQ expression to MQL [Documentation](http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-linq-2-mql-v-1-0)
MALinq2001 | MongoDB.Analyzer.LINQ | Warning | Unsupported LINQ expression [Documentation](http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-not-supported-linq-expression-v-1-0)
MALinq2002 | MongoDB.Analyzer.LINQ | Warning | Supported only in LINQ3 expression [Documentation](http://dochub.mongodb.org/core/mongodb-analyzer-diagnostic-rule-not-supported-linq-2-expression-v-1-0)
