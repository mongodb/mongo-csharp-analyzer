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

using System.Linq;
using Microsoft.EntityFrameworkCore;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.EF;

public sealed class NotSupportedEFExpressions : TestCasesBase
{
    [NotSupportedEF("Byte array type is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("Byte array type is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("Byte array type is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("Byte array type is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    public void ByteArrayProperties()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        _ = db.SimpleTypesArraysHolder.Where(s => s.ByteArray.Length == 1);
        _ = db.SimpleTypesArraysHolder.Where(s => s.IntArray.Length == 1).Where(s => s.IntArray.Length == 1).Where(s => s.ByteArray.Length == 1);
        _ = db.SimpleTypesArraysHolder.Where(s => s.IntArray.Length == 1).Where(s => s.ByteArray.Length == 1).Where(s => s.IntArray.Length == 1);
        _ = db.SimpleTypesArraysHolder.Where(s => s.ByteArray.Length == 1).Where(s => s.IntArray.Length == 1).Where(s => s.IntArray.Length == 1);
    }

    [NotSupportedEF("GroupBy is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("GroupBy is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("GroupBy is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("GroupBy is not supported by this version of the EF provider.", DriverVersions.Linq3OrGreater)]
    public void GroupBy()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        var users_query = db.Users.GroupBy(u => u.Address);
        var customers_query = db.Customers.GroupBy(c => c.LastName);
        _ = db.Users.Where(u => u.Age == 21).GroupBy(u => u.Address);
        _ = db.Users.OrderBy(u => u.Age).ThenBy(u => u.Height).GroupBy(u => u.Address);
    }
}

public class DbContextUnsupportedEFExpressions : DbContext
{
    public DbSet<SimpleTypesArraysHolder> SimpleTypesArraysHolder { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbContextUnsupportedEFExpressions(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}