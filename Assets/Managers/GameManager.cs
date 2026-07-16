using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Tower")]
    [SerializeField] private int startingTowerHP = 100;

    public int TowerHP { get; private set; }


    public TMP_Text towerHPText;
    public List<Enemy> activeEnemies;

    [Header("Game State")]
    public bool IsGameRunning = false;

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    

    public bool IsPaused {  get; private set; }

    
    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeEnemies = new List<Enemy>();

        TowerHP = startingTowerHP;

        IsGameRunning = true;

        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if(pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        UpdateUI();
    }

    // Update is called once per frame
    

    public void DamageTower(int dmg)
    {
        ApplyDamage(dmg);
    }

    public void SelfDamage(int dmg)
    {
        ApplyDamage(dmg);
    }

    private void ApplyDamage(int dmg)
    {
        TowerHP -= dmg;

        if(TowerHP <= 0)
        {
            TowerHP = 0;
            GameOver();
        }

        UpdateUI();
    }

    public void GameOver()
    {
        if (!IsGameRunning)
            return;

        IsGameRunning = false;
        Debug.Log("=====GAME OVER=====");
        if(FindFirstObjectByType<TypingManager>() != null)
        {
            FindFirstObjectByType<TypingManager>().inputField.interactable = false;
        }

        if (TwitchConnect.Instance != null)
        {
            TwitchConnect.Instance.StopGameplay();
        }

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i]!=null)
                activeEnemies[i].StopEnemy();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void PauseGame()
    {
        if (!IsGameRunning)
            return;

        if (IsPaused)
            return;

        IsPaused = true;

        Time.timeScale = 0f;

        if(pausePanel!=null)
            pausePanel.SetActive(true);

        if (FindFirstObjectByType<TypingManager>() != null)
        {
            FindFirstObjectByType<TypingManager>().inputField.interactable=false;
        }

        if(TwitchConnect.Instance != null)
        {
            TwitchConnect.Instance.StopGameplay();
        }
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;

        IsPaused = false;

        Time.timeScale = 1f;

        if (pausePanel!=null)
            pausePanel.SetActive(false);

        if(FindFirstObjectByType<TypingManager>() != null)
        {
            FindFirstObjectByType<TypingManager>().inputField.interactable = true;
            FindFirstObjectByType<TypingManager>().inputField.ActivateInputField();
        }

        if(TwitchConnect.Instance!= null)
        {
            TwitchConnect.Instance.StartGameplay();
        }
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        //Resume Gameplay
        IsGameRunning = true;

        if (FindFirstObjectByType<TypingManager>() != null)
        {
            FindFirstObjectByType<TypingManager>().inputField.interactable = true;
            FindFirstObjectByType<TypingManager>().inputField.text = "";
            FindFirstObjectByType<TypingManager>().inputField.ActivateInputField();
        }

        //Reset HP
        TowerHP = startingTowerHP;
        UpdateUI();

        TypingManager typingManager = FindFirstObjectByType<TypingManager>();
        if(typingManager != null)
        {
            typingManager.ResetScore();
        }

        //Resume Twitch chat
        if(TwitchConnect.Instance != null)
        {
            TwitchConnect.Instance.StartGameplay();
        }

        //Destroy all remaining enemies
        for(int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
                Destroy(activeEnemies[i].gameObject);
        }

        activeEnemies.Clear();

        //Hide Game Over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if(pausePanel!=null)
            pausePanel.SetActive(false);

        Debug.Log("=====GAME RESET=====");
    }

    public void PlayAgain()
    {
        ResetGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        IsGameRunning = false;

        if (TwitchConnect.Instance != null)
            TwitchConnect.Instance.StopGameplay();

        if(gameOverPanel!=null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        //We'll show the Main Menu in the next feature.
    }

    void UpdateUI()
    {
        towerHPText.text = "HP: " + TowerHP;
    }
}
