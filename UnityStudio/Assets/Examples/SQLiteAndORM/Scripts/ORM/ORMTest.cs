using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ORM;
using UnityEngine;

public class ORMTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
        DateTime d = DateTime.Now;
        Debug.Log(d);
	    TestUserORM();


	}

    void TestUserAttribute()   
    {
        Type type = typeof(User);  
        PropertyInfo[] properties = type.GetProperties();
        Debug.Log(AttributeProcess.GetTableName(type));
        foreach (var item in properties)
        {
            Debug.Log(AttributeProcess.GetColumnName(item));
        }  
    }

    void TestUserORM()
    {
        DbAccess dao = new DbAccess();
        Sql sql = new Sql();
        sql.Select("*").From("userinfo");
        sql.Where("Id=@0", 1);
        User user = dao.FirstOrDefault<User>(sql);
        Debug.Log(user.UserName);
        user.UserName = "tczhoulan";
        Debug.Log(dao.Update<User>(user, new string[] { "UserName" }));
        Debug.Log(user.RoleType);  
    }
	// Update is called once per frame
	void Update () {
		
	}
}
