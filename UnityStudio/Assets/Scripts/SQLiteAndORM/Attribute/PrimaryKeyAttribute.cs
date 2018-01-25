using System;

namespace ORM
{
    //AttributeTargets是用来表名attribute类应该在何种程序实体前放置，class表示应该在类声明前放置，Field表示可以在字段前放置，Property表示可以在属性前放置
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(string primaryKey)
        {
            this.Value = primaryKey;
        }

        public string Value { get; protected set; }
        public bool autoIncrement = false;

    }
}