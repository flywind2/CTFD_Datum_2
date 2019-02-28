using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Model.Base
{
    public class DataBaseOperation
    {
        public readonly string connectionString = string.Empty;

        public DataBaseOperation(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private DataSet Search(string commandString)
        {
            var result = default(DataSet);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString))
                {
                 
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(commandString, connection))
                    {
                        connection.Open();
                        adapter.Fill(result = new DataSet());
                    }
                }
            }
            catch { }
            return result;
        }

        public DataTable Search(string commandString,int tableIndex=0)
        {
            return this.Search(commandString).Tables[tableIndex];
        }

        public bool Execute(string commandString)
        {
            bool result = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.connectionString))
                {
                    using (MySqlCommand commd = new MySqlCommand(commandString, connection) { CommandType = CommandType.Text })
                    {
                        connection.Open();
                        commd.ExecuteNonQuery();
                    }
                }
                result = true;
            }
            catch { }
            return result;
        }
    }
}
