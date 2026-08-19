using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public PlayerHealth playerHealth;
    public GameObject gameOverUI;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float patrolRadius = 20f;
    public float attackCooldown = 2f;
    public float patrolIdleTime = 3f;
    public float rotationSpeed = 7f;
    public float attackDuration = 1.0f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private float idleTimer;
    private float attackTimer;

    private Vector3 patrolPoint;
    private bool isPatrolling;
    private bool isIdle;
    private bool isAttacking;
    private bool hasDealtDamage; // Prevents double damage in a single strike

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerHealth == null && player != null) playerHealth = player.GetComponent<PlayerHealth>();

        SetNewPatrolPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Cancel attack if player backs out of range
        if (isAttacking && distanceToPlayer > attackRange)
        {
            CancelAttack();
            currentState = State.Chase;
        }

        // Handle active attack state
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            // Deal damage halfway through the attack window if using timed duration
            if (attackTimer <= (attackDuration * 0.5f) && !hasDealtDamage)
            {
                DealDamage();
            }

            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }

        // State Machine evaluation
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
                currentState = State.Attack;
            else if (distanceToPlayer <= detectionRadius)
                currentState = State.Chase;
            else
                currentState = State.Patrol;
        }

        // Execute active state
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: Attack(); break;
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking);

        if (!isAttacking)
            RotateTowardsMovementDirection();
    }

    void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= patrolIdleTime)
            {
                SetNewPatrolPoint();
                idleTimer = 0f;
            }
            return;
        }

        if (!isPatrolling || Vector3.Distance(transform.position, patrolPoint) < 1.5f)
        {
            isIdle = true;
            isPatrolling = false;
            agent.ResetPath();
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(patrolPoint);
                isPatrolling = true;
                isIdle = false;
            }
        }
    }

    void ChasePlayer()
    {
        isIdle = false;
        isPatrolling = false;

        if (agent.isOnNavMesh)
            agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        hasDealtDamage = false;
        cooldownTimer = attackCooldown;
        attackTimer = attackDuration;

        if (agent.isOnNavMesh)
            agent.ResetPath();

        // Rotate instantly towards target
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        if (lookPos != transform.position)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position), Time.deltaTime * rotationSpeed);
        }

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    // Call via Animation Event or automatically triggered via Update timer above
    public void DealDamage()
    {
        if (hasDealtDamage) return;

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            hasDealtDamage = true;

            if (playerHealth != null)
                playerHealth.TakeDamage(50);

            if (gameOverUI != null)
                gameOverUI.SetActive(true);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;

        animator.ResetTrigger("Attack");

        if (animator.HasState(0, Animator.StringToHash("Walk")))
            animator.CrossFade("Walk", 0.1f);

        if (agent.isOnNavMesh)
            agent.SetDestination(player.position);
    }

    void RotateTowardsMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}