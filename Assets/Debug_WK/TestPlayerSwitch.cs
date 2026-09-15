using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerSwitch : MonoBehaviour
{
    [SerializeField] PlayerAnimation playerAnimation;
    //Vector2 readVector;
    //InputAction moveAction;
    InputAction attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            PlayerAnimation.targetState = 3;
        }
        if (playerAnimation.CheckNormalizedAnimation("P_Attack01") == true)
        {
            PlayerAnimation.targetState = 0;
        }
    }

    //private void FixedUpdate()
    //{
    //    readVector = moveAction.ReadValue<Vector2>();

    //    if (readVector.magnitude > 0.1f)
    //    {
    //        PlayerAnimation.targetState = 1;
    //    }
    //}
}
