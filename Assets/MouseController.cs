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

   private bool isEscaping = false;
   private float stuckCheckTime = 0.5f; // Time to wait before checking if stuck
   private float lastMovedPosition;
   private float stuckTimer;

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
       MoveToRandomPosition();
   }

   void Update()
   {
       float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

       // Check if we're too close to player
       if (distanceToPlayer < minimumPlayerDistance)
       {
           if (!isEscaping)
           {
               isEscaping = true;
               EscapeFromPlayer();
           }
           else
           {
               // Check if we're stuck while trying to escape
               CheckIfStuck();
           }
       }
       // If we were escaping and now we're far enough, resume normal behavior
       else if (isEscaping && distanceToPlayer >= minimumPlayerDistance)
       {
           isEscaping = false;
           ResetStuckCheck();
           MoveToRandomPosition();
       }
       // Normal movement behavior
       else if (!isEscaping && !isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
       {
           StartCoroutine(WaitAndMove());
       }
   }

   private void CheckIfStuck()
   {
       float currentPosition = transform.position.magnitude;
       
       // If we haven't moved significantly
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
       // Calculate direction from player to this object (opposite of direction to player)
       Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
       
       // Calculate a point in that direction
       Vector3 escapePoint = transform.position + directionAwayFromPlayer * (moveRadius * 2);
       
       // Find valid NavMesh position
       NavMeshHit hit;
       if (NavMesh.SamplePosition(escapePoint, out hit, moveRadius, NavMesh.AllAreas))
       {
           agent.SetDestination(hit.position);
           ResetStuckCheck(); // Reset stuck check when starting new escape
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
               if (!IsPositionTooCloseToPlayer(hit.position))
               {
                   agent.SetDestination(hit.position);
                   Debug.Log($"Moving to new position: {hit.position}");
                   return;
               }
               else
               {
                   Debug.Log("Position too close to player, trying again");
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
       
       if (!isEscaping) // Only move to random position if not escaping
       {
           MoveToRandomPosition();
       }
       isWaiting = false;
   }
}