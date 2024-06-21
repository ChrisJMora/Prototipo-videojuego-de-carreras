using UnityEngine;

namespace CarRace
{
    public class EnemyLocomotionState : EnemyBaseState
    {
        public EnemyLocomotionState(Enemy enemy, Animator animator) : base(enemy, animator) { }
        public override void OnEnter()
        {
            Debug.Log("Enemy Locomotion State");
        }

        public override void Update()
        {
            enemy.DetectWalls();
            enemy.HandleMovement();
        }
    }

    public class EnemyRunState : EnemyBaseState
    {
        public EnemyRunState(Enemy enemy, Animator animator) : base(enemy, animator) { }

    }

    public abstract class EnemyBaseState : IState
    {
        protected readonly Enemy enemy;
        protected readonly Animator animator;

        protected const float crossFadeDuration = 0.1f;

        protected EnemyBaseState(Enemy enemy, Animator animator)
        {
            this.enemy = enemy;
            this.animator = animator;
        }

        public virtual void FixedUpdate()
        {
            // throw new System.NotImplementedException();
        }

        public virtual void OnEnter()
        {
            // throw new System.NotImplementedException();
        }

        public virtual void OnExit()
        {
            // throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            // throw new System.NotImplementedException();
        }
    }
}
