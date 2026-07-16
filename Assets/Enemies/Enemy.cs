using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyWord;
    public TMP_Text wordText;
    
    public string enemyName;
    public TMP_Text nameText;
    

    public SpriteRenderer spriteRenderer; // Assign in Inspector
    public Transform graphics;

    public float moveSpeed = 10f;
    public int atkdmg => enemyWord.Length;

    public Bounds BodyBounds => spriteRenderer.bounds;

    private bool isAlive = true;
    private bool canMove = true;

    [Header("Scaling")]
    public float baseHeight = 4f;

    public float tinyWidth = 2.0f;
    public float smallWidth = 3.0f;
    public float mediumWidth = 4.0f;
    public float largeWidth = 5.0f;
    public float hugeWidth = 6.0f;
    
    

    public float nameTextOffset = 1.2f; // Moves name above sprite

    void Awake()
    {
        
    }

    void Update()
    {
        if (!isAlive)
            return;

        if (!canMove)
            return;

        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive)
            return;

        if (other.CompareTag("Tower"))
        {
            ReachTower();
        }
    }

    public void SetEnemy(string word, string user)
    {
        enemyWord = word;
        enemyName = user;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (wordText != null)
            wordText.text = enemyWord;

        if (nameText != null)
            nameText.text = enemyName;

        ScaleEnemyToWord();
        AdjustNamePosition();
    }

    void ScaleEnemyToWord()
    {
        int longest = Mathf.Max(enemyWord.Length, enemyName.Length);

        float width;

        if (longest <= 5)
            width = tinyWidth;
        else if (longest <= 8)
            width = smallWidth;
        else if (longest <= 12)
            width = mediumWidth;
        else if (longest <= 16)
            width = largeWidth;
        else
            width = hugeWidth;

        spriteRenderer.size = new Vector2(width, baseHeight);
    }

    void AdjustNamePosition()
    {
        if (!spriteRenderer || !nameText) return;

        // Get sprite bounds in WORLD space
        Bounds bounds = spriteRenderer.bounds;

        // Calculate exact top edge of the sprite
        float topY = bounds.max.y;

        // A tiny offset above
        float offset = nameTextOffset; // You can tweak 0.01f–0.08f

        // Set username directly above the sprite top in WORLD space
        Vector3 newWorldPos = nameText.transform.position;
        newWorldPos.y = topY + offset;

        nameText.transform.position = newWorldPos;

        // Always keep username scale at 1 (no inherited scaling)
        nameText.transform.localScale = Vector3.one * 0.05f;


    }

    public void KillEnemy()
    {
        if (!isAlive) return;
        isAlive = false;

        GameManager.instance.activeEnemies.Remove(this);
        Destroy(gameObject);
    }

    public void StopEnemy()
    {
        canMove = false;
    }

    void ReachTower()
    {
        if (!isAlive) return;
        isAlive = false;

        GameManager.instance.activeEnemies.Remove(this);
        GameManager.instance.DamageTower(atkdmg);
        Destroy(gameObject);
    }
}
