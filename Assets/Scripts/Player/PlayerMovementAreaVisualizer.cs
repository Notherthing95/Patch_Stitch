using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// プレイヤーの移動範囲の制限を可視化するスクリプト
/// </summary>
public class PlayerMovementAreaVisualizer : MonoBehaviour
{
    [SerializeField] PlayerAttackController sc_attackController;

    [SerializeField] GameObject _areaCircle;
    GameObject areaCircle;

    DecalProjector decalProjector;

    /// <summary>
    /// エリア円のサイズ(x,width y,height z,depth)
    /// </summary>
    Vector3 areaCircleSize;

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
    bool isChangeRange;

    /// <summary>
    /// 戦闘状態移行時の制限距離
    /// </summary>
    float maxMoveRange;

    /// <summary>
    /// isChangeRangeがfalseの時の1フレーム前の制限距離
    /// </summary>
    float oldMoveRange;

    /// <summary>
    /// エリアの範囲が縮小するのにかかる時間
    /// </summary>
    const float Duration = 0.2f;

    /// <summary>
    /// エリアの範囲が縮小し始めてから現在経った時間
    /// </summary>
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        areaCircle = Instantiate(_areaCircle);
        decalProjector = areaCircle.GetComponent<DecalProjector>();
        areaCircle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //AttackPointがアクティブ状態 -> 戦闘状態 移動範囲制限がある
        if (sc_attackController.AttackPoint.activeSelf)
        {
            if (!areaCircle.activeSelf)
            {
                areaCircle.SetActive(true);
                areaCircleSize = new Vector3(sc_attackController.MoveRange * 2, sc_attackController.MoveRange * 2, DepthValue);
                decalProjector.size = areaCircleSize;
                maxMoveRange = sc_attackController.MoveRange;
                oldMoveRange = sc_attackController.MoveRange;
                decalProjector.material.SetColor("_DecalColor", new Color(0, 1f, 0, AlphaValue));
            }

            //制限距離が変わった時
            if (oldMoveRange != sc_attackController.MoveRange)
            {
                isChangeRange = true;
            }
            if (isChangeRange)
            {
                float temp = Mathf.Lerp(oldMoveRange, sc_attackController.MoveRange, timer / Duration);
                areaCircleSize = new Vector3(temp * 2, temp * 2, DepthValue);
                timer += Time.deltaTime;
                decalProjector.size = areaCircleSize;

                if (timer >= Duration)
                {
                    isChangeRange = false;
                    timer = 0;
                    oldMoveRange = sc_attackController.MoveRange;
                }

                ChangeColor();
            }
            else
            {
                oldMoveRange = sc_attackController.MoveRange;
            }

            areaCircle.transform.position = sc_attackController.AttackPoint.transform.position;
        }
        else if (areaCircle.activeSelf)
        {
            areaCircle.SetActive(false);
        }
    }

    /// <summary>
    /// 制限距離に応じてエリアの色を変化させる 緑->黄->赤
    /// </summary>
    void ChangeColor()
    {
        if (sc_attackController.MoveRange >= maxMoveRange / 2)
        {
            float temp = maxMoveRange - sc_attackController.MoveRange;
            float r = Mathf.Lerp(Color.green.r, Color.yellow.r, 1 - (sc_attackController.MoveRange - temp) / maxMoveRange);
            float g = Mathf.Lerp(Color.green.g, Color.yellow.g, 1 - (sc_attackController.MoveRange - temp) / maxMoveRange);
            float b = Mathf.Lerp(Color.green.b, Color.yellow.b, 1 - (sc_attackController.MoveRange - temp) / maxMoveRange);
            decalProjector.material.SetColor("_DecalColor", new Color(r, g, b, AlphaValue));
        }
        else
        {
            float r = Mathf.Lerp(Color.yellow.r, Color.red.r, 1 - sc_attackController.MoveRange / (maxMoveRange / 2));
            float g = Mathf.Lerp(Color.yellow.g, Color.red.g, 1 - sc_attackController.MoveRange / (maxMoveRange / 2));
            float b = Mathf.Lerp(Color.yellow.b, Color.red.b, 1 - sc_attackController.MoveRange / (maxMoveRange / 2));
            decalProjector.material.SetColor("_DecalColor", new Color(r, g, b, AlphaValue));
        }
    }
}
