namespace WebPageTestAutomation.Core.Enumerators
{
    public enum Mobile
    {
        MotoG4
    }

    public static class ExtensionsMobile
    {
        public static string GetString(this Mobile connection)
        {
            if (connection == Mobile.MotoG4)
                return "Moto G4 - Chrome";
            return connection.ToString();
        }
    }
}