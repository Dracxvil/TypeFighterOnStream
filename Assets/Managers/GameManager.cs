using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int towerHP = 100;
    public TMP_Text towerHPText;
    public List<Enemy> activeEnemies;
    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeEnemies = new List<Enemy>();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        towerHPText.text = "HP: " + towerHP;
    }

    public void DamageTower(int dmg)
    {
        towerHP-=dmg;
        if (towerHP <= 0)
        {
            towerHP = 0;
            Debug.Log("GameOver");
            //GameOver Code goes here
        }
        UpdateUI();
    }

    public void SelfDamage(int dmg)
    {
        towerHP -= dmg;
        if (towerHP <= 0)
        {
            towerHP = 0;
            Debug.Log("GameOver");
            //GameOver Code goes here
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        towerHPText.text = "HP: " + towerHP;
    }
}
