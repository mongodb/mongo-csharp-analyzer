﻿// Copyright 2021-present MongoDB Inc.
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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Analyzer.Core.Poco;
using MongoDB.Analyzer.Tests.Common.DataModel;

namespace MongoDB.Analyzer.Tests.Core;

[TestClass]
public sealed class PocoDataFillerTests
{
    [DataTestMethod]
    public void TestSimplePoco()
    {
        var poco = new Address();
        PocoDataFiller.PopulatePoco(poco);
        Assert.AreEqual(poco.City, "City_val");
        Assert.AreEqual(poco.Province, "Province_val");
        Assert.AreEqual(poco.ZipCode, "ZipCode_val");
    }

    [DataTestMethod]
    public void TestNestedPoco()
    {
        var poco = new Person();
        PocoDataFiller.PopulatePoco(poco);
        Assert.AreEqual(poco.Name, "Name_val");
        Assert.AreEqual(poco.LastName, "LastName_val");
        Assert.AreEqual(poco.Address.City, "City_val");
        Assert.AreEqual(poco.Address.Province, "Province_val");
        Assert.AreEqual(poco.Address.ZipCode, "ZipCode_val");
        Assert.AreEqual(poco.Vehicle.LicenceNumber, "LicenceNumber_val");
        Assert.AreEqual(poco.Vehicle.VehicleType.Category, "Category_val");
        Assert.AreEqual(poco.Vehicle.VehicleType.MPG, 3.0);
        Assert.AreEqual(poco.Vehicle.VehicleType.Type, VehicleTypeEnum.Bus);
        Assert.AreEqual(poco.Vehicle.VehicleType.VehicleMake.Name, "Name_val");
    }

    [DataTestMethod]
    public void TestCollectionHolder()
    {
        var poco = new ListsHolder();
        PocoDataFiller.PopulatePoco(poco);

        CollectionAssert.AreEqual(poco.PesonsList, new List<Person>(0));
        CollectionAssert.AreEqual(poco.StringList, new List<string>(0));
        CollectionAssert.AreEqual(poco.NestedListsHolderList, new List<ListsHolder>(0));
        CollectionAssert.AreEqual((List<int>)poco.IntIList, new List<int>(0));
        CollectionAssert.AreEqual((List<int>)poco.IntIList, new List<int>(0));
        CollectionAssert.AreEqual((List<ListsHolder>)poco.NestedListsHolderIList, new List<ListsHolder>(0));
    }

    [DataTestMethod]
    public void TestSimpleArrayHolder()
    {
        var poco = new SimpleTypesArraysHolder();
        PocoDataFiller.PopulatePoco(poco);
        CollectionAssert.AreEqual(poco.ByteArray, new byte[] {});
        CollectionAssert.AreEqual(poco.IntArray, new int[] { });
        CollectionAssert.AreEqual(poco.ObjectArray, new object[] { });
        CollectionAssert.AreEqual(poco.JaggedStringArray2, new string[][] { });
        CollectionAssert.AreEqual(poco.JaggedIntArray3, new int[][][] { });
        CollectionAssert.AreEqual(poco.JaggedLongArray4, new long[][][][] { });
        CollectionAssert.AreEqual(poco.JaggedShortArray5, new short[][][][][] { });
    }

    [DataTestMethod]
    public void TestMultiDimentionalArrayHolder()
    {
        var poco = new MultiDimentionalArrayHolder();
        PocoDataFiller.PopulatePoco(poco);
        CollectionAssert.AreEqual(poco.Matrix2, new int[,] { });
        CollectionAssert.AreEqual(poco.Matrix3, new int[,,] { });
    }

    [DataTestMethod]
    public void TestPrimitiveTypeHolder()
    {
        var poco = new PrimitiveTypeHolder();
        PocoDataFiller.PopulatePoco(poco);
        Assert.AreEqual(poco.BooleanValue, true);
        Assert.AreEqual(poco.ByteValue, 9);
        Assert.AreEqual(poco.SByteValue, 0);
        Assert.AreEqual(poco.ShortValue, 0);
        Assert.AreEqual(poco.UShortValue, 1);
        Assert.AreEqual(poco.IntValue, 8);
        Assert.AreEqual(poco.UIntValue, (uint)9);
        Assert.AreEqual(poco.LongValue, 9);
        Assert.AreEqual(poco.ULongValue, (ulong)0);
        Assert.AreEqual(poco.CharValue, 9);
        Assert.AreEqual(poco.DoubleValue, 1.0);
        Assert.AreEqual(poco.StringValue, "StringValue_val");
        Assert.AreEqual(poco.FloatValue, 0.0);
    }

    [DataTestMethod]
    public void TestPropertiesAndFields()
    {
        var poco = new Fruit();
        PocoDataFiller.PopulatePoco(poco);
        Assert.AreEqual(poco.Name, "Name_val");
        Assert.AreEqual(poco.Weight, 6.0);
        Assert.AreEqual(poco.Color, "Color_val");
        Assert.AreEqual(poco.Quantity, 8);
        Assert.AreEqual(poco.TotalCost, 9.0);
        Assert.AreEqual(poco.Volume, 6.0);
    }

    [DataTestMethod]
    public void TestSelfReferencingPoco()
    {
        var poco = new TreeNode();
        PocoDataFiller.PopulatePoco(poco);

        Assert.AreEqual(poco.Data, 4);
        Assert.AreEqual(poco.Left.Data, 4);
        Assert.AreEqual(poco.Left.Left.Data, 4);
        Assert.AreEqual(poco.Left.Left.Left.Data, 4);
        Assert.AreEqual(poco.Left.Left.Right.Data, 4);

        Assert.AreEqual(poco.Left.Left.Left.Left, null);
        Assert.AreEqual(poco.Left.Left.Left.Right, null);
        Assert.AreEqual(poco.Left.Left.Right.Left, null);
        Assert.AreEqual(poco.Left.Left.Right.Right, null);

        Assert.AreEqual(poco.Left.Right.Data, 4);
        Assert.AreEqual(poco.Left.Right.Left.Data, 4);
        Assert.AreEqual(poco.Left.Right.Right.Data, 4);

        Assert.AreEqual(poco.Left.Right.Left.Left, null);
        Assert.AreEqual(poco.Left.Right.Left.Right, null);
        Assert.AreEqual(poco.Left.Right.Right.Left, null);
        Assert.AreEqual(poco.Left.Right.Right.Right, null);

        Assert.AreEqual(poco.Right.Data, 4);
        Assert.AreEqual(poco.Right.Left.Data, 4);
        Assert.AreEqual(poco.Right.Left.Left.Data, 4);
        Assert.AreEqual(poco.Right.Left.Right.Data, 4);

        Assert.AreEqual(poco.Right.Left.Left.Left, null);
        Assert.AreEqual(poco.Right.Left.Left.Right, null);
        Assert.AreEqual(poco.Right.Left.Right.Left, null);
        Assert.AreEqual(poco.Right.Left.Right.Right, null);

        Assert.AreEqual(poco.Right.Right.Data, 4);
        Assert.AreEqual(poco.Right.Right.Left.Data, 4);
        Assert.AreEqual(poco.Right.Right.Right.Data, 4);

        Assert.AreEqual(poco.Right.Right.Left.Left, null);
        Assert.AreEqual(poco.Right.Right.Left.Right, null);
        Assert.AreEqual(poco.Right.Right.Right.Left, null);
        Assert.AreEqual(poco.Right.Right.Right.Right, null);
    }
}