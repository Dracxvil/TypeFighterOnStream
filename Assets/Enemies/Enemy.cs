using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public string enemyWord;                    //Word to type in order to kill the enemy
    public TMP_Text wordText;                   //
    public string enemyName;                    //Username of the chatter

    public float moveSpeed = 5f;
    public int atkdmg => enemyWord.Length;

    private bool isAlive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wordText.text= enemyWord;
    }

    public void SetEnemy(string word, string user)
    {
        enemyWord = word;
        enemyName = user;
        wordText.text = $"{user}: {word}";
    }

    
    

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;

        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if(transform.position.x<= -5) { 
            ReachTower();
        }
    }



    void ReachTower()
    {
        if(!isAlive) return;
        isAlive = false;
        GameManager.instance.DamageTower(atkdmg);
        Destroy(gameObject);
    }

    public void KillEnemy()
    {
        if (!isAlive) return;
        isAlive = false;
        Destroy(gameObject);
    }
}
