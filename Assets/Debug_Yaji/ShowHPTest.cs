using Unity.VisualScripting;
using UnityEngine;

public class ShowHPTest : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] GameObject bar;
    [SerializeField] RectTransform hp;
    [SerializeField] RectTransform preHp;
    [SerializeField] GameObject line;
    Canvas _canvas;
    BodyInfo _bodyInfo;
    LineRenderer _lineRenderer;

    float _maxLife;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _bodyInfo = body.GetComponent<BodyInfo>();
        _lineRenderer = line.GetComponent<LineRenderer>();
        _maxLife = _bodyInfo.Life;
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

            _lineRenderer.positionCount = 3;
            _lineRenderer.SetPosition(0, line.transform.position);
            Vector3 temp = (body.transform.position - line.transform.position).normalized;
            _lineRenderer.SetPosition(1, new Vector3(temp.x, 0, temp.z) * 0.5f + line.transform.position);
            _lineRenderer.SetPosition(2, body.transform.position);


            bar.transform.LookAt(new Vector3(Camera.main.transform.position.x, bar.transform.position.y, Camera.main.transform.position.z));

            preHp.sizeDelta = new Vector2(Mathf.Lerp(0.9f, 0f, 1 - (_bodyInfo.Life - _bodyInfo.attackCount) / _maxLife), preHp.sizeDelta.y);
        }
        else
        {
            if(Mathf.Lerp(0.9f, 0f, 1 - _bodyInfo.Life / _maxLife) == preHp.sizeDelta.x)
            {
                hp.sizeDelta = preHp.sizeDelta;
            }
            if (_canvas.enabled)
            {
                _canvas.enabled = false;
                line.SetActive(false);
            }
        }
    }
}
