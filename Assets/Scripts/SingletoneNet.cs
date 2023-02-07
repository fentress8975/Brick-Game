using Unity.Netcode;
using UnityEngine;

public class SingletonNetWork<T> : NetworkBehaviour where T : Component
{
    private static T _singletone;
    public static T Singletone
    {
        get
        {
            if (_singletone == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                {
                    _singletone = objs[0];
                }
                if (objs.Length > 1)
                {
                    Debug.LogError("More than 1 instance of " + typeof(T).Name);
                }
                if (_singletone == null)
                {
                    GameObject single = new();
                    single.name = typeof(T).Name;
                    _singletone = single.AddComponent<T>();
                }
            }
            return _singletone;
        }
    }
}
