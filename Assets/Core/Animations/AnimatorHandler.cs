using UnityEngine;


[RequireComponent(typeof(Animator))]
public class AnimatorHandler : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    internal void SetAnimator(RuntimeAnimatorController controller)
    {
        if (controller == null || animator.runtimeAnimatorController == controller)
            return;
        
        animator.runtimeAnimatorController = controller;
    }
    internal void SetAnimatorTrigger(string TriggerName)
    {
        animator.SetTrigger(TriggerName);
    }
    internal void SetAnimatorBool(string BoolName,bool Value)
    {
        animator.SetBool(BoolName,Value);
    }
}
