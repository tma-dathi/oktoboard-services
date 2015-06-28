using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Synchronizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting bulk data...");
            var srcConnection = ConfigurationManager.ConnectionStrings["GWdb"].ConnectionString;
            var destConnection = ConfigurationManager.ConnectionStrings["OTBService"].ConnectionString;

            PerformBulkCopyToVessels(srcConnection, destConnection);
            PerformBulkCopyToArrangements(srcConnection, destConnection);

            Console.WriteLine("done");
            Console.ReadLine();
        }
        
        private static void PerformBulkCopyToVessels(string srcConnection, string destConnection)
        {
            string not_in = String.Empty;
            using (SqlConnection destination = new SqlConnection(destConnection))
            {
                destination.Open();
                SqlCommand destGetListIDs = new SqlCommand("SELECT Id FROM Vessels", destination);
                SqlDataReader reader = destGetListIDs.ExecuteReader();
                not_in = PopulateStringNotIn(reader);
            }

            using (SqlConnection source = new SqlConnection(srcConnection))
            {
                var cmd = String.Format("SELECT ccsnShipNameID, ccsnName FROM tblCCShipName");
                if (!String.IsNullOrEmpty(not_in)) 
                {
                    cmd = String.Format("SELECT ccsnShipNameID, ccsnName FROM tblCCShipName WHERE ccsnShipNameID NOT IN {0}", not_in);
                }
                SqlCommand myCommand = new SqlCommand(cmd, source);
                source.Open();
                SqlDataReader reader = myCommand.ExecuteReader();
                // open the destination data
                using (SqlConnection destination = new SqlConnection(destConnection))
                {
                    destination.Open();

                    // Perform an initial count on the destination table.
                    SqlCommand commandRowCount = new SqlCommand("SELECT COUNT(*) FROM Vessels", destination);
                    long countStart = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
                    Console.WriteLine("Starting row count = {0}", countStart);

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destination.ConnectionString))
                    {
                        bulkCopy.ColumnMappings.Add("ccsnShipNameID", "Id");
                        bulkCopy.ColumnMappings.Add("ccsnName", "Name");
                        bulkCopy.DestinationTableName = "Vessels";
                        bulkCopy.WriteToServer(reader);
                    }

                    // Perform a final count on the destination 
                    // table to see how many rows were added.
                    long countEnd = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
                    Console.WriteLine("Ending row count = {0}", countEnd);
                    Console.WriteLine("{0} rows were added.", countEnd - countStart);
                }
                reader.Close();
            }           
        }

        private static void PerformBulkCopyToArrangements(string srcConnection, string destConnection)
        {
            string not_in = String.Empty;
            using (SqlConnection destination = new SqlConnection(destConnection))
            {
                destination.Open();
                SqlCommand destGetListIDs = new SqlCommand("SELECT Id FROM Arrangements", destination);
                SqlDataReader reader = destGetListIDs.ExecuteReader();
                not_in = PopulateStringNotIn(reader);
            }

            using (SqlConnection source = new SqlConnection(srcConnection))
            {
                var cmd = String.Format("SELECT ccarArrangeID, ccarShipNameID, ccarETADate FROM tblCCArrangement");
                if (!String.IsNullOrEmpty(not_in))
                {
                    cmd = String.Format("SELECT ccarArrangeID, ccarShipNameID, ccarETADate FROM tblCCArrangement WHERE ccarArrangeID NOT IN {0}", not_in);
                }
                SqlCommand myCommand = new SqlCommand(cmd, source);
                source.Open();
                SqlDataReader reader = myCommand.ExecuteReader();
                // open the destination data
                using (SqlConnection destination = new SqlConnection(destConnection))
                {
                    destination.Open();

                    // Perform an initial count on the destination table.
                    SqlCommand commandRowCount = new SqlCommand("SELECT COUNT(*) FROM Arrangements", destination);
                    long countStart = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
                    Console.WriteLine("Starting row count = {0}", countStart);

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destination.ConnectionString))
                    {
                        bulkCopy.ColumnMappings.Add("ccarArrangeID", "Id");
                        bulkCopy.ColumnMappings.Add("ccarShipNameID", "VesselId");
                        bulkCopy.ColumnMappings.Add("ccarETADate", "ETADate");
                        bulkCopy.DestinationTableName = "Arrangements";
                        bulkCopy.WriteToServer(reader);
                    }

                    // Perform a final count on the destination 
                    // table to see how many rows were added.
                    long countEnd = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
                    Console.WriteLine("Ending row count = {0}", countEnd);
                    Console.WriteLine("{0} rows were added.", countEnd - countStart);
                }
                reader.Close();
            }
        }

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
