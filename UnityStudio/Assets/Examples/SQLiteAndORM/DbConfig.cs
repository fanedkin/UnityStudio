using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    public class DbConfig
    {

        /// <summary>  
        /// 数据库连接信息  
        /// </summary>  
        public static string Host = "C:/Users/Administrator/Desktop/ORMTest.db";

        /// <summary>  
        /// 数据库类型  
        /// </summary>  
        public static DbType Type = DbType.Sqlite;

    }

    public enum DbType
    {
        Sqlite,
        Mysql,
        SqlServer,
        Oracle
    }
}