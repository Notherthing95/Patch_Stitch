using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WoundVisualController : MonoBehaviour
{
    [SerializeField] DecalProjector[] woundDecals;
    BodyInfo _bodyInfo;

    /// <summary>
    /// 部位の最大ライフ
    /// </summary>
    float _maxLife;

    /// <summary>
    /// 最大ライフと現在のライフの差
    /// </summary>
    float _lostLife;

    /// <summary>
    /// 1フレーム前の部位の攻撃回数
    /// </summary>
    float _previousAttackCount;

    /// <summary>
    /// 傷のテクスチャの最大高さ
    /// </summary>
    float[] _maxHeight;

    /// <summary>
    /// 1フレーム前の傷のテクスチャの高さ
    /// </summary>
    float[] _previousHeight;

    /// <summary>
    /// 現在の傷のテクスチャの高さ
    /// </summary>
    float[] _Height;

    /// <summary>
    /// 1つの傷あたりのライフ量
    /// </summary>
    float _lifePerWound;

    /// <summary>
    /// 1フレーム前の糸が繋がっているか
    /// </summary>
    bool _isJointPrevious;

    /// <summary>
    /// 傷のテクスチャの最小高さ
    /// </summary>
    const float MinHeight = 0.04f;

    /// <summary>
    /// 傷が縮まるのにかかる時間
    /// </summary>
    const float Duration = 0.17f;

    /// <summary>
    /// 傷が縮まり始めてから現在経った時間 0で開始 -1で停止
    /// </summary>
    float _timer = -1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bodyInfo = GetComponent<BodyInfo>();
        _maxLife = _bodyInfo.Life;
        _maxHeight = new float[woundDecals.Length];
        _previousHeight = new float[woundDecals.Length];
        for (int i = 0; i < woundDecals.Length; i++)
        {
            _maxHeight[i] = woundDecals[i].size.y;
            _previousHeight[i] = _maxHeight[i];
        }
        _Height = new float[woundDecals.Length];
        _lifePerWound = _maxLife / woundDecals.Length;
    }

    // Update is called once per frame
    void Update()
    {
        _lostLife = _maxLife - _bodyInfo.Life;
        if (_bodyInfo.isJoint)
        {
            if (!_isJointPrevious)
            {
                _isJointPrevious = true;
            }

            if (_previousAttackCount != _bodyInfo.attackCount)
            {
                for (int i = 0; i < woundDecals.Length; i++)
                {
                    _Height[i] = Mathf.Lerp(_maxHeight[i], MinHeight, 1 - (_lifePerWound - ((_bodyInfo.attackCount + _lostLife) - _lifePerWound * (woundDecals.Length - 1 - i))) / _lifePerWound);
                }
                _timer = 0;
            }
        }
        else if (_isJointPrevious)
        {
            for (int i = 0; i < woundDecals.Length; i++)
            {
                _Height[i] = Mathf.Lerp(_maxHeight[i], MinHeight, 1 - (_lifePerWound - (_lostLife - _lifePerWound * (woundDecals.Length - 1 - i))) / _lifePerWound);
                _timer = 0;

                if (_Height[i] == MinHeight)
                {
                    woundDecals[i].gameObject.SetActive(false);
                }
            }
            _isJointPrevious = false;
            _previousAttackCount = 0;
        }
        _previousAttackCount = _bodyInfo.attackCount;


        if (_timer >= 0)
        {
            _timer += Time.deltaTime;

            for (int i = 0; i < woundDecals.Length; i++)
            {
                woundDecals[i].size = new Vector3(woundDecals[i].size.x, Mathf.Lerp(_previousHeight[i], _Height[i], _timer / Duration), woundDecals[i].size.z);
            }

            if (_timer >= Duration)
            {
                _timer = -1f;
                for (int i = 0; i < woundDecals.Length; i++)
                {
                    _previousHeight[i] = _Height[i];
                }
            }
        }
    }
}
