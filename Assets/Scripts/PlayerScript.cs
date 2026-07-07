using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    //[SerializeField] bool isSewed = false;
    Vector3 moveInput;
    InputAction AttackAction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        Vector3 pos = transform.position;
        pos += moveInput * playerSpeed * Time.deltaTime;

        transform.position = pos;


    }


    
}
