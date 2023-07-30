using UnityEngine;

public class FixedAnimationLength : StateMachineBehaviour
{
	public bool isActive = true;
	public float fixedTime = 1;
	public string speedMultiParaName;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isActive)
		{
			animator.SetFloat(speedMultiParaName, stateInfo.length / fixedTime);
		}
	}
}
