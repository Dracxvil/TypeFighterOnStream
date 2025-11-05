using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public string enemyWord;
    public TMP_Text wordText;
    public string enemyName;
    public TMP_Text nameText;

    public SpriteRenderer spriteRenderer; // Assign in Inspector

    public float moveSpeed = 5f;
    public int atkdmg => enemyWord.Length;

    private bool isAlive = true;

    [Header("Scaling")]
    public float baseScale = 1f;
    public float scalePerCharacter = 0.07f;
    public float nameTextOffset = 1.2f; // Moves name above sprite

    void Start()
    {
        UpdateVisuals();
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
        int length = Mathf.Max(enemyWord.Length, 1);
        float scale = baseScale + (length * scalePerCharacter);

        transform.localScale = Vector3.one * scale; // Uniform scaling
    }

    void AdjustNamePosition()
    {
        if (!spriteRenderer || !nameText) return;

        // Get sprite bounds in WORLD space
        Bounds bounds = spriteRenderer.bounds;

        // Calculate exact top edge of the sprite
        float topY = bounds.max.y;

        // A tiny offset above
        float offset = 0.1f; // You can tweak 0.01f–0.08f

        // Set username directly above the sprite top in WORLD space
        Vector3 newWorldPos = nameText.transform.position;
        newWorldPos.y = topY + offset;

        nameText.transform.position = newWorldPos;

        // Always keep username scale at 1 (no inherited scaling)
        nameText.transform.localScale = Vector3.one*0.05f;


    }



    void Update()
    {
        if (!isAlive) return;

        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.position.x <= -5)
            ReachTower();
    }

    public void KillEnemy()
    {
        if (!isAlive) return;
        isAlive = false;

        GameManager.instance.activeEnemies.Remove(this);
        Destroy(gameObject);
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
