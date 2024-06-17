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

using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersIndexKeys: TestCasesBase
    {
        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ \"Age\" : 1 }")]
        public void Ascending()
        {
            _ = Builders<User>.IndexKeys.Ascending(x => x.Age);
            _ = Builders<User>.IndexKeys.Ascending("Age");
        }

        [BuildersMQL("{ \"Address\" : -1, \"Name\" : 1, \"LastName\" : -1 }")]
        [BuildersMQL("{ \"SiblingsCount\" : 1, \"TicksSinceBirth\" : -1, \"Vehicle\" : 1 }")]
        public void Ascending_descending()
        {
            _ = Builders<Person>.IndexKeys.Descending(x => x.Address).Ascending("Name").Descending(x => x.LastName);
            _ = Builders<Person>.IndexKeys.Ascending(x => x.SiblingsCount).Descending("TicksSinceBirth").Ascending(x => x.Vehicle);
        }

        [BuildersMQL("{ \"Name\" : \"text\", \"LastName\" : \"hashed\", \"Vehicle\" : \"2d\", \"Address\" : 1 }")]
        [BuildersMQL("{ \"Name\" : \"2d\", \"LastName\" : \"2dsphere\", \"Vehicle\" : \"text\", \"Address\" : -1, \"SiblingsCount\" : \"hashed\", \"TicksSinceBirth\" : 1 }")]
        [BuildersMQL("{ \"Name\" : \"2d\", \"LastName\" : \"2d\", \"Address\" : \"text\", \"Vehicle\" : \"text\" }")]
        public void Combined()
        {
            _ = Builders<Person>.IndexKeys.Text(u => u.Name).Hashed(u => u.LastName).Geo2D(u => u.Vehicle).Ascending(u => u.Address);
            _ = Builders<Person>.IndexKeys.Geo2D(u => u.Name).Geo2DSphere(u => u.LastName).Text(u => u.Vehicle).Descending(u => u.Address).Hashed(u => u.SiblingsCount).Ascending(u => u.TicksSinceBirth);
            _ = Builders<Person>.IndexKeys.Combine(Builders<Person>.IndexKeys.Geo2D(u => u.Name).Geo2D("LastName"), Builders<Person>.IndexKeys.Text(u => u.Address).Text("Vehicle"));
        }

        [BuildersMQL("{ \"Address\" : -1 }")]
        [BuildersMQL("{ \"Address\" : -1 }")]
        public void Descending()
        {
            _ = Builders<Person>.IndexKeys.Descending(x => x.Address);
            _ = Builders<Person>.IndexKeys.Descending("Address");
        }

        [BuildersMQL("{ \"Vehicle\" : \"2d\" }")]
        [BuildersMQL("{ \"Address\" : \"2d\", \"Vehicle\" : \"2d\" }")]
        public void Geo2D()
        {
            _ = Builders<Person>.IndexKeys.Geo2D(u => u.Vehicle);
            _ = Builders<Person>.IndexKeys.Geo2D(u => u.Address).Geo2D("Vehicle");
        }

        [BuildersMQL("{ \"Vehicle\" : \"2dsphere\" }")]
        [BuildersMQL("{ \"Address\" : \"2dsphere\", \"Vehicle\" : \"2dsphere\" }")]
        public void Geo2DSphere()
        {
            _ = Builders<Person>.IndexKeys.Geo2DSphere(u => u.Vehicle);
            _ = Builders<Person>.IndexKeys.Geo2DSphere(u => u.Address).Geo2DSphere("Vehicle");
        }

        [BuildersMQL("{ \"Vehicle\" : \"geoHaystack\" }")]
        [BuildersMQL("{ \"Address\" : \"geoHaystack\", \"Vehicle\" : \"geoHaystack\" }")]
        [System.Obsolete]
        public void GeoHaystack()
        {
            _ = Builders<Person>.IndexKeys.GeoHaystack(u => u.Vehicle);
            _ = Builders<Person>.IndexKeys.GeoHaystack(u => u.Address).GeoHaystack("Vehicle");
        }

        [BuildersMQL("{ \"Vehicle\" : \"hashed\" }")]
        [BuildersMQL("{ \"Address\" : \"hashed\", \"Vehicle\" : \"hashed\" }")]
        public void Hashed()
        {
            _ = Builders<Person>.IndexKeys.Hashed(u => u.Vehicle);
            _ = Builders<Person>.IndexKeys.Hashed(u => u.Address).Hashed("Vehicle");
        }

        [BuildersMQL("{ \"Vehicle\" : \"text\" }")]
        [BuildersMQL("{ \"Address\" : \"text\", \"Vehicle\" : \"text\" }")]
        public void Text()
        {
            _ = Builders<Person>.IndexKeys.Text(u => u.Vehicle);
            _ = Builders<Person>.IndexKeys.Text(u => u.Address).Text("Vehicle");
        }

        [BuildersMQL("{ \"$**\" : 1 }")]
        [BuildersMQL("{ \"Address.$**\" : 1 }")]
        [BuildersMQL("{ \"SiblingsCount.$**\" : 1 }")]
        [BuildersMQL("{ wildcardField.$** : 1 }")]
        public void Wildcard()
        {
            _ = Builders<Person>.IndexKeys.Wildcard();
            _ = Builders<Person>.IndexKeys.Wildcard("Address");
            _ = Builders<Person>.IndexKeys.Wildcard(p => p.SiblingsCount);

            var wildcardField = "Vehicle";
            _ = Builders<Person>.IndexKeys.Wildcard(wildcardField);
        }
    }
}

