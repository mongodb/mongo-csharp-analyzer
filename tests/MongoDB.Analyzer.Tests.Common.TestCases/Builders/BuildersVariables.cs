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
    public sealed class BuildersVariables : TestCasesBase
    {
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 25)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 26)]
        //public void variable_tracking_1()
        //{
        //    var x = Builders<User>.Filter.Exists(u => u.Address, false);
        //    var z = x;
        //}

        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 36)]
        //[BuildersMQL("{ \"Age\" : 21 }", 37)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 38)]
        //[BuildersMQL("{ \"Age\" : 21 }", 38)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false }, \"Age\" : 21 }", 39)]
        //public void variable_tracking_2()
        //{
        //    var x = Builders<User>.Filter.Exists(u => u.Address, false);
        //    var y = Builders<User>.Filter.Eq(u => u.Age, 21);
        //    var z = x & y;
        //    var u = z;
        //}

        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 53)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 54)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 55)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 56)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 57)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 57)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 57)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 57)]
        //[BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false }, \"Age\" : { \"$lt\" : 21 } }, { \"Age\" : { \"$gt\" : 65 }, \"Address\" : { \"$exists\" : true } }] }", 58)]
        //public void variable_tracking_3()
        //{
        //    var x = Builders<User>.Filter.Exists(u => u.Address, false);
        //    var y = Builders<User>.Filter.Lt(u => u.Age, 21);
        //    var z = Builders<User>.Filter.Gt(u => u.Age, 65);
        //    var w = Builders<User>.Filter.Exists(u => u.Address, true);
        //    var u = (x & y) | (z & w);
        //    var t = u;
        //}

        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 66)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 67)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 68)]
        //public void variable_tracking_4()
        //{
        //    var x = Builders<User>.Filter.Gt(u => u.Age, 21);
        //    var y = x & Builders<User>.Filter.Lt(u => u.Age, 40);
        //    var z = y;
        //}

        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 76)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 77)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 78)]
        //public void variable_tracking_5()
        //{
        //    var x = Builders<User>.Filter.Exists(u => u.Address, false);
        //    var z = Builders<User>.Filter.Exists(u => u.Address, true);
        //    z = x;
        //}

        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 86)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 87)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 88)]
        //public void variable_tracking_6()
        //{
        //    var x = Builders<User>.Filter.Gt(u => u.Age, 21);
        //    x = x & Builders<User>.Filter.Lt(u => u.Age, 40);
        //    var z = x;
        //}

        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 96)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 97)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 98)]
        //public void variable_tracking_7()
        //{
        //    var x = Builders<User>.Filter.Gt(u => u.Age, 21);
        //    x &= Builders<User>.Filter.Lt(u => u.Age, 40);
        //    var z = x;
        //}

        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 106)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 107)]
        //[BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$gt\" : 21 } }, { \"Age\" : { \"$lt\" : 40 } }] }", 108)]
        //public void variable_tracking_8()
        //{
        //    var x = Builders<User>.Filter.Gt(u => u.Age, 21);
        //    x |= Builders<User>.Filter.Lt(u => u.Age, 40);
        //    var z = x;
        //}

        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 122)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 123)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 124)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 125)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 126)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 126)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 126)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 126)]
        //[BuildersMQL("{ \"$and\" : [{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Age\" : { \"$gt\" : 65 } }] }, { \"$or\" : [{ \"Age\" : { \"$lt\" : 21 } }, { \"Address\" : { \"$exists\" : true } }] }] }", 127)]
        //public void variable_tracking_9()
        //{
        //    var x = Builders<User>.Filter.Exists(u => u.Address, false);
        //    var y = Builders<User>.Filter.Gt(u => u.Age, 65);
        //    var z = Builders<User>.Filter.Lt(u => u.Age, 21);
        //    var w = Builders<User>.Filter.Exists(u => u.Address, true);
        //    var u = (x | y) & (z | w);
        //    var t = u;
        //}

        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 139)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 140)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 141)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 142)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 143)]
        //[BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 40 } }, { \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }] }", 143)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 }, \"$or\" : [{ \"Age\" : { \"$lt\" : 40 } }, { \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }] }", 144)]
        //public void variable_tracking_10()
        //{
        //    var x = Builders<User>.Filter.Gt(u => u.Age, 21);
        //    var y = Builders<User>.Filter.Lt(u => u.Age, 40);
        //    x &= y;
        //    y |= x;
        //    x = x & y;
        //    var z = x;
        //}

        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 159)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 160)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 25 } }", 161)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 25 } }", 162)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 162)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 39 } }", 163)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 29 } }", 164)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 29 } }", 165)]
        //[BuildersMQL("{ \"Age\" : { \"$gt\" : 39 } }", 165)]
        //[BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$gt\" : 29 } }, { \"Age\" : { \"$gt\" : 39 } }] }", 166)]
        //public void variable_tracking_11()
        //{
        //    var x = Builders<User>.Filter.Gt(u => u.Age, 21);
        //    var y = Builders<User>.Filter.Lt(u => u.Age, 40);
        //    x = Builders<User>.Filter.Gt(u => u.Age, 25);
        //    var z = x & y;
        //    y = Builders<User>.Filter.Gt(u => u.Age, 39);
        //    x = Builders<User>.Filter.Gt(u => u.Age, 29);
        //    z = x | y;
        //    var w = z;
        //}

        //[BuildersMQL("{ \"Name\" : \"John\" }", 185)]
        //[BuildersMQL("{ \"LastName\" : \"Doe\" }", 186)]
        //[BuildersMQL("{ \"LastName\" : \"Doe\" }", 187)]
        //[BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }", 188)]
        //[BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }", 189)]
        //[BuildersMQL("{ \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\", \"LastName\" : \"Doe\" }] }", 189)]
        //[BuildersMQL("{ \"Name\" : \"Jane\" }", 190)]
        //[BuildersMQL("{ \"Name\" : \"Jane\" }", 191)]
        //[BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\", \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\", \"LastName\" : \"Doe\" }] }", 191)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 192)]
        //[BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 193)]
        //[BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 194)]
        //[BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 194)]
        //[BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }, { \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 195)]
        //public void variable_tracking_12()
        //{
        //    var x = Builders<User>.Filter.Eq(u => u.Name, "John");
        //    var y = Builders<User>.Filter.Eq(u => u.LastName, "Doe");
        //    x &= y;
        //    y |= x;
        //    x = x & y;
        //    var w = Builders<User>.Filter.Eq(u => u.Name, "Jane");
        //    var z = w | x;
        //    w |= Builders<User>.Filter.Exists(u => u.Address, true);
        //    x = w;
        //    y = x | w;
        //    var p = y;
        //}

        //[BuildersMQL("{ \"Age\" : 20 }", 211)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 212)]
        //[BuildersMQL("{ \"Age\" : 20 }", 213)]
        //[BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 213)]
        //[BuildersMQL("{ \"Scores\" : 20 }", 214)]
        //[BuildersMQL("{ \"Age\" : 20, \"Address\" : { \"$exists\" : true } }", 215)]
        //[BuildersMQL("{ \"Scores\" : 20 }", 215)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 60 } }", 216)]
        //[BuildersMQL("{ \"Age\" : { \"$lt\" : 60 } }", 217)]
        //[BuildersMQL("{ \"Age\" : 20, \"Address\" : { \"$exists\" : true }, \"Scores\" : 20 }", 217)]
        //[BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 60 } }, { \"Age\" : 20, \"Address\" : { \"$exists\" : true }, \"Scores\" : 20 }] }", 218)]
        //public void variable_tracking_13()
        //{
        //    var x = Builders<User>.Filter.Eq(u => u.Age, 20);
        //    var y = Builders<User>.Filter.Exists(u => u.Address, true);
        //    x = x & y;
        //    y = Builders<User>.Filter.AnyEq(u => u.Scores, 20);
        //    y = x & y;
        //    x = Builders<User>.Filter.Lt(u => u.Age, 60);
        //    x = x | y;
        //    y = x;
        //}

        //public FilterDefinition<User> Foo(FilterDefinition<User> x, FilterDefinition<User> y)
        //{
        //    return x | y;
        //}

        //[BuildersMQL("{ \"Age\" : 20 }")]
        //[BuildersMQL("{ \"Age\" : 40 }")]
        //[BuildersMQL("{ \"Age\" : 20 }", 234)]
        //[BuildersMQL("{ \"Age\" : 40 }", 234)]
        //public void variable_tracking_14()
        //{
        //    var x = Builders<User>.Filter.Eq(u => u.Age, 20);
        //    var y = Builders<User>.Filter.Eq(u => u.Age, 40);
        //    var z = Foo(x,y);
        //}

        [BuildersMQL("{ \"Age\" : 20 }")]
        [BuildersMQL("{ \"Age\" : 40 }")]
        [BuildersMQL("{ \"Age\" : 60 }")]
        public void variable_tracking_14()
        {
            FilterDefinition<User> x = Builders<User>.Filter.Eq(u => u.Age, 20), y = Builders<User>.Filter.Eq(u => u.Age, 10);
            if (true)
            {
                FilterDefinition<User> x3 = Builders<User>.Filter.Eq(u => u.Age, 20), y3 = Builders<User>.Filter.Eq(u => u.Age, 10);
            }
            x = y = Builders<User>.Filter.Eq(u => u.Age, 60);
            var w = Builders<User>.Filter.Lt(u => u.Age, 20);
        }

        [BuildersMQL("{ \"Age\" : 20 }")]
        [BuildersMQL("{ \"Age\" : 40 }")]
        [BuildersMQL("{ \"Age\" : 60 }")]
        public void variable_tracking_15()
        {
            FilterDefinition<User> x2 = Builders<User>.Filter.Eq(u => u.Age, 20), y2 = Builders<User>.Filter.Eq(u => u.Age, 10);
            x2 = y2 = Builders<User>.Filter.Eq(u => u.Age, 60);
            var w2 = Builders<User>.Filter.Lt(u => u.Age, 20);
        }
        int apple = 10;
    }

}

