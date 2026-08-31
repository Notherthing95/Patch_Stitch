using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator m_Animator;

    /// <summary>
    /// アニメーションを動かします 
    /// <para>0:歩く</para>
    /// <para>1:任意のアニメーション</para>
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
        if (SetAnimation[0] == true)
        {
            m_Animator.SetBool("isWalk", true);
        }
        else
            m_Animator.SetBool("isWalk", false);

        if (SetAnimation[1] == true)
        {
            m_Animator.SetBool("isAttack01", true);
        }
        else
            m_Animator.SetBool("isAttack01", false);



    }
}
