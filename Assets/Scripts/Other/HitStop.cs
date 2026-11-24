using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    /// <summary>
    /// スクリプトを取り込まなくてもHitStopを呼べる魔法の記述
    /// </summary>
    public static HitStop instance { get; private set; }


    [Header("通常攻撃でのヒットストップ秒数")]
    public float normalHitStopTime = 0.06f;
    [Header("玉留めフィニッシュでのヒットストップ秒数")]
    public float finishHitStopTime = 0.15f;

    /// <summary>
    /// 止めているAnimatorと、止める前の速さの保持
    /// </summary>
    Dictionary<Animator, float> _stopped = new Dictionary<Animator, float>();

    /// <summary>
    /// 残りの停止時間
    /// </summary>
    float _timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopped.Count == 0)
            return;

        _timer -= Time.unscaledDeltaTime;
        if (_timer <= 0f)
            Release();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void PlayHitStop(float duration, params Animator[] animators)
    {
        foreach(Animator animator in animators)
        {
            if (animator == null)
                continue;
            if (!_stopped.ContainsKey(animator))
            {
                _stopped.Add(animator, animator.speed);
                animator.speed = 0f;
            }
        }
        _timer = Mathf.Max(_timer, duration);
    }

    void Release()
    {
        foreach(KeyValuePair<Animator, float> pair in _stopped)
        {
            if (pair.Key != null)
                pair.Key.speed = pair.Value;
        }
        _stopped.Clear();
        _timer = 0f;
    }
}
