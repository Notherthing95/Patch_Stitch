using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    /// <summary>
    /// ボスの攻撃1つ分の設定
    /// </summary>
    [System.Serializable]
    public class BossAttack
    {
        public string name = "右パンチ";
        [Tooltip("BossAnimation.bossStateに渡す番号 1:右パンチ 2: 左パンチ")]
        public int animationState = 1;
        [Tooltip("Animator のステート名（終了判定用）")]
        public string stateName = "Punch_Right";
        [Tooltip("この攻撃に使う部位。全部治ると使わなくなる")]
        public BodyInfo[] parts;
        [Tooltip("この距離以内なら使う")]
        public float range = 5f;
        [Tooltip("突進など前に進む攻撃だけ 0 より大きくする")]
        public float dashSpeed = 0f;
    }

    enum State { Idle, Chase, Windup, Attack, Recover }

    [SerializeField] GameObject Player;
    [SerializeField] BossAnimation bossAnimation;
    [SerializeField] List<BossAttack> attacks = new List<BossAttack>();
    public float enemySpeed = 20.0f;
    private bool _isAttacking;

    /// <summary>
    /// 予備動作秒数
    /// </summary>
    [SerializeField] float windUpTime = 0.5f;

    /// <summary>
    /// 攻撃後の隙
    /// </summary>
    [SerializeField] float recoverTime = 1.0f;

    /// <summary>
    /// 攻撃の最大秒数(保険)
    /// </summary>
    const float MaxAttackTime = 5.0f;

    /// <summary>
    /// プレイヤーとボスの距離
    /// </summary>
    public float distance = 0;

    /// <summary>
    /// ボスがアタックを始めるプレイヤーとの距離
    /// </summary>
    public float attackRange = 10;

    /// <summary>
    /// ボスがプレイヤーへ移動を始める距離
    /// </summary>
    public float trackPlayerRange = 50;

    /// <summary>
    /// 二乗したPlayerのx,z座標の意
    /// </summary>
    float distanceX, distanceZ;

    /// <summary>
    /// ボスの攻撃パターン
    /// 1:右叩きつけパンチ
    /// 2.左叩きつけパンチ
    /// </summary>
    private int _bossPattern = 1;

    /// <summary>
    /// ボスの攻撃パターンの総数(正直無いように作りたい)
    /// </summary>
    public int amountBossPattern = 2;

    private NavMeshAgent navMeshAgent;
    private State _state = State.Idle;
    private float _timer = 0f;
    private BossAttack _current;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        if (Player == null)
            Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // 距離の更新
        distanceX = Mathf.Abs(Player.transform.position.x - gameObject.transform.position.x);
        distanceZ = Mathf.Abs(Player.transform.position.z - gameObject.transform.position.z);
        distance = Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceZ, 2));

        // 現時点の状態からどれくらい経過したかの計算
        _timer += Time.deltaTime;

        switch (_state)
        {
            case State.Idle: // 通常
                if(distance < trackPlayerRange) 
                {
                    ChangeState(State.Chase);
                }
                break;

            case State.Chase: // 追跡
                if (distance >= trackPlayerRange)
                {
                    ChangeState(State.Idle);
                }
                else
                {
                    _current = ChooseAttack(distance);  // 攻撃を選ぶ
                    if(_current != null)
                    {
                        ChangeState(State.Windup);
                    }
                    else
                    {
                        navMeshAgent.isStopped = false;
                        navMeshAgent.SetDestination(Player.transform.position);
                    }
                }
                    break;

            case State.Windup: // 予備動作
                FacePlayer();
                if(_timer >= windUpTime)
                {
                    bossAnimation.bossState = _current.animationState;
                    ChangeState(State.Attack);
                }
                break;

            case State.Attack: // 攻撃
                
                // 突進攻撃の移動
                if(_current.dashSpeed > 0)
                {
                    navMeshAgent.Move(transform.forward * _current.dashSpeed * Time.deltaTime);
                }

                // アニメーション終了後にRecoverへ移行
                if(bossAnimation.CheckNormalizedAnimation(_current.stateName) || _timer >= MaxAttackTime)
                {
                    bossAnimation.SetIdle();
                    ChangeState(State.Recover);
                }
                break;

            case State.Recover: // 隙の時間
                if(_timer >= recoverTime)
                {
                    ChangeState(State.Idle);
                }
                break;
        }

    //    // Idleに戻す
    //    if (bossAnimation.bossState != 0 &&
    //        (bossAnimation.CheckNormalizedAnimation("Punch_Right") || bossAnimation.CheckNormalizedAnimation("Punch_Left")))
    //    {
    //        bossAnimation.SetIdle();
    //    }

        

    //    // 実際にIdleに戻り終わるまでは攻撃中扱い
    //    _isAttacking = !(bossAnimation.bossState == 0 && bossAnimation.IsIdle());

    //    if (distance < attackRange && !_isAttacking)
    //    {
    //        bossAnimation.bossState = _bossPattern;
    //        _bossPattern = (_bossPattern == 1) ? 2 : 1;   // 攻撃開始時にパターン切り替え
    //        _isAttacking = true;
    //    }
    //    else if (distance < trackPlayerRange && !_isAttacking)
    //    {
    //        navMeshAgent.destination = Player.transform.position;
    //    }

    }

    /// <summary>
    /// 状態を切り替える
    /// </summary>
    void ChangeState(State next)
    {
        _state = next;
        _timer = 0f;
        navMeshAgent.isStopped = true; // これが無いと攻撃中に止まらない
    }

    bool IsAllRepaired(BossAttack attack)
    {
        if(attack.parts == null || attack.parts.Length == 0)
        {
            return false;
        }

        foreach(BodyInfo part in attack.parts)
        {
            if(part.isRepaired == false)
            {
                return false;
            }
        }

        return true;
    }

    BossAttack ChooseAttack(float distance)
    {
        List<BossAttack> usable = new List<BossAttack>();

        foreach(BossAttack attack in attacks)
        {
            if (distance > attack.range)
                continue;
            if (IsAllRepaired(attack))
                continue;

            usable.Add(attack);
        }

        if(usable.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, usable.Count);
        return usable[index];
    }

    /// <summary>
    /// 少しずつプレイヤーの方角を向く
    /// </summary>
   void FacePlayer()
    {
        Vector3 direction = Player.transform.position - transform.position;
        direction.y = 0;
        if (direction.magnitude < 0.1f) // 大体同じ位置で向きが決まってなかったら何もしない
            return;

        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 10f * Time.deltaTime);
    }

}
