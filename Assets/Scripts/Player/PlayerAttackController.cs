using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの攻撃及び移動範囲制限のスクリプト 攻撃点と移動範囲を外部から参照できます
/// </summary>
public class PlayerAttackController : MonoBehaviour
{
    InputAction _attackAction;
    InputAction _finalAttackAction;

    BodyInfo _bodyInfo;

    /// <summary>
    /// プレイヤーと敵への攻撃点をつなぐ糸を表現するためのバネ
    /// </summary>
    SpringJoint _spring;

    const float SpringValue = 1000f;
    const float DamperValue = 0.2f;
    const float ToleranceValue = 0.0001f;

    /// <summary>
    /// バネの接続先である敵への攻撃点
    /// </summary>
    public GameObject attackPoint { get; private set; }

    /// <summary>
    /// 攻撃の当たり判定検知用
    /// </summary>
    RaycastHit _hit;

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
    bool _isInCombat;

    /// <summary>
    /// 攻撃している状態
    /// </summary>
    bool _isAttacking;

    /// <summary>
    /// 攻撃にかかる時間
    /// </summary>
    const float AttckDuration = 0.3f;

    /// <summary>
    /// 攻撃し始めてから現在経った時間
    /// </summary>
    float _attackTimer;

    /// <summary>
    /// プレイヤーの最大行動範囲(糸の最大距離) 敵の部位から取得
    /// </summary>
    float _maxMoveRange;

    /// <summary>
    /// プレイヤーの行動範囲(糸の距離)
    /// </summary>
    public float moveRange { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _attackAction = InputSystem.actions.FindAction("Attack");
        _finalAttackAction = InputSystem.actions.FindAction("FinalAttack");
        attackPoint = new GameObject("AttackPoint");
        attackPoint.SetActive(false);
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
        if (_finalAttackAction.WasPressedThisFrame())
        {
            if (_isInCombat)
            {
                FinalAttack();
            }
        }
        else if (_attackAction.WasPressedThisFrame())
        {
            if (!_isAttacking)
            {
                Attack();
            }
        }

        if (_isAttacking)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= AttckDuration)
            {
                _isAttacking = false;
                _attackTimer = 0;
            }
        }
        if (_isInCombat)
        {
            _spring.connectedAnchor = attackPoint.transform.position;
        }
    }

    /// <summary>
    /// isInCombatをtureにして、敵への攻撃点を設定し、糸の代わりのバネを生成する
    /// </summary>
    void StartCombat()
    {
        _isInCombat = true;

        _bodyInfo = _hit.collider.GetComponent<BodyInfo>();
        _bodyInfo.Hit();

        attackPoint.SetActive(true);
        attackPoint.transform.parent = _hit.collider.transform;
        attackPoint.transform.position = _hit.point;

        _maxMoveRange = _bodyInfo.stringRadius;
        moveRange = _maxMoveRange;

        _spring = gameObject.AddComponent<SpringJoint>();
        _spring.spring = SpringValue;
        _spring.damper = DamperValue;
        _spring.maxDistance = moveRange;
        _spring.tolerance = ToleranceValue;
        _spring.autoConfigureConnectedAnchor = false;

        _spring.connectedAnchor = attackPoint.transform.position;

    }

    /// <summary>
    /// moveRangeをデクリメントして、BodyInfoのHitを呼ぶ
    /// </summary>
    void OnHit()
    {
        if (moveRange > 1)
        {
            moveRange--;
            _spring.maxDistance = moveRange;
            _bodyInfo.Hit();
        }
    }

    /// <summary>
    /// isAttackingをtrueにして、攻撃の当たり判定を行う
    /// </summary>
    void Attack()
    {
        _isAttacking = true;
        Physics.SphereCast(transform.position - transform.forward.normalized * RayOriginOffset, attackRange, transform.forward, out _hit, attackReach);//後々レイヤーマスクつけます
        if (_hit.collider != null)
        {
            if (!_isInCombat)
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
        _bodyInfo.Finish();
        _bodyInfo = null;
        attackPoint.SetActive(false);
        attackPoint.transform.parent = null;
        Destroy(_spring);
        _isInCombat = false;
    }
}
