using TMPro;
using UnityEngine;

public class MenuManager: MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI")]
    public GameObject mainMenuUI;
    public GameObject gameplayUI;

    [Header("Buttons")]
    public GameObject loginButton;
    public GameObject playButton;
    public GameObject settingsButton;
    public GameObject quitButton;

    [Header("Connection Status")]
    public TMP_Text connectionStatus;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        mainMenuUI.SetActive(true);
        gameplayUI.SetActive(false);

        Time.timeScale = 0f;

        connectionStatus.text = "Not Connected";
        connectionStatus.color = Color.red;
    }

    public void PlayGame()
    {
        mainMenuUI.SetActive(false);
        gameplayUI.SetActive(true);

        Time.timeScale = 1f;
    }

    public void LoginToTwitch()
    {
        TwitchManager.Instance.Login();
    }

    public void OpenSettings()
    {
        Debug.Log("Settings coming soon...");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetConnectionStatus(string username)
    {
        connectionStatus.text = $"Connected as {username}";
    }
}