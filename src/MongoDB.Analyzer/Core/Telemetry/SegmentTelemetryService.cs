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

using System.Security.Cryptography;
using Segment;

namespace MongoDB.Analyzer.Core;

internal sealed class SegmentTelemetryService : ITelemetryService
{
    private static int s_isInitialized = 0;
    private static readonly string s_userId;

    private readonly string _correlationId;
    private readonly List<(string EvenName, (string, object)[] Data)> _events;

    static SegmentTelemetryService()
    {
        try
        {
            var userId = Environment.UserName + Environment.UserDomainName;

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.Default.GetBytes(userId));
            s_userId = new Guid(hash).ToString();
        }
        catch
        {
            s_userId = new Guid().ToString();
        }
    }

    public SegmentTelemetryService(string secret, string correlationId)
    {
        _correlationId = correlationId;
        _events = new List<(string EvenName, (string, object)[] Data)>();
        Initialize(secret);
    }

    public void Dispose()
    {
    }

    public void Flush()
    {
        foreach (var (eventName, data) in _events)
        {
            var traits = data.ToDictionary(d => d.Item1, d => d.Item2);
            traits.Add("analysis_id", _correlationId);

            Analytics.Client.Track(s_userId, eventName, traits);
        }
    }

    public void Event(string eventName, params (string, object)[] data)
    {
        _events.Add((eventName, data));
    }

    private void Initialize(string secret)
    {
        if (Interlocked.Exchange(ref s_isInitialized, 1) != 0)
        {
            return;
        }

        Analytics.Initialize(secret, new Config(flushAt: 40, flushInterval: 10));

        Analytics.Client.Identify(s_userId, new Dictionary<string, object>()
        {
            { "os_version", Environment.OSVersion.VersionString },
            { "ide_net_version", Environment.Version.ToString() },
        });
    }
}
