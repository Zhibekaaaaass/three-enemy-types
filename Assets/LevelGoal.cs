using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    bool won;
    GUIStyle label, button;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            won = true;
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        if (!won) return;

        if (label == null)
        {
            label = new GUIStyle(GUI.skin.label);
            label.fontSize = Mathf.RoundToInt(Screen.height * 0.06f);
            label.alignment = TextAnchor.MiddleCenter;
            label.normal.textColor = Color.white;

            button = new GUIStyle(GUI.skin.button);
            button.fontSize = Mathf.RoundToInt(Screen.height * 0.05f);
        }

        float w = Screen.width * 0.6f;
        float x = (Screen.width - w) / 2f;
        GUI.Label(new Rect(x, Screen.height * 0.25f, w, Screen.height * 0.12f),
                  "You win! Attempts: " + PlayerHealth.Attempts, label);

        if (GUI.Button(new Rect(x, Screen.height * 0.45f, w, Screen.height * 0.12f), "Play again", button))
        {
            PlayerHealth.Attempts = 1;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
