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
        private FilterDefinition<User> Foo(FilterDefinition<User> x)
        {
            return x;
        }
        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 30)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 31)]
        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 32)]
        public void variable_tracking_1()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);
            x = Foo(x);
            y = Foo(x);
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 43)]
        [BuildersMQL("{ \"Age\" : 21 }", 44)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 45)]
        [BuildersMQL("{ \"Age\" : 21 }", 45)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false }, \"Age\" : 21 }", 46)]
        public void variable_tracking_2()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            var y = Builders<User>.Filter.Eq(u => u.Age, 21);
            var z = x & y;
            var u = z;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 60)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 61)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 62)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 63)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 64)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 64)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 64)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 64)]
        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false }, \"Age\" : { \"$lt\" : 21 } }, { \"Age\" : { \"$gt\" : 65 }, \"Address\" : { \"$exists\" : true } }] }", 65)]
        public void variable_tracking_3()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            var y = Builders<User>.Filter.Lt(u => u.Age, 21);
            var z = Builders<User>.Filter.Gt(u => u.Age, 65);
            var w = Builders<User>.Filter.Exists(u => u.Address, true);
            var u = (x & y) | (z & w);
            var t = u;
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 74)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 75)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 75)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 76)]
        public void variable_tracking_4()
        {
            var x = Builders<User>.Filter.Gt(u => u.Age, 21);
            var y = x & Builders<User>.Filter.Lt(u => u.Age, 40);
            var z = y;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 84)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 85)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 86)]
        public void variable_tracking_5()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            var z = Builders<User>.Filter.Exists(u => u.Address, true);
            z = x;
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 95)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 96)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 96)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 97)]
        public void variable_tracking_6()
        {
            var x = Builders<User>.Filter.Gt(u => u.Age, 21);
            x = x & Builders<User>.Filter.Lt(u => u.Age, 40);
            var z = x;
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 105)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 106)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 107)]
        public void variable_tracking_7()
        {
            var x = Builders<User>.Filter.Gt(u => u.Age, 21);
            x &= Builders<User>.Filter.Lt(u => u.Age, 40);
            var z = x;
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 115)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 116)]
        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$gt\" : 21 } }, { \"Age\" : { \"$lt\" : 40 } }] }", 117)]
        public void variable_tracking_8()
        {
            var x = Builders<User>.Filter.Gt(u => u.Age, 21);
            x |= Builders<User>.Filter.Lt(u => u.Age, 40);
            var z = x;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 131)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 132)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 133)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 134)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 135)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 65 } }", 135)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 135)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 135)]
        [BuildersMQL("{ \"$and\" : [{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Age\" : { \"$gt\" : 65 } }] }, { \"$or\" : [{ \"Age\" : { \"$lt\" : 21 } }, { \"Address\" : { \"$exists\" : true } }] }] }", 136)]
        public void variable_tracking_9()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            var y = Builders<User>.Filter.Gt(u => u.Age, 65);
            var z = Builders<User>.Filter.Lt(u => u.Age, 21);
            var w = Builders<User>.Filter.Exists(u => u.Address, true);
            var u = (x | y) & (z | w);
            var t = u;
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 148)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 149)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 150)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 151)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }", 152)]
        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 40 } }, { \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }] }", 152)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 }, \"$or\" : [{ \"Age\" : { \"$lt\" : 40 } }, { \"Age\" : { \"$gt\" : 21, \"$lt\" : 40 } }] }", 153)]
        public void variable_tracking_10()
        {
            var x = Builders<User>.Filter.Gt(u => u.Age, 21);
            var y = Builders<User>.Filter.Lt(u => u.Age, 40);
            x &= y;
            y |= x;
            x = x & y;
            var z = x;
        }

        [BuildersMQL("{ \"Age\" : { \"$gt\" : 21 } }", 168)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 169)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 25 } }", 170)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 25 } }", 171)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 40 } }", 171)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 39 } }", 172)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 29 } }", 173)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 29 } }", 174)]
        [BuildersMQL("{ \"Age\" : { \"$gt\" : 39 } }", 174)]
        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$gt\" : 29 } }, { \"Age\" : { \"$gt\" : 39 } }] }", 175)]
        public void variable_tracking_11()
        {
            var x = Builders<User>.Filter.Gt(u => u.Age, 21);
            var y = Builders<User>.Filter.Lt(u => u.Age, 40);
            x = Builders<User>.Filter.Gt(u => u.Age, 25);
            var z = x & y;
            y = Builders<User>.Filter.Gt(u => u.Age, 39);
            x = Builders<User>.Filter.Gt(u => u.Age, 29);
            z = x | y;
            var w = z;
        }

        [BuildersMQL("{ \"Name\" : \"John\" }", 194)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 195)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 196)]
        [BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }", 197)]
        [BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }", 198)]
        [BuildersMQL("{ \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\", \"LastName\" : \"Doe\" }] }", 198)]
        [BuildersMQL("{ \"Name\" : \"Jane\" }", 199)]
        [BuildersMQL("{ \"Name\" : \"Jane\" }", 200)]
        [BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\", \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\", \"LastName\" : \"Doe\" }] }", 200)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 201)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 202)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 203)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 203)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }, { \"Name\" : \"Jane\" }, { \"Address\" : { \"$exists\" : true } }] }", 204)]
        public void variable_tracking_12()
        {
            var x = Builders<User>.Filter.Eq(u => u.Name, "John");
            var y = Builders<User>.Filter.Eq(u => u.LastName, "Doe");
            x &= y;
            y |= x;
            x = x & y;
            var w = Builders<User>.Filter.Eq(u => u.Name, "Jane");
            var z = w | x;
            w |= Builders<User>.Filter.Exists(u => u.Address, true);
            x = w;
            y = x | w;
            var p = y;
        }

        [BuildersMQL("{ \"Age\" : 20 }", 220)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 221)]
        [BuildersMQL("{ \"Age\" : 20 }", 222)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 222)]
        [BuildersMQL("{ \"Scores\" : 20 }", 223)]
        [BuildersMQL("{ \"Age\" : 20, \"Address\" : { \"$exists\" : true } }", 224)]
        [BuildersMQL("{ \"Scores\" : 20 }", 224)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 60 } }", 225)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 60 } }", 226)]
        [BuildersMQL("{ \"Age\" : 20, \"Address\" : { \"$exists\" : true }, \"Scores\" : 20 }", 226)]
        [BuildersMQL("{ \"$or\" : [{ \"Age\" : { \"$lt\" : 60 } }, { \"Age\" : 20, \"Address\" : { \"$exists\" : true }, \"Scores\" : 20 }] }", 227)]
        public void variable_tracking_13()
        {
            var x = Builders<User>.Filter.Eq(u => u.Age, 20);
            var y = Builders<User>.Filter.Exists(u => u.Address, true);
            x = x & y;
            y = Builders<User>.Filter.AnyEq(u => u.Scores, 20);
            y = x & y;
            x = Builders<User>.Filter.Lt(u => u.Age, 60);
            x = x | y;
            y = x;
        }

        [BuildersMQL("{ \"Age\" : 20 }", 233)]
        public void variable_tracking_14<T>()
        {
            var u = Builders<User>.Filter.Eq(u => u.Age, 20);
            T obj = default(T);
            var x = Builders<T>.Filter.Eq(u => u, obj);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 243)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 244)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 248)]
        public void variable_tracking_15()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);
            int a = 19;
            if (a > 29)
            {
                x = Builders<User>.Filter.Exists((x => x.Address), false);
            }
            var w = x;
        }

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 259)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 260)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 264)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 265)]
        public void variable_tracking_16()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);
            int a = 19;
            for (int i = 10; i < 22; i++)
            {
                x = Builders<User>.Filter.Exists((x => x.Address), false);
                y = x;
            }
            var w = y;
        }

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 281)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 282)]
        [BuildersMQL("{ \"Address.City\" : \"New York City\" }", 283)]
        [BuildersMQL("{ \"Name\" : \"Person\" }", 284)]
        [BuildersMQL("{ \"Address.City\" : \"New York City\" }", 287)]
        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 288)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 288)]
        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }, { \"Address\" : { \"$exists\" : false } }] }", 289)]
        [BuildersMQL("{ \"Address.City\" : \"Detroit\" }", 292)]
        public void variable_tracking_17()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);
            var z = Builders<Person>.Filter.Eq(p => p.Address.City, "New York City");
            var w = Builders<Person>.Filter.Eq(p => p.Name, "Person");
            for (int i = 10; i < 22; i++)
            {
                w = z;
                x = x | y;
                y = x;
                if (i == 22)
                {
                    z = Builders<Person>.Filter.Eq(p => p.Address.City, "Detroit");
                }
            }
            y = x;
        }

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 306)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 307)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 313)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 316)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 318)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 323)]
        public void variable_tracking_18()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);

            if (true)
            {
                if (true)
                {
                    x = Builders<User>.Filter.Exists((x => x.Address), false);
                    if (true)
                    {
                        y = x;
                    }
                    var w = x;
                }
                var w2 = x;
            }
            var w3 = x;
            x = Builders<User>.Filter.Exists((x => x.Address), true);
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 329)]
        public void variable_tracking_19()
        {
            var x = Builders<User>.Filter.Exists((x => x.Address), false);
            var obj = new { x = 10, y = 20 };
        }

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 340)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 341)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 347)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 350)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 352)]
        public void variable_tracking_20()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);

            if (true)
            {
                if (true)
                {
                    x = Builders<User>.Filter.Exists((x => x.Address), false);
                    if (true)
                    {
                        y = x;
                    }
                    var w = x;
                }
                var w2 = x;
            }
            var w3 = x;
        }

        private int Foo2(FilterDefinition<User> x)
        {
            return 1;
        }


        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 369)]
        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 370)]
        public void variable_tracking_21()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            Foo2(x);
            var y = x;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 384)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 385)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 386)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 387)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 387)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 387)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 395)]
        [BuildersMQL("{ \"Age\" : 20 }", 398)]
        public void variable_tracking_22()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            var y = Builders<User>.Filter.Exists(u => u.Address, false);
            var z = Builders<User>.Filter.Exists(u => u.Address, false);
            F2(x, y, z);
            var a = x;
            var b = y;
            var c = z;

            void F2(FilterDefinition<User> x, FilterDefinition<User> y, FilterDefinition<User> z)
            {
                var z2 = x;
                x = Builders<User>.Filter.Exists((x => x.Address), true);
                if (true)
                {
                    x = Builders<User>.Filter.Eq(u => u.Age, 20);
                }
                var y2 = x;
            }
        }

        private FilterDefinition<User> Foo3(FilterDefinition<User> x, FilterDefinition<User> y)
        {
            return x;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 417)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 418)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 419)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 420)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 420)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 420)]
        public void variable_tracking_23()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false);
            var y = Builders<User>.Filter.Exists(u => u.Address, false);
            var z = Builders<User>.Filter.Exists(u => u.Address, false);
            Foo3(x, Foo3(y, z));
            var a = x;
            var b = y;
            var c = z;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 433)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 434)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 438)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 441)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 444)]
        public void variable_tracking_24()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);

            if (true)
            {
                y |= x;
                if (true)
                {
                    x = Builders<User>.Filter.Exists((x => x.Address), false);
                    if (true)
                    {
                        y = x;
                    }
                    else
                    {
                        x = y;
                    }
                    var w = x;
                }
                else
                {
                    y = x;
                }
                var w2 = x;
            }
            else
            {
                y &= x;
            }
            var w3 = x;
        }
    }
}

