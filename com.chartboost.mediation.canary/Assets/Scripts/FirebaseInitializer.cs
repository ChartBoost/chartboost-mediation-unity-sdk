using Firebase;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private FirebaseApp _app;
    
    /// <summary>
    /// Accessor to current Firebase Initialization status
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static bool DidInitialize { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                _app = FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                DidInitialize = true;
                Debug.Log("Firebase Initialized Successfully in Canary");
            } else {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
                DidInitialize = false;
            }
        });
    }
}
