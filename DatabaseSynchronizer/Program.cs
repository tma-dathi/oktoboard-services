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

            Console.WriteLine("done");
            //Console.ReadLine();
        }

        private static void PopulateDatabase()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DBContext, DatabaseSynchronizer.Migrations.Configuration>());
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
                var cmd = @"SELECT ccarArrangeID, ccarShipNameID, ccarETADate, ccarETDDate FROM tblCCArrangement 
                            INNER JOIN dbo.tblCCShipName ON ccsnShipNameID = ccarShipNameID 
                            WHERE ccarIsActive=1 AND ccsnIsActive=1";
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
                        bulkCopy.DestinationTableName = "Arrangements";
                        bulkCopy.WriteToServer(reader);
                    }
                }
                reader.Close();
            }
        }

        // NOT IN USE ANYMORE
        private static List<string> GetListId(SqlDataReader reader)
        {
            List<string> listIDs = new List<string>();

            // write each record
            while (reader.Read())
            {
                listIDs.Add(reader["Id"].ToString());
            }

            return listIDs;
        }

        // NOT IN USE ANYMORE
        private static string PopulateStringNotIn(SqlDataReader reader)
        {
            var listIDs = GetListId(reader);

            StringBuilder result = new StringBuilder();
            if (listIDs.Count > 0)
            {
                result = result.Append("(");
                foreach (var id in listIDs)
                {
                    result = result.Append(String.Format("'{0}'", id));
                    result = result.Append(", ");
                }
                result = result.Append(")");
                result = result.Replace(", )", ")");
            }
            return result.ToString();
        }

    }
}
