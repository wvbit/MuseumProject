using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Transform eyePoint;

    [Header("Vision")]
    public float detectionRadius = 12f;

    [Range(0f, 180f)]
    public float visionAngle = 90f;

    [Header("Movement")]
    public float patrolRadius = 20f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;

    [Header("Attack")]
    public float attackRange = 2f;

    [Header("Layers")]
    public LayerMask obstacleMask;

    private NavMeshAgent agent;

    private Vector3 patrolPoint;

    private float idleTimer;

    private bool isPatrolling;
    private bool isIdle;
    private bool gameOver;

    private enum State
    {
        Patrol,
        Chase
    }

    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        SetNewPatrolPoint();

        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null || gameOver)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        // إذا وصل الدكتور للاعب
        if (distanceToPlayer <= attackRange)
        {
            GameOver();
            return;
        }

        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
            currentState = State.Chase;
        else
            currentState = State.Patrol;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                ChasePlayer();
                break;
        }

        if (animator != null)
        {
            animator.SetBool(
                "isWalking",
                agent.velocity.magnitude > 0.1f
            );
        }

        RotateTowardsMovementDirection();
    }

    // =========================
    // رؤية اللاعب
    // =========================

    bool CanSeePlayer()
    {
        if (player == null || eyePoint == null)
            return false;

        Vector3 directionToPlayer =
            player.position - eyePoint.position;

        float distanceToPlayer =
            directionToPlayer.magnitude;

        if (distanceToPlayer > detectionRadius)
            return false;

        float angle = Vector3.Angle(
            eyePoint.forward,
            directionToPlayer
        );

        if (angle > visionAngle / 2f)
            return false;

        if (Physics.Raycast(
            eyePoint.position,
            directionToPlayer.normalized,
            out RaycastHit hit,
            distanceToPlayer,
            obstacleMask))
        {
            return false;
        }

        return true;
    }

    // =========================
    // الدورية
    // =========================

    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= patrolIdleTime)
            {
                idleTimer = 0f;
                SetNewPatrolPoint();
            }

            return;
        }

        if (!isPatrolling ||
            Vector3.Distance(
                transform.position,
                patrolPoint
            ) < 1.5f)
        {
            isIdle = true;
            isPatrolling = false;

            agent.ResetPath();
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection =
            Random.insideUnitSphere *
            patrolRadius +
            transform.position;

        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            patrolRadius,
            NavMesh.AllAreas))
        {
            patrolPoint = hit.position;

            agent.SetDestination(patrolPoint);

            isPatrolling = true;
            isIdle = false;
        }
    }

    // =========================
    // المطاردة
    // =========================

    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        agent.SetDestination(player.position);
    }

    // =========================
    // GAME OVER
    // =========================

    void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        agent.ResetPath();

        Debug.Log("GAME OVER - Doctor caught the player!");

        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // =========================
    // الدوران
    // =========================

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    agent.velocity.normalized
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
        }
    }

    // =========================
    // رسم مدى الرؤية
    // =========================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );

        if (eyePoint != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawRay(
                eyePoint.position,
                eyePoint.forward *
                detectionRadius
            );
        }
    }
}