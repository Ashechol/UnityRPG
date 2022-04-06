using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 泛型单例模式
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }
    public static bool IsInit
    {
        get { return instance != null; }
    }

    // protected: 只有子类可访问
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;  // 需显式转换为 T
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;  // 销毁
        }
    }
}
