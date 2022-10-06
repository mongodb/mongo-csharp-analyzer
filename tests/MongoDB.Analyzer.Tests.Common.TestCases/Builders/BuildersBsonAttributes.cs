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
    public sealed class BuildersBsonAttributes : TestCasesBase
    {
        [BuildersMQL("{ \"Name\" : university.Name }")]
        public void Bson_Default()
        {
            var university = new University();
            _ = Builders<University>.Filter.Eq(u => u.Name, university.Name);
        }

        [BuildersMQL("{ \"StudentAge\" : student.Age }")]
        public void Bson_Element()
        {
            var student = new Student("John");
            _ = Builders<Student>.Filter.Eq(s => s.Age, student.Age);
        }

        [NoDiagnostics]
        public void Bson_Extra_Elements()
        {
            var student = new Student("John");
            _ = Builders<Student>.Filter.Eq(s => s.Grades, student.Grades);
        }

        [BuildersMQL("{ \"_id\" : university.NumberOfStudents }")]
        public void Bson_Id()
        {
            var university = new University();
            _ = Builders<University>.Filter.Eq(u => u.NumberOfStudents, university.NumberOfStudents);
        }

        [NotSupportedBuilders("Unable to determine the serialization information for u => u.Ranking.")]
        public void Bson_Ignore()
        {
            var university = new University();
            university.Ranking = 1;

            _ = Builders<University>.Filter.Eq(u => u.Ranking, university.Ranking);
        }

        [BuildersMQL("{ \"StudentName\" : undergraduateStudent.StudentName }")]
        public void Bson_Ignore_Extra_Elements()
        {
            var undergraduateStudent = new UndergraduateStudent("John");
            _ = Builders<UndergraduateStudent>.Filter.Eq(u => u.StudentName, undergraduateStudent.StudentName);
        }
        [BuildersMQL("{ \"Height\" : student.Height }")]
        public void Bson_Ignore_If_Default()
        {
            var student = new Student("John");
            student.Height = 0;
            _ = Builders<Student>.Filter.Eq(s => s.Height, student.Height);
        }

        [NoDiagnostics]
        public void Bson_Ignore_If_Null()
        {
            var student = new Student("John");
            _ = Builders<Student>.Filter.Eq(s => s.EnrolledCourse, student.EnrolledCourse);
        }

        [BuildersMQL("{ \"Name\" : student.Name }")]
        [BuildersMQL("{ \"StudentName\" : undergraduateStudent.StudentName }")]
        [BuildersMQL("{ \"StudentName\" : graduateStudent.StudentName }")]
        public void Bson_Known_Types()
        {
            var student = new Student("John");
            var undergraduateStudent = new UndergraduateStudent("Bob");
            var graduateStudent = new GraduateStudent("Joe");

            _ = Builders<Student>.Filter.Eq(s => s.Name, student.Name);
            _ = Builders<UndergraduateStudent>.Filter.Eq(u => u.StudentName, undergraduateStudent.StudentName);
            _ = Builders<GraduateStudent>.Filter.Eq(g => g.StudentName, graduateStudent.StudentName);
        }

        [BuildersMQL("{ \"Name\" : 1, \"_id\" : 0 }")]
        public void Bson_NoId()
        {
            _ = Builders<Course>.Projection.Expression(c => c.Name);
        }

        [BuildersMQL("{ \"CostOfAttendance\" : university.CostOfAttendance }")]
        public void Bson_Representation()
        {
            var university = new University();
            _ = Builders<University>.Filter.Eq(u => u.CostOfAttendance, university.CostOfAttendance);
        }

        [BuildersMQL("{ \"Name\" : student.Name }")]
        public void Bson_Required()
        {
            var student = new Student("John");
            _ = Builders<Student>.Filter.Eq(s => s.Name, student.Name);
        }

        [BuildersMQL("{ \"StudentName\" : graduateStudent.StudentName }")]
        public void Bson_Serializer_Attribute()
        {
            var graduateStudent = new GraduateStudent("John");
            _ = Builders<GraduateStudent>.Filter.Eq(g => g.StudentName, graduateStudent.StudentName);
        }
    }
}

