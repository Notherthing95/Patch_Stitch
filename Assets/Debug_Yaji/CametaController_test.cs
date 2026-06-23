using UnityEngine;
using UnityEngine.InputSystem;

public class CametaController_test : MonoBehaviour
{
    InputAction camraAction;

    [SerializeField] GameObject playerObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camraAction = InputSystem.actions.FindAction("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
