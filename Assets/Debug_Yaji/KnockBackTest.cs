using UnityEngine;

public class KnockBackTest : MonoBehaviour
{
    //PlayerMoveController _moveController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.GetComponent<PlayerMoveController>() != null)
        {
            collision.gameObject.GetComponent<PlayerMoveController>().OnKnockBack(transform, 16f);
        }
    }
}
