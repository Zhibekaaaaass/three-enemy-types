using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    [SerializeField] float invulnerableTime = 1f;

    public static int Attempts = 1;  

    int health;
    float invulnTimer;
    SpriteRenderer sr;
    Color baseColor;
    GUIStyle style;

    void Start()
    {
        Time.timeScale = 1f;
        health = maxHealth;
        sr = GetComponentInChildren<SpriteRenderer>();
        baseColor = sr.color;
    }

    void Update()
    {
        if (invulnTimer > 0f)
        {
            invulnTimer -= Time.deltaTime;
            sr.color = Mathf.Repeat(Time.time * 10f, 1f) > 0.5f ? Color.white : baseColor;
            if (invulnTimer <= 0f) sr.color = baseColor;
        }
    }

    public void TakeDamage(int amount)
    {
        if (invulnTimer > 0f) return;
        health -= amount;
        invulnTimer = invulnerableTime;
        if (health <= 0) Die();
    }

    void Die()
    {
        Attempts++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = Mathf.RoundToInt(Screen.height * 0.04f);
            style.normal.textColor = Color.white;
        }
        GUI.Label(new Rect(10, 10, Screen.width, Screen.height * 0.08f),
                  "HP: " + health + "   Attempt: " + Attempts, style);
    }
}