using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー移動の制御のスクリプト
/// </summary>
public class PlayerMoveController : MonoBehaviour
{
    InputAction moveAction;
    InputAction dodgeAction;

    /// <summary>
    /// プレイヤーのリジッドボディ
    /// </summary>
    Rigidbody p_rigidbody;

    /// <summary>
    /// アナログジョイスティックから読み取った値
    /// </summary>
    Vector2 readVector;

    /// <summary>
    /// 移動する方向ベクトル yは0
    /// </summary>
    Vector3 moveVector;

    /// <summary>
    /// 歩き状態の速さ
    /// </summary>
    [SerializeField] float walkSpeed = 5;

    /// <summary>
    /// 走り状態の速さ
    /// </summary>
    [SerializeField] float dashSpeed = 10;

    /// <summary>
    /// 回避時の速さ
    /// </summary>
    [SerializeField] float dodgeSpeed = 30;

    /// <summary>
    /// 現在の移動する速さ
    /// </summary>
    float moveSpeed;

    /// <summary>
    /// 回避している状態
    /// </summary>
    bool IsDodge;

    /// <summary>
    /// 回避にかかる時間
    /// </summary>
    const float DodgeDuration = 0.33f;

    /// <summary>
    /// 回避し始めてから現在経った時間
    /// </summary>
    float dodgeTimer;

    /// <summary>
    /// 走ると歩くを切り替える閾値
    /// </summary>
    const float RunWalkThreshold = 0.75f;

    /// <summary>
    /// リジッドボディの減衰率の初期値
    /// </summary>
    const float DampingValue = 10f;

    /// <summary>
    /// 移動の力を加える際に掛ける補正倍率
    /// </summary>
    const float ForceMultiPlier = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        dodgeAction = InputSystem.actions.FindAction("Dodge");
        p_rigidbody = GetComponent<Rigidbody>();
        p_rigidbody.linearDamping = DampingValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (dodgeAction.triggered && !IsDodge)
        {
            IsDodge = true;
        }
    }

    void FixedUpdate()
    {
        readVector = moveAction.ReadValue<Vector2>();

        //移動速度の設定
        SetMoveSpeed();

        //移動する方向ベクトルの計算
        CalculationMoveVector();

        if (readVector.magnitude > 0.1f)
        {
            MovePlayer();
        }
    }

    /// <summary>
    /// 入力に応じてプレイヤーの移動速度を設定
    /// </summary>
    void SetMoveSpeed()
    {
        if (IsDodge)
        {
            moveSpeed = Mathf.Lerp(dodgeSpeed, dashSpeed, dodgeTimer / DodgeDuration);
            dodgeTimer += Time.deltaTime;
            if (dodgeTimer >= DodgeDuration)
            {
                IsDodge = false;
                dodgeTimer = 0;
            }
        }
        else if (readVector.magnitude >= RunWalkThreshold)
        {
            moveSpeed = dashSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    /// <summary>
    /// 移動する方向ベクトルの計算 y方向は0
    /// </summary>
    void CalculationMoveVector()
    {
        Vector3 cForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        Vector3 cRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
        moveVector = (readVector.y * cForward + readVector.x * cRight).normalized;
    }

    /// <summary>
    /// プレイヤーオブジェクトに移動方向の力を加え、その方向を向く
    /// </summary>
    void MovePlayer()
    {
        if (p_rigidbody.linearVelocity.magnitude <= moveSpeed)
        {
            p_rigidbody.AddForce(moveVector * moveSpeed * ForceMultiPlier);
        }
        transform.forward = moveVector;
    }
}
