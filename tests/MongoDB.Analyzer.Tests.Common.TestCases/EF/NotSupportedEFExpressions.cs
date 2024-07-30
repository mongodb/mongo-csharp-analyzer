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
    [NoDiagnostics]
    public void Arithmetic()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        _ = db.Users.Sum(u => u.Age);
        _ = db.Users.Average(u => u.Age);
        _ = db.Users.Min(u => u.Age);
        _ = db.Users.Max(u => u.Age);
    }

    [NotSupportedEF("Byte Array Property Not Supported.", DriverVersions.Linq3OrGreater)]
    public void ByteArrayProperties()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        _ = db.SimpleTypesArraysHolder.Where(s => s.ByteArray.Length == 1);
    }

    [NoDiagnostics]
    public void CombinedUnsupportedMethods()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        _ = db.Users.Include(u => u.Age).GroupBy(u => u.Address);
    }

    [NotSupportedEF("GroupBy Not Supported in EF.", DriverVersions.Linq3OrGreater)]
    [NotSupportedEF("GroupBy Not Supported in EF.", DriverVersions.Linq3OrGreater)]
    public void GroupBy()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        var users_query = db.Users.GroupBy(u => u.Address);
        var customers_query = db.Customers.GroupBy(c => c.LastName);
    }

    [NoDiagnostics]
    public void Includes()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);
        _ = db.Users.Include(u => u.Age);
    }

    [NoDiagnostics]
    public void Join()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DbContextUnsupportedEFExpressions>();
        var db = new DbContextUnsupportedEFExpressions(dbContextOptions.Options);

        _ = db.Users
                .Join(
                    db.Customers,
                    user => user.Age,
                    customer => customer.CustomerId,
                    (user, customer) => new
                        {
                            Age = user.Age,
                            ID = customer.CustomerId
                        }
                    );
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