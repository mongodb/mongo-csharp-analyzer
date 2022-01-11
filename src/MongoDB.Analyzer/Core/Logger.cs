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

namespace MongoDB.Analyzer.Core;

internal sealed class Logger : IDisposable
{
    private readonly string _correlationId;
    private readonly string _fileName;
    private readonly List<string> _logs;

    public static Logger Empty { get; } = new Logger();

    private Logger()
    {
        _logs = null;
    }

    public Logger(string fileName, string correlationId)
    {
        _correlationId = correlationId;
        _fileName = fileName;
        _logs = new List<string>();
    }

    public void Dispose()
    {
        Flush();
    }

    public void Flush()
    {
        if (_logs.EmptyOrNull())
        {
            return;
        }

        try
        {
            var fileExists = File.Exists(_fileName);
            if (fileExists)
            {
                var fileInfo = new FileInfo(_fileName);
                if (fileInfo.Length > 32 * 1024 * 1024)
                {
                    File.Delete(_fileName);
                    fileExists = false;
                }
            }

            if (!fileExists)
            {
                File.WriteAllLines(_fileName, _logs);
            }
            else
            {
                File.AppendAllLines(_fileName, _logs);
            }

            _logs.Clear();
        }
        catch { }
    }

    public void Log(string message)
    {
        _logs?.Add($"{_correlationId}:{DateTime.UtcNow} {message}");
    }
}
