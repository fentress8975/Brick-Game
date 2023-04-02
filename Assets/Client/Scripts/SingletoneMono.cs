using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : Component
{
    private static T _singletone;
    public static T Singletone
    {
        get
        {
            if (_singletone == null)
            {
                var obj = FindObjectsOfType(typeof(T)) as T[];
                if (obj.Length > 0) { _singletone = obj[0]; }
                if (obj.Length > 1)
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
