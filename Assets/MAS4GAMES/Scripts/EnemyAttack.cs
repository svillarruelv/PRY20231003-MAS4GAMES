using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : StateMachineBehaviour
{
    Enemy enemy;

	public float AttackRange = enemy.AttackRange;

	private Camera _camera; //Variable de cámara del jugador

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		if (Vector3.Distance(_camera.transform.position, enemy.transform.position) <= AttackRange)
		{
			animator.SetTrigger("Attack");
            Debug.Log("PLAYER ATTACKED!");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//Reset trigger to false
        animator.ResetTrigger("Attack");
        Debug.Log("Attack trigger reset to false");
	}
}