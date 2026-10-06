using UnityEngine;

public class KnockBackTest : MonoBehaviour
{
    PlayerMoveController _moveController;

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
            _moveController = collision.gameObject.GetComponent<PlayerMoveController>();
            _moveController.OnKnockBack(transform, 12f);
        }
    }
}
