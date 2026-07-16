using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");

            if (GameManager.instance.IsPaused)
                GameManager.instance.ResumeGame();
            else
                GameManager.instance.PauseGame();
        }
    }

    public void ResumeGame()
    {
        GameManager.instance.ResumeGame();
    }

    public void RestartGame()
    {
        GameManager.instance.ResetGame();
    }

    public void ReturnToMainMenu()
    {
        GameManager.instance.ReturnToMainMenu();
    }

    public void Logout()
    {
        //daiskuhg
    }


}
