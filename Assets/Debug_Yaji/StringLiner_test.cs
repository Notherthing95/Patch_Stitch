using UnityEngine;

public class StringLiner_test : MonoBehaviour
{
    LineRenderer lineRenderer;

    [SerializeField] Vector3 start;
    [SerializeField] Vector3 between;
    [SerializeField] Vector3 end;
    [SerializeField] float betY = 0;

    [SerializeField] GameObject playrObj;

    //float xMax = 5f;
    //float xMin = -5f;

    int komakasa = 100;

    //float a = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        
    }

    // Update is called once per frame
    void Update()
    {
        start = playrObj.transform.position;
        between = start + (end - start) / 2;
        Debug.Log(between);
        Debug.Log(Mathf.Cos(Time.time * 3f) / 3f);
        between.Set(between.x + Mathf.Cos(Time.time * 3f) / 3f, betY, between.z/* + Mathf.Sin(Time.time)*/);
        

        lineRenderer.positionCount = komakasa + 1;
        
        for(int i = 0;i <= komakasa;i++)
        {
            float t = (float)i / komakasa;
            float u = 1 -t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * start;
            p += 2 * u * t * between;
            p += tt * end;

            lineRenderer.SetPosition(i, p);
        }

    }
}
