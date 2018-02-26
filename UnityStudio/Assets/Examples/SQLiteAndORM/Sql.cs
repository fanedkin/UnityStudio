using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace ORM
{
    public class Sql
    {
        /// <summary>  
        /// sql语句  
        /// </summary>  
        private string sql;

        /// <summary>  
        /// 是否有where关键字  
        /// </summary>  
        private bool hasWhere;

        /// <summary>  
        /// 是否有order关键字  
        /// </summary>  
        private bool hasOrder;

        /// <summary>  
        /// 防sql注入  
        /// </summary>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public static bool InjectionDefend(string value)
        {
            //网上随便找的，不确定是否有效  
            string SqlStr = @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\/\*|\*\/|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            try
            {
                if ((value != null) && (value != String.Empty))
                {
                    //string str_Regex = @"\b(" + SqlStr + @")\b";  
                    Regex Regex = new Regex(SqlStr, RegexOptions.IgnoreCase);
                    if (true == Regex.IsMatch(value))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>  
        /// 构造函数  
        /// </summary>  
        public Sql()
        {
            sql = string.Empty;
            hasWhere = false;
            hasOrder = false;
        }

        /// <summary>  
        /// 添加select  
        /// </summary>  
        /// <param name="column"></param>  
        /// <returns></returns>  
        public Sql Select(string column)
        {
            sql += ("SELECT " + column + " ");
            return this;
        }

        /// <summary>  
        /// 添加from  
        /// </summary>  
        /// <param name="Table"></param>  
        /// <returns></returns>  
        public Sql From(string Table)
        {
            sql += ("FROM " + Table + " ");
            return this;
        }

        /// <summary>  
        /// 添加where  
        /// </summary>  
        /// <param name="query"></param>  
        /// <param name="values"></param>  
        /// <returns></returns>  
        public Sql Where(string query, params object[] values)
        {
            if (!hasWhere)
            {
                sql += "WHERE ";
                hasWhere = true;
            }
            else
            {
                sql += " AND ";
            }
            for (int i = 0; i < values.Length; i++)
            {
                Regex r = new Regex(@"@\d+");
                //bool类型需要特殊处理，不能直接用tostring转换，因为直接转换的结果为"True"或"False"，而不是1和0  
                if (values[i] is bool)
                {
                    bool value = bool.Parse(values[i].ToString());
                    query = r.Replace(query, (value ? "1" : "0"), 1);
                    continue;
                }
                else if (values[i].GetType().IsPrimitive)
                {
                    query = r.Replace(query, values[i].ToString(), 1);
                    continue;
                }
                else if (values[i].GetType().IsEnum)
                {
                    int intValue = (int)values[i];
                    query = r.Replace(query, intValue.ToString(), 1);
                    continue;
                }
                else
                {
                    if (InjectionDefend(values[i].ToString()))
                    {
                        query = r.Replace(query, "\"" + values[i].ToString() + "\"", 1);
                    }
                }
            }
            sql += query;
            return this;
        }

        /// <summary>  
        /// 在sql尾部插入任意sql语句  
        /// </summary>  
        /// <param name="sql"></param>  
        /// <returns></returns>  
        public Sql Append(string sql, params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Regex r = new Regex(@"@\d+");
                if (values[i] is bool)
                {
                    bool value = bool.Parse(values[i].ToString());
                    sql = r.Replace(sql, (value ? "1" : "0"), 1);
                    continue;
                }
                else if (values[i].GetType().IsPrimitive)
                {
                    sql = r.Replace(sql, values[i].ToString(), 1);
                    continue;
                }
                else if (values[i].GetType().IsEnum)
                {
                    int intValue = (int)values[i];
                    sql = r.Replace(sql, intValue.ToString(), 1);
                    continue;
                }
                else
                {
                    if (InjectionDefend(values[i].ToString()))
                    {
                        sql = r.Replace(sql, "\"" + values[i].ToString() + "\"", 1);
                    }
                }
            }
            this.sql += (" " + sql + " ");
            return this;
        }

        /// <summary>  
        /// 添加order  
        /// </summary>  
        /// <param name="column"></param>  
        /// <returns></returns>  
        public Sql OrderBy(string column)
        {
            if (!sql.EndsWith(" "))
            {
                sql += " ";
            }
            if (hasOrder)
            {
                sql += (", " + column);
            }
            else
            {
                sql += ("ORDER BY " + column);
            }
            return this;
        }


        /// <summary>  
        /// 获取当前完整的sql语句  
        /// </summary>  
        /// <returns></returns>  
        public string GetSql()
        {
            return sql;
        }
    }
}