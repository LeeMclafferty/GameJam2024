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
    private float minimumPlayerDistance = 5f;
    
    [SerializeField]
    private float pathCheckInterval = 1f; // How often to check distance along path
    
    [SerializeField]
    private float pathCheckThreshold = 3f; // How close to path is too close

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        
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

    private bool IsPathClearOfPlayer(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(destination, path))
        {
            // Check if we can create a path
            if (path.status != NavMeshPathStatus.PathComplete)
                return false;

            // Get the corners of the path
            Vector3[] corners = path.corners;
            
            // Check each segment of the path
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Vector3 start = corners[i];
                Vector3 end = corners[i + 1];
                float segmentLength = Vector3.Distance(start, end);
                
                // Check points along this path segment
                for (float t = 0; t <= segmentLength; t += pathCheckInterval)
                {
                    Vector3 pointOnPath = Vector3.Lerp(start, end, t / segmentLength);
                    float distanceToPlayer = Vector3.Distance(pointOnPath, player.transform.position);
                    
                    if (distanceToPlayer < pathCheckThreshold)
                    {
                        Debug.Log("Player too close to planned path");
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
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
                // Check if the destination and path are clear of player
                if (!IsPositionTooCloseToPlayer(hit.position) && IsPathClearOfPlayer(hit.position))
                {
                    agent.SetDestination(hit.position);
                    Debug.Log($"Moving to new position: {hit.position}");
                    return;
                }
                else
                {
                    Debug.Log("Position or path too close to player, trying again");
                }
            }
            attempts++;
        }

        Debug.LogWarning("Could not find valid position away from player after " + maxAttempts + " attempts");
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

    // Optionally visualize the path in the editor
    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.red);
            }
        }
    }
}