using UnityEngine;

public class StringLinerComplex : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
