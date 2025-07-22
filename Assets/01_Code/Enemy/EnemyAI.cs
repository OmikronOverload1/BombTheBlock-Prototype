using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Stuff")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    public float walkPointRange;
    private bool walkPointSet;
    private Vector3 walkPoint;

    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float damageAmount = 10f;

    [Header("Melee")]
    [SerializeField] private Collider meleeAttackCollider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip patrolClip;
    public AudioClip chaseClip;

    private void Awake()
    {
        player = GameObject.Find("PlayerObject").transform;
        agent = GetComponent<NavMeshAgent>();

        // Falls der AudioSource nicht über den Inspector zugewiesen wurde, holen wir ihn vom gleichen GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        ManageEnemyStates();
    }

    private void WalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) WalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }

        if (audioSource != null && patrolClip != null && audioSource.clip != patrolClip)
        {
            audioSource.Stop();
            audioSource.clip = patrolClip;
            audioSource.Play();
        }
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);

        if (audioSource != null && chaseClip != null && audioSource.clip != chaseClip)
        {
            audioSource.Stop();
            audioSource.clip = chaseClip;
            audioSource.Play();
        }
    }

    private void Attacking()
    {
        // Stoppe jegliche Audios beim Angriff
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log("Attacking() called.");

            if (meleeAttackCollider != null)
            {
                bool isPlayerInCollider = meleeAttackCollider.bounds.Contains(player.position);
                Debug.Log("Player in melee collider: " + isPlayerInCollider);

                if (isPlayerInCollider)
                {
                    Debug.Log("Melee Attack: Player hit!");
                    player.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
                }
                else
                {
                    Debug.Log("Melee Attack: Player is NOT in melee range.");
                }
            }
            else
            {
                Debug.LogWarning("Melee collider is NOT assigned in EnemyAI script.");
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttacking), timeBetweenAttacks);
        }
    }

    private void ResetAttacking()
    {
        alreadyAttacked = false;
    }

    private void ManageEnemyStates()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) Chasing();
        if (playerInSightRange && playerInAttackRange) Attacking();
    }
}