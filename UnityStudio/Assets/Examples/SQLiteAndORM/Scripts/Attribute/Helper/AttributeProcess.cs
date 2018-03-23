using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ORM
{
    public static class AttributeProcess
    {

        /// <summary>  
        /// 获取表名  
        /// </summary>  
        /// <param name="type"></param>  
        /// <returns></returns>  
        public static string GetTableName(Type type)
        {
            string tableName = string.Empty;
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is TableAttribute)
                {
                    TableAttribute tableAttribute = attr as TableAttribute;
                    tableName = tableAttribute.Value;
                }
            }
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = type.Name;
            }
            return tableName;
        }

        /// <summary>  
        /// 获取字段名  
        /// </summary>  
        /// <param name="property"></param>  
        /// <returns></returns>  
        public static string GetColumnName(PropertyInfo property)
        {
            string columnName = string.Empty;
            object[] attributes = property.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is ColumnAttribute)
                {
                    ColumnAttribute columnAttr = attr as ColumnAttribute;
                    columnName = columnAttr.Value;
                }
            }
            if (string.IsNullOrEmpty(columnName))
            {
                columnName = property.Name;
            }
            return columnName;
        }

        /// <summary>  
        /// 判断主键是否自增  
        /// </summary>  
        /// <param name="property"></param>  
        /// <returns></returns>  
        public static bool IsIncrement(Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is PrimaryKeyAttribute)
                {
                    PrimaryKeyAttribute primaryKeyAttr = attr as PrimaryKeyAttribute;
                    return primaryKeyAttr.autoIncrement;
                }
            }
            return false;
        }

        /// <summary>  
        /// 获取主键名  
        /// </summary>  
        /// <param name="type"></param>  
        /// <returns></returns>  
        public static string GetPrimary(Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is PrimaryKeyAttribute)
                {
                    PrimaryKeyAttribute primaryKeyAttr = attr as PrimaryKeyAttribute;
                    return primaryKeyAttr.Value;
                }
            }
            return null;
        }

        /// <summary>  
        /// 判断属性是否为主键  
        /// </summary>  
        /// <param name="type"></param>  
        /// <param name="property"></param>  
        /// <returns></returns>  
        public static bool IsPrimary(Type type, PropertyInfo property)
        {
            string primaryKeyName = GetPrimary(type);
            string columnName = GetColumnName(property);
            return (primaryKeyName == columnName);
        }

    }
}