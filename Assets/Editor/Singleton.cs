using UnityEngine;
using System;
using UnityEditor;

public class Singleton<T> : Editor where T : Singleton<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                {
                    //instance = new GameObject("_" + typeof(T).Name).AddComponent<T>();
                    //DontDestroyOnLoad(instance);
                }
                if (instance == null)
                    Debug.Log("Failed to create instance of " + typeof(T).FullName + ".");
            }
            return instance;
        }
    }

    void OnApplicationQuit() { if (instance != null) instance = null; }

}