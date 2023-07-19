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
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class JsonEmpiricalTest : TestCasesBase
    {
        [Json("{ \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }")]
        public void AddressEmpirical()
        {
        }

        [Json("{ \"AirportName\" : \"AirportName\", \"AirportCode\" : \"AirportCode\" }")]
        public void AirportEmpirical()
        {
        }

        [Json("{ \"Make\" : \"Make\", \"Model\" : \"Model\", \"Year\" : 0 }")]
        public void CarEmpirical()
        {
        }

        [Json("{ \"CustomerID\" : 0, \"Name\" : \"Name\", \"Address\" : \"Address\" }")]
        public void CustomerEmpirical()
        {
        }

        [Json("{ \"BooleanValue\" : false, \"ByteValue\" : 0, \"SByteValue\" : 0, \"ShortValue\" : 0, \"UShortValue\" : 0, \"IntValue\" : 0, \"UIntValue\" : 0, \"LongValue\" : NumberLong(0), \"ULongValue\" : NumberLong(0), \"CharValue\" : 0, \"DoubleValue\" : 0.0, \"StringValue\" : \"StringValue\", \"FloatValue\" : 0.0 }")]
        public void PrimitiveTypeHolderEmpirical()
        {
        }

        [Json("{ \"BooleanArray\" : [], \"ByteArray\" : new BinData(0, \"\"), \"SByteArray\" : [], \"ShortArray\" : [], \"UShortArray\" : [], \"IntArray\" : [], \"UIntArray\" : [], \"LongArray\" : [], \"ULongArray\" : [], \"CharArray\" : [], \"DoubleArray\" : [], \"StringArray\" : [], \"FloatArray\" : [], \"ObjectArray\" : [], \"JaggedStringArray2\" : [], \"JaggedIntArray3\" : [], \"JaggedLongArray4\" : [], \"JaggedShortArray5\" : [] }")]
        public void SimpleTypesArraysHolderEmpirical()
        {
        }

        [Json("{ \"Matrix2\" : [], \"Matrix3\" : [], \"Matrix4\" : [] }")]
        public void MultiDimensionalArrayHolderEmpirical()
        {
        }

        [Json("{ \"EnumArrayWithDimension1\" : [], \"JaggedEnumArray\" : [], \"TreeJaggedArray2\" : [], \"TreeNodeJaggedArray3\" : [] }")]
        public void CustomTypesArraysHolderEmpirical()
        {
        }

        [Json("{ \"Data\" : 0, \"Children\" : [] }")]
        public void NestedArrayHolderEmpirical()
        {
        }

        [Json("{ \"Style\" : \"Style\", \"year_built\" : 0, \"_id\" : \"Identifier\" }")]
        public void HouseEmpirical()
        {
        }

        [Json("{ \"ExpiryDate\" : ISODate(\"0001-01-01T00:00:00Z\"), \"Name\" : \"Name\", \"InStock\" : false, \"price\" : \"0\", \"Pair\" : { \"StringA\" : null, \"StringB\" : null }, \"Length\" : 0, \"Width\" : 0, \"SaleTime\" : \"00:00:00\" }")]
        public void ClothingEmpirical()
        {
        }

        [JsonAttribute("{ \"VegetableCost\" : 0.0 }")]
        public void VegetableEmpirical()
        {
        }

        [JsonAttribute("{ \"ComputerCost\" : 0.0 }")]
        public void ComputerEmpirical()
        {
        }

        [Json("{ \"IntList\" : [], \"PesonsList\" : [], \"StringList\" : [], \"NestedListsHolderList\" : [], \"IntIList\" : [], \"NestedListsHolderIList\" : [] }")]
        public void ListsHolderEmpirical()
        {
        }

        [Json("{ \"Enumerable1\" : [], \"Enumerable2\" : [] }")]
        public void EnumerableHolderEmpirical()
        {
        }

        [Json("{ \"NestedIntList\" : [], \"NestedStringList\" : [], \"ListOfIntArray\" : [], \"ListOfStringArray\" : [], \"NestedNestedIntList\" : [], \"NestedIntIList\" : [], \"NestedStringIList\" : [], \"NestedIListIntArray\" : [], \"NestedIntIEnumerable\" : [], \"NestedStringIEnumerable\" : [], \"NestedIntArrayIEnumerable\" : [] }")]
        public void NestedCollectionHolderEmpirical()
        {
        }

        [Json("{ \"EnumInt8\" : 0, \"EnumUInt8\" : 0, \"EnumInt16\" : 0, \"EnumUInt16\" : 0, \"EnumInt32\" : 0, \"EnumUInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"EnumUInt64\" : NumberLong(0) }")]
        public void EnumHolderEmpirical()
        {
        }

        [Json("{ \"ExpiryDate\" : ISODate(\"0001-01-01T00:00:00Z\"), \"DictionaryField\" : { }, \"Name\" : \"Name\", \"InStock\" : false, \"Price\" : \"0\", \"Pair\" : { \"StringA\" : null, \"StringB\" : null }, \"Length\" : 0, \"Width\" : 0, \"SaleTime\" : \"00:00:00\" }")]
        public void UnsupportedBsonAttributesEmpirical()
        {
        }

        [Json("{ \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Person\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"SiblingsCount\" : 0, \"TicksSinceBirth\" : NumberLong(0), \"IsRetired\" : false }, \"Tree\" : { \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : \"Address\", \"Age\" : 0, \"Height\" : 0, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolderWithFieldsEmpirical()
        {
        }

        [Json("{ \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Person\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"SiblingsCount\" : 0, \"TicksSinceBirth\" : NumberLong(0), \"IsRetired\" : false }, \"Tree\" : { \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : \"Address\", \"Age\" : 0, \"Height\" : 0, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolderEmpirical()
        {
        }

        [Json("{ \"NestedInt\" : 0, \"NestedDouble\" : 0.0, \"NestedString\" : \"NestedString\" }")]
        public void NestedTypeWithFieldsEmpirical()
        {
        }

        [Json("{ \"NestedInt\" : 0, \"NestedDouble\" : 0.0, \"NestedString\" : \"NestedString\" }")]
        public void NestedTypeEmpirical()
        {
        }

        [Json("{ \"NestedNestedInt\" : 0, \"NestedNestedDouble\" : 0.0, \"NestedNestedString\" : \"NestedNestedString\" }")]
        public void NestedNestedTypeWithFieldsEmpirical()
        {
        }

        [Json("{ \"NestedNestedInt\" : 0, \"NestedNestedDouble\" : 0.0, \"NestedNestedString\" : \"NestedNestedString\" }")]
        public void NestedNestedTypeEmpirical()
        {
        }

        [Json("{ \"DateTimeKindField\" : 0, \"DateTimeOffsetField\" : [NumberLong(0), 0], \"TimeSpanField\" : \"00:00:00\", \"TypeField\" : null }")]
        public void SystemTypeContainerEmpirical()
        {
        }

        [Json("{ \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }")]
        public void AddressEmpiricalEmpirical()
        {
        }

        [Json("{ \"AirportName\" : \"AirportName\", \"AirportCode\" : \"AirportCode\" }")]
        public void AirportEmpiricalEmpirical()
        {
        }

        [Json("{ \"Make\" : \"Make\", \"Model\" : \"Model\", \"Year\" : 0 }")]
        public void CarEmpiricalEmpirical()
        {
        }

        [Json("{ \"CustomerID\" : 0, \"Name\" : \"Name\", \"Address\" : \"Address\" }")]
        public void CustomerEmpiricalEmpirical()
        {
        }

        [Json("{ \"BooleanValue\" : false, \"ByteValue\" : 0, \"SByteValue\" : 0, \"ShortValue\" : 0, \"UShortValue\" : 0, \"IntValue\" : 0, \"UIntValue\" : 0, \"LongValue\" : NumberLong(0), \"ULongValue\" : NumberLong(0), \"CharValue\" : 0, \"DoubleValue\" : 0.0, \"StringValue\" : \"StringValue\", \"FloatValue\" : 0.0 }")]
        public void PrimitiveTypeHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"BooleanArray\" : [], \"ByteArray\" : new BinData(0, \"\"), \"SByteArray\" : [], \"ShortArray\" : [], \"UShortArray\" : [], \"IntArray\" : [], \"UIntArray\" : [], \"LongArray\" : [], \"ULongArray\" : [], \"CharArray\" : [], \"DoubleArray\" : [], \"StringArray\" : [], \"FloatArray\" : [], \"ObjectArray\" : [], \"JaggedStringArray2\" : [], \"JaggedIntArray3\" : [], \"JaggedLongArray4\" : [], \"JaggedShortArray5\" : [] }")]
        public void SimpleTypesArraysHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"Matrix2\" : [], \"Matrix3\" : [], \"Matrix4\" : [] }")]
        public void MultiDimensionalArrayHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"EnumArrayWithDimension1\" : [], \"JaggedEnumArray\" : [], \"TreeJaggedArray2\" : [], \"TreeNodeJaggedArray3\" : [] }")]
        public void CustomTypesArraysHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"Data\" : 0, \"Children\" : [] }")]
        public void NestedArrayHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"Style\" : \"Style\", \"year_built\" : 0, \"_id\" : \"Identifier\" }")]
        public void HouseEmpiricalEmpirical()
        {
        }

        [Json("{ \"ExpiryDate\" : ISODate(\"0001-01-01T00:00:00Z\"), \"Name\" : \"Name\", \"InStock\" : false, \"price\" : \"0\", \"Pair\" : { \"StringA\" : null, \"StringB\" : null }, \"Length\" : 0, \"Width\" : 0, \"SaleTime\" : \"00:00:00\" }")]
        public void ClothingEmpiricalEmpirical()
        {
        }

        [JsonAttribute("{ \"VegetableCost\" : 0.0 }")]
        public void VegetableEmpiricalEmpirical()
        {
        }

        [JsonAttribute("{ \"ComputerCost\" : 0.0 }")]
        public void ComputerEmpiricalEmpirical()
        {
        }

        [Json("{ \"IntList\" : [], \"PesonsList\" : [], \"StringList\" : [], \"NestedListsHolderList\" : [], \"IntIList\" : [], \"NestedListsHolderIList\" : [] }")]
        public void ListsHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"Enumerable1\" : [], \"Enumerable2\" : [] }")]
        public void EnumerableHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"NestedIntList\" : [], \"NestedStringList\" : [], \"ListOfIntArray\" : [], \"ListOfStringArray\" : [], \"NestedNestedIntList\" : [], \"NestedIntIList\" : [], \"NestedStringIList\" : [], \"NestedIListIntArray\" : [], \"NestedIntIEnumerable\" : [], \"NestedStringIEnumerable\" : [], \"NestedIntArrayIEnumerable\" : [] }")]
        public void NestedCollectionHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"EnumInt8\" : 0, \"EnumUInt8\" : 0, \"EnumInt16\" : 0, \"EnumUInt16\" : 0, \"EnumInt32\" : 0, \"EnumUInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"EnumUInt64\" : NumberLong(0) }")]
        public void EnumHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"ExpiryDate\" : ISODate(\"0001-01-01T00:00:00Z\"), \"DictionaryField\" : { }, \"Name\" : \"Name\", \"InStock\" : false, \"Price\" : \"0\", \"Pair\" : { \"StringA\" : null, \"StringB\" : null }, \"Length\" : 0, \"Width\" : 0, \"SaleTime\" : \"00:00:00\" }")]
        public void UnsupportedBsonAttributesEmpiricalEmpirical()
        {
        }

        [Json("{ \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Person\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"SiblingsCount\" : 0, \"TicksSinceBirth\" : NumberLong(0), \"IsRetired\" : false }, \"Tree\" : { \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : \"Address\", \"Age\" : 0, \"Height\" : 0, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolderWithFieldsEmpiricalEmpirical()
        {
        }

        [Json("{ \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Person\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : { \"City\" : \"City\", \"Province\" : \"Province\", \"ZipCode\" : \"ZipCode\" }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : null, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"SiblingsCount\" : 0, \"TicksSinceBirth\" : NumberLong(0), \"IsRetired\" : false }, \"Tree\" : { \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, \"User\" : { \"Name\" : \"Name\", \"LastName\" : \"LastName\", \"Address\" : \"Address\", \"Age\" : 0, \"Height\" : 0, \"Scores\" : [] }, \"Vehicle\" : { \"LicenceNumber\" : \"LicenceNumber\", \"VehicleType\" : { \"VehicleMake\" : { \"Name\" : \"Name\" }, \"Type\" : 0, \"Category\" : \"Category\", \"MPG\" : 0.0 } }, \"EnumInt16\" : 0, \"EnumInt32\" : 0, \"EnumInt64\" : NumberLong(0), \"Pair\" : { \"StringA\" : null, \"StringB\" : null } }")]
        public void NestedTypeHolderEmpiricalEmpirical()
        {
        }

        [Json("{ \"NestedInt\" : 0, \"NestedDouble\" : 0.0, \"NestedString\" : \"NestedString\" }")]
        public void NestedTypeWithFieldsEmpiricalEmpirical()
        {
        }

        [Json("{ \"NestedInt\" : 0, \"NestedDouble\" : 0.0, \"NestedString\" : \"NestedString\" }")]
        public void NestedTypeEmpiricalEmpirical()
        {
        }

        [Json("{ \"NestedNestedInt\" : 0, \"NestedNestedDouble\" : 0.0, \"NestedNestedString\" : \"NestedNestedString\" }")]
        public void NestedNestedTypeWithFieldsEmpiricalEmpirical()
        {
        }

        [Json("{ \"NestedNestedInt\" : 0, \"NestedNestedDouble\" : 0.0, \"NestedNestedString\" : \"NestedNestedString\" }")]
        public void NestedNestedTypeEmpiricalEmpirical()
        {
        }

        [Json("{ \"DateTimeKindField\" : 0, \"DateTimeOffsetField\" : [NumberLong(0), 0], \"TimeSpanField\" : \"00:00:00\", \"TypeField\" : null }")]
        public void SystemTypeContainerEmpiricalEmpirical()
        {
        }

        public class TestClasses
        {
            public class AddressEmpirical
            {
                public string City { get; set; }
                public string Province { get; set; }
                public string ZipCode { get; set; }
            }

            public class AirportEmpirical
            {
                public string AirportName { get; set; }
                public string AirportCode { get; set; }
            }

            public class CarEmpirical
            {
                public string Make { get; set; }
                public string Model { get; set; }
                public int Year { get; set; }
            }

            public class CustomerEmpirical
            {
                public int CustomerID { get; set; }
                public string Name { get; set; }
                public string Address { get; set; }
            }

            public class PrimitiveTypeHolderEmpirical
            {
                public bool BooleanValue { get; set; }
                public byte ByteValue { get; set; }
                public sbyte SByteValue { get; set; }

                public short ShortValue { get; set; }
                public ushort UShortValue { get; set; }

                public int IntValue { get; set; }
                public uint UIntValue { get; set; }

                public long LongValue { get; set; }
                public ulong ULongValue { get; set; }

                public char CharValue { get; set; }
                public double DoubleValue { get; set; }

                public string StringValue { get; set; }
                public float FloatValue { get; set; }
            }

            public class SimpleTypesArraysHolderEmpirical
            {
                public bool[] BooleanArray { get; set; }
                public byte[] ByteArray { get; set; }
                public sbyte[] SByteArray { get; set; }

                public short[] ShortArray { get; set; }
                public ushort[] UShortArray { get; set; }

                public int[] IntArray { get; set; }
                public uint[] UIntArray { get; set; }

                public long[] LongArray { get; set; }
                public ulong[] ULongArray { get; set; }

                public char[] CharArray { get; set; }
                public double[] DoubleArray { get; set; }

                public string[] StringArray { get; set; }
                public float[] FloatArray { get; set; }
                public object[] ObjectArray { get; set; }

                public string[][] JaggedStringArray2 { get; set; }
                public int[][][] JaggedIntArray3 { get; set; }
                public long[][][][] JaggedLongArray4 { get; set; }
                public short[][][][][] JaggedShortArray5 { get; set; }
            }

            public class MultiDimensionalArrayHolderEmpirical
            {
                public int[,] Matrix2 { get; set; }
                public int[,,] Matrix3 { get; set; }
                public int[,][,] Matrix4 { get; set; }
            }

            public class CustomTypesArraysHolderEmpirical
            {
                public EnumInt16[] EnumArrayWithDimension1 { get; set; }
                public EnumInt16[][] JaggedEnumArray { get; set; }

                public Tree[][] TreeJaggedArray2 { get; set; }
                public TreeNode[][][] TreeNodeJaggedArray3 { get; set; }
            }

            public class NestedArrayHolderEmpirical
            {
                public int Data { get; set; }
                public NestedArrayHolder[] Children { get; set; }
            }

            public class HouseEmpirical
            {
                [BsonId]
                public string Identifier { get; set; }

                [BsonElement("year_built", Order = 2)]
                public int YearBuilt { get; set; }

                [BsonElement(Order = 1)]
                public string Style { get; set; }

                [BsonIgnore]
                public double Cost { get; set; }
            }

            public class ClothingEmpirical
            {
                [BsonConstructor("Name", "InStock", "Price")]
                public ClothingEmpirical(string Name, bool InStock, decimal Price)
                {
                    this.Name = Name;
                    this.InStock = InStock;
                    this.Price = Price;
                }

                [BsonFactoryMethod("Name", "InStock", "Price")]
                public void Factory_Method()
                {
                }

                [BsonIgnoreIfDefault]
                public string Name { get; set; }

                [BsonIgnoreIfNull]
                public bool InStock { get; set; }

                [BsonElement("price")]
                [BsonRepresentation(BsonType.Decimal128)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }

                [BsonDateTimeOptions(DateOnly = false, Kind = DateTimeKind.Unspecified, Representation = BsonType.DateTime)]
                public DateTime ExpiryDate;

                [BsonDefaultValue(defaultValue: 10)]
                public int Length { get; set; }

                [BsonRequired]
                public int Width { get; set; }

                [BsonTimeSpanOptions(representation: Bson.BsonType.String)]
                public TimeSpan SaleTime { get; set; }
            }

            [BsonIgnoreExtraElements]
            [BsonDiscriminatorAttribute("Carrot")]
            public class VegetableEmpirical
            {
                public double VegetableCost { get; set; }
            }

            [BsonNoId]
            public class ComputerEmpirical
            {
                public double ComputerCost { get; set; }

                [BsonExtraElementsAttribute]
                public BsonDocument CatchAll { get; set; }
            }

            public class ListsHolderEmpirical
            {
                public List<int> IntList { get; set; }
                public List<Person> PesonsList { get; set; }
                public System.Collections.Generic.List<string> StringList { get; set; }
                public System.Collections.Generic.List<ListsHolder> NestedListsHolderList { get; set; }

                public IList<int> IntIList { get; set; }
                public System.Collections.Generic.IList<ListsHolder> NestedListsHolderIList { get; set; }
            }

            public class EnumerableHolderEmpirical
            {
                public IEnumerable<int> Enumerable1 { get; set; }
                public System.Collections.Generic.IEnumerable<EnumerableHolder> Enumerable2 { get; set; }
            }

            public class NestedCollectionHolderEmpirical
            {
                public List<List<int>> NestedIntList { get; set; }
                public List<List<string>> NestedStringList { get; set; }
                public List<int[]> ListOfIntArray { get; set; }
                public List<string[]> ListOfStringArray { get; set; }
                public List<List<List<int>>> NestedNestedIntList { get; set; }

                public IList<IList<int>> NestedIntIList { get; set; }
                public IList<IList<string>> NestedStringIList { get; set; }
                public IList<IList<int[]>> NestedIListIntArray { get; set; }

                public IEnumerable<IEnumerable<int>> NestedIntIEnumerable { get; set; }
                public IEnumerable<IEnumerable<string>> NestedStringIEnumerable { get; set; }
                public IEnumerable<IEnumerable<int[]>> NestedIntArrayIEnumerable { get; set; }
            }

            public class EnumHolderEmpirical
            {
                public EnumInt8 EnumInt8 { get; set; }
                public EnumUInt8 EnumUInt8 { get; set; }
                public EnumInt16 EnumInt16 { get; set; }
                public EnumUInt16 EnumUInt16 { get; set; }
                public EnumInt32 EnumInt32 { get; set; }
                public EnumUInt32 EnumUInt32 { get; set; }
                public EnumInt64 EnumInt64 { get; set; }
                public EnumUInt64 EnumUInt64 { get; set; }
            }

            public class UnsupportedBsonAttributesEmpirical
            {
                public string Name { get; set; }
                public bool InStock { get; set; }

                [BsonRepresentation(BsonType.Double)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }
                public DateTime ExpiryDate;
                public int Length { get; set; }
                public int Width { get; set; }
                public TimeSpan SaleTime { get; set; }

                [BsonDictionaryOptions(Bson.Serialization.Options.DictionaryRepresentation.Document)]
                public Dictionary<string, string> DictionaryField;
            }

            public class NestedTypeHolderEmpirical
            {
                public Address Address { get; set; }
                public Person Person { get; set; }
                public Tree Tree { get; set; }
                public User User { get; set; }
                public Vehicle Vehicle { get; set; }

                public EnumInt16 EnumInt16 { get; set; }
                public EnumInt32 EnumInt32 { get; set; }
                public EnumInt64 EnumInt64 { get; set; }

                public Pair Pair { get; set; }

                public class NestedTypeEmpirical
                {
                    public int NestedInt { get; set; }
                    public double NestedDouble { get; set; }
                    public string NestedString { get; set; }

                    public class NestedNestedTypeEmpirical
                    {
                        public int NestedNestedInt { get; set; }
                        public double NestedNestedDouble { get; set; }
                        public string NestedNestedString { get; set; }
                    }
                }
            }

            public class NestedTypeHolderWithFieldsEmpirical
            {
                public Address Address;
                public Person Person;
                public Tree Tree;
                public User User;
                public Vehicle Vehicle;

                public EnumInt16 EnumInt16;
                public EnumInt32 EnumInt32;
                public EnumInt64 EnumInt64;

                public Pair Pair;

                public class NestedTypeWithFieldsEmpirical
                {
                    public int NestedInt;
                    public double NestedDouble;
                    public string NestedString;

                    public class NestedNestedTypeWithFieldsEmpirical
                    {
                        public int NestedNestedInt;
                        public double NestedNestedDouble;
                        public string NestedNestedString;
                    }
                }
            }

            public class SystemTypeContainerEmpirical
            {
                public DateTimeKind DateTimeKindField { get; set; }
                public DateTimeOffset DateTimeOffsetField { get; set; }
                public TimeSpan TimeSpanField { get; set; }
                public Type TypeField { get; set; }
            }

            public class AddressEmpiricalEmpirical
            {
                public string City { get; set; }
                public string Province { get; set; }
                public string ZipCode { get; set; }
            }

            public class AirportEmpiricalEmpirical
            {
                public string AirportName { get; set; }
                public string AirportCode { get; set; }
            }

            public class CarEmpiricalEmpirical
            {
                public string Make { get; set; }
                public string Model { get; set; }
                public int Year { get; set; }
            }

            public class CustomerEmpiricalEmpirical
            {
                public int CustomerID { get; set; }
                public string Name { get; set; }
                public string Address { get; set; }
            }

            public class PrimitiveTypeHolderEmpiricalEmpirical
            {
                public bool BooleanValue { get; set; }
                public byte ByteValue { get; set; }
                public sbyte SByteValue { get; set; }

                public short ShortValue { get; set; }
                public ushort UShortValue { get; set; }

                public int IntValue { get; set; }
                public uint UIntValue { get; set; }

                public long LongValue { get; set; }
                public ulong ULongValue { get; set; }

                public char CharValue { get; set; }
                public double DoubleValue { get; set; }

                public string StringValue { get; set; }
                public float FloatValue { get; set; }
            }

            public class SimpleTypesArraysHolderEmpiricalEmpirical
            {
                public bool[] BooleanArray { get; set; }
                public byte[] ByteArray { get; set; }
                public sbyte[] SByteArray { get; set; }

                public short[] ShortArray { get; set; }
                public ushort[] UShortArray { get; set; }

                public int[] IntArray { get; set; }
                public uint[] UIntArray { get; set; }

                public long[] LongArray { get; set; }
                public ulong[] ULongArray { get; set; }

                public char[] CharArray { get; set; }
                public double[] DoubleArray { get; set; }

                public string[] StringArray { get; set; }
                public float[] FloatArray { get; set; }
                public object[] ObjectArray { get; set; }

                public string[][] JaggedStringArray2 { get; set; }
                public int[][][] JaggedIntArray3 { get; set; }
                public long[][][][] JaggedLongArray4 { get; set; }
                public short[][][][][] JaggedShortArray5 { get; set; }
            }

            public class MultiDimensionalArrayHolderEmpiricalEmpirical
            {
                public int[,] Matrix2 { get; set; }
                public int[,,] Matrix3 { get; set; }
                public int[,][,] Matrix4 { get; set; }
            }

            public class CustomTypesArraysHolderEmpiricalEmpirical
            {
                public EnumInt16[] EnumArrayWithDimension1 { get; set; }
                public EnumInt16[][] JaggedEnumArray { get; set; }

                public Tree[][] TreeJaggedArray2 { get; set; }
                public TreeNode[][][] TreeNodeJaggedArray3 { get; set; }
            }

            public class NestedArrayHolderEmpiricalEmpirical
            {
                public int Data { get; set; }
                public NestedArrayHolder[] Children { get; set; }
            }

            public class HouseEmpiricalEmpirical
            {
                [BsonId]
                public string Identifier { get; set; }

                [BsonElement("year_built", Order = 2)]
                public int YearBuilt { get; set; }

                [BsonElement(Order = 1)]
                public string Style { get; set; }

                [BsonIgnore]
                public double Cost { get; set; }
            }

            public class ClothingEmpiricalEmpirical
            {
                [BsonConstructor("Name", "InStock", "Price")]
                public ClothingEmpiricalEmpirical(string Name, bool InStock, decimal Price)
                {
                    this.Name = Name;
                    this.InStock = InStock;
                    this.Price = Price;
                }

                [BsonFactoryMethod("Name", "InStock", "Price")]
                public void Factory_Method()
                {
                }

                [BsonIgnoreIfDefault]
                public string Name { get; set; }

                [BsonIgnoreIfNull]
                public bool InStock { get; set; }

                [BsonElement("price")]
                [BsonRepresentation(BsonType.Decimal128)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }

                [BsonDateTimeOptions(DateOnly = false, Kind = DateTimeKind.Unspecified, Representation = BsonType.DateTime)]
                public DateTime ExpiryDate;

                [BsonDefaultValue(defaultValue: 10)]
                public int Length { get; set; }

                [BsonRequired]
                public int Width { get; set; }

                [BsonTimeSpanOptions(representation: Bson.BsonType.String)]
                public TimeSpan SaleTime { get; set; }
            }

            [BsonIgnoreExtraElements]
            [BsonDiscriminatorAttribute("Carrot")]
            public class VegetableEmpiricalEmpirical
            {
                public double VegetableCost { get; set; }
            }

            [BsonNoId]
            public class ComputerEmpiricalEmpirical
            {
                public double ComputerCost { get; set; }

                [BsonExtraElementsAttribute]
                public BsonDocument CatchAll { get; set; }
            }

            public class ListsHolderEmpiricalEmpirical
            {
                public List<int> IntList { get; set; }
                public List<Person> PesonsList { get; set; }
                public System.Collections.Generic.List<string> StringList { get; set; }
                public System.Collections.Generic.List<ListsHolder> NestedListsHolderList { get; set; }

                public IList<int> IntIList { get; set; }
                public System.Collections.Generic.IList<ListsHolder> NestedListsHolderIList { get; set; }
            }

            public class EnumerableHolderEmpiricalEmpirical
            {
                public IEnumerable<int> Enumerable1 { get; set; }
                public System.Collections.Generic.IEnumerable<EnumerableHolder> Enumerable2 { get; set; }
            }

            public class NestedCollectionHolderEmpiricalEmpirical
            {
                public List<List<int>> NestedIntList { get; set; }
                public List<List<string>> NestedStringList { get; set; }
                public List<int[]> ListOfIntArray { get; set; }
                public List<string[]> ListOfStringArray { get; set; }
                public List<List<List<int>>> NestedNestedIntList { get; set; }

                public IList<IList<int>> NestedIntIList { get; set; }
                public IList<IList<string>> NestedStringIList { get; set; }
                public IList<IList<int[]>> NestedIListIntArray { get; set; }

                public IEnumerable<IEnumerable<int>> NestedIntIEnumerable { get; set; }
                public IEnumerable<IEnumerable<string>> NestedStringIEnumerable { get; set; }
                public IEnumerable<IEnumerable<int[]>> NestedIntArrayIEnumerable { get; set; }
            }

            public class EnumHolderEmpiricalEmpirical
            {
                public EnumInt8 EnumInt8 { get; set; }
                public EnumUInt8 EnumUInt8 { get; set; }
                public EnumInt16 EnumInt16 { get; set; }
                public EnumUInt16 EnumUInt16 { get; set; }
                public EnumInt32 EnumInt32 { get; set; }
                public EnumUInt32 EnumUInt32 { get; set; }
                public EnumInt64 EnumInt64 { get; set; }
                public EnumUInt64 EnumUInt64 { get; set; }
            }

            public class UnsupportedBsonAttributesEmpiricalEmpirical
            {
                public string Name { get; set; }
                public bool InStock { get; set; }

                [BsonRepresentation(BsonType.Double)]
                public decimal Price { get; set; }

                public Pair Pair { get; set; }
                public DateTime ExpiryDate;
                public int Length { get; set; }
                public int Width { get; set; }
                public TimeSpan SaleTime { get; set; }

                [BsonDictionaryOptions(Bson.Serialization.Options.DictionaryRepresentation.Document)]
                public Dictionary<string, string> DictionaryField;
            }

            public class NestedTypeHolderEmpiricalEmpirical
            {
                public Address Address { get; set; }
                public Person Person { get; set; }
                public Tree Tree { get; set; }
                public User User { get; set; }
                public Vehicle Vehicle { get; set; }

                public EnumInt16 EnumInt16 { get; set; }
                public EnumInt32 EnumInt32 { get; set; }
                public EnumInt64 EnumInt64 { get; set; }

                public Pair Pair { get; set; }

                public class NestedTypeEmpiricalEmpirical
                {
                    public int NestedInt { get; set; }
                    public double NestedDouble { get; set; }
                    public string NestedString { get; set; }

                    public class NestedNestedTypeEmpiricalEmpirical
                    {
                        public int NestedNestedInt { get; set; }
                        public double NestedNestedDouble { get; set; }
                        public string NestedNestedString { get; set; }
                    }
                }
            }

            public class NestedTypeHolderWithFieldsEmpiricalEmpirical
            {
                public Address Address;
                public Person Person;
                public Tree Tree;
                public User User;
                public Vehicle Vehicle;

                public EnumInt16 EnumInt16;
                public EnumInt32 EnumInt32;
                public EnumInt64 EnumInt64;

                public Pair Pair;

                public class NestedTypeWithFieldsEmpiricalEmpirical
                {
                    public int NestedInt;
                    public double NestedDouble;
                    public string NestedString;

                    public class NestedNestedTypeWithFieldsEmpiricalEmpirical
                    {
                        public int NestedNestedInt;
                        public double NestedNestedDouble;
                        public string NestedNestedString;
                    }
                }
            }

            public class SystemTypeContainerEmpiricalEmpirical
            {
                public DateTimeKind DateTimeKindField { get; set; }
                public DateTimeOffset DateTimeOffsetField { get; set; }
                public TimeSpan TimeSpanField { get; set; }
                public Type TypeField { get; set; }
            }
        }
    }
}

