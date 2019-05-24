using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;

namespace ANSqlBuilder
{
    public class DbHelper:IDisposable
    {
        protected string _ConnectionStringName;
        protected string _ConnectionString;
        protected string _ProviderName;
        protected DbProviderFactory _ProviderFactory; 

        #region Constructor

        public DbHelper(string connection_string_name)
        {
            _ConnectionStringName = connection_string_name ?? "SqlServer";
            _ConnectionString =
                System.Configuration.ConfigurationManager.ConnectionStrings
                [_ConnectionStringName].ConnectionString;

            _ProviderName =
                System.Configuration.ConfigurationManager.ConnectionStrings
                [_ConnectionStringName].ProviderName;

            _ProviderFactory = DbProviderFactories.GetFactory(_ProviderName);

        }

        #endregion
        
        #region Methods

        public void Dispose()
        { }

        public DbDataReader ExecuteReader(string command_text)
        {
            DbConnection conn = _ProviderFactory.CreateConnection();
            conn.ConnectionString = _ConnectionString;
            
            DbCommand command = conn.CreateCommand();
            command.CommandText = command_text;
            command.CommandType = CommandType.Text;
            LogQuery(command_text);
            conn.Open();

            DbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;
        }

        public void ExecuteReaders(List<string> command_texts, Func<IDataReader,int,bool> reader_handler )
        {
            using (DbConnection conn = _ProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _ConnectionString;
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandType = CommandType.Text;
                for (var x = 0; x < command_texts.Count; x++ )
                {
                    command.CommandText = command_texts[x];
                    var dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                    reader_handler(dr, x);
                }
                conn.Close();
            }
        }

        public int ExecuteNonQuery(string command_text)
        {
            int result = 0;
            using (DbConnection conn = _ProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _ConnectionString;
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = command_text;
                command.CommandType = CommandType.Text;

                result = command.ExecuteNonQuery();
                conn.Close();
            }
            return result;
        }

        public object ExecuteScalar(string command_text)
        {
            object result = null;
            using (DbConnection conn = _ProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _ConnectionString;
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = command_text;
                command.CommandType = CommandType.Text;
               
                result = command.ExecuteScalar();
                conn.Close();
            }
            return result;
        }

        public object ExecuteNonQueryAndScalar(string nonquery_command_text, string scalar_command_text)
        {
            object result = null;
            using (DbConnection conn = _ProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _ConnectionString;
                conn.Open();

                DbCommand command = conn.CreateCommand();
                command.CommandText = nonquery_command_text;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                command.CommandText = scalar_command_text;
                result = command.ExecuteScalar();
                conn.Close();
            }
            return result;
        }

        public DataSet ExecuteDataSet(string command_text)
        {
            return ExecuteDataSet(command_text, "Table1");
        }

        public DataSet ExecuteDataSet(string command_text,string table_name)
        {
            DataSet dataSet = new DataSet();
            using (DbConnection conn = _ProviderFactory.CreateConnection())
            {
                DbDataAdapter adapter = _ProviderFactory.CreateDataAdapter();

                conn.ConnectionString = _ConnectionString;
                conn.Open();

                DbCommand command = conn.CreateCommand();
                command.CommandText = command_text;
                command.CommandType = CommandType.Text;

                LogQuery(command_text);

                adapter.SelectCommand = command;

                adapter.Fill(dataSet,table_name);
                conn.Close();
            }
            return dataSet;
        }

        public DataSet ExecuteDataSet(string command_text,string table_name,int start_record,int max_records)
        {
            DataSet dataSet = new DataSet();
            using (DbConnection conn = _ProviderFactory.CreateConnection())
            {

                DbDataAdapter adapter = _ProviderFactory.CreateDataAdapter();

                adapter.SelectCommand.CommandText = command_text;
                adapter.SelectCommand.CommandType = CommandType.Text;
                adapter.SelectCommand.Connection = conn;

                LogQuery(command_text);


                conn.ConnectionString = _ConnectionString;
                conn.Open();
                adapter.Fill(dataSet,start_record,max_records,table_name);
                conn.Close();
            }
            return dataSet;
        }
        #endregion

        private void LogQuery(string qry)
        {
            //var sw = new System.IO.StreamWriter(@"c:\dev\sqllog.txt", true);
            //sw.WriteLine(qry);
            //sw.Close();
        }

    }
}
