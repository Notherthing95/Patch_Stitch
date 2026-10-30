using UnityEngine;

public class PlayerLifeBar : MonoBehaviour
{
    RectTransform _playerLifeBar;
    [SerializeField] PlayerInfo playerInfo;

    float _maxLife;
    float _maxWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerLifeBar = GetComponent<RectTransform>();
        _maxLife = playerInfo.PlayerLife;
        _maxWidth = _playerLifeBar.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        _playerLifeBar.sizeDelta = new Vector2(Mathf.Lerp(0, _maxWidth, playerInfo.PlayerLife / _maxLife), _playerLifeBar.rect.height);
    }
}
