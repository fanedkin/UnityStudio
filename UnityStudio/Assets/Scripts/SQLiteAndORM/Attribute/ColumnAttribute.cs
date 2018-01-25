using System;  
  
namespace ORM  
{
    //Inherited表示在描述类属性时，这个attribute能否被子类继承，这里我也设为了false，因为orm映射的类不会涉及到继承的问题。
    //Inherited表示在描述类属性时，这个attribute能否被子类继承，这里我也设为了false，因为orm映射的类不会涉及到继承的问题。
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]//AllowMultiple表示同一个程序体前能否放置多个相同的该自定义attribute类，这里我设为false，因为一个属性在数据表中只能对应一个字段
    public class ColumnAttribute : Attribute  
    {  
        public ColumnAttribute(string columnName)  
        {  
            this.Value = columnName;  
        }  
  
        public string Value { get; protected set; }  
    }  
}  
