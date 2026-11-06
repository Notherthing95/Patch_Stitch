using UnityEngine;

public class EnemyLifeBarController : MonoBehaviour
{
    /// <summary>
    /// 敵の部位
    /// </summary>
    [SerializeField] GameObject enemyBodyParts;

    /// <summary>
    /// バーの外枠
    /// </summary>
    [SerializeField] GameObject barUI;

    /// <summary>
    /// 実際のライフ
    /// </summary>
    [SerializeField] RectTransform lifeBar;

    /// <summary>
    /// 攻撃確定前のライフ
    /// </summary>
    [SerializeField] RectTransform lifePreview;

    /// <summary>
    /// 敵の部位とバーをつなぐ線
    /// </summary>
    [SerializeField] GameObject line;

    Canvas _canvas;
    BodyInfo _bodyInfo;
    LineRenderer _lineRenderer;

    /// <summary>
    /// 敵の最大ライフ
    /// </summary>
    float _maxLife;

    Vector3 _initialPosition;

    float _previousAttackCount;

    float _vibrationTimer = -1f;

    const float VibrationDuration = 0.15f;

    /// <summary>
    /// lifeBarの最大長さ
    /// </summary>
    float _maxLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _bodyInfo = enemyBodyParts.GetComponent<BodyInfo>();
        _lineRenderer = line.GetComponent<LineRenderer>();
        _maxLife = _bodyInfo.Life;
        _previousAttackCount = _maxLife;
        _lineRenderer.positionCount = 3;
        _maxLength = lifeBar.rect.width;
        _initialPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bodyInfo.isJoint)
        {
            if (!_canvas.enabled)
            {
                _canvas.enabled = true;
                line.SetActive(true);
            }

            if (_bodyInfo.attackCount != _previousAttackCount)
            {
                _vibrationTimer = 0;
            }
            if (_vibrationTimer >= 0)
            {
                _vibrationTimer += Time.deltaTime;

                this.transform.localPosition = _initialPosition + new Vector3(Mathf.Sin(_vibrationTimer * 10) * 0.5f, Mathf.Sin(2 * _vibrationTimer * 10) * 0.5f, 0);

                if (_vibrationTimer >= VibrationDuration)
                {
                    _vibrationTimer = -1f;
                }
            }

            //攻撃がヒットした回数分lifePreviewを短くする
            lifePreview.sizeDelta = new Vector2(Mathf.Lerp(_maxLength, 0f, 1 - (_bodyInfo.Life - _bodyInfo.attackCount) / _maxLife), lifePreview.rect.height);

            //バーをカメラ方向に向ける
            barUI.transform.LookAt(new Vector3(Camera.main.transform.position.x, barUI.transform.position.y, Camera.main.transform.position.z));

            //バーと敵の部位をつなぐ線の描画
            DrawLine();
        }
        else
        {
            //フィニッシュ攻撃が完了していた際に、lifeBarを短くする
            if (Mathf.Lerp(_maxLength, 0f, 1 - _bodyInfo.Life / _maxLife) == lifePreview.rect.width)
            {
                lifeBar.sizeDelta = lifePreview.sizeDelta;
            }

            if (_canvas.enabled)
            {
                _canvas.enabled = false;
                line.SetActive(false);
            }
        }

        _previousAttackCount = _bodyInfo.attackCount;
    }

    void DrawLine()
    {
        _lineRenderer.SetPosition(0, line.transform.position);
        _lineRenderer.SetPosition(2, enemyBodyParts.transform.position);

        Vector3 temp = (enemyBodyParts.transform.position - line.transform.position).normalized;
        _lineRenderer.SetPosition(1, new Vector3(temp.x, 0, temp.z) * 0.5f + line.transform.position);
    }
}
