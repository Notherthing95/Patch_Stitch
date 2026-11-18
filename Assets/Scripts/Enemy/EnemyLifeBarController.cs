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

    /// <summary>
    /// lifeBarの最大長さ
    /// </summary>
    float _maxLength;

    /// <summary>
    /// 現在のlifeBarのサイズ
    /// </summary>
    float _width;

    /// <summary>
    /// lifePreviewが減った時に、減る前のlifePreview
    /// </summary>
    float _previousWidth;

    /// <summary>
    /// lifePreviewが減るのにかかる時間
    /// </summary>
    const float LifePreviewDuration = 0.17f;

    /// <summary>
    /// lifePreviewが減ってから現在経った時間 0で開始 -1で停止
    /// </summary>
    float _lifePreviewTimer = -1f;

    /// <summary>
    /// 1フレーム前のAttackCount
    /// </summary>
    float _previousAttackCount;

    /// <summary>
    /// 糸が切れてからバーを見せる時間
    /// </summary>
    const float ShowDuration = 1f;

    /// <summary>
    /// 糸が切れてから現在経った時間 0で開始 -1で停止
    /// </summary>
    float _showTimer = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _bodyInfo = enemyBodyParts.GetComponent<BodyInfo>();
        _lineRenderer = line.GetComponent<LineRenderer>();
        _maxLife = _bodyInfo.Life;
        _lineRenderer.positionCount = 3;
        _maxLength = lifeBar.rect.width;
        _width = _maxLength;
        _previousWidth = _width;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bodyInfo.isJoint)
        {
            _showTimer = -1f;

            if (!_canvas.enabled)
            {
                _canvas.enabled = true;
                line.SetActive(true);
            }

            //攻撃がヒットした回数分lifePreviewを短くする
            if (_previousAttackCount != _bodyInfo.attackCount && _lifePreviewTimer == -1f)
            {
                _lifePreviewTimer = 0;
                _width = Mathf.Lerp(_maxLength, 0f, 1 - (_bodyInfo.Life - _bodyInfo.attackCount) / _maxLife);
                _previousAttackCount = _bodyInfo.attackCount;
            }

            //実際の幅変更
            UpdateLifePreview();

            //バーをカメラ方向に向ける
            barUI.transform.LookAt(new Vector3(Camera.main.transform.position.x, barUI.transform.position.y, Camera.main.transform.position.z));
        }
        else
        {
            //フィニッシュ攻撃が完了していた際に、lifeBarを短くする
            if (Mathf.Lerp(_maxLength, 0f, 1 - _bodyInfo.Life / _maxLife) == lifePreview.rect.width)
            {
                lifeBar.sizeDelta = lifePreview.sizeDelta;
            }
            else
            {
                lifePreview.sizeDelta = new Vector2(Mathf.Lerp(_maxLength, 0f, 1 - _bodyInfo.Life / _maxLife), lifePreview.rect.height);
            }

            //糸が切れた後、ShowDurationだけ残す
            if (_canvas.enabled && _showTimer == -1f)
            {
                _showTimer = 0;
            }
            if (_showTimer >= 0)
            {
                _showTimer += Time.deltaTime;
                if (_showTimer >= ShowDuration)
                {
                    _canvas.enabled = false;
                    line.SetActive(false);
                    _showTimer = -1f;
                }
            }
        }

        //バーと敵の部位をつなぐ線の描画
        DrawLine();
    }

    /// <summary>
    /// バーと敵の部位をつなぐ線を描画
    /// </summary>
    void DrawLine()
    {
        _lineRenderer.SetPosition(0, line.transform.position);
        _lineRenderer.SetPosition(2, enemyBodyParts.transform.position);

        Vector3 temp = (enemyBodyParts.transform.position - line.transform.position).normalized;
        _lineRenderer.SetPosition(1, new Vector3(temp.x, 0, temp.z) * 0.5f + line.transform.position);
    }

    /// <summary>
    /// lifePreviewのwidthが変更された時に線形補完をかける
    /// </summary>
    void UpdateLifePreview()
    {
        if (_lifePreviewTimer >= 0)
        {
            _lifePreviewTimer += Time.deltaTime;
            float temp = Mathf.Lerp(_previousWidth, _width, _lifePreviewTimer / LifePreviewDuration);
            lifePreview.sizeDelta = new Vector2(temp, lifePreview.rect.height);
            if (_lifePreviewTimer >= LifePreviewDuration)
            {
                _lifePreviewTimer = -1f;
                lifePreview.sizeDelta = new Vector2(_width, lifePreview.rect.height);
                _previousWidth = _width;
            }
        }
    }
}
