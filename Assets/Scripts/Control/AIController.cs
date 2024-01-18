using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
    public class AIController : MonoBehaviour {

        #region Caching variables
        private ActionScheduler actionScheduler;
        private Fighter fighter;
        private Mover mover;
        private GameObject player;
        private Health health;
        #endregion

        [SerializeField]
        private float chaseDistance;

        [SerializeField]
        private float suspiciousTime;

        [SerializeField]
        [Tooltip("If patrolPath equal null, the enemy will not patrol and stay at guardPosition.")]
        private PatrolPath patrolPath;

        [SerializeField]
        [Range(0f, 1f)]
        private float patrolSpeedFraction;

        [SerializeField]
        private float waypointDwellTime;

        private float waypointTolerance = 1f;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int currentWaypointIndex;
        private Vector3 guardPosition;

        private void Awake() {
            actionScheduler = GetComponent<ActionScheduler>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();

            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Start() {
            guardPosition = transform.position;
        }

        private void Update() {
            if (health.IsDeath()) return;

            if (PlayerInAttackRange() && fighter.CanAttack(player)) {
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspiciousTime) {
                SuspiciousBehaviour();
            } else {
                PatrolBehaviour();
            }

            UpdateTimer();
        }

        private bool PlayerInAttackRange() {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        private void AttackBehaviour() {
            timeSinceLastSawPlayer = 0f;
            fighter.StartAttackAction(player);
        }

        private void SuspiciousBehaviour() {
            actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition;

            if (patrolPath != null) {
                if (AtWaypoint()) {
                    CycleWaypoint();
                    timeSinceArrivedAtWaypoint = 0f;
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime) {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint() {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());

            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint() {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypointPosition(currentWaypointIndex);
        }

        private void UpdateTimer() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
