using JetBrains.Annotations;
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
    /// <para>2:攻撃01</para>
    /// <para>3:攻撃02</para>
    /// <para>4:攻撃03</para>
    /// <para>5:攻撃04</para>
    /// <para>6:玉留めフィニッシュ</para>
    /// <para>7:転ぶ</para>
    /// <para>8:吹っ飛ばされる</para>
    /// <para>9:引っ張られる</para>
    /// <para>10:のけぞり</para>
    /// <para>11:前回避</para>
    /// <para>12:後ろ回避</para>
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
