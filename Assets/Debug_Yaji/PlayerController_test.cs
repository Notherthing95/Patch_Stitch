using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_test : MonoBehaviour
{
    InputAction moveAction;
    InputAction attackAction;
    InputAction finalAttackAction;

    Rigidbody p_Rigidbody;

    [SerializeField] GameObject _attackRangePreview;
    GameObject attackRangePreview;

    Vector2 readVector;
    Vector3 moveVector;
    Vector3 attackPoint;

    [SerializeField]float moveSpeed = 10;
    [SerializeField] float moveRange = 15;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        finalAttackAction = InputSystem.actions.FindAction("FinalAttack");
        p_Rigidbody = GetComponent<Rigidbody>();
        attackRangePreview = Instantiate(_attackRangePreview);
        attackRangePreview.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction.triggered)
        {
            attackPoint = transform.position;
            attackRangePreview.transform.position = attackPoint;
            attackRangePreview.SetActive (true);
        }
    }

    void FixedUpdate()
    {
        readVector = moveAction.ReadValue<Vector2>();
        moveVector = readVector.y * transform.forward + readVector.x * transform.right;

        if(Vector3.Distance(attackPoint, transform.position) >= moveRange)
        {
            if(Vector3.Distance(attackPoint,transform.position) > Vector3.Distance(attackPoint, transform.position + moveVector))
            {
                p_Rigidbody.linearVelocity = moveVector * moveSpeed;

            }
            else
            {
                p_Rigidbody.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            p_Rigidbody.linearVelocity = moveVector * moveSpeed;
        }


        //p_Rigidbody.linearVelocity = moveVector;
    }
}
