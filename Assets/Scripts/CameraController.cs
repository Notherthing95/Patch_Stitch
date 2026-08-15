using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// カメラ制御のスクリプト 感度と反転を外部から変更できます
/// </summary>
public class CameraController : MonoBehaviour
{
    InputAction cameraAction;
    InputAction cameraResetAction;

    /// <summary>
    /// プレイヤーのトランスフォーム
    /// </summary>
    [SerializeField] Transform playerTransform;

    /// <summary>
    /// カメラとプレイヤーとの最大距離
    /// </summary>
    float maxCameraDistance;

    /// <summary>
    /// カメラとプレイヤーとの距離
    /// </summary>
    float cameraDistance;

    /// <summary>
    /// カメラの方向を決めるレイ
    /// </summary>
    Ray cameraRay;

    /// <summary>
    /// カメラのヨー回転の位置 0から2πの範囲
    /// </summary>
    float cameraYawAngle;

    /// <summary>
    /// カメラのピッチ回転の位置
    /// </summary>
    float cameraPitchAngle;

    /// <summary>
    /// カメラ上向きの制限
    /// </summary>
    [Header("視点上向きの制限")]
    [SerializeField] float maxPitch = 5f;

    /// <summary>
    /// カメラ下向きの制限
    /// </summary>
    [Header("視点下向きの制限")]
    [SerializeField] float minPitch = 6f;

    /// <summary>
    /// カメラのヨー回転の感度 初期値0.01f
    /// </summary>
    public float YawSensitivity { get; set; } = 0.01f;

    /// <summary>
    /// カメラのピッチ回転の感度 初期値0.01f
    /// </summary>
    public float PitchSensitivity { get; set; } = 0.01f;

    /// <summary>
    /// カメラのヨー回転の反転
    /// </summary>
    public bool IsInvertYaw { get; set; }

    /// <summary>
    /// カメラのピッチ回転の反転
    /// </summary>
    public bool IsInvertPitch { get; set; }

    /// <summary>
    /// カメラ回転リセットフラグ
    /// </summary>
    bool IsReset;

    /// <summary>
    /// リセットにかかる時間
    /// </summary>
    const float ResetDuration = 0.2f;

    /// <summary>
    /// リセットし始めてから現在経った時間
    /// </summary>
    float resettingTimer;

    /// <summary>
    /// リセットボタンを押した時のカメラのヨー回転
    /// </summary>
    float beforeResetYawAngle;

    /// <summary>
    /// リセット後のカメラのヨー回転
    /// </summary>
    float afterResetYawAngle;

    /// <summary>
    /// リセットボタンを押した時のカメラのピッチ回転
    /// </summary>
    float beforeResetPitchAngle;

    /// <summary>
    /// カメラレイが最後にヒットした時のカメラとプレイヤーの距離
    /// </summary>
    float hitCameraDistance;

    /// <summary>
    /// カメラレイがヒットしてからcameraDistanceが最大に戻るまでの時間
    /// </summary>
    const float ReturnDelay = 0.2f;

    /// <summary>
    /// カメラレイがヒットしてからcameraDistanceが最大に戻るまでの現在経った時間
    /// </summary>
    float returnTimer;

    /// <summary>
    /// アナログジョイスティックから読み取った値
    /// </summary>
    Vector2 readVector2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraAction = InputSystem.actions.FindAction("Camera");
        cameraResetAction = InputSystem.actions.FindAction("CameraReset");

        //シーンのカメラの位置から情報を取得
        maxCameraDistance = Vector3.Distance(playerTransform.position, Camera.main.transform.position);
        cameraDistance = maxCameraDistance;
        hitCameraDistance = cameraDistance;
        cameraRay.origin = playerTransform.position;
        cameraRay.direction = Camera.main.transform.position - playerTransform.position;
        cameraYawAngle = Mathf.Atan2(cameraRay.direction.z, cameraRay.direction.x);
    }

    // Update is called once per frame
    void Update()
    {
        readVector2 = cameraAction.ReadValue<Vector2>();

        //カメラレイの原点をプレイヤーの位置に常に更新
        cameraRay.origin = playerTransform.position;

        //カメラの回転角度計算
        CalculationRayAngles();

        //カメラ回転リセット
        if (cameraResetAction.triggered && !IsReset)
        {
            ResetTiggered();
        }
        if (IsReset)
        {
            CameraResetting();
        }

        //ヨー回転はXZに分解し、ピッチ回転はXZの値に影響されないようにしてカメラレイの向きを計算
        cameraRay.direction = new Vector3(Mathf.Cos(cameraYawAngle) * Mathf.Cos(cameraPitchAngle), Mathf.Sin(cameraPitchAngle), Mathf.Sin(cameraYawAngle) * Mathf.Cos(cameraPitchAngle));

        //デバッグ用 カメラレイの表示
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * maxCameraDistance, Color.blue);

        //カメラの位置更新
        CameraPositionSet();
    }

    /// <summary>
    /// 入力値を角度に代入し、角度が範囲を超えないように制限
    /// </summary>
    void CalculationRayAngles()
    {
        float yawSign = IsInvertYaw ? 1f : -1f;
        float pitchSign = IsInvertPitch ? 1f : -1f;

        //入力値から角度に変換
        cameraYawAngle += readVector2.x * YawSensitivity * yawSign;
        cameraYawAngle %= Mathf.PI * 2f;
        if (cameraYawAngle < 0)
        {
            cameraYawAngle += Mathf.PI * 2f;
        }
        cameraPitchAngle += readVector2.y * PitchSensitivity * pitchSign;

        //ピッチ回転制限
        if (cameraPitchAngle <= Mathf.PI / -maxPitch)
        {
            cameraPitchAngle = Mathf.PI / -maxPitch;
        }
        else if (cameraPitchAngle >= Mathf.PI / minPitch)
        {
            cameraPitchAngle = Mathf.PI / minPitch;
        }
    }

    /// <summary>
    /// IsResetをtrueにし、リセットに必要な情報の計算
    /// </summary>
    void ResetTiggered()
    {
        IsReset = true;
        //リセット前と後の回転の角度の代入
        beforeResetYawAngle = cameraYawAngle;
        afterResetYawAngle = Mathf.Atan2(-playerTransform.forward.z, -playerTransform.forward.x);
        if (afterResetYawAngle < 0)
        {
            afterResetYawAngle += Mathf.PI * 2f;
        }
        beforeResetPitchAngle = cameraPitchAngle;

        //リセットのヨー回転の回転量が少ない方になるよう調整
        if (Mathf.Abs(afterResetYawAngle - beforeResetYawAngle) > Mathf.Abs(afterResetYawAngle - (beforeResetYawAngle + Mathf.PI * 2f)))
        {
            beforeResetYawAngle += Mathf.PI * 2f;
        }
        else if (Mathf.Abs(afterResetYawAngle - beforeResetYawAngle) > Mathf.Abs(afterResetYawAngle - (beforeResetYawAngle - Mathf.PI * 2f)))
        {
            beforeResetYawAngle -= Mathf.PI * 2f;
        }
    }

    /// <summary>
    /// resetTime秒でリセットボタンを押した時の方向からプレイヤー前方向への線形補間
    /// </summary>
    void CameraResetting()
    {
        resettingTimer += Time.deltaTime;

        cameraYawAngle = Mathf.Lerp(beforeResetYawAngle, afterResetYawAngle, resettingTimer / ResetDuration);
        cameraPitchAngle = Mathf.Lerp(beforeResetPitchAngle, 0, resettingTimer / ResetDuration);

        if (resettingTimer >= ResetDuration)
        {
            IsReset = false;
            resettingTimer = 0;
        }
    }

    /// <summary>
    ///障害物を検知しカメラが埋まらないようにして、カメラレイの線上にカメラを移動
    /// </summary>
    void CameraPositionSet()
    {
        returnTimer += Time.deltaTime;

        RaycastHit hit;
        Physics.Raycast(cameraRay, out hit, maxCameraDistance);
        if (hit.collider != null)
        {
            hitCameraDistance = (hit.point - cameraRay.origin).magnitude - 0.1f;
            returnTimer = 0;
        }

        cameraDistance = Mathf.Lerp(hitCameraDistance, maxCameraDistance, returnTimer / ReturnDelay);

        Camera.main.transform.position = cameraRay.origin + cameraRay.direction * cameraDistance;

        Camera.main.transform.LookAt(cameraRay.origin);
    }
}
