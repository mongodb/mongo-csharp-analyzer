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
    public class BuildersProjection : TestCasesBase
    {
        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ \"Address\" : 1, \"LastName\" : 1 }")]
        [BuildersMQL("{ \"Name\" : 1, \"TicksSinceBirth\" : 1, \"Vehicle\" : 0 }")]
        [BuildersMQL("{ \"LicenceNumber\" : 0, \"VehicleType\" : 1 }")]
        [BuildersMQL("{ \"Name\" : 1, \"TicksSinceBirth\" : 0, \"Vehicle\" : 1, \"Address\" : 0 }")]
        public void Include_exclude()
        {
            _ = Builders<User>.Projection.Include(u => u.Age);
            _ = Builders<Person>.Projection.Include(u => u.Address).Include(u => u.LastName);
            _ = Builders<Person>.Projection.Include(u => u.Name).Include(u => u.TicksSinceBirth)
                .Exclude(u => u.Vehicle);
            _ = Builders<Vehicle>.Projection.Exclude(v => v.LicenceNumber).Include(v => v.VehicleType);
            _ = Builders<Person>.Projection.Include(u => u.Name).Exclude(u => u.TicksSinceBirth)
                .Include(u => u.Vehicle).Exclude(u => u.Address);
        }

        [BuildersMQL("{ \"Address\" : 1, \"_id\" : 0 }")]
        [BuildersMQL("{ \"Address\" : 1, \"LastName\" : 1, \"Name\" : 1, \"_id\" : 0 }")]
        public void Expression()
        {
            _ = Builders<Person>.Projection.Expression(u => u.Address);
            _ = Builders<User>.Projection.Expression(u => u.LastName.Length + u.Address.Length + u.Name.Length);
        }

        [BuildersMQL("{ \"Age\" : 1 }")]
        [BuildersMQL("{ \"Name\" : 1 }")]
        [BuildersMQL("{ \"Age\" : 1, \"Name\" : 1 }")]
        public void Combined_projections()
        {
            var projection1 = Builders<User>.Projection.Include(u => u.Age);
            var projection2 = Builders<User>.Projection.Include(u => u.Name);
            _ = Builders<User>.Projection.Combine(Builders<User>.Projection.Include(u => u.Age), Builders<User>.Projection.Include(u => u.Name));
        }

        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$lt\" : 11 } } } }")]
        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$gt\" : 11, \"$lt\" : 15 } } } }")]
        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$lt\" : 12, \"$gt\" : 3 } } } }")]
        [BuildersMQL("{ \"PesonsList\" : { \"$elemMatch\" : { \"SiblingsCount\" : { \"$lt\" : 12, \"$gt\" : 3 } } }, \"NestedListsHolderIList\" : { \"$elemMatch\" : { \"PesonsList\" : { \"$size\" : 22 } } } }")]
        public void ElemMatch()
        {
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, g => g.SiblingsCount < 11);
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, Builders<Person>.Filter.Gt(u => u.SiblingsCount, 11) & Builders<Person>.Filter.Lt(u => u.SiblingsCount, 15));
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, g => g.SiblingsCount < 12 && g.SiblingsCount > 3);
            _ = Builders<ListsHolder>.Projection.ElemMatch(u => u.PesonsList, g => g.SiblingsCount < 12 && g.SiblingsCount > 3).ElemMatch(u => u.NestedListsHolderIList, g => g.PesonsList.Count == 22);
        }

        [BuildersMQL("{ \"IntArray\" : { \"$slice\" : [10, 5] } }")]
        [BuildersMQL("{ \"IntArray\" : { \"$slice\" : [10, 5] }, \"JaggedStringArray2\" : { \"$slice\" : [3, 9] } }")]
        public void Slice()
        {
            _ = Builders<SimpleTypesArraysHolder>.Projection.Slice(u => u.IntArray, 10, 5);
            _ = Builders<SimpleTypesArraysHolder>.Projection.Slice(u => u.IntArray, 10, 5).Slice(u => u.JaggedStringArray2, 3, 9);
        }

        [NoDiagnostics]
        public void As()
        {
            _ = Builders<User>.Projection.As<Person>();
        }
    }
}

