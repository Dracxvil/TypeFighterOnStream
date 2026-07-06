using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyWord;
    public TMP_Text wordText;
    private RectTransform wordRect;
    public string enemyName;
    public TMP_Text nameText;
    private RectTransform nameRect;

    public SpriteRenderer spriteRenderer; // Assign in Inspector
    public Transform graphics;

    public float moveSpeed = 5f;
    public int atkdmg => enemyWord.Length;

    private bool isAlive = true;

    [Header("Scaling")]
    public float baseWidth = 1f;
    public float baseHeight = 1f;
    public float widthPerCharacter = 0.07f;

    public float nameTextOffset = 1.2f; // Moves name above sprite

    void Awake()
    {
        if (wordRect == null)
            wordRect = wordText.GetComponent<RectTransform>();
        if (nameRect == null)
            nameRect = nameText.GetComponent<RectTransform>();

        //UpdateVisuals();
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
        //Enemy width
        int longestLength = Mathf.Max(enemyWord.Length, enemyName.Length);

        //Scale only the graphics
        float width = baseWidth + (longestLength * widthPerCharacter);

        graphics.localScale = new Vector3(
            width,
            baseHeight,
            1f
        );

        //Make TMP calculate the real width needed
        wordText.ForceMeshUpdate();
        nameText.ForceMeshUpdate();

        float textWidth = Mathf.Max(
            wordText.preferredWidth,
            nameText.preferredWidth
        );

        //Padding so text isn't touching the edges
        float padding = 0f;

        //Resize the text boxes
        wordRect.sizeDelta = new Vector2(textWidth + padding, wordRect.sizeDelta.y);
        nameRect.sizeDelta = new Vector2(textWidth + padding, nameRect.sizeDelta.y);



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
        nameText.transform.localScale = Vector3.one * 0.05f;


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
