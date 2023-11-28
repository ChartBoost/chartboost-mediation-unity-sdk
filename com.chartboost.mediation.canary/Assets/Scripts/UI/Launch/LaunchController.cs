using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// If not signed in, loads the sign-in scene.  Otherwise, goes to the Root scene.
/// </summary>
public class LaunchController : MonoBehaviour
{
    /// <summary>
    /// Environment settings.
    /// </summary>
    private readonly Lazy<Environment> _environment = new Lazy<Environment>(() => Environment.Shared);
    private Environment Environment => _environment.Value;

    /// <summary>
    /// Standard Unity Awake handler.
    /// </summary>
    private void Start()
    {
        if (IsSignedIn)
        {
            // Enable the Chartboost Mediation Initializer
            var chartboostMediation = FindObjectOfType<ChartboostMediationInitializer>();
            chartboostMediation.enabled = true;
            
            LoadRoot();
        }
        else
        {
            LoadSignIn();
        }
    }

    private bool IsSignedIn => Environment.AppIdentifier.Length > 0 && Environment.AppIdentifier.Length > 0;

    private void LoadSignIn()
    {
        SceneManager.LoadScene("Scenes/SignIn", LoadSceneMode.Single);
    }

    private void LoadRoot()
    {
        SceneManager.LoadScene("Scenes/Root", LoadSceneMode.Single);
    }
}
