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
        [Tooltip("突進など、BodyInfoを使わない攻撃はここにHitBoxを入れましょう")]
        public GameObject hitBox;
    }

    /// <summary>
    /// アニメーションごとのボディの設定    
    /// 0:idle
    /// 1:右パンチ
    /// 2:左パンチ
    /// 3:右キック
    /// 4:左キック
    /// 5:突進
    /// </summary>
    [SerializeField] Body[] AnimationBody;

    /// <summary>
    /// ボスのアニメーションを動かします
    /// 0:idle
    /// 1:右パンチ
    /// 2:左パンチ
    /// 3:右キック
    /// 4:左キック
    /// 5:突進
    /// </summary>
    public int bossState = 0;


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

    public bool IsIdle()
    {
        AnimatorStateInfo animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.IsName("P_Idle") && !m_Animator.IsInTransition(0);
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
        Body body = AnimationBody[animationNumber];
        if (body.bodyInfo != null)
            body.bodyInfo.SetHitboxOn();       // パンチ・キックなど
        else if (body.hitBox != null)
            body.hitBox.SetActive(true);       // 突進(部位関係無い攻撃)
    }

    public void SetHitBoxOff(int animationNumber)
    {
        Body body = AnimationBody[animationNumber];
        if (body.bodyInfo != null)
            body.bodyInfo.SetHitboxOff();       // パンチ・キックなど
        else if (body.hitBox != null)
            body.hitBox.SetActive(false);       // 突進(部位関係無い攻撃)
    }

    public void PlayPunchSE()
    {
        SoundManager.playSound.PlayBossPunchSE();
    }

    public void PlayKickSE()
    {
        SoundManager.playSound.PlayBossKickSE();
    }

    public bool IsPlaying(string stateName)
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

}
