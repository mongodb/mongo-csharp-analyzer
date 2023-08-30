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

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public int TrackingNumber { get; set; }
        public int ConfirmationNumber { get; set; }
        public Customer Customer { get; set; }
        public string Date { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
    }
}

