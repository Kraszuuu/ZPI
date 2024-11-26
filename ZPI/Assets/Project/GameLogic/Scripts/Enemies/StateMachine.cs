using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    private Animator animator;

    public void Initialize()
    {
        ChangeState(new PatrolState());
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeState != null)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if (activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;
        if (activeState != null)
        {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }

    public void ResetCurrentState()
    {
        activeState.Exit();
        activeState.Enter();
    }

    public void SetAnimatorTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetAnimatorFloat(string floatName, float value)
    {
        animator.SetFloat(floatName, value);
    }

    public void SetAnimatorBool(string boolName, bool value)
    {
        animator.SetBool(boolName, value);
    }

    public void SetAnimatorInteger(string integerName, int value)
    {
        animator.SetInteger(integerName, value);
    }
}
