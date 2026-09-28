using NUnit;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 糸の表示と動きの制御のスクリプト 糸の動きシンプルVer
/// </summary>
public class StringLinerSimple : MonoBehaviour
{
    LineRenderer _lineRenderer;
    [SerializeField] GameObject player;
    PlayerAttackController _playerAttackController;
    [SerializeField] Transform needleThreadPosition;

    /// <summary>
    /// 糸の始点
    /// </summary>
    Vector3 _start;

    /// <summary>
    /// 糸の終点
    /// </summary>
    Vector3 _end;

    /// <summary>
    /// 糸のベジェ曲線の制御点 3つ
    /// </summary>
    Vector3[] _controlPoints = new Vector3[3];

    /// <summary>
    /// 制御点のY座標 糸のたるみの表現用
    /// </summary>
    float[] _pointHeights = new float[3];

    /// <summary>
    /// 線の描画の細かさ
    /// </summary>
    const int Resolution = 100;

    /// <summary>
    /// 糸が揺れるように動かすための遅延のフレーム数
    /// </summary>
    const int DelayFrames = 60;

    /// <summary>
    /// 中心の制御点の位置のバッファ DelayFramesの数記録
    /// </summary>
    Vector3[] _positionBuffer = new Vector3[DelayFrames];

    /// <summary>
    /// 中心の制御点を揺らすためのベクトルのバッファ DelayFrameの2倍の数記録
    /// </summary>
    Vector3[] _shakeVectorBuffer = new Vector3[DelayFrames * 2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _playerAttackController = player.GetComponent<PlayerAttackController>();
        _lineRenderer.positionCount = Resolution + 1;
        _lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerAttackController.attackPoint.activeSelf)
        {
            _start = needleThreadPosition.position;
            _end = _playerAttackController.attackPoint.transform.position;
            _controlPoints[0] = _start;
            _controlPoints[2] = _end;

            if (!_lineRenderer.enabled)
            {
                _lineRenderer.enabled = true;

                for (int i = 0; i < DelayFrames; i++)
                {
                    _positionBuffer[i] = _start + (_end - _start) / 2f;
                }
            }

            UpdateDelayedControlPoint();

            UpdateShakeControlPoint();

            float distance = Vector3.Distance(new Vector3(_start.x, 0, _start.z), new Vector3(_end.x, 0, _end.z));
            _pointHeights[1] = Mathf.Lerp(-_start.y * 1.5f, _start.y + (_end.y - _start.y) / 2f, distance / _playerAttackController.moveRange);
            _controlPoints[1].Set(_controlPoints[1].x, _pointHeights[1], _controlPoints[1].z);


            DrawString();
        }
        else
        {
            if (_lineRenderer.enabled)
            {
                _lineRenderer.enabled = false;
            }
        }
    }

    /// <summary>
    /// 糸を表す線の描画
    /// </summary>
    void DrawString()
    {
        for (int i = 0; i <= Resolution; i++)
        {
            float t = (float)i / Resolution;
            float u = 1 - t;


            Vector3 p = u * u * u * u * _start +
                u * u * u * t * _controlPoints[0] * 4f +
                u * u * t * t * _controlPoints[1] * 6f +
                u * t * t * t * _controlPoints[2] * 4f +
                t * t * t * t * _end;

            _lineRenderer.SetPosition(i, p);
        }
    }

    /// <summary>
    /// 中心の制御点の位置を決め、バッファに記録 DerayFrames遅れて反映
    /// </summary>
    void UpdateDelayedControlPoint()
    {
        _controlPoints[1] = _positionBuffer[DelayFrames - 1];

        for (int i = DelayFrames - 1; i > 0; i--)
        {
            _positionBuffer[i] = _positionBuffer[i - 1];
        }
        _positionBuffer[0] = _start + (_end - _start) / 2f;
    }

    /// <summary>
    /// 中心の制御点を揺らすベクトルを計算し、バッファに記録 DelayFramesの2倍遅れて反映
    /// </summary>
    void UpdateShakeControlPoint()
    {
        _controlPoints[1] -= _shakeVectorBuffer[DelayFrames * 2 - 1];

        for (int i = DelayFrames * 2 - 1; i > 0; i--)
        {
            _shakeVectorBuffer[i] = _shakeVectorBuffer[i - 1];
        }
        _shakeVectorBuffer[0] = (_controlPoints[1] - _positionBuffer[0]) * 0.5f;
    }
}
