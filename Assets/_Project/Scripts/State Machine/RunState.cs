using UnityEngine;

namespace CarRace
{
    public class RunState : BaseState
    {
        public RunState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            //Debug.Log("Entering Run State");
            animator.CrossFade(RunHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleRun();
            player.HandleMovement();
        }
    }
}