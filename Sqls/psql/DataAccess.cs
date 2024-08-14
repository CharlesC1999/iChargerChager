using System;
using System.Collections;
using System.Data;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace backend.Sqls.psql
{
    public class DataAccess
    {
        public DataTable GetDataTable(NpgsqlConnection conn, string sql)
        {
            DataTable Result = new DataTable();
            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.CommandTimeout = 0;          
                NpgsqlDataReader Reader = cmd.ExecuteReader(CommandBehavior.Default);
                Result.Load(Reader);
                Reader.Close();
                return Result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable GetDataTable(NpgsqlConnection conn, string sql, Hashtable ht)
        {
            DataTable Result = new DataTable();
            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                SQLParameter parameters;
                cmd.CommandTimeout = 0;
                if (ht != null)
                {
                    foreach (object obj in ht.Keys)
                    {
                        parameters = ht[obj] as SQLParameter;
                        if (parameters.SqlDbType == NpgsqlDbType.Text) 
                        {
                            int l_len = Encoding.UTF8.GetByteCount(parameters.ObjValue.ToString())+3;
                            cmd.Parameters.Add(obj.ToString(), parameters.SqlDbType, l_len).Value = parameters.ObjValue;
                        }
                        else cmd.Parameters.Add(obj.ToString(), parameters.SqlDbType).Value = parameters.ObjValue;
                    }
                }
                NpgsqlDataReader Reader = cmd.ExecuteReader(CommandBehavior.Default);
                Result.Load(Reader);
                Reader.Close();
                return Result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public void Execute(NpgsqlConnection conn, string sql)
        {
            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public void Execute(NpgsqlConnection conn, string sql, Hashtable ht)
        {
            try
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                SQLParameter parameters;
                cmd.CommandTimeout = 0;
                if (ht != null)
                {
                    foreach (object obj in ht.Keys)
                    {
                        parameters = ht[obj] as SQLParameter;
                        if (parameters.SqlDbType == NpgsqlDbType.Text) 
                        {
                            int l_len = Encoding.UTF8.GetByteCount(parameters.ObjValue.ToString())+3;
                            cmd.Parameters.Add(obj.ToString(), parameters.SqlDbType, l_len).Value = parameters.ObjValue;
                        }
                        else cmd.Parameters.Add(obj.ToString(), parameters.SqlDbType).Value = parameters.ObjValue;
                    }
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
    }
}