using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// カメラ制御のスクリプト 感度と反転を外部から変更できます
/// </summary>
public class CameraController : MonoBehaviour
{
    InputAction _cameraAction;
    InputAction _cameraResetAction;


    /// <summary>
    /// プレイヤーのトランスフォーム
    /// </summary>
    [SerializeField] Transform playerTransform;

    /// <summary>
    /// カメラとプレイヤーとの最大距離
    /// </summary>
    float _maxCameraDistance;

    /// <summary>
    /// カメラとプレイヤーとの距離
    /// </summary>
    float _cameraDistance;

    /// <summary>
    /// カメラの方向を決めるレイ
    /// </summary>
    Ray _cameraRay;

    /// <summary>
    /// カメラの始点の高さ補正
    /// </summary>
    float _cameraOriginHeight;

    /// <summary>
    /// カメラのヨー回転の位置 0から2πの範囲
    /// </summary>
    float _cameraYawAngle;

    /// <summary>
    /// カメラのピッチ回転の位置
    /// </summary>
    float _cameraPitchAngle;

    /// <summary>
    /// カメラ上向きの制限 度数法
    /// </summary>
    [Header("視点上向きの制限 XX°")]
    [SerializeField] float maxPitch = 36f;

    /// <summary>
    /// カメラ下向きの制限 度数法
    /// </summary>
    [Header("視点下向きの制限 XX°")]
    [SerializeField] float minPitch = 30f;

    /// <summary>
    /// カメラのヨー回転の感度 初期値2f
    /// </summary>
    public float yawSensitivity { get; set; } = 2f;

    /// <summary>
    /// カメラのピッチ回転の感度 初期値2f
    /// </summary>
    public float pitchSensitivity { get; set; } = 2f;

    /// <summary>
    /// カメラのヨー回転の反転
    /// </summary>
    public bool isInvertYaw { get; set; }

    /// <summary>
    /// カメラのピッチ回転の反転
    /// </summary>
    public bool isInvertPitch { get; set; }

    /// <summary>
    /// カメラ回転リセットフラグ
    /// </summary>
    bool _isReset;

    /// <summary>
    /// リセットにかかる時間
    /// </summary>
    const float ResetDuration = 0.2f;

    /// <summary>
    /// リセットし始めてから現在経った時間
    /// </summary>
    float _resettingTimer;

    /// <summary>
    /// リセットボタンを押した時のカメラのヨー回転
    /// </summary>
    float _beforeResetYawAngle;

    /// <summary>
    /// リセット後のカメラのヨー回転
    /// </summary>
    float _afterResetYawAngle;

    /// <summary>
    /// リセットボタンを押した時のカメラのピッチ回転
    /// </summary>
    float _beforeResetPitchAngle;

    /// <summary>
    /// カメラレイが最後にヒットした時のカメラとプレイヤーの距離
    /// </summary>
    float _hitCameraDistance;

    /// <summary>
    /// カメラレイがヒットしてからcameraDistanceが最大に戻るまでの時間
    /// </summary>
    const float ReturnDelay = 0.2f;

    /// <summary>
    /// カメラレイがヒットしてからcameraDistanceが最大に戻るまでの現在経った時間
    /// </summary>
    float _returnTimer;

    /// <summary>
    /// アナログジョイスティックから読み取った値
    /// </summary>
    Vector2 _readVector2;

    /// <summary>
    /// カメラ衝突判定のマスク
    /// </summary>
    int _layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraAction = InputSystem.actions.FindAction("Camera");
        _cameraResetAction = InputSystem.actions.FindAction("CameraReset");
        _layerMask = LayerMask.GetMask("Field", "Eenmy");

        //シーンのカメラの位置から情報を取得
        _maxCameraDistance = Vector3.Distance(playerTransform.position, Camera.main.transform.position);
        _cameraDistance = _maxCameraDistance;
        _hitCameraDistance = _cameraDistance;
        _cameraOriginHeight = Camera.main.transform.position.y - playerTransform.position.y;
        _cameraRay.origin = playerTransform.position + Vector3.up * _cameraOriginHeight;
        _cameraRay.direction = Camera.main.transform.position - playerTransform.position;
        _cameraYawAngle = Mathf.Atan2(_cameraRay.direction.z, _cameraRay.direction.x);
    }

    // Update is called once per frame
    void Update()
    {
        _readVector2 = _cameraAction.ReadValue<Vector2>();

        //カメラレイの原点をプレイヤーの位置に常に更新
        _cameraRay.origin = playerTransform.position + Vector3.up * _cameraOriginHeight;

        //カメラの回転角度計算
        CalculationRayAngles();

        //カメラ回転リセット
        if (_cameraResetAction.WasPressedThisFrame() && !_isReset)
        {
            ResetTiggered();
        }
        if (_isReset)
        {
            CameraResetting();
        }

        //ヨー回転はXZに分解し、ピッチ回転はXZの値に影響されないようにしてカメラレイの向きを計算
        _cameraRay.direction = new Vector3(Mathf.Cos(_cameraYawAngle) * Mathf.Cos(_cameraPitchAngle), Mathf.Sin(_cameraPitchAngle), Mathf.Sin(_cameraYawAngle) * Mathf.Cos(_cameraPitchAngle));

        //デバッグ用 カメラレイの表示
        Debug.DrawRay(_cameraRay.origin, _cameraRay.direction * _maxCameraDistance, Color.blue);

        //カメラの位置更新
        CameraPositionSet();
    }

    /// <summary>
    /// 入力値を角度に代入し、角度が範囲を超えないように制限
    /// </summary>
    void CalculationRayAngles()
    {
        float yawSign = isInvertYaw ? 1f : -1f;
        float pitchSign = isInvertPitch ? 1f : -1f;

        //入力値から角度に変換
        _cameraYawAngle += _readVector2.x * Time.deltaTime * yawSensitivity * yawSign;
        _cameraYawAngle %= Mathf.PI * 2f;
        if (_cameraYawAngle < 0)
        {
            _cameraYawAngle += Mathf.PI * 2f;
        }
        _cameraPitchAngle += _readVector2.y * Time.deltaTime * pitchSensitivity * pitchSign;

        //ピッチ回転制限
        if (_cameraPitchAngle <= -maxPitch * Mathf.Deg2Rad)
        {
            _cameraPitchAngle = -maxPitch * Mathf.Deg2Rad;
        }
        else if (_cameraPitchAngle >= minPitch * Mathf.Deg2Rad)
        {
            _cameraPitchAngle = minPitch * Mathf.Deg2Rad;
        }
    }

    /// <summary>
    /// IsResetをtrueにし、リセットに必要な情報の計算
    /// </summary>
    void ResetTiggered()
    {
        _isReset = true;
        //リセット前と後の回転の角度の代入
        _beforeResetYawAngle = _cameraYawAngle;
        _afterResetYawAngle = Mathf.Atan2(-playerTransform.forward.z, -playerTransform.forward.x);
        if (_afterResetYawAngle < 0)
        {
            _afterResetYawAngle += Mathf.PI * 2f;
        }
        _beforeResetPitchAngle = _cameraPitchAngle;

        //リセットのヨー回転の回転量が少ない方になるよう調整
        if (Mathf.Abs(_afterResetYawAngle - _beforeResetYawAngle) > Mathf.Abs(_afterResetYawAngle - (_beforeResetYawAngle + Mathf.PI * 2f)))
        {
            _beforeResetYawAngle += Mathf.PI * 2f;
        }
        else if (Mathf.Abs(_afterResetYawAngle - _beforeResetYawAngle) > Mathf.Abs(_afterResetYawAngle - (_beforeResetYawAngle - Mathf.PI * 2f)))
        {
            _beforeResetYawAngle -= Mathf.PI * 2f;
        }
    }

    /// <summary>
    /// resetTime秒でリセットボタンを押した時の方向からプレイヤー前方向への線形補間
    /// </summary>
    void CameraResetting()
    {
        _resettingTimer += Time.deltaTime;

        _cameraYawAngle = Mathf.Lerp(_beforeResetYawAngle, _afterResetYawAngle, _resettingTimer / ResetDuration);
        _cameraPitchAngle = Mathf.Lerp(_beforeResetPitchAngle, 0, _resettingTimer / ResetDuration);

        if (_resettingTimer >= ResetDuration)
        {
            _isReset = false;
            _resettingTimer = 0;
        }
    }

    /// <summary>
    ///障害物を検知しカメラが埋まらないようにして、カメラレイの線上にカメラを移動
    /// </summary>
    void CameraPositionSet()
    {
        _returnTimer += Time.deltaTime;

        RaycastHit hit;
        Physics.Raycast(_cameraRay, out hit, _maxCameraDistance, _layerMask);
        if (hit.collider != null)
        {
            _hitCameraDistance = (hit.point - _cameraRay.origin).magnitude - 0.1f;
            _returnTimer = 0;
        }

        _cameraDistance = Mathf.Lerp(_hitCameraDistance, _maxCameraDistance, _returnTimer / ReturnDelay);

        Camera.main.transform.position = _cameraRay.origin + _cameraRay.direction * _cameraDistance;

        Camera.main.transform.LookAt(_cameraRay.origin);
    }
}
