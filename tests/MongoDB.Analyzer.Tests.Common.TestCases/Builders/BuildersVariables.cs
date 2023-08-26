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

#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable CS0168 // Variable is declared but never used

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersVariables : TestCasesBase
    {
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 1, 2, 5)]
        public void Class_fields_should_not_be_processed()
        {
            var x = Builders<Tree>.Filter.Lte(t => t.Root.Left.Data, 22);
            var y = x | field;
            var z = y;

            var finalResultX = x;
            var finalResultY = y;
            var finalResultZ = z;
        }

        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 1, 3)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 2, 3)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 5, 7)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 6, 7)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }", 9)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\" }, { \"LastName\" : \"Doe\" }] }", 9)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\" }, { \"LastName\" : \"Doe\" }, { \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }] }", 10)]
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

        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 1)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 2)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 4)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 5)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }", 7)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\" }, { \"LastName\" : \"Doe\" }, { \"Age\" : { \"$lt\" : 21 }, \"Height\" : { \"$gt\" : 65 } }] }", 8)]
        public void Compound_expression_resolution()
        {
            var x = Builders<User>.Filter.Lt(u => u.Age, 21);
            x &= Builders<User>.Filter.Gt(u => u.Height, 65);

            var a = Builders<User>.Filter.Eq(u => u.Name, "John");
            a |= Builders<User>.Filter.Eq(u => u.LastName, "Doe");

            a |= x;
            var finalResult = a;
        }

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1, 4)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2, 4, 7)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 3)]
        [BuildersMQL("{ \"Root.Data\" : 26 }", 4)]
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

        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 1)]
        [BuildersMQL("{ \"Root.Data\" : 25 }", 2, 4)]
        [BuildersMQL("{ \"Root.Data\" : 27 }", 7)]
        [BuildersMQL("{ \"Root.Data\" : 28 }", 8)]
        [BuildersMQL("{ \"Root.Data\" : 29 }", 9)]
        [BuildersMQL("{ \"Root.Data\" : 30 }", 10)]
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 18)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4, 19)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 5)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 6)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 7, 13, 22)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 11)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 12)]
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1, 9)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2, 9, 13)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 13, 17)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4, 21)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 5)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 6, 28, 36)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 7)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 11)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 61 } }", 17)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 81 } }", 21)]
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

        [BuildersMQL("{ \"$or\" : [{ \"Address\" : { \"$exists\" : false } }, { \"Address\" : { \"$exists\" : true } }] }", 1)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 2)]
        [BuildersMQL("{ \"Address\" : \"New York City\" }", 3, 11)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 4, 18)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 5)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 6)]
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2, 15)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 15)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4, 20)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 5)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 6)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 7, 13, 23)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 11)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 12)]
        public void Loops_do_while_statements()
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 18)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4, 19)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 5)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 6)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 7, 13, 22)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 11)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 12)]
        public void Loops_for_statements()
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2, 9)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 9)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4, 19)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 5)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 6)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 7, 13, 22)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 11)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 50 } }", 12)]
        public void Loops_while_statements()
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

        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 1)]
        [BuildersMQL("{ \"Root.Data\" : 25 }", 3)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 8, 10)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 9)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 12, 14)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 13)]
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

        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 1)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 2)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 55 } }", 3)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 4)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 7, 10, 29)]
        [BuildersMQL("{ \"StringB\" : \"StringBValue\" }", 14, 26)]
        [BuildersMQL("{ \"StringA\" : \"StringAValue\" }", 20, 27)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 23, 28)]
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1, 10, 12)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2, 13)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 3, 10, 15)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 4, 10, 16)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 5)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 68 } }", 6, 10, 18)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 66 } }", 7)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 8)]
        [BuildersMQL("{ \"Root.Data\" : 26 }", 10)]
        [BuildersMQL("{ \"Root.Data\" : 36 }", 10)]
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

        [BuildersMQL("{ \"Age\" : 22 }", 1, 6)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 22 } }", 2)]
        [BuildersMQL("{ \"Scores\" : 11 }", 9, 12)]
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

        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 1, 3, 11)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 2, 3, 11)]
        [BuildersMQL("{ \"StringA\" : \"StringA_NestedMethod\" }", 7)]
        [BuildersMQL("{ \"StringB\" : \"StringB_NestedMethod\" }", 8)]
        [BuildersMQL("{ \"StringA\" : \"StringA\", \"StringB\" : \"StringB\" }", 12)]
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

        [BuildersMQL("{ \"Age\" : 22 }", 1, 6)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 22 } }", 2)]
        [BuildersMQL("{ \"Scores\" : 11 }", 9, 12)]
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

        [BuildersMQL("{ \"Height\" : 54 }", 1)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 2)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 56 } }", 3)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 99 } }", 4)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 101 } }", 5)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 106 } }", 6)]
        [BuildersMQL("{ \"Height\" : { \"$lte\" : 57 } }", 9)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 100 } }", 12)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 102 } }", 16)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 103 } }", 20)]
        [BuildersMQL("{ \"Age\" : 21 }", 25)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 58 } }", 29)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 105 } }", 30)]
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
            catch (Exception ex)
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

        [BuildersMQL("{ \"Height\" : { \"$lte\" : 55 } }", 1, 2, 2)]
        [BuildersMQL("{ \"Height\" : { \"$gte\" : 55 } }", 2)]
        [BuildersMQL("{ \"LicenceNumber\" : \"12345\" }", 6, 14, 9)]
        [BuildersMQL("{ \"VehicleType.Type\" : 0 }", 6, 15)]
        [BuildersMQL("{ \"VehicleType.MPG\" : 31.5 }", 6)]
        [BuildersMQL("{ \"StringA\" : \"StringA\" }", 7, 17, 20)]
        [BuildersMQL("{ \"StringB\" : \"StringB\" }", 11)]
        [BuildersMQL("{ \"StringB\" : \"StringB_Backup\" }", 12, 19)]
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 19)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4, 35, 51)]
        [BuildersMQL("{ \"Age\" : { \"$lt\" : 21 } }", 5)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 65 } }", 6, 34, 53)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 66 } }", 7, 42, 54)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 67 } }", 8)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 68 } }", 9)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 69 } }", 10)]
        [BuildersMQL("{ \"Height\" : { \"$gt\" : 70 } }", 11)]
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

        [BuildersMQL("{ \"Root.Data\" : 25 }", 1)]
        [BuildersMQL("{ \"Root.Left.Data\" : { \"$lte\" : 22 } }", 2)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 30 } }", 3, 19, 24)]
        [BuildersMQL("{ \"Root.Data\" : { \"$ne\" : 20 } }", 4)]
        [BuildersMQL("{ \"Root.Data\" : 99 }", 5)]
        [BuildersMQL("{ \"Root.Data\" : 100 }", 6)]
        [BuildersMQL("{ \"Root.Data\" : 101 }", 7)]
        [BuildersMQL("{ \"Root.Right.Data\" : { \"$gte\" : 60 } }", 11)]
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
            catch
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

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 1)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 2)]
        [BuildersMQL("{ \"SiblingsCount\" : { \"$lte\" : 5 } }", 4)]
        [BuildersMQL("{ \"Vehicle.VehicleType.MPG\" : { \"$gte\" : 25.0 } }", 5)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 7)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 8, 10)]
        public void Variable_tracking_single_reasignment()
        {
            var x = Builders<Person>.Filter.Exists(p => p.Address, false);
            x = Builders<Person>.Filter.Exists(p => p.Address, true);

            x = Builders<Person>.Filter.Lte(p => p.SiblingsCount, 5);
            x = Builders<Person>.Filter.Gte(p => p.Vehicle.VehicleType.MPG, 25);

            x = Builders<Person>.Filter.Eq(p => p.Name, "John");
            x = Builders<Person>.Filter.Eq(p => p.LastName, "Doe");

            var finalResult = x;
        }

        [BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }", 1, 3, 4, 5, 6)]
        [BuildersMQL("{ \"Name\" : \"Bob\" }", 8, 9, 10, 12, 13, 14, 14, 16, 16)]
        public void Variable_tracking_single_multi_reference()
        {
            var x = Builders<Person>.Filter.Eq(p => p.Name, "John") &
                    Builders<Person>.Filter.Eq(x => x.LastName, "Doe");
            var y = x;
            Foo(x);
            Foo(y);
            GetMongoCollection<Person>().Find(x);

            x = Builders<Person>.Filter.Eq(p => p.Name, "Bob");
            Foo(x);
            GetMongoCollection<Person>().Find(x);

            y = x;
            Foo(y);
            GetMongoCollection<Person>().Find(y | x);

            Bar(x, y);

            if (true)
            {
                x = y = null;
            }

            Bar(x, y); // no diagnostics
        }

        [BuildersMQL("{ \"Address\" : { \"$exists\" : false } }", 1)]
        [BuildersMQL("{ \"Address\" : { \"$exists\" : true } }", 2)]
        [BuildersMQL("{ \"Vehicle.LicenceNumber\" : \"12345\" }", 3)]
        [BuildersMQL("{ \"SiblingsCount\" : { \"$lte\" : 5 } }", 5)]
        [BuildersMQL("{ \"Vehicle.VehicleType.MPG\" : { \"$gte\" : 25.0 } }", 6)]
        [BuildersMQL("{ \"Name\" : \"John\" }", 8, 12)]
        [BuildersMQL("{ \"Address.City\" : \"New York City\" }", 9, 14)]
        [BuildersMQL("{ \"LastName\" : \"Doe\" }", 10, 13)]
        public void Variable_tracking_multiple()
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

        [BuildersMQL("{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }", 1, 4, 5, 7, 8, 9, 13)]
        [BuildersMQL("{ \"Name\" : \"Bob\" }", 3, 4, 6, 7, 8, 10, 13)]
        [BuildersMQL("{ \"$or\" : [{ \"Name\" : \"John\", \"LastName\" : \"Doe\" }, { \"Name\" : \"Bob\" }] }", 8, 11, 13, 21, 22)]
        public void Variable_tracking_multiple_multi_reference()
        {
            var x = Builders<Person>.Filter.Eq(p => p.Name, "John") &
                    Builders<Person>.Filter.Eq(x => x.LastName, "Doe");
            var y = Builders<Person>.Filter.Eq(p => p.Name, "Bob");
            var z = x | y;
            Foo(x);
            Foo(y);
            Foo(x, y);
            Bar(x, y, z);
            GetMongoCollection<Person>().Find(x);
            GetMongoCollection<Person>().Find(y);
            GetMongoCollection<Person>().Find(z);

            GetMongoCollection<Person>().Find(x | y | z);

            if (true)
            {
                x = null;
                y = null;
            }

            Bar(x, y, z); // diagnostics for z only
            Bar(z);
        }

        private FilterDefinition<T> GenerateBuildersExpression<T>(FilterDefinition<T> buildersExpression) => buildersExpression;
        private FilterDefinition<T> GenerateCompoundFilter<T>(FilterDefinition<T> A, FilterDefinition<T> B) => A & B;
        private (FilterDefinition<T> A, FilterDefinition<T> B) GenerateMultipleBuildersExpressions<T>(FilterDefinition<T> A, FilterDefinition<T> B) => (A, B);
        private FilterDefinition<Tree> field = Builders<Tree>.Filter.Eq(t => t.Root.Data, 27);

        private void Bar(params object[] @params) { }
        private void Foo(object x) { }
        private void Foo(object x, object y) { }
    }
}

#pragma warning restore CS0162 // Unreachable code detected
#pragma warning restore CS0168 // Variable is declared but never used
