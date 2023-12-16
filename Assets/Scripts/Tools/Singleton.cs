using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//范型传入的对象必须继承自Singleton
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T Instance => _instance;
    public static bool isInitialize => _instance != null;

    // private void OnEnable()
    // {
    //     Debug.Log("Singleton OnEnable");
    // }

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            //如果检查发现已存在instance的实例，遵从单例原则，删除当前创建的实例，使用之前已有的实例
        }
        else
        {
            _instance = (T)this;
        }
    }

    protected void OnDestroy()
    {
        //Debug.Log("Singleton OnDestory");
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
