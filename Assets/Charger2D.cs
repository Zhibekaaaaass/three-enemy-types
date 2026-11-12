using UnityEngine;

// Враг 3: Charger. Состояния: Idle -> WindUp -> Dash -> Recover
public class Charger2D : MonoBehaviour
{
    enum State { Idle, WindUp, Dash, Recover }

    [Header("Vision")]
    [SerializeField] float visionRange = 7f;
    [SerializeField] LayerMask obstacleMask;   // слой Walls

    [Header("Charge")]
    [SerializeField] float windUpTime = 0.7f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashTime = 0.6f;
    [SerializeField] float recoverTime = 1.5f;
    [SerializeField] float hitRadius = 0.8f;
    [SerializeField] float wallCheckDistance = 0.5f;
    [SerializeField] int damage = 1;

    Transform player;
    SpriteRenderer sr;
    Color baseColor;

    State state = State.Idle;
    Vector2 dashDir;
    float timer;
    bool hitDone;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        baseColor = sr.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle: UpdateIdle(); break;
            case State.WindUp: UpdateWindUp(); break;
            case State.Dash: UpdateDash(); break;
            case State.Recover: UpdateRecover(); break;
        }
    }

    void UpdateIdle()
    {
        if (CanSeePlayer()) EnterWindUp();
    }

    void EnterWindUp()
    {
        state = State.WindUp;
        timer = windUpTime;
        sr.color = Color.red;
        // направление фиксируем сейчас, чтобы игрок успел увернуться
        dashDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
    }

    void UpdateWindUp()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) EnterDash();
    }

    void EnterDash()
    {
        state = State.Dash;
        timer = dashTime;
        hitDone = false;
    }

    void UpdateDash()
    {
        timer -= Time.deltaTime;
        float step = dashSpeed * Time.deltaTime;

        if (Physics2D.Raycast(transform.position, dashDir, step + wallCheckDistance, obstacleMask))
        {
            EnterRecover();
            return;
        }

        transform.position += (Vector3)(dashDir * step);

        if (!hitDone && Vector2.Distance(transform.position, player.position) < hitRadius)
        {
            hitDone = true;
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }

        if (timer <= 0f) EnterRecover();
    }

    void EnterRecover()
    {
        state = State.Recover;
        timer = recoverTime;
        sr.color = Color.gray;   // оглушён
    }

    void UpdateRecover()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            sr.color = baseColor;
            state = State.Idle;
        }
    }

    bool CanSeePlayer()
    {
        Vector2 origin = transform.position;
        Vector2 toPlayer = (Vector2)player.position - origin;
        float dist = toPlayer.magnitude;

        if (dist > visionRange) return false;
        return !Physics2D.Raycast(origin, toPlayer.normalized, dist, obstacleMask);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}