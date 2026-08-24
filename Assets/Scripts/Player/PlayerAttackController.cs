using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの攻撃及び移動範囲制限のスクリプト 攻撃点と移動範囲を外部から参照できます
/// </summary>
public class PlayerAttackController : MonoBehaviour
{
    InputAction attackAction;
    InputAction finalAttackAction;

    BodyInfo sc_bodyInfo;

    /// <summary>
    /// プレイヤーと敵への攻撃点をつなぐ糸を表現するためのバネ
    /// </summary>
    SpringJoint spring;

    const float SpringValue = 1000;
    const float DamperValue = 0.2f;
    const float ToleranceValue = 0.0001f;

    /// <summary>
    /// バネの接続先である敵への攻撃点
    /// </summary>
    public GameObject AttackPoint { get; private set; }

    /// <summary>
    /// 攻撃の当たり判定検知用
    /// </summary>
    RaycastHit hit;

    /// <summary>
    /// 攻撃範囲の半径
    /// </summary>
    [Header("攻撃の範囲")]
    [SerializeField] float attackRange = 0.5f;

    /// <summary>
    /// 攻撃の届く距離
    /// </summary>
    [Header("攻撃の届く距離")]
    [SerializeField] float attackReach = 1f;

    /// <summary>
    /// レイの原点をずらす数値
    /// </summary>
    const float RayOriginOffset = 0.2f;

    /// <summary>
    /// 戦闘している状態
    /// </summary>
    bool isInCombat;

    /// <summary>
    /// 攻撃している状態
    /// </summary>
    bool isAttacking;

    /// <summary>
    /// 攻撃にかかる時間
    /// </summary>
    const float AttckDuration = 0.3f;

    /// <summary>
    /// 攻撃し始めてから現在経った時間
    /// </summary>
    float attackTimer;

    /// <summary>
    /// プレイヤーの最大行動範囲(糸の最大距離) 敵の部位から取得
    /// </summary>
    float maxMoveRange;

    /// <summary>
    /// プレイヤーの行動範囲(糸の距離)
    /// </summary>
    public float MoveRange { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        finalAttackAction = InputSystem.actions.FindAction("FinalAttack");
        AttackPoint = new GameObject("AttackPoint");
        AttackPoint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //円形の当たり判定の中心と４端の描画
        Debug.DrawRay(transform.position - transform.forward.normalized * RayOriginOffset, transform.forward * attackReach + transform.forward * attackRange, Color.red);
        Debug.DrawRay(transform.position + transform.right.normalized * attackRange - transform.forward.normalized * RayOriginOffset, transform.forward * attackReach, Color.red);
        Debug.DrawRay(transform.position + -transform.right.normalized * attackRange - transform.forward.normalized * RayOriginOffset, transform.forward * attackReach, Color.red);
        Debug.DrawRay(transform.position + transform.up.normalized * attackRange - transform.forward.normalized * RayOriginOffset, transform.forward * attackReach, Color.red);
        Debug.DrawRay(transform.position + -transform.up.normalized * attackRange - transform.forward.normalized * RayOriginOffset, transform.forward * attackReach, Color.red);

        //攻撃が入力された時
        if (finalAttackAction.WasPressedThisFrame())
        {
            if (isInCombat)
            {
                FinalAttack();
            }
        }
        else if (attackAction.WasPressedThisFrame())
        {
            if (!isAttacking)
            {
                Attack();
            }
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= AttckDuration)
            {
                isAttacking = false;
                attackTimer = 0;
            }
        }
        if (isInCombat)
        {
            spring.connectedAnchor = AttackPoint.transform.position;
        }
    }

    /// <summary>
    /// isInCombatをtureにして、敵への攻撃点を設定し、糸の代わりのバネを生成する
    /// </summary>
    void StartCombat()
    {
        isInCombat = true;

        sc_bodyInfo = hit.collider.GetComponent<BodyInfo>();
        sc_bodyInfo.Hit();

        AttackPoint.SetActive(true);
        AttackPoint.transform.parent = hit.collider.transform;
        AttackPoint.transform.position = hit.point;

        maxMoveRange = sc_bodyInfo.stringRadius;
        MoveRange = maxMoveRange;

        spring = gameObject.AddComponent<SpringJoint>();
        spring.spring = SpringValue;
        spring.damper = DamperValue;
        spring.maxDistance = MoveRange;
        spring.tolerance = ToleranceValue;
        spring.autoConfigureConnectedAnchor = false;

        spring.connectedAnchor = AttackPoint.transform.position;

    }

    /// <summary>
    /// moveRangeをデクリメントして、BodyInfoのHitを呼ぶ
    /// </summary>
    void OnHit()
    {
        if (MoveRange > 1)
        {
            MoveRange--;
            spring.maxDistance = MoveRange;
            sc_bodyInfo.Hit();
        }
    }

    /// <summary>
    /// isAttackingをtrueにして、攻撃の当たり判定を行う
    /// </summary>
    void Attack()
    {
        isAttacking = true;
        Physics.SphereCast(transform.position - transform.forward.normalized * RayOriginOffset, attackRange, transform.forward, out hit, attackReach);//後々レイヤーマスクつけます
        if (hit.collider != null)
        {
            if (!isInCombat)
            {
                StartCombat();
            }
            else
            {
                OnHit();
            }
        }
    }

    /// <summary>
    /// BodyInfoのFinishを呼び、いくつかの変数を初期化して、isInCombatをfalseにする
    /// </summary>
    void FinalAttack()
    {
        sc_bodyInfo.Finish();
        sc_bodyInfo = null;
        AttackPoint.SetActive(false);
        AttackPoint.transform.parent = null;
        Destroy(spring);
        isInCombat = false;
    }
}
