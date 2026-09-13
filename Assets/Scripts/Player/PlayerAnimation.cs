using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator m_Animator;
    public bool[] isAnimationActive;

    /// <summary>
    /// アニメーションを動かします 
    /// <para>0:idle</para>
    /// <para>1:歩く</para>
    /// <para>2:任意のアニメーション</para>
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
        if (SetAnimation[0] == true)    // idle
        {
            m_Animator.SetInteger("AnimationTransition", 0);
        }
        else if (SetAnimation[2] == true)    // Attack1
        {
            m_Animator.SetInteger("AnimationTransition", 2);
        }
        else if (SetAnimation[1] == true)    // walk
        {
            m_Animator.SetInteger("AnimationTransition", 1);
        }
        else
            m_Animator.SetInteger("AnimationTransition", 0);

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
}
