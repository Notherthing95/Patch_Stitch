using JetBrains.Annotations;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator m_Animator;

    private PlayerMoveController _playerMoveController;

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
    static public int playerState = 0;

    ///// <summary>
    ///// プレイヤーのアニメーション総数
    ///// </summary>
    //[SerializeField] int amountAnimation;

    /// <summary>
    /// 一回だけ再生して待機に戻すアニメーションステート
    /// </summary>
    static readonly string[] OneShotStates =
    {
    "P_Attack01",        // 3 攻撃01
    "P_TamadomeFINISH",  // 7 玉留め
    "P_Knockdown01",     // 8 転ぶ
    "P_Knockdown02",     // 9 吹っ飛び
    "P_Pulled",          // 10 引っ張られ
    "P_HitReact",        // 11 のけぞり
    "P_Dodge_Forward",   // 12 前回避
    "P_Dodge_Backward",  // 13 後ろ回避
    };

    [Header("玉留めのスロー演出の長さ")]
    [SerializeField] float slowThrowScale = 0.7f;

    [Tooltip("EndSlowの保険")]
    [SerializeField] float maxSlowTime = 2f;

    private float _defaultFixedDeltaTime;
    private bool _isSlow = false;
    private float _slowTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        _playerMoveController = GetComponentInParent<PlayerMoveController>();
        _defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {

        m_Animator.SetInteger("AnimationTransition", playerState);
        foreach (string stateName in OneShotStates)
        {
            if (CheckNormalizedAnimation(stateName))
            {
                SetIdle();
            }
        }
        if (_isSlow)
        {
            _slowTimer += Time.unscaledDeltaTime;   // スローの影響を受けない、本当の経過時間
            if (_slowTimer >= maxSlowTime)
                EndSlow();
        }
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
        playerState = 0;
        
    }

    /// <summary>
    /// 攻撃アニメーションを最初から再生する
    /// </summary>
    public void PlayAttack()
    {
        playerState = 3;
        m_Animator.Play("P_Attack01", 0, 0f);
    }

    public void StartSlow()
    {
        Time.timeScale = slowThrowScale;
        Time.fixedDeltaTime = _defaultFixedDeltaTime * slowThrowScale;
        m_Animator.updateMode = AnimatorUpdateMode.UnscaledTime; // プレイヤーのアニメーションは通常の速さになる
        _isSlow = true;
        _slowTimer = 0f;

        SoundManager.playSound.PlayTamadomeStartSE();
    }

    public void EndSlow()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultFixedDeltaTime;
        m_Animator.updateMode = AnimatorUpdateMode.Normal;
        _isSlow = false;

        if (SoundManager.playSound != null)
            SoundManager.playSound.PlayTamadomeFinishSE();
    }

    public void StartDodgeMove()
    {
        //if (_playerMoveController != null)
        //    _playerMoveController.StartDodgeMove();
    }

    public void PlayWalkSE()
    {
        SoundManager.playSound.PlayPlayerWalkSE();
    }

    void OnDestroy()
    {
        if (_isSlow)
            EndSlow();
    }
}
