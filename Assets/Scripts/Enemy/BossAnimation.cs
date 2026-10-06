using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class BossAnimation : MonoBehaviour
{
    public Animator m_Animator;

    /// <summary>
    /// HitBox用
    /// </summary>
    [System.Serializable]
    class Body
    {
        public BodyInfo bodyInfo;
        //public bool[] isHitBoxOn;
    }

    /// <summary>
    /// アニメーションごとのボディの設定    
    /// 0:idle
    /// 1:右パンチ
    /// 2:左パンチ
    /// </summary>
    [SerializeField] Body[] AnimationBody;

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
        //Debug.Log("NowBossState: " + bossState);
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
        bossState = 0;
        //Debug.Log("Idled");
    }
    
    public void SetHitBoxOn(int animationNumber)
    {
        AnimationBody[animationNumber].bodyInfo.SetHitboxOn();
    }

    public void SetHitBoxOff(int animationNumber)
    { 
        AnimationBody[animationNumber].bodyInfo.SetHitboxOff();
    }

}
