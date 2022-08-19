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
using System.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqBsonAttributes : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : university.Name } }])")]
        public void Bson_Default()
        {
            var university = new University();
            _ = GetMongoQueryable<University>().Where(u => u.Name == university.Name);
        }

        [MQL("aggregate([{ \"$match\" : { \"StudentAge\" : student.Age } }])")]
        public void Bson_Element()
        {
            var student = new Student("John");
            _ = GetMongoQueryable<Student>().Where(s => s.Age == student.Age);
        }

        [NoDiagnostics]
        public void Bson_Extra_Elements()
        {
            var student = new Student("John");
            _ = GetMongoQueryable<Student>().Where(s => s.Grades == student.Grades);
        }

        [MQL("aggregate([{ \"$match\" : { \"_id\" : university.NumberOfStudents } }])")]
        public void Bson_Id()
        {
            var university = new University();
            _ = GetMongoQueryable<University>().Where(u => u.NumberOfStudents == university.NumberOfStudents);
        }

        [InvalidLinq("{document}.Ranking is not supported.")]
        public void Bson_Ignore()
        {
            var university = new University();
            university.Ranking = 1;
            _ = GetMongoQueryable<University>().Where(u => u.Ranking == university.Ranking);
        }

        [MQL("aggregate([{ \"$match\" : { \"StudentName\" : undergraduateStudent.StudentName } }])")]
        public void Bson_Ignore_Extra_Elements()
        {
            var undergraduateStudent = new UndergraduateStudent("John");
            _ = GetMongoQueryable<UndergraduateStudent>().Where(u => u.StudentName == undergraduateStudent.StudentName);
        }
        [MQL("aggregate([{ \"$match\" : { \"Height\" : student.Height } }])")]
        public void Bson_Ignore_If_Default()
        {
            var student = new Student("John");
            student.Height = 0;
            _ = GetMongoQueryable<Student>().Where(s => s.Height == student.Height);
        }

        [NoDiagnostics]
        public void Bson_Ignore_If_Null()
        {
            var student = new Student("John");
            _ = GetMongoQueryable<Student>().Where(s => s.EnrolledCourse == student.EnrolledCourse);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : student.Name } }])")]
        [MQL("aggregate([{ \"$match\" : { \"StudentName\" : undergraduateStudent.StudentName } }])")]
        [MQL("aggregate([{ \"$match\" : { \"StudentName\" : graduateStudent.StudentName } }])")]
        public void Bson_Known_Types()
        {
            var student = new Student("John");
            var undergraduateStudent = new UndergraduateStudent("Bob");
            var graduateStudent = new GraduateStudent("Joe");

            _ = GetMongoQueryable<Student>().Where(s => s.Name == student.Name);
            _ = GetMongoQueryable<UndergraduateStudent>().Where(u => u.StudentName == undergraduateStudent.StudentName);
            _ = GetMongoQueryable<GraduateStudent>().Where(g => g.StudentName == graduateStudent.StudentName);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Computer Science\" } }])")]
        public void Bson_NoId()
        {
            _ = GetMongoQueryable<Course>().Where(c => c.Name == "Computer Science");
        }

        [MQL("aggregate([{ \"$match\" : { \"CostOfAttendance\" : university.CostOfAttendance } }])")]
        public void Bson_Representation()
        {
            var university = new University();
            _ = GetMongoQueryable<University>().Where(u => u.CostOfAttendance == university.CostOfAttendance);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : student.Name } }])")]
        public void Bson_Required()
        {
            var student = new Student("John");
            _ = GetMongoQueryable<Student>().Where(s => s.Name == student.Name);
        }

        [MQL("aggregate([{ \"$match\" : { \"StudentName\" : graduateStudent.StudentName } }])")]
        public void Bson_Serializer_Attribute()
        {
            var graduateStudent = new GraduateStudent("John");
            _ = GetMongoQueryable<GraduateStudent>().Where(g => g.StudentName == graduateStudent.StudentName);
        }
    }
}

