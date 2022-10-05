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

using System;
using System.Collections.Generic;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersVariables : TestCasesBase
    {
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 32)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 33)]
        [BuildersMQL("{ \"SiblingsCount\" : { \"$lte\" : 5 } }", 35)]
        [BuildersMQL("{ \"Vehicle.VehicleType.MPG\" : { \"$gte\" : 25.0 } }", 36)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 38)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 39)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 41)]
        public void Single_variable_tracking()
        {
            var x = Builders<Person>.Filter.Exists(p => p.Address, false);
            x = Builders<Person>.Filter.Exists(p => p.Address, true);

            x = Builders<Person>.Filter.Lte(p => p.SiblingsCount, 5);
            x = Builders<Person>.Filter.Gte(p => p.Vehicle.VehicleType.MPG, 25);

            x = Builders<Person>.Filter.Eq(p => p.Name, "John");
            x = Builders<Person>.Filter.Eq(p => p.LastName, "Doe");

            var finalResult = x;
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 57)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 58)]
        [BuildersMQL("{ \"Vehicle.LicenceNumber\" : \"12345\" }", 59)]
        [BuildersMQL("{ \"SiblingsCount\" : { \"$lte\" : 5 } }", 61)]
        [BuildersMQL("{ \"Vehicle.VehicleType.MPG\" : { \"$gte\" : 25.0 } }", 62)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 64)]
        [BuildersMQL("{ \"Address.City\" : \"New York City\" }", 65)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 66)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 68)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 69)]
        [BuildersMQL("{ \"Address.City\" : \"New York City\" }", 70)]
        public void Multiple_variables_tracking()
        {
            var x = Builders<Person>.Filter.Exists(p => p.Address, false);
            var y = Builders<Person>.Filter.Exists(p => p.Address, true);
            var z = Builders<Person>.Filter.Eq(p => p.Vehicle.LicenceNumber, "12345");

            y = Builders<Person>.Filter.Lte(p => p.SiblingsCount, 5);
            x = Builders<Person>.Filter.Gte(p => p.Vehicle.VehicleType.MPG, 25);

            x = Builders<Person>.Filter.Eq(p => p.Name, "John");
            z = Builders<Person>.Filter.Eq(p => p.Address.City, "New York City");
            y = Builders<Person>.Filter.Eq(p => p.LastName, "Doe");

            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
        }

        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 86)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 87)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 88)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 88)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 90)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 91)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 92)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 92)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\" }, { \"LastName\" : \"Doe\" }] }", 94)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }", 94)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\" }, { \"LastName\" : \"Doe\" }, { \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }] }", 95)]
        public void Combined_filter_expressions()
        {
            var x = Builders<User>.Filter.Lt(u => u.Age, 21);
            var y = Builders<User>.Filter.Gt(u => u.Height, 65);
            var z = x & y;

            var a = Builders<User>.Filter.Eq(u => u.Name, "John");
            var b = Builders<User>.Filter.Eq(u => u.LastName, "Doe");
            var c = a | b;

            var compoundFilter = c | z;
            var finalResult = compoundFilter;
        }

        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 106)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 107)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 109)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 110)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }", 112)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\" }, { \"LastName\" : \"Doe\" }, { \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }] }", 113)]
        public void Compound_expression_resolution()
        {
            var x = Builders<User>.Filter.Lt(u => u.Age, 21);
            x &= Builders<User>.Filter.Gt(u => u.Height, 65);

            var a = Builders<User>.Filter.Eq(u => u.Name, "John");
            a |= Builders<User>.Filter.Eq(u => u.LastName, "Doe");

            a |= x;
            var finalResult = a;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 132)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 133)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 134)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 135)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 136)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 137)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 138)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 140)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 142)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 144)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 61 } }", 148)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 81 } }", 152)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 159)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 167)]
        public void If_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<Tree>.Filter.Eq(t => t.Root.Data, 99);
            var b = Builders<Tree>.Filter.Eq(t => t.Root.Data, 100);
            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 101);

            if (w == Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22))
            {
                w = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 50);
            }
            else if (x == Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30))
            {
                x = w;
            }
            else if (y == Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 61))
            {
                y = x;
            }
            else if (z == Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 81))
            {
                z = y;
            }
            else
            {
                a = z;
                c = b;
            }

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 186)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 187)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 188)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 189)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 190)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 191)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 192)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 196)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 197)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 198)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 203)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 204)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 207)]
        public void For_loop_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<Tree>.Filter.Eq(t => t.Root.Data, 99);
            var b = Builders<Tree>.Filter.Eq(t => t.Root.Data, 100);
            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 101);

            for (int i = 0; i < 10; i++)
            {
                x = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 60);
                w = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 50);
                a = b = c;
            }

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 226)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 227)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 228)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 229)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 230)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 231)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 232)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 234)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 236)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 237)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 238)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 243)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 244)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 247)]
        public void While_loop_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<Tree>.Filter.Eq(t => t.Root.Data, 99);
            var b = Builders<Tree>.Filter.Eq(t => t.Root.Data, 100);
            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 101);

            while (y == Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22))
            {
                x = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 60);
                w = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 50);
                a = b = c;
            }

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 266)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 267)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 268)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 269)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 270)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 271)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 272)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 276)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 277)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 278)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 280)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 284)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 285)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 288)]
        public void Do_while_loop_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<Tree>.Filter.Eq(t => t.Root.Data, 99);
            var b = Builders<Tree>.Filter.Eq(t => t.Root.Data, 100);
            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 101);

            do
            {
                x = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 60);
                w = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 50);
                a = b = c;
            }
            while (y == Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22));

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 311)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 312)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 313)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 314)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 315)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 316)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 66 } }", 317)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 318)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 68 } }", 319)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 69 } }", 320)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 70 } }", 321)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 329)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 344)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 345)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 66 } }", 352)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 361)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 363)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 66 } }", 364)]
        public void Switch_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<User>.Filter.Lt(u => u.Age, 21);
            var b = Builders<User>.Filter.Gt(u => u.Height, 65);
            var d = Builders<User>.Filter.Gt(u => u.Height, 66);
            var e = Builders<User>.Filter.Gt(u => u.Height, 67);
            var f = Builders<User>.Filter.Gt(u => u.Height, 68);
            var g = Builders<User>.Filter.Gt(u => u.Height, 69);
            var h = Builders<User>.Filter.Gt(u => u.Height, 70);

            int counter = 1;

            switch (counter)
            {
                case 1:
                    {
                        w = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
                        break;
                    }
                case 2:
                    {
                        x = w;
                        break;
                    }
                case 3:
                    {
                        y = x;
                        break;
                    }
                default:
                    {
                        a = b;
                        var c = z;
                        break;
                    }
            }

            var filter = counter switch
            {
                1 => e = d,
                2 => f = e,
                3 => g = f,
                _ => h = g
            };

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultD = d;
            var finalResultE = e;
            var finalResultF = f;
            var finalResultG = g;
            var finalResultH = h;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 386)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 387)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 388)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 389)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 390)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 391)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 392)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 396)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 397)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 398)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 403)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 404)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 407)]
        public void For_each_loop_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<Tree>.Filter.Eq(t => t.Root.Data, 99);
            var b = Builders<Tree>.Filter.Eq(t => t.Root.Data, 100);
            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 101);

            foreach (var item in new List<string>())
            {
                x = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 60);
                w = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 50);
                a = b = c;
            }

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 422)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 423)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 424)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 425)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 426)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 427)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 428)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 432)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 440)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 445)]
        public void Try_catch_finally_statements()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 30);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<Tree>.Filter.Eq(t => t.Root.Data, 99);
            var b = Builders<Tree>.Filter.Eq(t => t.Root.Data, 100);
            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 101);

            try
            {
                x = Builders<Tree>.Filter.Gte(t => t.Root.Right.Data, 60);
            }
            catch (Exception exe)
            {
                w = x;
            }
            finally
            {
                a = b = c = z = y;
            }

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"Age\" : 22 }", 459)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 22 } }", 460)]
        [BuildersMQL("{ \"Age\" : 22 }", 464)]
        [BuildersMQL("{ \"Scores\" : 11 }", 467)]
        [BuildersMQL("{ \"Scores\" : 11 }", 470)]
        public void Nested_conditional_statements()
        {
            var x = Builders<User>.Filter.Eq(u => u.Age, 22);
            var y = Builders<User>.Filter.Gte(x => x.Height, 22);

            if (true)
            {
                y |= x;
                if (true)
                {
                    x = Builders<User>.Filter.AnyEq(x => x.Scores, 11);
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

        [BuildersMQL("{ \"Age\" : 22 }", 498)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 22 } }", 499)]
        [BuildersMQL("{ \"Age\" : 22 }", 503)]
        [BuildersMQL("{ \"Scores\" : 11 }", 506)]
        [BuildersMQL("{ \"Scores\" : 11 }", 509)]
        public void Nested_loop_statements()
        {
            var x = Builders<User>.Filter.Eq(u => u.Age, 22);
            var y = Builders<User>.Filter.Gte(x => x.Height, 22);

            for (int i = 0; i < 10; i++)
            {
                y |= x;
                for (int j = 0; j < 10; j++)
                {
                    x = Builders<User>.Filter.AnyEq(x => x.Scores, 11);
                    for (int k = 0; k < 10; k++)
                    {
                        y = x;
                    }
                    for (int k = 0; k < 10; k++)
                    {
                        x = y;
                    }
                    var w = x;
                }
                for (int j = 0; j < 10; j++)
                {
                    y = x;
                }
                var w2 = x;
            }
            for (int i = 0; i < 10; i++)
            {
                y &= x;
            }
            var w3 = x;
        }

        [BuildersMQL("{ \"Height\" : 54 }", 545)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 546)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 56 } }", 547)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 99 } }", 548)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 101 } }", 549)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 106 } }", 550)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 57 } }", 553)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 100 } }", 556)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 102 } }", 560)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 103 } }", 564)]
        [BuildersMQL("{ \"Age\" : 21 }", 569)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 58 } }", 573)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 105 } }", 574)]
        public void Nested_try_catch_finally_Statements()
        {
            var t = Builders<User>.Filter.Eq(u => u.Height, 54);
            var x = Builders<User>.Filter.Lte(u => u.Height, 55);
            var w = Builders<User>.Filter.Gte(u => u.Height, 56);
            var a = Builders<User>.Filter.Gte(u => u.Height, 99);
            var b = Builders<User>.Filter.Gte(u => u.Height, 101);
            var c = Builders<User>.Filter.Gte(u => u.Height, 106);
            try
            {
                x = Builders<User>.Filter.Lte(u => u.Height, 57);
                if (true)
                {
                    a = Builders<User>.Filter.Gte(u => u.Height, 100);
                }
                try
                {
                    b = Builders<User>.Filter.Gte(u => u.Height, 102);
                }
                finally
                {
                    c = Builders<User>.Filter.Gte(u => u.Height, 103);
                }
            }
            catch (Exception exe)
            {
                t = Builders<User>.Filter.Eq(u => u.Age, 21);
            }
            finally
            {
                w = Builders<User>.Filter.Gte(u => u.Height, 58);
                b = Builders<User>.Filter.Gte(u => u.Height, 105);
                a = c;
            }
            var finalResultX = x;
            var finalResultW = w;
            var finalResultT = t;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
        }

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 594)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 595)]
        [BuildersMQL("{ \"Address\" : \"New York City\" }", 596)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 597)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 598)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 599)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 611)]
        public void Invalid_builders_expressions()
        {
            var x = Builders<User>.Filter.Exists(u => u.Address, false) | Builders<User>.Filter.Exists(u => u.Address, true);
            var y = Builders<User>.Filter.Exists((x => x.Address), false);
            var z = Builders<User>.Filter.Eq(u => u.Address, "New York City");
            var w = Builders<Person>.Filter.Exists(p => p.Address, true);
            var a = Builders<Person>.Filter.Eq(p => p.Name, "John");
            var b = Builders<Person>.Filter.Eq(p => p.LastName, "Doe");

            var c = GenerateCompoundFilter(a, b);
            x = GenerateBuildersExpression(x);
            y = GenerateBuildersExpression(x);
            GenerateBuildersExpression(z);

            (a, b) = GenerateMultipleBuildersExpressions(a, b);

            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
        }


        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 635)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 636)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 55 } }", 636)]
        [BuildersMQL("{ \"LicenceNumber\" : \"12345\" }", 640)]
        [BuildersMQL("{ \"VehicleType.Type\" : 0 }", 640)]
        [BuildersMQL("{ \"VehicleType.MPG\" : 31.5 }", 640)]
        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 641)]
        [BuildersMQL("{ \"LicenceNumber\" : \"12345\" }", 643)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 645)]
        [BuildersMQL("{ \"StringB\" : \"StringB_Backup\" }", 646)]
        [BuildersMQL("{ \"LicenceNumber\" : \"12345\" }", 648)]
        [BuildersMQL("{ \"VehicleType.Type\" : 0 }", 649)]
        [BuildersMQL("{ \"StringB\" : \"StringB_Backup\" }", 651)]
        [BuildersMQL("{ \"StringB\" : \"StringB_Backup\" }", 652)]
        [BuildersMQL("{ \"StringB\" : \"StringB_Backup\" }", 653)]
        [BuildersMQL("{ \"StringB\" : \"StringB_Backup\" }", 654)]
        public void Single_line_statements()
        {
            var x = Builders<User>.Filter.Lte(u => u.Height, 55);
            if (x == Builders<User>.Filter.Lte(u => u.Height, 55)) x = Builders<User>.Filter.Gte(u => u.Height, 55);

            var w = x;

            FilterDefinition<Vehicle> a = Builders<Vehicle>.Filter.Eq(v => v.LicenceNumber, "12345"), b = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.Type, VehicleTypeEnum.Bus), c = Builders<Vehicle>.Filter.Eq(v => v.VehicleType.MPG, 31.5);
            FilterDefinition<Pair> d = Builders<Pair>.Filter.Eq(p => p.StringA, "StringA"), e = GenerateBuildersExpression(d), f = e;

            for (int i = 0; i < 10; i++) c = Builders<Vehicle>.Filter.Eq(v => v.LicenceNumber, "12345");

            d = e = f = Builders<Pair>.Filter.Eq(p => p.StringB, "StringB");
            var g = d = e = f = Builders<Pair>.Filter.Eq(p => p.StringB, "StringB_Backup");

            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
            var finalResultD = d;
            var finalResultE = e;
            var finalResultF = f;
            var finalResultG = g;
            var finalResultX = x;
            var finalResultW = w;
        }

        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 674)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 675)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 55 } }", 676)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 677)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 680)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 683)]
        [BuildersMQL("{ \"StringB\" : \"StringBValue\" }", 687)]
        [BuildersMQL("{ \"StringA\" : \"StringAValue\" }", 693)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 696)]
        [BuildersMQL("{ \"StringB\" : \"StringBValue\" }", 699)]
        [BuildersMQL("{ \"StringA\" : \"StringAValue\" }", 700)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 701)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 702)]
        public void Miscellaneous_block_statements()
        {
            var x = Builders<Pair>.Filter.Eq(p => p.StringA, "StringA");
            var y = Builders<Pair>.Filter.Eq(p => p.StringB, "StringB");
            var a = Builders<User>.Filter.Gte(u => u.Height, 55);
            var z = Builders<Person>.Filter.Exists(p => p.Address, true);

            {
                a = Builders<User>.Filter.Lte(u => u.Height, 55);
            }

            var w = a;

            checked
            {
                x = Builders<Pair>.Filter.Eq(p => p.StringB, "StringBValue");
            }

            goto Label;

            Label:
                y = Builders<Pair>.Filter.Eq(p => p.StringA, "StringAValue");

            {
                z = Builders<Person>.Filter.Exists(p => p.Address, false);
            }

            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
        }

        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 714)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 715)]
        [BuildersMQL("{ \"StringA\" : \"StringA_NestedMethod\" }", 720)]
        [BuildersMQL("{ \"StringB\" : \"StringB_NestedMethod\" }", 721)]
        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 724)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 724)]
        [BuildersMQL("{ \"StringA\" : \"StringA\", \"StringB\" : \"StringB\" }", 725)]
        public void Nested_methods()
        {
            var x = Builders<Pair>.Filter.Eq(p => p.StringA, "StringA");
            var y = Builders<Pair>.Filter.Eq(p => p.StringB, "StringB");
            Nested_builders_method(x, y);

            void Nested_builders_method(FilterDefinition<Pair> x, FilterDefinition<Pair> y)
            {
                x = Builders<Pair>.Filter.Eq(p => p.StringA, "StringA_NestedMethod");
                y = Builders<Pair>.Filter.Eq(p => p.StringB, "StringB_NestedMethod");
            }

            var z = x & y;
            var finalResult = z;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 736)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 737)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 738)]
        [BuildersMQL("{ \"Root.Data\" : 26 }", 739)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 739)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 742)]
        public void Conditional_expressions()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var y = w == Builders<Tree>.Filter.Eq(t => t.Root.Data, 26) ? z = x : w = z;

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 766)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 767)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 768)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 769)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 770)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 68 } }", 771)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 66 } }", 772)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 773)]
        [BuildersMQL("{ \"Root.Data\" : 26 }", 775)]
        [BuildersMQL("{ \"Root.Data\" : 36 }", 775)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 775)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 68 } }", 775)]
        [BuildersMQL("{ \"Root.Data\" : 25 }", 777)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 778)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 780)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 781)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 68 } }", 783)]
        public void Nested_conditional_expressions()
        {
            var w = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var z = Builders<Tree>.Filter.Ne(t => t.Root.Data, 20);
            var a = Builders<User>.Filter.Lt(u => u.Age, 21);
            var b = Builders<User>.Filter.Gt(u => u.Height, 65);
            var c = Builders<User>.Filter.Gt(u => u.Height, 68);
            var d = Builders<User>.Filter.Gt(u => u.Height, 66);
            var e = Builders<User>.Filter.Gt(u => u.Height, 67);

            var y = w == Builders<Tree>.Filter.Eq(t => t.Root.Data, 26) ? z == Builders<Tree>.Filter.Eq(t => t.Root.Data, 36) ? e = a : d = e : b = c;

            var finalResultW = w;
            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
            var finalResultD = d;
            var finalResultE = e;
        }

        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 798)]
        [BuildersMQL("{ \"Root.Data\" : 25 }", 800)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 805)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 806)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 807)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 809)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 810)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 811)]
        public void Member_access_and_element_access_should_not_be_displayed()
        {
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var array = new FilterDefinition<Tree>[1];
            array[0] = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);
            var arrayRef = array;

            // Member Name and Variable Name must be treated as different
            var nestedFilter = new NestedFilter();
            var PersonFilter = Builders<Person>.Filter.Exists(p => p.Address, true);
            nestedFilter.PersonFilter = Builders<Person>.Filter.Exists(p => p.Address, false);
            var finalResultPersonFilter = PersonFilter;

            var UserFilter = Builders<User>.Filter.Gt(u => u.Height, 67);
            nestedFilter.UserFilter = Builders<User>.Filter.Gt(u => u.Height, 65);
            var finalResultUserFilter = UserFilter;
        }

        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 823)]
        [BuildersMQL("{ \"Root.Data\" : 25 }", 824)]
        [BuildersMQL("{ \"Root.Data\" : 25 }", 826)]
        [BuildersMQL("{ \"Root.Data\" : 27 }", 829)]
        [BuildersMQL("{ \"Root.Data\" : 28 }", 830)]
        [BuildersMQL("{ \"Root.Data\" : 29 }", 831)]
        [BuildersMQL("{ \"Root.Data\" : 30 }", 832)]
        public void Declarations_and_assignments_with_parenthesis_and_tuples()
        {
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            ((((((((x)))))))) = Builders<Tree>.Filter.Eq(t => t.Root.Data, 25);

            var y = x;
            (var a, var b) = (x, y) = GenerateMultipleBuildersExpressions(x, y);

            var c = Builders<Tree>.Filter.Eq(t => t.Root.Data, 27);
            var d = Builders<Tree>.Filter.Eq(t => t.Root.Data, 28);
            var e = Builders<Tree>.Filter.Eq(t => t.Root.Data, 29);
            var f = Builders<Tree>.Filter.Eq(t => t.Root.Data, 30);

            (c, d) = (e, f) = GenerateMultipleBuildersExpressions(x, y);

            var finalResultA = a;
            var finalResultB = b;
            var finalResultC = c;
            var finalResultD = d;
            var finalResultE = e;
            var finalResultF = f;
            var finalResultX = x;
            var finalResultY = y;
        }

        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 851)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 852)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 855)]
        public void Class_fields_should_not_be_processed()
        {
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = x | field;
            var z = y;

            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
        }

        private FilterDefinition<T> GenerateBuildersExpression<T>(FilterDefinition<T> buildersExpression) => buildersExpression;
        private FilterDefinition<T> GenerateCompoundFilter<T>(FilterDefinition<T> A, FilterDefinition<T> B) => A & B;
        private (FilterDefinition<T> A, FilterDefinition<T> B) GenerateMultipleBuildersExpressions<T>(FilterDefinition<T> A, FilterDefinition<T> B) => (A, B);
        private FilterDefinition<Tree> field = Builders<Tree>.Filter.Eq(t => t.Root.Data, 27);
    }
}

