using UnityEngine;

public class PlayerLifeBar : MonoBehaviour
{
    RectTransform _playerLifeBar;
    [SerializeField] PlayerInfo playerInfo;

    /// <summary>
    /// 最大ライフ
    /// </summary>
    float _maxLife;

    /// <summary>
    /// 最大の幅
    /// </summary>
    float _maxWidth;

    /// <summary>
    /// ライフが減った時の減る前の幅
    /// </summary>
    float _previousWidth;

    /// <summary>
    /// 現在の幅
    /// </summary>
    float _width;

    /// <summary>
    /// 幅が縮まるのにかかる時間
    /// </summary>
    const float Duration = 0.17f;

    /// <summary>
    /// ライフが減ってから現在経った時間 0で開始 -1で停止
    /// </summary>
    float _timer = -1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerLifeBar = GetComponent<RectTransform>();
        _maxLife = playerInfo.PlayerLife;
        _maxWidth = _playerLifeBar.rect.width;
        _previousWidth = _maxWidth;
    }

    // Update is called once per frame
    void Update()
    {
        //ライフが減った時
        if (_previousWidth != playerInfo.PlayerLife && _timer == -1f)
        {
            _width = Mathf.Lerp(0, _maxWidth, playerInfo.PlayerLife / _maxLife);
            _timer = 0;
        }

        //実際の幅変更
        UpdateWidth();
    }

    void UpdateWidth()
    {
        if (_timer >= 0)
        {
            _timer += Time.deltaTime;
            float temp = Mathf.Lerp(_previousWidth, _width, _timer / Duration);
            _playerLifeBar.sizeDelta = new Vector2(temp, _playerLifeBar.rect.height);
            if (_timer >= Duration)
            {
                _timer = -1f;
                _previousWidth = _width;
                _playerLifeBar.sizeDelta = new Vector2(_width, _playerLifeBar.rect.height);
            }
        }
    }
}
