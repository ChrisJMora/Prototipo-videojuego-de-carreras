using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace CarRace
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] NavMeshAgent agent;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float frontViewDistance = 5f;
        [SerializeField] float normalSpeed = 6f;
        [SerializeField] float rotationStep = 10f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Run Settings")]
        [SerializeField] float runSpeed = 10f;
        [SerializeField] float runDuration = 1f;
        [SerializeField] float runCooldown = 0f;

        float angle;
        Vector3 target;
        float currentSpeed;

        List<Timer> timers;
        CountdownTimer runTimer;
        CountdownTimer runCooldownTimer;

        StateMachine stateMachine;

        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            SetUpTimers();
            SetUpStateMachine();

        }

        private void SetUpStateMachine()
        {
            // State Machine
            stateMachine = new StateMachine();

            // Declare States
            var locomotionState = new LocomotionState(this, animator);
            var runState = new RunState(this, animator);

            // Define Transitions
            At(locomotionState, runState, new FuncPredicate(() => runTimer.IsRunning));

            Any(locomotionState, new FuncPredicate(() => !runTimer.IsRunning));

            // Set initial state
            stateMachine.SetState(locomotionState);
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

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Start()
        {
            input.EnablePlayerActions();
            currentSpeed = normalSpeed;
        }

        void OnEnable()
        {
            input.Run += OnRun;
            input.TurnRight += OnTurnRight;
            input.TurnLeft += OnTurnLeft;
        }

        void OnDisable()
        {
            input.Run -= OnRun;
            input.TurnRight -= OnTurnRight;
            input.TurnLeft -= OnTurnLeft;
        }

        void Update()
        {
            stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();

            HandleRun();
        }

        public void OnTurnRight(bool performed)
        {
            if (performed) angle -= rotationStep;
        }

        public void OnTurnLeft(bool performed)
        {
            if (performed) angle += rotationStep;
        }

        void OnRun(bool performed)
        {
            if (performed && !runTimer.IsRunning && !runCooldownTimer.IsRunning)
            {
                runTimer.Start();
            }
            else if (!performed && runTimer.IsRunning)
            {
                runTimer.Stop();
            }
        }

        public void HandleRun()
        {
            if (!runTimer.IsRunning)
            {
                currentSpeed = normalSpeed;
                runTimer.Stop();
                return;
            }
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
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
            float newX = transform.localPosition.x + frontViewDistance * Mathf.Cos(angleInRadians);
            float newZ = transform.localPosition.z + frontViewDistance * Mathf.Sin(angleInRadians);
            target = new Vector3(newX, transform.localPosition.y, newZ);
        }
    }
}
