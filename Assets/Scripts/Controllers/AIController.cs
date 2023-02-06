using System;
using System.Collections;
using Attributes;
using Combat;
using Core;
using Movement;
using UnityEngine;

namespace Controllers
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private float agroCooldownTime = 3f;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private GameObject player;
        private Fighter fighter;
        private Health health;
        private Mover mover;
        Vector3 guardPosition;
        private ActionScheduler actionScheduler;
        [SerializeField] private PatrolPath patrolPath = null;
        [SerializeField] private float waypointTolerance = 1.8f;
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] float shoutDistance = 5f;
        private int currentWaypointIndex = 0;

        //max speed could be multiply in this speed to have different speed for patrol movement and for attack movement
        //the value should be between 0 and 1
        [Range(0, 1)] [SerializeField] float patrolSpeedFraction = 0.2f;

        private void Awake()
        {
            actionScheduler = GetComponent<ActionScheduler>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
        }

        private void Start()
        {
            //start position of the enemy
            guardPosition = transform.position;
        }

        private void Update()
        {
            //if enemy is dead then do nothing from next steps in update func
            if (health.IsDead()) return;

            //if player is within the distance for attack then check if it is not dead and attack
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            //wait for some time before return to the guard position
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            //if player is outside the zone for attack then enemy back to  his patrol behaviour
            else
            {
                PatrolBehaviour();
            }

            //increasing waiting time to know, when to return to guard position
            timeSinceLastSawPlayer += Time.deltaTime;
            //increasing waiting time to know, when to go to next waypoint
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            //increasing waiting time to know, when enemy was attacked last time 
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            //by default enemy is going to his first position where it appears on the scene
            Vector3 nextPosition = guardPosition;
            //if there is patrol path component in enemy object then 
            if (patrolPath != null)
            {
                //if we are at the waypoint
                if (IsAtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    //then get position of next waypoint where to go
                    GetPositionOfNextWaypoint();
                }

                nextPosition = GetPositionOfCurrentWaypoint();
            }

            //go to next waypoint after waiting for some dwell time
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                //AI are patrolling with lower speed that attacking
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        //check if we are close enought to the waypoint position
        private bool IsAtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetPositionOfCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        //get position of current waypoint
        private Vector3 GetPositionOfCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        //get position of next endpoint
        private void GetPositionOfNextWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            
            AggrevateNearbyEnemies();
        }
        
        //enemy that is closed to attacked enemy became aggressive as well and start to attack the player
        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;

                ai.Aggrevate();
            }
        }

        //check if distance is valid for attack or if enemy was recently attacked by player
        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggrevated < agroCooldownTime;
        }

        // Called by Unity. unity api native call
        //allow us to see radius of chase distance on the scene
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
        //used in TakeDamage event in Health component
        public void  Aggrevate()
        {
            timeSinceAggrevated = 0;
        }
    }
}