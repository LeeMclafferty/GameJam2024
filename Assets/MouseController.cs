using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class MouseControler : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isWaiting = false;
    private GameObject player;

    [SerializeField]
    private float minWaitTime = 1f;
    
    [SerializeField]
    private float maxWaitTime = 3f;
    
    [SerializeField]
    private float moveRadius = 10f;

    [SerializeField]
    private float minimumPlayerDistance = 5f; // Minimum distance from player

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player"); // Make sure your player has the "Player" tag
        
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure your player object has the 'Player' tag.");
            return;
        }

        MoveToRandomPosition();
    }

    void Update()
    {
        if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }
    }

    private bool IsPositionTooCloseToPlayer(Vector3 position)
    {
        float distanceToPlayer = Vector3.Distance(position, player.transform.position);
        return distanceToPlayer < minimumPlayerDistance;
    }

    private void MoveToRandomPosition()
    {
        int maxAttempts = 30; // Prevent infinite loops
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            NavMeshHit hit;
            Vector3 randomPosition = Random.insideUnitSphere * moveRadius;
            randomPosition += transform.position;
            
            if (NavMesh.SamplePosition(randomPosition, out hit, moveRadius, NavMesh.AllAreas))
            {
                // Check if the position is far enough from the player
                if (!IsPositionTooCloseToPlayer(hit.position))
                {
                    agent.SetDestination(hit.position);
                    Debug.Log($"Moving to new position: {hit.position}");
                    return; // Valid position found, exit the method
                }
                else
                {
                    Debug.Log("Position too close to player, trying again");
                }
            }
            
            attempts++;
        }

        Debug.LogWarning("Could not find valid position away from player after " + maxAttempts + " attempts");
        // If we couldn't find a valid position after max attempts, just pick any valid NavMesh position
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
            Debug.Log($"Moving to fallback position: {hit.position}");
        }
    }

    private IEnumerator WaitAndMove()
    {
        isWaiting = true;
        Debug.Log("Waiting at current position");
        
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);
        
        MoveToRandomPosition();
        isWaiting = false;
    }
}