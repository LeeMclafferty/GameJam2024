using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseControler : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isWaiting = false;
    private GameObject player;
    private bool isEscapingToHole = false;

    public bool isMirrorMouse = false;

    private Animator animator; // Reference to the Animator component
    private float originalSpeed; // Variable to store the original speed

    [SerializeField]
    private float minWaitTime = 1f;

    [SerializeField]
    private float maxWaitTime = 3f;

    [SerializeField]
    private float moveRadius = 10f;

    [SerializeField]
    private float minimumPlayerDistance = 5f;

    [SerializeField]
    private float holeReachDistance = 1f; // Distance to consider "reached" the hole

    [SerializeField]
    private int previousLocationsToStore = 5;

    [SerializeField]
    private float previousLocationMinDistance = 5f;

    private List<Vector3> previousLocations = new List<Vector3>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get the Animator component
        originalSpeed = agent.speed; // Store the original speed
        player = GameObject.FindGameObjectWithTag("Player");
        Invoke("MakeVisible", 3f);

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure your player object has the 'Player' tag.");
            return;
        }

        AddToPreviousLocations(transform.position);
        MoveToRandomPosition();
        animator.SetBool("isMoving", false); // Set to idle initially
    }
    void MakeVisible()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.white);
    }
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (isEscapingToHole)
        {
            CheckHoleReached();
        }
        else if (distanceToPlayer < minimumPlayerDistance)
        {
            EscapeToMouseHole();
        }
        else if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }

        // Update animator based on current state
        UpdateAnimationState();
    }

    private void EscapeToMouseHole()
    {
        GameObject[] mouseHoles = GameObject.FindGameObjectsWithTag("Mouse Hole");

        if (mouseHoles.Length > 0)
        {
            GameObject targetHole = mouseHoles[Random.Range(0, mouseHoles.Length)];
            agent.speed = originalSpeed * 1.5f; // Increase speed by 1.5x
            agent.SetDestination(targetHole.transform.position);
            isEscapingToHole = true;
            animator.SetTrigger("flee"); // Trigger flee animation
            Debug.Log("Escaping to mouse hole!");
        }
        else
        {
            Debug.LogWarning("No mouse holes found in the scene!");
            MoveToRandomPosition();
        }
    }

    private void CheckHoleReached()
    {
        if (!agent.pathPending && agent.remainingDistance <= holeReachDistance)
        {
            Debug.Log("Reached mouse hole, despawning!");
            agent.speed = originalSpeed; // Reset speed to original
            Destroy(gameObject); // Despawn the object
        }
    }

    private void AddToPreviousLocations(Vector3 location)
    {
        previousLocations.Add(location);
        if (previousLocations.Count > previousLocationsToStore)
        {
            previousLocations.RemoveAt(0);
        }
    }

    private bool IsTooCloseToOldLocations(Vector3 position)
    {
        foreach (Vector3 oldLocation in previousLocations)
        {
            if (Vector3.Distance(position, oldLocation) < previousLocationMinDistance)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPositionTooCloseToPlayer(Vector3 position)
    {
        float distanceToPlayer = Vector3.Distance(position, player.transform.position);
        return distanceToPlayer < minimumPlayerDistance;
    }

    private void MoveToRandomPosition()
    {
        int maxAttempts = 30;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            NavMeshHit hit;
            Vector3 randomPosition = Random.insideUnitSphere * moveRadius;
            randomPosition += transform.position;

            if (NavMesh.SamplePosition(randomPosition, out hit, moveRadius, NavMesh.AllAreas))
            {
                if (!IsPositionTooCloseToPlayer(hit.position) && !IsTooCloseToOldLocations(hit.position))
                {
                    agent.SetDestination(hit.position);
                    AddToPreviousLocations(hit.position);
                    animator.SetBool("isMoving", true); // Start moving animation
                    Debug.Log($"Moving to new position: {hit.position}");
                    return;
                }
            }
            attempts++;
        }

        Debug.LogWarning("Could not find valid position after " + maxAttempts + " attempts");
        FallbackRandomPosition();
    }

    private void FallbackRandomPosition()
    {
        NavMeshHit hit;
        Vector3 randomPosition = Random.insideUnitSphere * moveRadius;
        randomPosition += transform.position;

        if (NavMesh.SamplePosition(randomPosition, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            AddToPreviousLocations(hit.position);
            animator.SetBool("isMoving", true); // Start moving animation
            Debug.Log($"Moving to fallback position: {hit.position}");
        }
    }

    private IEnumerator WaitAndMove()
    {
        isWaiting = true;
        Debug.Log("Waiting at current position");

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        if (!isEscapingToHole)
        {
            MoveToRandomPosition();
        }
        isWaiting = false;
    }

    private void UpdateAnimationState()
    {
        if (agent.velocity.magnitude <= 0.1f)
        {
            animator.SetBool("isMoving", false); // Stop moving animation
        }
        else
        {
            animator.SetBool("isMoving", true); // Start moving animation
        }
    }

    // Optional: Visualize previous locations in the editor
    private void OnDrawGizmos()
    {
        if (previousLocations != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Vector3 location in previousLocations)
            {
                Gizmos.DrawWireSphere(location, previousLocationMinDistance);
            }
        }
    }
}
