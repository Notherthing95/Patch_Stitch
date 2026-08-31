using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// プレイヤーの移動範囲の制限を可視化するスクリプト
/// </summary>
public class PlayerMovementAreaVisualizer : MonoBehaviour
{
    [SerializeField] PlayerAttackController _attackController;

    [SerializeField] GameObject areaCircle;
    GameObject _areaCircle;

    DecalProjector _decalProjector;

    /// <summary>
    /// エリア円のサイズ(x,width y,height z,depth)
    /// </summary>
    Vector3 _areaCircleSize;

    /// <summary>
    /// DecalProjectorのsizeのDepthの値
    /// </summary>
    const float DepthValue = 10f;

    /// <summary>
    /// 透明度の値
    /// </summary>
    const float AlphaValue = 0.5f;

    /// <summary>
    /// 制限距離が変わりエリアが縮小している状態
    /// </summary>
    bool _isChangeRange;

    /// <summary>
    /// 戦闘状態移行時の制限距離
    /// </summary>
    float _maxMoveRange;

    /// <summary>
    /// isChangeRangeがfalseの時の1フレーム前の制限距離
    /// </summary>
    float _oldMoveRange;

    /// <summary>
    /// エリアの範囲が縮小するのにかかる時間
    /// </summary>
    const float Duration = 0.2f;

    /// <summary>
    /// エリアの範囲が縮小し始めてから現在経った時間
    /// </summary>
    float _timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _areaCircle = Instantiate(areaCircle);
        _decalProjector = _areaCircle.GetComponent<DecalProjector>();
        _areaCircle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //AttackPointがアクティブ状態 -> 戦闘状態 移動範囲制限がある
        if (_attackController.attackPoint.activeSelf)
        {
            if (!_areaCircle.activeSelf)
            {
                _areaCircle.SetActive(true);
                _areaCircleSize = new Vector3(_attackController.moveRange * 2, _attackController.moveRange * 2, DepthValue);
                _decalProjector.size = _areaCircleSize;
                _maxMoveRange = _attackController.moveRange;
                _oldMoveRange = _attackController.moveRange;
                _decalProjector.material.SetColor("_DecalColor", new Color(0, 1f, 0, AlphaValue));
            }

            //制限距離が変わった時
            if (_oldMoveRange != _attackController.moveRange)
            {
                _isChangeRange = true;
            }
            if (_isChangeRange)
            {
                float temp = Mathf.Lerp(_oldMoveRange, _attackController.moveRange, _timer / Duration);
                _areaCircleSize = new Vector3(temp * 2, temp * 2, DepthValue);
                _timer += Time.deltaTime;
                _decalProjector.size = _areaCircleSize;

                if (_timer >= Duration)
                {
                    _isChangeRange = false;
                    _timer = 0;
                    _oldMoveRange = _attackController.moveRange;
                }

                ChangeColor();
            }
            else
            {
                _oldMoveRange = _attackController.moveRange;
            }

            _areaCircle.transform.position = _attackController.attackPoint.transform.position;
        }
        else if (_areaCircle.activeSelf)
        {
            _areaCircle.SetActive(false);
        }
    }

    /// <summary>
    /// 制限距離に応じてエリアの色を変化させる 緑->黄->赤
    /// </summary>
    void ChangeColor()
    {
        if (_attackController.moveRange >= _maxMoveRange / 2)
        {
            float temp = _maxMoveRange - _attackController.moveRange;
            float r = Mathf.Lerp(Color.green.r, Color.yellow.r, 1 - (_attackController.moveRange - temp) / _maxMoveRange);
            float g = Mathf.Lerp(Color.green.g, Color.yellow.g, 1 - (_attackController.moveRange - temp) / _maxMoveRange);
            float b = Mathf.Lerp(Color.green.b, Color.yellow.b, 1 - (_attackController.moveRange - temp) / _maxMoveRange);
            _decalProjector.material.SetColor("_DecalColor", new Color(r, g, b, AlphaValue));
        }
        else
        {
            float r = Mathf.Lerp(Color.yellow.r, Color.red.r, 1 - _attackController.moveRange / (_maxMoveRange / 2));
            float g = Mathf.Lerp(Color.yellow.g, Color.red.g, 1 - _attackController.moveRange / (_maxMoveRange / 2));
            float b = Mathf.Lerp(Color.yellow.b, Color.red.b, 1 - _attackController.moveRange / (_maxMoveRange / 2));
            _decalProjector.material.SetColor("_DecalColor", new Color(r, g, b, AlphaValue));
        }
    }
}
