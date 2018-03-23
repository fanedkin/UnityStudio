

using System;

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Field,     //特性应用的目标           
                           AllowMultiple=true,                      //是否可以在同一目标上放置多次
                           Inherited = true)]                         //是否允许子类使用此特性
public class SettingAttribute : Attribute 
{



}
