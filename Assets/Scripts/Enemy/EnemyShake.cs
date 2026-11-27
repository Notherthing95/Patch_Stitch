using UnityEngine;


/// <summary>
/// プレイヤーの攻撃が当たった時に揺れる演出
/// </summary>
public class EnemyShake : MonoBehaviour
{
    [Header("揺れる大きさ")]
    [SerializeField] float shakePower = 0.05f;
    [Header("揺れる秒数")]
    [SerializeField] float shakeDuration = 0.1f;

    Vector3 _basePosition;  // 揺れる直前の位置
    float _timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _basePosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // 動いてなかったらreturn;
        if (_timer <= 0f)
            return;

        _timer -= Time.unscaledDeltaTime;
        if (_timer > 0f)
            transform.localPosition = _basePosition + Random.insideUnitSphere * shakePower;
        else
            transform.localPosition = _basePosition;    // 終わり次第、元の位置に
    }

    public void Shake()
    {
        _timer = shakeDuration;
    }
}
