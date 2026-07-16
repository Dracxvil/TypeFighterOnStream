using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

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

    private Queue<(string word, string user)> spawnQueue = new();
    

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
        if (!GameManager.instance.IsGameRunning)
            return;

        if (spawnQueue.Count == 0)
            return;

        if (GameManager.instance.activeEnemies.Count >= maxEnemies)
            return;

        var nextEnemy = spawnQueue.Peek();

        TrySpawnEnemy(nextEnemy.word, nextEnemy.user);
    }

    public void QueueEnemy(string word, string user)
    {
        spawnQueue.Enqueue((word, user));
    }

    void TrySpawnEnemy(string word, string user)
    {
        Vector2 spawnPos = GetSpawnPosition();

        if (spawnPos == Vector2.zero)
            return;

        var spawnedObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        var enemy = spawnedObject.GetComponent<Enemy>();

        enemy.SetEnemy(word, user);
        GameManager.instance.activeEnemies.Add(enemy);

        spawnQueue.Dequeue();
    }

    //public void SpawnEnemyWithWord(string word, string user)
    //{
    //    if (!GameManager.instance.IsGameRunning)
    //        return;

    //    if (GameManager.instance.activeEnemies.Count >= maxEnemies)
    //        return;

    //    Vector2 spawnPos = GetSpawnPosition();
    //    if (spawnPos == Vector2.zero)
    //        return;

    //    var spawnedObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    //    var enemy = spawnedObject.GetComponent<Enemy>();

    //    enemy.SetEnemy(word, user);

    //    //Check spacing against enemies already in this line
    //    const float minGap = 0.2f;

    //    foreach(Enemy other in GameManager.instance.activeEnemies)
    //    {
    //        if (other == null)
    //            continue;

    //        //Only compare enemies in the same lane
    //        if (Mathf.Abs(other.transform.position.y - enemy.transform.position.y) > 0.1f)
    //            continue;

    //        float otherRight = other.BodyBounds.max.x;
    //        float newLeft = enemy.BodyBounds.min.x;

    //        if (newLeft - otherRight < minGap)
    //        {
    //            Destroy(spawnedObject);
    //            return;
    //        }
    //    }

    //    GameManager.instance.activeEnemies.Add(enemy);
    //}



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
                if (enemy == null)
                    continue;

                //Different Lane
                if (Mathf.Abs(enemy.transform.position.y - laneY) > 0.1f)
                    continue;

                //Distance from the right edge of the existing enemy
                float gap = spawnX - enemy.BodyBounds.max.x;

                if (gap < laneSpawnSpacing)
                {
                    blocked = true;
                    break;
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
