using UnityEngine;

public class EnemySpawnTester : MonoBehaviour
{
    [Header("Test Enemy")]
    public string username = "dracxvil";
    public string word = "Testing";

    public void SpawnEnemy()
    {
        if (EnemySpawner.instance == null)
        {
            Debug.LogError("EnemySpawner not found!");
            return;
        }

        EnemySpawner.instance.QueueEnemy(word, username);
    }
}