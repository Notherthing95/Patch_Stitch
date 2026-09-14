using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator m_Animator;
    public bool[] isAnimationActive;

    /// <summary>
    /// ƒAƒjƒ[ƒVƒ‡ƒ“‚ğ“®‚©‚µ‚Ü‚· 
    /// <para>0:idle</para>
    /// <para>1:•à‚­</para>
    /// <para>2:UŒ‚01</para>
    /// <para>3:UŒ‚02</para>
    /// <para>4:UŒ‚03</para>
    /// <para>5:UŒ‚04</para>
    /// <para>6:UŒ‚01</para>
    /// <para>7:UŒ‚01</para>
    /// <para>8:UŒ‚01</para>
    /// 
    /// </summary>
    public bool[] SetAnimation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int targetState = 0;

        for (int i = SetAnimation.Length - 1; i >= 0; i--)
        {
            if (SetAnimation[i] == true)
            {
                targetState = i;
                break;
            }
        }

        m_Animator.SetInteger("AnimationTransition", targetState);
        Debug.Log("TargetState: " + targetState);

    }

    public bool CheckNormalizedAnimation(string animationName)
    {
        AnimatorStateInfo animatorState = m_Animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.IsName(animationName) && animatorState.normalizedTime >= 1.0f && !m_Animator.IsInTransition(0))
        {
            return true;
        }

        return false;
    }

    public void SetIdle()
    {
        m_Animator.SetInteger("AnimationTransition", 0);
    }
}
