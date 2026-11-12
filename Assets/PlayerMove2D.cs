using UnityEngine;

// Движение: WASD/стрелки на ПК, перетаскивание пальцем на телефоне.
// Нужно: Project Settings -> Player -> Active Input Handling = Both
public class PlayerMove2D : MonoBehaviour
{
    [SerializeField] float speed = 4f;

    Rigidbody2D rb;
    Vector2 input;
    Vector2 touchStart;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) touchStart = t.position;

            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                input = Vector2.zero;
            }
            else
            {
                Vector2 d = t.position - touchStart;
                float dead = Screen.height * 0.03f;
                input = d.magnitude > dead ? d.normalized : Vector2.zero;
            }
        }
        else
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }
}