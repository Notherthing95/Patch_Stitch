using UnityEngine;

public class StringPosition : MonoBehaviour
{
    [SerializeField] GameObject AttachObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(AttachObject != null)
        transform.position = AttachObject.transform.position;
    }
}
