using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
   private int previousLocationsToStore = 5; // How many previous locations to remember

   [SerializeField]
   private float previousLocationMinDistance = 5f; // Minimum distance from previous locations

   private bool isEscaping = false;
   private float stuckCheckTime = 0.5f;
   private float lastMovedPosition;
   private float stuckTimer;
   
   private List<Vector3> previousLocations = new List<Vector3>();

   void Start()
   {
       agent = GetComponent<NavMeshAgent>();
       player = GameObject.FindGameObjectWithTag("Player");
       
       if (player == null)
       {
           Debug.LogError("Player not found! Make sure your player object has the 'Player' tag.");
           return;
       }
       
       lastMovedPosition = transform.position.magnitude;
       // Add starting position to previous locations
       AddToPreviousLocations(transform.position);
       MoveToRandomPosition();
   }

   void Update()
   {
       float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

       if (distanceToPlayer < minimumPlayerDistance)
       {
           if (!isEscaping)
           {
               isEscaping = true;
               EscapeFromPlayer();
           }
           else
           {
               CheckIfStuck();
           }
       }
       else if (isEscaping && distanceToPlayer >= minimumPlayerDistance)
       {
           isEscaping = false;
           ResetStuckCheck();
           MoveToRandomPosition();
       }
       else if (!isEscaping && !isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
       {
           StartCoroutine(WaitAndMove());
       }
   }

   private void AddToPreviousLocations(Vector3 location)
   {
       previousLocations.Add(location);
       if (previousLocations.Count > previousLocationsToStore)
       {
           previousLocations.RemoveAt(0); // Remove oldest location
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

   private void CheckIfStuck()
   {
       float currentPosition = transform.position.magnitude;
       
       if (Mathf.Abs(currentPosition - lastMovedPosition) < 0.1f)
       {
           stuckTimer += Time.deltaTime;
           if (stuckTimer >= stuckCheckTime)
           {
               Debug.Log("Stuck while escaping, reverting to random movement");
               isEscaping = false;
               ResetStuckCheck();
               MoveToRandomPosition();
           }
       }
       else
       {
           ResetStuckCheck();
       }
       
       lastMovedPosition = currentPosition;
   }

   private void ResetStuckCheck()
   {
       stuckTimer = 0f;
       lastMovedPosition = transform.position.magnitude;
   }

   private void EscapeFromPlayer()
   {
       Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
       Vector3 escapePoint = transform.position + directionAwayFromPlayer * (moveRadius * 2);
       
       NavMeshHit hit;
       if (NavMesh.SamplePosition(escapePoint, out hit, moveRadius, NavMesh.AllAreas))
       {
           agent.SetDestination(hit.position);
           ResetStuckCheck();
           Debug.Log("Escaping from player!");
       }
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
               // Check both player distance and previous locations
               if (!IsPositionTooCloseToPlayer(hit.position) && !IsTooCloseToOldLocations(hit.position))
               {
                   agent.SetDestination(hit.position);
                   AddToPreviousLocations(hit.position); // Add new destination to history
                   Debug.Log($"Moving to new position: {hit.position}");
                   return;
               }
               else
               {
                   Debug.Log("Position too close to player or previous locations, trying again");
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
           AddToPreviousLocations(hit.position); // Add fallback position to history
           Debug.Log($"Moving to fallback position: {hit.position}");
       }
   }

   private IEnumerator WaitAndMove()
   {
       isWaiting = true;
       Debug.Log("Waiting at current position");
       
       float waitTime = Random.Range(minWaitTime, maxWaitTime);
       yield return new WaitForSeconds(waitTime);
       
       if (!isEscaping)
       {
           MoveToRandomPosition();
       }
       isWaiting = false;
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