using UnityEngine;
using TMPro;
using Unity.Jobs;
using System.Collections.Generic;
using System.Linq;

public class TypingManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text scoreText;
    private int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckTypedWord();
        }
    }

    Enemy GetClosestEnemy(string nameFilter,List<Enemy> enemies)
    {
        // Filter by matching name (case-insensitive)
        var filtered = enemies
            .Where(e => e != null && e.enemyWord.Equals(nameFilter, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        Debug.Log("filter count : "+filtered.Count);
        // Return null if no matches
        if (filtered.Count == 0)
            return null;

        // Find closest transform using LINQ
        Enemy closest = filtered
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();

        return closest;
    }
    void CheckTypedWord()
    {
        string typedWord = inputField.text.Trim();
        if (string.IsNullOrEmpty(typedWord))
            return;

        var enemies = GameManager.instance.activeEnemies;
        var e = GetClosestEnemy(typedWord, enemies);

        if (e == null)
        {
            inputField.text = "";
            return;
        }

        GameManager.instance.activeEnemies.Remove(e);
        e.KillEnemy();

        score += e.enemyWord.Length;
        scoreText.text = "Score: " + score;
        inputField.text = "";
    }

}
