using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arhiva
{
    class Konekcija
    {
        public static string konekcionistring = "";

        public static bool testConnection()
        {
            try
            {
               using (var connection = new SqlConnection(konekcionistring))
                {
                    try
                    {
                        connection.Open();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                var a = e.Message;
                return false;
            }
         }

        public static void AddConnectionStrings(string csName, string connectionString)
        {

            // Get the configuration file.
            System.Configuration.Configuration config =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);

            // Add the connection string.
            ConnectionStringsSection csSection = config.ConnectionStrings;
            csSection.ConnectionStrings.Add(
                new ConnectionStringSettings(csName, connectionString));

            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
         public static void UpdateConnectionStrings(string csName, string connectionString)
        {
            // Get the configuration file
            System.Configuration.Configuration config =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);

            // Remove the existing connectionstring.
            config.ConnectionStrings.ConnectionStrings.Remove(csName);
            // Add the connectionstring
            ConnectionStringsSection csSection = config.ConnectionStrings;
            csSection.ConnectionStrings.Add(
                new ConnectionStringSettings(csName, connectionString));

            // Save the configuration file
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}
