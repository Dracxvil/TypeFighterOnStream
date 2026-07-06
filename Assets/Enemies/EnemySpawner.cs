using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public GameObject enemyPrefab;
    public string[] testWords;

    [Header("Spawning")]
    public int maxEnemies = 5;

    [Header("Lane Settings")]
    public Transform[] lanes;
    public float spawnX = 11f;
    public float laneSpawnSpacing = 2f;
    

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

        Vector2 spawnPos = GetSpawnPosition();
        if (spawnPos == Vector2.zero)
            return;

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

        Vector2 spawnPos = GetSpawnPosition();
        if (spawnPos == Vector2.zero)
            return;

        var spawnedObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        var enemy = spawnedObject.GetComponent<Enemy>();

        enemy.SetEnemy(word, user);
        GameManager.instance.activeEnemies.Add(enemy);
    }



    Vector2 GetSpawnPosition()
    {
        if (lanes.Length == 0)
        {
            Debug.LogError("No lanes assigned");
            return Vector2.zero;
        }

        //Randomize the lane order so we don't always start at lane 0
        int startIndex=Random.Range(0,lanes.Length);

        for(int i = 0; i < lanes.Length; i++)
        {
            int laneIndex=(startIndex+i)%lanes.Length;
            float laneY = lanes[laneIndex].position.y;

            bool blocked = false;

            foreach(Enemy enemy in GameManager.instance.activeEnemies)
            {
                if (Mathf.Abs(enemy.transform.position.y - laneY) < 0.1f)
                {
                    if (Mathf.Abs(enemy.transform.position.x - spawnX) < laneSpawnSpacing)
                    {
                        blocked = true;
                        break;
                    }
                }
            }

            if (!blocked)
            {
                return new Vector2(spawnX, laneY);
            }
        }

        //No free lane near the spawn point
        return Vector2.zero;
    }
}
