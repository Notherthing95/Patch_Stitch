using UnityEngine;

/// <summary>
/// 仮でボスを治したら出てくるアイテム
/// </summary>
public class ClearItem : MonoBehaviour 
{
    /// <summary>
    /// 既に取ったか
    /// </summary>
    private bool _isPicked = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPicked)
            return;
        if (!other.CompareTag("Player"))
            return;

        _isPicked = true;
        Debug.Log("STAGE CLEAR!!");
        Destroy(gameObject);

    }
}
