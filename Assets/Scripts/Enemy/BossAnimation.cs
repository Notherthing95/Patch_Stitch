using UnityEngine;
using UnityEngine.Playables;

public class BossAnimation : MonoBehaviour
{
    public Animator m_Animator;

    /// <summary>
    /// ボスのアニメーションを動かします
    /// 0:idle
    /// 1:右パンチ
    /// 2:左パンチ
    /// </summary>
    static public int bossState = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetInteger("AnimationTransition", bossState);
        Debug.Log("NowState: " + bossState);
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
