using UnityEngine;

/// <summary>
/// プレイヤーの移動範囲の制限を可視化するスクリプト
/// </summary>
public class PlayerMovementAreaVisualizer : MonoBehaviour
{
    [SerializeField] PlayerAttackController sc_attackController;

    [SerializeField] GameObject _areaCircle;
    GameObject areaCircle;

    [SerializeField] Material areaCircleMaterial;

    /// <summary>
    /// エリアのスケール
    /// </summary>
    Vector3 areaCircleScale;

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
                areaCircleScale = new Vector3(sc_attackController.MoveRange * 2, 1, sc_attackController.MoveRange * 2);
                areaCircle.transform.localScale = areaCircleScale;
                maxMoveRange = sc_attackController.MoveRange;
                oldMoveRange = sc_attackController.MoveRange;
                areaCircleMaterial.color = Color.green;
            }

            //制限距離が変わった時
            if (oldMoveRange != sc_attackController.MoveRange)
            {
                isChangeRange = true;
            }
            if (isChangeRange)
            {
                float temp = Mathf.Lerp(oldMoveRange, sc_attackController.MoveRange, timer / Duration);
                areaCircleScale = new Vector3(temp * 2, 1, temp * 2);
                timer += Time.deltaTime;
                areaCircle.transform.localScale = areaCircleScale;

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

            areaCircle.transform.position = new Vector3(sc_attackController.AttackPoint.transform.position.x, 0.001f, sc_attackController.AttackPoint.transform.position.z);
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
            areaCircleMaterial.color = new Color(r, g, b, 0.5f);
        }
        else
        {
            float r = Mathf.Lerp(Color.yellow.r, Color.red.r, 1 - sc_attackController.MoveRange / (maxMoveRange / 2));
            float g = Mathf.Lerp(Color.yellow.g, Color.red.g, 1 - sc_attackController.MoveRange / (maxMoveRange / 2));
            float b = Mathf.Lerp(Color.yellow.b, Color.red.b, 1 - sc_attackController.MoveRange / (maxMoveRange / 2));
            areaCircleMaterial.color = new Color(r, g, b, 0.5f);
        }
    }
}
