using UnityEngine;

public class TranslateScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0,0,Mathf.Sin(Time.time)* Time.deltaTime * 30);
    }
}
