using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public GameObject enemyPrefab;
    public string[] testWords;
    public int maxEnemies = 5;



    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //for (int i = 0; i < maxEnemies; i++)
        //    SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy()
    {
        string randomWord = testWords[Random.Range(0, testWords.Length)];

        Vector2 spawnPos = GetValidSpawnPosition();

        //Vector2 pos = new Vector2(Random.Range(5.5f, 8.5f), Random.Range(-4.5f, 4.5f));

        var spawnedObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        var enemy = spawnedObject.GetComponent<Enemy>();
        enemy.SetEnemy(randomWord, "AI");
        GameManager.instance.activeEnemies.Add(enemy);
    }

    public void SpawnEnemyWithWord(string word, string user)
    {
        if (GameManager.instance.activeEnemies.Count >= maxEnemies)
            return;

        Vector2 spawnPos = GetValidSpawnPosition();

        var spawnedObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        var enemy = spawnedObject.GetComponent<Enemy>();

        enemy.SetEnemy(word, user);
        GameManager.instance.activeEnemies.Add(enemy);
    }



    Vector2 GetValidSpawnPosition()
    {
        int maxAttempts = 20;
        float minDistance = 5f;

        for(int i=0; i < maxAttempts; i++)
        {
            Vector2 pos = new Vector2(Random.Range(5.5f, 8.5f), Random.Range(-4.5f, 4.5f));
            bool isValid = true;

            foreach(var other in FindObjectsOfType<Enemy>())
            {
                if (Vector2.Distance(pos, other.transform.position) < minDistance)
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid) 
                return pos;
        }
        return new Vector2(Random.Range(5.5f, 8.5f), Random.Range(-4.5f, 4.5f));
    }
}
