using UnityEngine;

// Враг 1: Patroller. Состояния: Patrol -> Chase -> WindUp -> Cooldown
public class Patroller2D : MonoBehaviour
{
    enum State { Patrol, Chase, WindUp, Cooldown }

    [Header("Movement")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float chaseSpeed = 3.5f;

    [Header("Vision")]
    [SerializeField] float visionRange = 6f;
    [SerializeField] float visionAngle = 90f;
    [SerializeField] LayerMask obstacleMask;   // слой Walls
    [SerializeField] float loseSightTime = 3f;

    [Header("Attack")]
    [SerializeField] float hitRange = 1.8f;   // радиус, в котором удар попадает
    [SerializeField] float attackRange = 1f;
    [SerializeField] float windUpTime = 0.6f;
    [SerializeField] float cooldownTime = 1.2f;
    [SerializeField] int damage = 1;

    Transform player;
    SpriteRenderer sr;
    Color baseColor;

    State state = State.Patrol;
    Vector2 facing = Vector2.right;
    int wpIndex;
    float timer;
    float lostTimer;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        baseColor = sr.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnterPatrol();
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Chase: UpdateChase(); break;
            case State.WindUp: UpdateWindUp(); break;
            case State.Cooldown: UpdateCooldown(); break;
        }
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 pos = transform.position;
        Vector2 dir = target - pos;
        if (dir.sqrMagnitude > 0.001f) facing = dir.normalized;
        transform.position = Vector2.MoveTowards(pos, target, speed * Time.deltaTime);
    }

    void EnterPatrol()
    {
        state = State.Patrol;
        sr.color = baseColor;
    }

    void UpdatePatrol()
    {
        if (CanSeePlayer()) { EnterChase(); return; }
        if (waypoints.Length == 0) return;

        Vector2 target = waypoints[wpIndex].position;
        MoveTowards(target, patrolSpeed);
        if (Vector2.Distance(transform.position, target) < 0.1f)
            wpIndex = (wpIndex + 1) % waypoints.Length;
    }

    void EnterChase()
    {
        state = State.Chase;
        lostTimer = 0f;
    }

    void UpdateChase()
    {
        MoveTowards(player.position, chaseSpeed);

        if (CanSeePlayer()) lostTimer = 0f;
        else lostTimer += Time.deltaTime;

        if (lostTimer > loseSightTime) { EnterPatrol(); return; }

        if (Vector2.Distance(transform.position, player.position) < attackRange)
            EnterWindUp();
    }

    void EnterWindUp()
    {
        state = State.WindUp;
        timer = windUpTime;
        sr.color = Color.red;   // телеграф атаки
    }

    void UpdateWindUp()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (Vector2.Distance(transform.position, player.position) < attackRange * hitRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }
        EnterCooldown();
    }

    void EnterCooldown()
    {
        state = State.Cooldown;
        timer = cooldownTime;
        sr.color = baseColor;
    }

    void UpdateCooldown()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) EnterChase();
    }

    bool CanSeePlayer()
    {
        Vector2 origin = transform.position;
        Vector2 toPlayer = (Vector2)player.position - origin;
        float dist = toPlayer.magnitude;

        if (dist > visionRange) return false;
        if (Vector2.Angle(facing, toPlayer) > visionAngle * 0.5f) return false;
        return !Physics2D.Raycast(origin, toPlayer.normalized, dist, obstacleMask);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}