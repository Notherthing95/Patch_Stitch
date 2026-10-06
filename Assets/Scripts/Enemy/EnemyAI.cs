using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float enemySpeed = 20.0f;
    [SerializeField] GameObject Player;
    [SerializeField] BossAnimation bossAnimation;
    private bool _isAttacking;
    
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bossAnimation.CheckNormalizedAnimation("Punch_Right") || bossAnimation.CheckNormalizedAnimation("Punch_Left"))
        {
            _isAttacking = false;
            if (_bossPattern == 1)
                _bossPattern++;
            else
                _bossPattern--;
            //Debug.Log("checkBossPattern: " + _bossPattern);
            
        }

        //Debug.Log("isAttacking: " + _isAttacking);

        // 距離の更新
        distanceX = Mathf.Abs(Player.transform.position.x - gameObject.transform.position.x);
        distanceZ = Mathf.Abs(Player.transform.position.z - gameObject.transform.position.z);
        distance = Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceZ,2));
        //Debug.Log("Distance: " + distance);

        // 攻撃判定
        if(distance < attackRange && !_isAttacking)
        {
            BossAnimation.bossState = _bossPattern;
            _isAttacking = true;
            
        }
        else if(distance < trackPlayerRange && !_isAttacking)    // 移動
        {
            navMeshAgent.destination = Player.transform.position;

        }

    }
}
