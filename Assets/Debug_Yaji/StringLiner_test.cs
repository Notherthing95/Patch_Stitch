using UnityEngine;
using UnityEngine.InputSystem;


//controlPoint は約4-5倍の距離（ｚ  -2.8 : -11.2）
//disがrangeより減ったら、プレイヤーの移動方向と逆向きにcPを移動させる？
//cPが地面に近づく つく      cPSEを下げると同時にcPを上げる      cPSEが下がり切ったら↑
//戦うところは平らです
public class StringLiner_test : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] PlayerAttackController attackController;

    [SerializeField] Vector3 start;
    Vector3 controlPointS;
    [SerializeField] Vector3 controlPoint;
    Vector3 controlPointE;
    [SerializeField] Vector3 end;
    [SerializeField] float pointS_Y = 0;
    [SerializeField] float point_Y = 0;
    [SerializeField] float pointE_Y = 0;

    [SerializeField] bool Yurasu;

    [SerializeField] GameObject playrObj;

    float playerPosX;
    float playerPosZ;

    //float xMax = 5f;
    //float xMin = -5f;

    int komakasa = 100;

    float range;

    float length;

    float distance;

    //float a = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerPosX = playrObj.transform.position.x;
        playerPosZ = playrObj.transform.position.z;
        lineRenderer.positionCount = komakasa + 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (attackController.attackPoint.activeSelf)
        {
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;

                start = playrObj.transform.position;
                end = attackController.attackPoint.transform.position;
                controlPointS = start;
                controlPointE = end;

                range = attackController.moveRange;
                pointS_Y = start.y;
                pointE_Y = end.y;
            }
        }
        else
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }
        }

        start = playrObj.transform.position;
        end = attackController.attackPoint.transform.position;
        range = attackController.moveRange;
        distance = Vector3.Distance(new Vector3(end.x, 0, end.z), new Vector3(start.x, 0, start.z));

        //

        controlPointS = start;
        controlPointE = end;
        pointE_Y = pointS_Y;


        if (range + 0.5f >= distance && range - 0.5f <= distance)
        {
            controlPoint = start/* + (end - start)*/;
        }
        else
        {
            Vector3 temp = new Vector3(playerPosX - playrObj.transform.position.x, 0, playerPosZ - playrObj.transform.position.z);
            controlPoint += temp;
        }

        while (attackController.attackPoint.activeSelf)
        {
            for (int i = 0; i <= komakasa; i++)
            {
                float t = (float)i / komakasa;
                float u = 1 - t;


                Vector3 p = u * u * u * u * start +
                    u * u * u * t * controlPointS * 4f +
                    u * u * t * t * controlPoint * 6f +
                    u * t * t * t * controlPointE * 4f +
                    t * t * t * t * end;
                lineRenderer.SetPosition(i, p);
            }
            length = 0;
            for (int i = 0; i < komakasa; i++)
            {
                Vector3 pos1 = lineRenderer.GetPosition(i);
                Vector3 pos2 = lineRenderer.GetPosition(i + 1);
                length += Vector3.Distance(pos1, pos2);
            }

            if (length >= range + 0.5f)
            {
                Vector3 tempVector = new Vector3(-(end - start).normalized.z, 0, (end - start).normalized.x);
                if (Vector3.Distance(controlPoint, start) >= Vector3.Distance(controlPoint + tempVector, start))
                {
                    controlPoint += tempVector * 0.1f;
                }
                else
                {
                    controlPoint -= tempVector * 0.1f;
                }
            }
            else
            {
                break;
            }
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                break;
            }
        }
        //
        //Vector3 tempVector = new Vector3(-(end - start).normalized.z, 0, (end - start).normalized.x);
        //float temp;

        //temp = 1 - (Vector3.Distance(new Vector3(end.x, 0, end.z), new Vector3(start.x, 0, start.z)) / _range);

        //controlPoint = (playrObj.transform.position + tempVector * temp * 20);




        controlPointS.Set(controlPointS.x, pointS_Y, controlPointS.z);
        //controlPoint.Set(controlPoint.x, point_Y, controlPoint.z);
        controlPointE.Set(controlPointE.x, pointE_Y, controlPointE.z);

        if (Yurasu)
        {
            //Debug.Log(Mathf.Cos(Time.time * 3f) / 3f);
            //Debug.Log(Mathf.Abs(Mathf.Cos(Time.time * 3f) / 10f));
            controlPoint.Set(controlPoint.x + (end - start).normalized.z * Mathf.Cos(Time.time * 3f) / 3f, point_Y + Mathf.Abs(Mathf.Cos(Time.time * 3f) / 10f), controlPoint.z + (end - start).normalized.x * Mathf.Sin(Time.time * 3f) / 3f);
        }



        //描画部分

        for (int i = 0; i <= komakasa; i++)
        {
            float t = (float)i / komakasa;
            float u = 1 - t;


            Vector3 p = u * u * u * u * start +
                u * u * u * t * controlPointS * 4f +
                u * u * t * t * controlPoint * 6f +
                u * t * t * t * controlPointE * 4f +
                t * t * t * t * end;

            lineRenderer.SetPosition(i, p);
            //if (i == (int)komakasa / 2)
            //{
            //    Debug.Log(p.y);
            //}
        }
        length = 0;
        for (int i = 0; i < komakasa; i++)
        {
            Vector3 pos1 = lineRenderer.GetPosition(i);
            Vector3 pos2 = lineRenderer.GetPosition(i + 1);
            length += Vector3.Distance(pos1, pos2);
        }
        Debug.Log(length);

        playerPosX = playrObj.transform.position.x;
        playerPosZ = playrObj.transform.position.z;
    }
}
