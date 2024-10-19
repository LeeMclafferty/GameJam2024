using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class MouseControler : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isWaiting = false;

    [SerializeField]
    private float minWaitTime = 1f;
    
    [SerializeField]
    private float maxWaitTime = 3f;
    
    [SerializeField]
    private float moveRadius = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Start the movement loop
        MoveToRandomPosition();
    }

    void Update()
    {
        // Check if we've reached the destination and we're not already waiting
        if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }
    }

    private void MoveToRandomPosition()
    {
        NavMeshHit hit;
        Vector3 randomPosition = Random.insideUnitSphere * moveRadius;
        randomPosition += transform.position;
        
        // Sample a random position on the NavMesh
        if (NavMesh.SamplePosition(randomPosition, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log($"Moving to new position: {hit.position}"); // Debug log
        }
        else
        {
            Debug.LogWarning("Couldn't find valid position, trying again"); // Debug log
            MoveToRandomPosition();
        }
    }

    private IEnumerator WaitAndMove()
    {
        isWaiting = true;
        Debug.Log("Waiting at current position"); // Debug log
        
        // Wait for a random amount of time
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);
        
        // Move to next random position
        MoveToRandomPosition();
        isWaiting = false;
    }
}