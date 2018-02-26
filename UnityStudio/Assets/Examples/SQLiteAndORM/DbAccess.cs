using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Mono.Data.Sqlite;
using UnityEngine;

//教程地址:http://www.cnblogs.com/vaevvaev/p/7054951.html
namespace ORM
{
    public class DbAccess
    {
        private DbConnection dbConnection;

        private DbCommand dbCommand;

        private DbDataReader reader;

        //打开数据库连接  
        public void OpenDB()
        {
            try
            {
                switch (DbConfig.Type)
                {
                    case DbType.Sqlite: dbConnection = new SqliteConnection("data source = " + DbConfig.Host); break;
                    //case DbType.Mysql: dbConnection = new MySqlConnection(DbConfig.Host); break;
                    default: break;
                }
                dbConnection.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //关闭数据库连接  
        public void CloseSqlConnection()
        {
            if (dbCommand != null)
            {
                dbCommand.Dispose();
            }
            dbCommand = null;
            if (reader != null)
            {
                reader.Dispose();
            }
            reader = null;
            if (dbConnection != null && dbConnection.State == ConnectionState.Open)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
            dbConnection = null;
        }

        //执行Sql命令  
        public int ExecuteQuery(string sql)
        {
            OpenDB();
            dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sql;
            reader = dbCommand.ExecuteReader();
            return reader.RecordsAffected;
        }

        //查询符合条件的第一个记录  
        public T FirstOrDefault<T>(Sql sql)
        {
            try
            {
                ExecuteQuery(sql.GetSql());
                T result = default(T);
                if (reader.Read())
                {
                    Type type = typeof(T);
                    if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type.IsEnum)
                    {
                        if (type.IsEnum)
                        {
                            result = (T)Enum.ToObject(type, reader.GetValue(0));
                        }
                        else
                        {
                            result = (T)Convert.ChangeType(reader.GetValue(0), type);
                        }
                    }
                    else
                    {
                        result = Activator.CreateInstance<T>();
                        PropertyInfo[] properties = type.GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            string columName = AttributeProcess.GetColumnName(property);
                            if (property.PropertyType.IsEnum)
                            {
                                property.SetValue(result, Enum.ToObject(property.PropertyType, reader.GetValue(reader.GetOrdinal(columName))), null);
                            }
                            else
                            {
                                  
                                property.SetValue(result, Convert.ChangeType(reader.GetValue(reader.GetOrdinal(columName)), property.PropertyType), null);
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseSqlConnection();
            }
        }

        //查询所有符合条件的记录  
        public List<T> Fetch<T>(Sql sql)
        {
            try
            {
                ExecuteQuery(sql.GetSql());
                List<T> list = new List<T>();
                Type type = typeof(T);
                if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type.IsEnum)
                {
                    while (reader.Read())
                    {
                        if (type.IsEnum)
                        {
                            list.Add((T)Enum.ToObject(type, reader.GetValue(0)));
                        }
                        else
                        {
                            list.Add((T)Convert.ChangeType(reader.GetValue(0), type));
                        }
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        T result = Activator.CreateInstance<T>();
                        PropertyInfo[] properties = type.GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            string columName = AttributeProcess.GetColumnName(property);
                            if (property.PropertyType.IsEnum)
                            {
                                property.SetValue(result, Enum.ToObject(property.PropertyType, reader.GetValue(reader.GetOrdinal(columName))), null);
                            }
                            else
                            {
                                property.SetValue(result, Convert.ChangeType(reader.GetValue(reader.GetOrdinal(columName)), property.PropertyType), null);
                            }
                        }
                        list.Add(result);
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseSqlConnection();
            }
        }

        /// <summary>  
        /// 更新指定的列  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="data"></param>  
        /// <param name="columns"></param>  
        /// <returns></returns>  
        public bool Update<T>(T data, IEnumerable<string> columns)
        {
            try
            {
                if (columns == null || columns.Count() == 0)
                {
                    Update<T>(data);
                }
                Type type = data.GetType();
                string table = AttributeProcess.GetTableName(type);
                string sql = "Update " + table + " Set ";
                string where = " Where ";
                List<string> sets = new List<string>();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string column = AttributeProcess.GetColumnName(property);
                    if (!AttributeProcess.IsPrimary(type, property))
                    {
                        if (columns.Any(a => a == column))
                        {
                            if (property.PropertyType == typeof(bool))
                            {
                                bool value = bool.Parse(property.GetValue(data, null).ToString());
                                sets.Add(column + "=" + (value ? "1" : "0"));
                            }
                            else if (property.PropertyType.IsPrimitive)
                            {
                                sets.Add(column + "=" + property.GetValue(data, null));
                            }
                            else if (property.PropertyType.IsEnum)
                            {
                                int intValue = (int)property.GetValue(data, null);
                                sets.Add(column + "=" + intValue);
                            }
                            else
                            {
                                if (Sql.InjectionDefend(property.GetValue(data, null).ToString()))
                                {
                                    sets.Add(column + "=\'" + property.GetValue(data, null) + "\'");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (property.PropertyType.IsPrimitive)
                        {
                            where += column + "=" + property.GetValue(data, null);
                        }
                        else
                        {
                            where += column + "=\'" + property.GetValue(data, null) + "\'";
                        }
                    }
                }
                sql += (string.Join(",", sets.ToArray()) + where);
                ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseSqlConnection();
            }
        }

        //更新指定的记录  
        public bool Update<T>(T data)
        {
            try
            {
                Type type = data.GetType();
                string table = AttributeProcess.GetTableName(type);
                string sql = "Update " + table + " Set ";
                List<string> sets = new List<string>();
                string where = " Where ";
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string column = AttributeProcess.GetColumnName(property);
                    if (!AttributeProcess.IsPrimary(type, property))
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            bool value = bool.Parse(property.GetValue(data, null).ToString());
                            sets.Add(column + "=" + (value ? "1" : "0"));
                        }
                        else if (property.PropertyType.IsPrimitive)
                        {
                            sets.Add(column + "=" + property.GetValue(data, null));
                        }
                        else if (property.PropertyType.IsEnum)
                        {
                            int intValue = (int)property.GetValue(data, null);
                            sets.Add(column + "=" + intValue);
                        }
                        else
                        {
                            if (Sql.InjectionDefend(property.GetValue(data, null).ToString()))
                            {
                                sets.Add(column + "=\'" + property.GetValue(data, null) + "\'");
                            }
                        }
                    }
                    else
                    {
                        if (property.PropertyType.IsPrimitive)
                        {
                            where += column + "=" + property.GetValue(data, null);
                        }
                        else
                        {
                            where += column + "=\'" + property.GetValue(data, null) + "\'";
                        }
                    }
                }
                sql += (string.Join(",", sets.ToArray()) + where);
                ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseSqlConnection();
            }
        }

        //插入新数据  
        public bool Insert<T>(T data)
        {
            try
            {
                Type type = data.GetType();
                string table = AttributeProcess.GetTableName(type);
                List<string> columns = new List<string>();
                List<string> values = new List<string>();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (!(AttributeProcess.IsPrimary(type, property) && AttributeProcess.IsIncrement(type)))
                    {
                        if (property.GetValue(data, null) != null)
                        {
                            columns.Add(AttributeProcess.GetColumnName(property));
                            if (property.PropertyType == typeof(bool))
                            {
                                bool value = bool.Parse(property.GetValue(data, null).ToString());
                                values.Add((value ? "1" : "0"));
                            }
                            else if (property.PropertyType.IsPrimitive)
                            {
                                values.Add(property.GetValue(data, null).ToString());
                            }
                            else if (property.PropertyType.IsEnum)
                            {
                                int intValue = (int)property.GetValue(data, null);
                                values.Add(intValue.ToString());
                            }
                            else
                            {
                                if (Sql.InjectionDefend(property.GetValue(data, null).ToString()))
                                {
                                    values.Add("\'" + property.GetValue(data, null) + "\'");
                                }
                            }
                        }
                    }
                }
                string sql = "INSERT INTO " + table + "(" + string.Join(",", columns.ToArray()) + ")" + "VALUES" + "(" + string.Join(",", values.ToArray()) + ")";
                ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseSqlConnection();
            }
        }

        //删除数据  
        public bool Delete<T>(object id)
        {
            try
            {
                Type type = typeof(T);
                string table = AttributeProcess.GetTableName(type);
                string sql = "DELETE FROM " + table + " WHERE ";
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (AttributeProcess.IsPrimary(type, property))
                    {
                        sql += (AttributeProcess.GetColumnName(property) + "=");
                        if (property.PropertyType.IsPrimitive)
                        {
                            sql += (id.ToString() + ";");
                        }
                        else
                        {
                            sql += ("\'" + id.ToString() + "\';");
                        }
                    }
                }
                ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseSqlConnection();
            }
        }
    }
}