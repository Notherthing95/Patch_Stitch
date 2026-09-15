using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator m_Animator;

    /// <summary>
    /// アニメーションを動かします 
    /// <para>0:idle</para>
    /// <para>1:歩く</para>
    /// <para>2:ダッシュ</para>
    /// <para>3:攻撃01</para>
    /// <para>4:攻撃02</para>
    /// <para>5:攻撃03</para>
    /// <para>6:攻撃04</para>
    /// <para>7:玉留めフィニッシュ</para>
    /// <para>8:転ぶ</para>
    /// <para>9:吹っ飛ばされる</para>
    /// <para>10:引っ張られる</para>
    /// <para>11:のけぞり</para>
    /// <para>12:前回避</para>
    /// <para>13:後ろ回避</para>
    /// 
    /// </summary>
    static public int targetState = 0;
    [SerializeField] int amountAnimation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        m_Animator.SetInteger("AnimationTransition", targetState);
        Debug.Log("TargetState: " + targetState);

        if (CheckNormalizedAnimation("P_Dodge_Forward"))
            SetIdle();

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
