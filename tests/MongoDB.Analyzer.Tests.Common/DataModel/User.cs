namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public class User
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        public int Age { get; set; }
        public int Height { get; set; }

        public int[] Scores { get; set; }
    }
}
