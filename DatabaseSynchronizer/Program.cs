using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using DatabaseSynchronizer.Models;
using System.IO;

namespace Synchronizer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting bulk data...");
            var srcConnection = ConfigurationManager.ConnectionStrings["GWdb"].ConnectionString;
            var destConnection = ConfigurationManager.ConnectionStrings["OTBService"].ConnectionString;

            PopulateDatabase();
            PerformBulkCopyToVessels(srcConnection, destConnection);
            PerformBulkCopyToArrangements(srcConnection, destConnection);

            if (ConfigurationManager.AppSettings["IsDevelopment"].ToLower() == "true")
            {
                GenerateDemoData(destConnection);
            }
            Console.WriteLine("done");
            //Console.ReadLine();
        }

        private static void PopulateDatabase()
        {
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DBContext, DatabaseSynchronizer.Migrations.Configuration>());
            var db = new DBContext();
            db.Database.Initialize(true);
        }

        private static void PerformBulkCopyToVessels(string srcConnection, string destConnection)
        {
            using (SqlConnection source = new SqlConnection(srcConnection))
            {
                var cmd = @"SELECT ccarShipNameID, ccsnName FROM tblCCArrangement 
                            INNER JOIN tblCCShipName ON ccsnShipNameID = ccarShipNameID 
                            WHERE ccarIsActive=1 AND ccsnIsActive=1 
                            GROUP BY ccarShipNameID, ccsnName";
                SqlCommand myCommand = new SqlCommand(cmd, source);
                source.Open();

                SqlDataReader reader = myCommand.ExecuteReader();
                // open the destination data
                using (SqlConnection destination = new SqlConnection(destConnection))
                {
                    destination.Open();

                    // Empty the destination table. 
                    SqlCommand deleteHeader = new SqlCommand("DELETE FROM Vessels;", destination);
                    deleteHeader.ExecuteNonQuery();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destination.ConnectionString))
                    {
                        bulkCopy.ColumnMappings.Add("ccarShipNameID", "Id");
                        bulkCopy.ColumnMappings.Add("ccsnName", "Name");
                        bulkCopy.DestinationTableName = "Vessels";
                        bulkCopy.WriteToServer(reader);
                    }
                }
                reader.Close();
            }
        }

        private static void PerformBulkCopyToArrangements(string srcConnection, string destConnection)
        {
            using (SqlConnection source = new SqlConnection(srcConnection))
            {
                //                int days = 1825; // 5 years
                //                try { days = int.Parse(ConfigurationManager.AppSettings["LastDays"]); }
                //                catch (Exception) { }

                //                var cmd = String.Format(@"SELECT ccarArrangeID, ccarShipNameID, ccarETADate, ccarETDDate, ccarETATime, ccarETDTime FROM tblCCArrangement 
                //                            INNER JOIN dbo.tblCCShipName ON ccsnShipNameID = ccarShipNameID 
                //                            WHERE ccarIsActive=1 AND ccsnIsActive=1 AND DATEDIFF(day, ccarETADate, getdate()) between 0 and {0}", days);

                var cmd = String.Format(@"SELECT ccarArrangeID, ccarShipNameID, ccarETADate, ccarETDDate, ccarETATime, ccarETDTime FROM tblCCArrangement 
                            INNER JOIN dbo.tblCCShipName ON ccsnShipNameID = ccarShipNameID 
                            WHERE ccarIsActive=1 AND ccsnIsActive=1");

                SqlCommand myCommand = new SqlCommand(cmd, source);
                source.Open();
                SqlDataReader reader = myCommand.ExecuteReader();

                // open the destination data
                using (SqlConnection destination = new SqlConnection(destConnection))
                {
                    destination.Open();

                    // Empty the destination table. 
                    SqlCommand deleteHeader = new SqlCommand("DELETE FROM Arrangements;", destination);
                    deleteHeader.ExecuteNonQuery();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destination.ConnectionString))
                    {
                        bulkCopy.ColumnMappings.Add("ccarArrangeID", "Id");
                        bulkCopy.ColumnMappings.Add("ccarShipNameID", "VesselId");
                        bulkCopy.ColumnMappings.Add("ccarETADate", "ETADate");
                        bulkCopy.ColumnMappings.Add("ccarETDDate", "ETDDate");
                        bulkCopy.ColumnMappings.Add("ccarETATime", "ETATime");
                        bulkCopy.ColumnMappings.Add("ccarETDTime", "ETDTime");
                        bulkCopy.DestinationTableName = "Arrangements";
                        bulkCopy.WriteToServer(reader);
                    }
                }
                reader.Close();
            }
        }

        private static void GenerateDemoData(string connectionString)
        {
            try
            {
                Console.WriteLine("connectionString: " + connectionString);

                string fn = "demodata.sql";
                try { fn = ConfigurationManager.AppSettings["FakeData"]; }
                catch (Exception) { } 
                FileInfo file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + fn);
                Console.WriteLine("file: " + file);
                string script = file.OpenText().ReadToEnd();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand command = new SqlCommand(script, con);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                System.Threading.Thread.Sleep(5000);
                Console.ReadLine();
            }
        }
    }
}
