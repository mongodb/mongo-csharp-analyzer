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

namespace MongoDB.Analyzer.Tests.Common.TestCases.EF;

public sealed class EFBasic
{
    [MQLEF("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }])")]
    [MQLEF("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"CustomerId\" : 21 } }])")]
    public void EF_Where_Queries()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
        var customers_query = db.Customers.Where(c => c.Name == "Bob" & c.CustomerId == 21);
    }

    [MQLEF("aggregate([{ \"$sort\" : { \"Age\" : 1 } }])")]
    [MQLEF("aggregate([{ \"$sort\" : { \"DateOfBirth\" : 1 } }])")]
    public void EF_OrderBy_Queries()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.OrderBy(u => u.Age);
        var customers_query = db.Customers.OrderBy(c => c.DateOfBirth);
    }

    [MQLEF("aggregate([{ \"$match\" : { \"Age\" : { \"$lte\" : 21 } } }, { \"$project\" : { \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
    [MQLEF("aggregate([{ \"$match\" : { \"Name\" : \"Bob\" } }, { \"$project\" : { \"LastName\" : \"$LastName\", \"_id\" : 0 } }])")]
    public void EF_Select_Queries()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.Where(u => u.Age <= 21).Select(u => u.Address);
        var customers_query = db.Customers.Where(c => c.Name == "Bob").Select(c => c.LastName);
    }

    [MQLEF("aggregate([{ \"$group\" : { \"_id\" : \"$Address\" } }])")]
    [MQLEF("aggregate([{ \"$group\" : { \"_id\" : \"$LastName\" } }])")]
    public void EF_GroupBy_Queries()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.GroupBy(u => u.Address);
        var customers_query = db.Customers.GroupBy(c => c.LastName);
    }

    [MQLEF("aggregate([{ \"$unwind\" : \"$Scores\" }, { \"$project\" : { \"Scores\" : \"$Scores\", \"_id\" : 0 } }])")]
    public void EF_SelectMany_Queries()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.SelectMany(u => u.Scores);
    }

    [MQLEF("aggregate([{ \"$sort\" : { \"Age\" : 1, \"Height\" : 1 } }])")]
    [MQLEF("aggregate([{ \"$sort\" : { \"Age\" : 1, \"Height\" : -1 } }])")]
    public void EF_ThenBy_Queries()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MyDbContext>();
        var db = new MyDbContext(dbContextOptions.Options);
        var users_query = db.Users.OrderBy(u => u.Age).ThenBy(u => u.Height);
        var users_query2 = db.Users.OrderBy(u => u.Age).ThenByDescending(u => u.Height);
    }
}

internal class MyDbContext : DbContext
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