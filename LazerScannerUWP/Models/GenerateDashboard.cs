using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerScannerUWP.Models
{
    public class GenerateDashboard
    {
        public static int GetUniqueItemCount(string uid)
        {
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                string oString = $"SELECT (SELECT COUNT(*) FROM Items WHERE userId = '{uid}')";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        var dbnull = oReader.GetValue(0);
                        if (dbnull == DBNull.Value)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)oReader.GetValue(0);
                        }
                    }
                }
                return 0;
            }
        }
        public static int GetShoppingListCount(string uid)
        {
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                string oString = $"SELECT (SELECT COUNT(*) FROM ShoppingList WHERE userId = '{uid}')";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        var dbnull = oReader.GetValue(0);
                        if (dbnull == DBNull.Value)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)oReader.GetValue(0);
                        }
                    }
                }
                return 0;
            }
        }
        public static int GetInventoryListCount(string uid)
        {
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                string oString = $"SELECT(SELECT SUM(quantity) FROM Items WHERE userId = '{uid}')";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        var dbnull = oReader.GetValue(0);
                        if (dbnull == DBNull.Value)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)oReader.GetValue(0);
                        }
                    }
                }
                return 0;
            }
        }

    }
}
