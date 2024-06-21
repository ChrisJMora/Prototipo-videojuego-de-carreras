using UnityEngine;
using UnityEngine.AI;
using KBCore.Refs;
using System.Collections.Generic;
using Utilities;

namespace CarRace
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Enemy : Entity
    {
        [Header("References")]
        [SerializeField, Self] NavMeshAgent agent;
        [SerializeField, Child] Animator animator;

        [Header("Movement Settings")]
        [SerializeField] float frontViewDistance = 5f;
        [SerializeField] float normalSpeed = 6f;
        [SerializeField] float rotationStep = 10f;

        [Header("Run Settings")]
        [SerializeField] float runSpeed = 10f;
        [SerializeField] float runDuration = 1f;
        [SerializeField] float runCooldown = 0f;

        [Header("Rotation Settings")]

        float angle;
        Vector3 target;
        float currentSpeed;

        List<Timer> timers;
        CountdownTimer runTimer;
        CountdownTimer runCooldownTimer;

        StateMachine stateMachine;

        void OnValidate() => this.ValidateRefs();

        void Awake()
        {
            SetUpTimers();
            SetUpStateMachine();
        }

        void Start()
        {
            currentSpeed = normalSpeed;
        }

        private void SetUpTimers()
        {
            // Setup timers
            runTimer = new CountdownTimer(runDuration);
            runCooldownTimer = new CountdownTimer(runCooldown);


            runTimer.OnTimeStart += () => currentSpeed = runSpeed;
            runTimer.OnTimeStop += () => runCooldownTimer.Start();

            timers = new List<Timer>(capacity: 2) {
                runTimer, runCooldownTimer};
        }

        private void SetUpStateMachine()
        {
            // State Machine
            stateMachine = new StateMachine();

            // Declare States
            var locomotionState = new EnemyLocomotionState(this, animator);
            var runState = new EnemyRunState(this, animator);

            // Add Transitions
            At(locomotionState, runState, new FuncPredicate(() => runTimer.IsRunning));

            // Set initial state
            stateMachine.SetState(locomotionState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Update()
        {
            stateMachine.Update();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void DetectWalls()
        {
            Collider[] hitWalls = Physics.OverlapSphere(target + Vector3.up, frontViewDistance);

            foreach (var wall in hitWalls)
            {
                if (wall.CompareTag("Wall"))
                {
                    angle += rotationStep;
                }
            }
        }

        public void HandleMovement()
        {
            UpdateTarget();
            HandleHorizontalMovement();
        }

        void HandleHorizontalMovement()
        {
            agent.speed = currentSpeed;
            agent.SetDestination(target);
        }

        public void UpdateTarget()
        {
            var angleInRadians = angle * Mathf.Deg2Rad; // Convert to radians
            float newX = transform.position.x + frontViewDistance * Mathf.Cos(angleInRadians);
            float newZ = transform.position.z + frontViewDistance * Mathf.Sin(angleInRadians);
            target = new Vector3(newX, transform.position.y, newZ);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target, 0.5f);
        }
    }
}
