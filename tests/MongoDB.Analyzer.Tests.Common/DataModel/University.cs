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
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public class University
    {
        [BsonDefaultValue("Princeton University")]
        public string Name { get; set; }

        [BsonIgnore]
        public int Ranking { get; set; }

        [BsonId]
        public int NumberOfStudents { get; set; }

        public Course[] Courses { get; set; }

        public Student[] Students { get; set; }

        [BsonRepresentation(Bson.BsonType.Double)]
        public int CostOfAttendance { get; set; }
    }

    [BsonNoId]
    public class Course
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }

    [BsonKnownTypes(typeof(UndergraduateStudent), typeof(GraduateStudent))]
    public class Student
    {
        [BsonConstructor]
        public Student(string name)
        {
            Name = name;
        }

        [BsonRequired]
        public string Name { get; set; }

        [BsonElement("StudentAge")]
        public int Age { get; set; }

        [BsonExtraElements]
        public IDictionary<string, object> Grades { get; set; }

        [BsonIgnoreIfDefault]
        public int Height { get; set; }

        [BsonIgnoreIfNull]
        public Course EnrolledCourse { get; set; }
    }

    [BsonIgnoreExtraElements]
    public sealed class UndergraduateStudent : Student
    {
        public UndergraduateStudent(string name) : base(name)
        {
        }

        [BsonRequired]
        public string StudentName { get; set; }
    }

    [BsonSerializer(typeof(Student))]
    public sealed class GraduateStudent : Student
    {
        public GraduateStudent(string name) : base(name)
        {
        }

        [BsonRequired]
        public string StudentName { get; set; }
    }
}

