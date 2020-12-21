using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace NCCSAN.Source.Persistent
{
    class DBController
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["NCCSANConnectionString"].ConnectionString;

        public static SqlConnection getNewConnection()
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();

                return conn;
            }
            catch (SqlException e)
            {
                return null;
            }
        }


        public static SqlTransaction getNewTransaction(SqlConnection conn)
        {
            try
            {
                SqlTransaction trans = conn.BeginTransaction();

                return trans;
            }
            catch (SqlException e)
            {
                conn.Close();

                return null;
            }
        }


        public static SqlCommand getNewCommand(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;
                cmd.Transaction = trans;

                return cmd;
            }
            catch (SqlException e)
            {
                conn.Close();

                return null;
            }
        }


        public static bool addQuery(SqlCommand cmd)
        {
            try
            {
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }


        public static bool doCommit(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                trans.Commit();
                conn.Close();

                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }


        public static void doRollback(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                trans.Rollback();
                conn.Close();
            }
            catch (SqlException e)
            {
            }
        }
    }
}