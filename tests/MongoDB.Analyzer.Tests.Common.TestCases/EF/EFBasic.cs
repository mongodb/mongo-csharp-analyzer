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

public sealed class EFBasic
{
    public MyDbContext GetDbContext() => new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().Options);
    public DbContextOptions GetDbContextOptions() => new DbContextOptionsBuilder<MyDbContext>().Options;
    public DbSet<Customer> GetDbSet_Customers() => new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().Options).Customers;
    public DbSet<User> GetDbSet_Users() => new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().Options).Users;


    [MQLEF("db.coll.Aggregate([{ \"$group\" : { \"_id\" : \"$Address\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$group\" : { \"_id\" : \"$LastName\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])", DriverVersions.Linq3OrGreater)]
    public void GroupBy()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.GroupBy(u => u.Address);
        var customers_query = db.Customers.GroupBy(c => c.LastName);
    }

    [MQLEF("db.coll.Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$group\" : { \"_id\" : \"$LastName\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$sort\" : { \"Age\" : 1, \"Height\" : 1 } }])", DriverVersions.Linq3OrGreater)]
    public void Method()
    {
        var users_query = GetDbSet_Users().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
        var customers_query = GetDbSet_Customers().GroupBy(c => c.LastName);
        _ = GetDbContext().Users.OrderBy(u => u.Age).ThenBy(u => u.Height);
    }

    [MQLEF("db.coll.Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$group\" : { \"_id\" : \"$LastName\", \"_elements\" : { \"$push\" : \"$$ROOT\" } } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$sort\" : { \"Age\" : 1, \"Height\" : 1 } }])", DriverVersions.Linq3OrGreater)]
    public void Object_invocation()
    {
        var users_query = new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().Options).Users.Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
        var customers_query = new MyDbContext(new DbContextOptionsBuilder<MyDbContext>().Options).Customers.GroupBy(c => c.LastName);
        _ = new MyDbContext(GetDbContextOptions()).Users.OrderBy(u => u.Age).ThenBy(u => u.Height);
    }

    [MQLEF("db.coll.Aggregate([{ \"$sort\" : { \"Age\" : 1 } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$sort\" : { \"DateOfBirth\" : 1 } }])", DriverVersions.Linq3OrGreater)]
    public void OrderBy()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.OrderBy(u => u.Age);
        var customers_query = db.Customers.OrderBy(c => c.DateOfBirth);
    }

    [MQLEF("db.coll.Aggregate([{ \"$match\" : { \"Age\" : { \"$lte\" : 21 } } }, { \"$project\" : { \"_v\" : \"$Address\", \"_id\" : 0 } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$match\" : { \"Name\" : \"Bob\" } }, { \"$project\" : { \"_v\" : \"$LastName\", \"_id\" : 0 } }])", DriverVersions.Linq3OrGreater)]
    public void Select()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.Where(u => u.Age <= 21).Select(u => u.Address);
        var customers_query = db.Customers.Where(c => c.Name == "Bob").Select(c => c.LastName);
    }

    [MQLEF("db.coll.Aggregate([{ \"$project\" : { \"_v\" : \"$Scores\", \"_id\" : 0 } }, { \"$unwind\" : \"$_v\" }])", DriverVersions.Linq3OrGreater)]
    public void SelectMany()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.SelectMany(u => u.Scores);
    }

    [MQLEF("db.coll.Aggregate([{ \"$sort\" : { \"Age\" : 1, \"Height\" : 1 } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$sort\" : { \"Age\" : 1, \"Height\" : -1 } }])", DriverVersions.Linq3OrGreater)]
    public void ThenBy()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.OrderBy(u => u.Age).ThenBy(u => u.Height);
        var users_query2 = db.Users.OrderBy(u => u.Age).ThenByDescending(u => u.Height);
    }

    [MQLEF("db.coll.Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])", DriverVersions.Linq3OrGreater)]
    [MQLEF("db.coll.Aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"CustomerId\" : 21 } }])", DriverVersions.Linq3OrGreater)]
    public void Where()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
        var customers_query = db.Customers.Where(c => c.Name == "Bob" & c.CustomerId == 21);
    }
}

public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public MyDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}