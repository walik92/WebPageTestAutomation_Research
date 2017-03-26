namespace WebPageTestAutomation.Core.Enumerators
{
    public enum Connection
    {
        Cable,
        DSL,
        ThreeG
    }

    public static class Extensions
    {
        public static string GetString(this Connection connection)
        {
            if (connection == Connection.ThreeG)
                return "3G";
            return connection.ToString();
        }
    }
}