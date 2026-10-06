using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー移動の制御のスクリプト 接地判定とノックバックの関数を外部から呼べます
/// </summary>
public class PlayerMoveController : MonoBehaviour
{
    InputAction _moveAction;
    InputAction _dodgeAction;
    PlayerAttackController _attackController;
    [SerializeField] PlayerInfo playerInfo;

    /// <summary>
    /// プレイヤーのリジッドボディ
    /// </summary>
    Rigidbody _playerRigidbody;

    /// <summary>
    /// アナログジョイスティックから読み取った値
    /// </summary>
    Vector2 _readVector;

    /// <summary>
    /// 移動する方向ベクトル yは0
    /// </summary>
    Vector3 _moveVector;

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
    float _moveSpeed;

    /// <summary>
    /// 回避している状態
    /// </summary>
    bool _isDodge;

    /// <summary>
    /// 回避にかかる時間
    /// </summary>
    const float DodgeDuration = 0.33f;

    /// <summary>
    /// 回避し始めてから現在経った時間
    /// </summary>
    float _dodgeTimer;

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

    /// <summary>
    /// プレイヤーの背の高さの半分
    /// </summary>
    const float PlayerHeightHalf = 0.8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _dodgeAction = InputSystem.actions.FindAction("Dodge");
        _playerRigidbody = GetComponent<Rigidbody>();
        _attackController = GetComponent<PlayerAttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_dodgeAction.WasPressedThisFrame() && !_isDodge)
        {
            _isDodge = true;
        }
    }

    void FixedUpdate()
    {
        _readVector = _moveAction.ReadValue<Vector2>();

        //移動速度の設定
        SetMoveSpeed();

        //移動する方向ベクトルの計算 回避中は方向転換不可
        if (!_isDodge)
        {
            CalculationMoveVector();
        }

        //接地しているか
        if (IsGround())
        {
            //プレイヤーが停止するのを早める
            _playerRigidbody.linearDamping = DampingValue;

            //攻撃中は移動不可
            if (!_attackController.isAttacking)
            {
                //入力があればプレイヤーを移動させる
                if (_readVector.magnitude > 0.1f)
                {
                    MovePlayer();
                }
                else if (_isDodge)
                {
                    MovePlayer();
                }
            }

        }
        else
        {
            _playerRigidbody.linearDamping = 0;
            _playerRigidbody.AddForce(-Vector3.up * 9.8f);
        }
    }

    /// <summary>
    /// 入力に応じてプレイヤーの移動速度を設定
    /// </summary>
    void SetMoveSpeed()
    {
        if (_isDodge)
        {
            //入力が無ければ前方に
            if (_moveVector == Vector3.zero)
            {
                _moveVector = transform.forward.normalized;
            }
            _moveSpeed = Mathf.Lerp(dodgeSpeed, dashSpeed, _dodgeTimer / DodgeDuration);
            PlayerAnimation.playerState = 12;
            Debug.Log("State: 12");
            _dodgeTimer += Time.deltaTime;
            if (_dodgeTimer >= DodgeDuration)
            {
                _isDodge = false;
                _dodgeTimer = 0;
            }
        }
        else if (_readVector.magnitude >= RunWalkThreshold)
        {
            _moveSpeed = dashSpeed;
            PlayerAnimation.playerState = 2;
        }
        else if (_readVector.magnitude >= 0.1f)
        {
            _moveSpeed = walkSpeed;
            PlayerAnimation.playerState = 1;
        }
        else if (PlayerAnimation.playerState == 1 || PlayerAnimation.playerState == 2)
        {
            PlayerAnimation.playerState = 0;
        }
    }

    /// <summary>
    /// 移動する方向ベクトルの計算 y方向は0
    /// </summary>
    void CalculationMoveVector()
    {
        Vector3 cForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        Vector3 cRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
        _moveVector = (_readVector.y * cForward + _readVector.x * cRight).normalized;
    }

    /// <summary>
    /// プレイヤーオブジェクトに移動方向の力を加え、その方向を向く
    /// </summary>
    void MovePlayer()
    {
        if (_playerRigidbody.linearVelocity.magnitude <= _moveSpeed)
        {
            _playerRigidbody.AddForce(_moveVector * _moveSpeed * ForceMultiPlier);
        }
        transform.forward = _moveVector;
    }

    /// <summary>
    /// プレイヤーが接地していればtrueを返す
    /// </summary>
    public bool IsGround()
    {
        Debug.DrawRay(transform.position, -Vector3.up * (PlayerHeightHalf + 0.05f), Color.brown);
        if (Physics.Raycast(transform.position, -Vector3.up, PlayerHeightHalf + 0.05f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnKnockBack(Transform transform, float power)
    {
        Vector3 direction = (this.transform.position - transform.position).normalized;
        direction.y = 0.5f;

        _playerRigidbody.AddForce(direction * power, ForceMode.Impulse);
        PlayerAnimation.playerState = 9;
        playerInfo.PlayerLife--;
    }
}
