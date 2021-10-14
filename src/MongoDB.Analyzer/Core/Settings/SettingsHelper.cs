// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Newtonsoft.Json;

namespace MongoDB.Analyzer.Core;

internal static class SettingsHelper
{
    public const string SettingsFileName = "mongodb.analyzer.json";

    private static readonly JsonSerializerSettings s_jsonSettings = new()
    {
        DefaultValueHandling = DefaultValueHandling.Populate
    };

    public static MongoDBAnalyzerSettings GetSettings(AnalyzerOptions analyzerOptions)
    {
        var result = new MongoDBAnalyzerSettings();

        try
        {
            var settingsFile = (analyzerOptions?.AdditionalFiles)?.FirstOrDefault(f => Path.GetFileName(f.Path) == SettingsFileName);

            if (settingsFile != null)
            {
                result = JsonConvert.DeserializeObject<MongoDBAnalyzerSettings>(settingsFile.GetText().ToString(), s_jsonSettings);
            }
        }
        catch
        {
            result = new MongoDBAnalyzerSettings();
        }

        return result;
    }

    public static Logger CreateLogger(MongoDBAnalyzerSettings settings, string correlationId)
    {
        if (settings?.OutputInternalLogsToFile == true && !string.IsNullOrWhiteSpace(settings.LogFileName))
        {
            return new Logger(settings.LogFileName, correlationId);
        }

        return Logger.Empty;
    }

    public static ITelemetryService CreateTelemetryService(MongoDBAnalyzerSettings settings, string correlationId)
    {
        if (settings?.SendTelemetry == true)
        {
            return new SegmentTelemetryService("TELEMETRY-KEY", correlationId);
        }

        return EmptyTelemetryService.Instance;
    }
}
