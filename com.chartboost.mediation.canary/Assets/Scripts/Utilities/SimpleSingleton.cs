using UnityEngine;

/// <summary>
/// Generic class to create simple singleton instances in Unity.
/// </summary>
/// <typeparam name="T">Reference Class for Instance</typeparam>
public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();

            if (_instance == null)
                _instance = CreateSingleton();

            return _instance;
        }
        internal set => _instance = value;
    }

    private static T CreateSingleton()
    {
        var ownerObject = new GameObject($"{typeof(T)}(singleton)");
        var instance = ownerObject.AddComponent<T>();
        DontDestroyOnLoad(ownerObject);
        return instance;
    }
}
