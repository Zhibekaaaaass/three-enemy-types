
using UnityEngine;

public class Shooter2D : MonoBehaviour
{
    private enum State
    {
        Idle,
        Aim,
        Reload
    }

    [Header("Vision")]
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Attack")]
    [SerializeField] private float aimTime = 0.8f;
    [SerializeField] private float reloadTime = 1.6f;
    [SerializeField] private int damage = 1;

    private Transform player;
    private PlayerHealth playerHealth;

    private SpriteRenderer sr;
    private LineRenderer line;

    private Color baseColor = Color.white;

    private State state = State.Idle;
    private float timer;

    private void Start()
    {
        Debug.Log("Shooter: Start started");

        // Find enemy sprite
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            baseColor = sr.color;
        }
        else
        {
            Debug.LogWarning("Shooter: SpriteRenderer not found");
        }

        // Find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("Shooter: Player with tag Player not found");
            enabled = false;
            return;
        }

        player = playerObject.transform;

        // Find PlayerHealth
        playerHealth = playerObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = playerObject.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = playerObject.GetComponentInChildren<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("Shooter: PlayerHealth component not found");
            enabled = false;
            return;
        }

        // Create aiming line
        GameObject lineObject = new GameObject("ShooterAimLine");
        lineObject.transform.SetParent(transform, false);

        line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = 0.06f;
        line.endWidth = 0.06f;

        line.startColor = Color.red;
        line.endColor = Color.red;

        line.numCapVertices = 4;
        line.sortingOrder = 20;

        // Use the built-in default material
        line.material = new Material(Shader.Find("Hidden/Internal-Colored"));

        line.enabled = false;

        Debug.Log("Shooter: Ready!");
    }

    private void Update()
    {
        if (player == null || playerHealth == null)
            return;

        switch (state)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Aim:
                UpdateAim();
                break;

            case State.Reload:
                UpdateReload();
                break;
        }
    }

    // IDLE
    private void UpdateIdle()
    {
        if (CanSeePlayer())
        {
            EnterAim();
        }
    }

    // AIM
    private void EnterAim()
    {
        state = State.Aim;
        timer = aimTime;

        if (sr != null)
            sr.color = Color.red;

        if (line != null)
            line.enabled = true;
    }


    private void UpdateAim()
    {
        if (!CanSeePlayer())
        {
            CancelAim();
            return;
        }

        if (line != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, player.position);
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Shoot();
            EnterReload();
        }
    }


    private void CancelAim()
    {
        state = State.Idle;

        if (line != null)
            line.enabled = false;

        if (sr != null)
            sr.color = baseColor;
    }

    // SHOOT
    private void Shoot()
    {
        if (!CanSeePlayer())
            return;

        Debug.Log("Shooter: Shooting! Damage = " + damage);

        playerHealth.TakeDamage(damage);
    }

    // RELOAD
    private void EnterReload()
    {
        state = State.Reload;
        timer = reloadTime;

        if (line != null)
            line.enabled = false;

        if (sr != null)
            sr.color = baseColor;
    }

    private void UpdateReload()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            state = State.Idle;
        }
    }

    // LINE OF SIGHT



    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 origin = transform.position;
        Vector2 target = player.position;

        Vector2 direction = target - origin;
        float distance = direction.magnitude;

        if (distance > visionRange)
            return false;

        // Получаем все объекты на пути луча
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            origin,
            direction.normalized,
            distance
        );

        // Сортируем столкновения от ближайшего к дальнему
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            // Игнорируем самого Shooter и его дочерние объекты
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                continue;

            // Если встретили игрока — он виден
            if (hit.transform == player || hit.transform.IsChildOf(player))
                return true;

            // Если встретили стену — игрок скрыт
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                Debug.Log("Shooter: Wall blocks the player!");
                return false;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
