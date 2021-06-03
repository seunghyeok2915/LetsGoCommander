using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, IPool> poolDic = new Dictionary<string, IPool>();

    private static PoolManager instance = null;

    public string[] poolClassList;
    public GameObject[] prefabs;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("없습니다.");
        }
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolClassList.Length; i++)
        {
            string name = poolClassList[i];
            Type T = Type.GetType(name);

            MethodInfo createMethod = typeof(PoolManager).GetMethod("CreatePool", BindingFlags.Public | BindingFlags.Instance);
            createMethod = createMethod.MakeGenericMethod(T);
            createMethod.Invoke(this, new object[] { prefabs[i], transform, 5 });
        }
    }

    public static T GetItem<T>() where T : MonoBehaviour
    {
        return instance.GetPoolItem<T>();
    }

    public void CreatePool<T>(GameObject prefab, Transform parent, int count = 5) where T : MonoBehaviour
    {
        Type t = typeof(T);

        ObjectPooling<T> pool = new ObjectPooling<T>(prefab, parent, count);

        poolDic.Add(t.ToString(), pool);
    }

    public T GetPoolItem<T>() where T : MonoBehaviour
    {
        Type t = typeof(T);
        ObjectPooling<T> pool = (ObjectPooling<T>)poolDic[t.ToString()];
        return pool.GetOrCreate();
    }
}
