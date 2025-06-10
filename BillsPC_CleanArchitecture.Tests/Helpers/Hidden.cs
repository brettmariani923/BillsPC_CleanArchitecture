namespace Final_BillsPC.Data.Tests.Helpers
{
    //add to git ignore
    internal static class Hidden
    {
        public const string ConnectionString = "Server=Brett;Database=BillsPC_CleanArchitecture;Trusted_Connection=True;Trust Server Certificate=True;";

        public static IEnumerable<object[]> ConnectionStrings =>
            new List<object[]>
            {
                new object[] { ConnectionString } 
            };
    }

}
