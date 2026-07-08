using UnityEngine;
using System.Diagnostics;

public class TwitchManager: MonoBehaviour
{
    public static TwitchManager Instance;

    public bool isAuthenticated { get; private set; }

    public string Username {  get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Login()
    {
        DeviceCodeAuth.Instance.BeginDeviceLogin();
    }

    public void Logout()
    {
        isAuthenticated = false;
        Username = "";

        UnityEngine.Debug.Log("Logged out.");
    }

    public void AuthenticationSuccessful(string username)
    {
        Username= username;
        isAuthenticated = true;

        UnityEngine.Debug.Log($"Authenticated as {username}");

        if (MenuManager.Instance != null)
            MenuManager.Instance.SetConnectionStatus(username);
    }
}