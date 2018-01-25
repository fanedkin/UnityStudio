using System;

namespace ORM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName)
        {
            this.Value = tableName;
        }

        public string Value { get; protected set; }
    }
}