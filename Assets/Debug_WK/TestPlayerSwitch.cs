using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerSwitch : MonoBehaviour
{
    [SerializeField] PlayerAnimation playerAnimation;
    Vector2 readVector;
    InputAction moveAction;
    InputAction attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            playerAnimation.SetAnimation[2] = true;
        }
        if (playerAnimation.CheckNormalizedAnimation("Attack1") == true)
        {
            playerAnimation.SetAnimation[2] = false;
        }
    }

    private void FixedUpdate()
    {
        readVector = moveAction.ReadValue<Vector2>();

        if (readVector.magnitude > 0.1f)
        {
            playerAnimation.SetAnimation[1] = true;
        }
        else
            playerAnimation.SetAnimation[1] = false;
    }
}
