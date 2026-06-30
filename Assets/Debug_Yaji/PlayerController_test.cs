using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_test : MonoBehaviour
{
    InputAction moveAction;
    InputAction attackAction;
    InputAction finalAttackAction;

    SpringJoint spring;

    Rigidbody p_Rigidbody;

    [SerializeField] GameObject _attackRangePreview;
    GameObject attackRangePreview;

    Vector2 readVector;
    Vector3 moveVector;
    Vector3 attackPoint;

    [SerializeField] float moveSpeed = 10;
    [SerializeField] float moveRange = 15;
    float rangeCoefficient;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        finalAttackAction = InputSystem.actions.FindAction("FinalAttack");
        p_Rigidbody = GetComponent<Rigidbody>();
        spring = GetComponent<SpringJoint>();
        attackRangePreview = Instantiate(_attackRangePreview);
        attackRangePreview.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.SphereCast(transform.position, .5f, transform.forward, out hit, 1);
        Debug.DrawRay(transform.position + transform.forward.normalized * 0.5f, transform.forward.normalized * 1);
        if (attackAction.triggered)
        {
            if (attackRangePreview.activeSelf)
            {
                if (hit.collider != null)
                {
                    moveRange--;
                    attackRangePreview.transform.localScale = new Vector3(moveRange * 2 * rangeCoefficient, attackRangePreview.transform.localScale.y, moveRange * 2 * rangeCoefficient);
                }
            }
            if (hit.collider != null)
            {
                attackRangePreview.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
                attackRangePreview.transform.parent = hit.collider.transform;
                rangeCoefficient = 1 / hit.collider.transform.parent.transform.localScale.x / hit.collider.transform.localScale.x;
                attackRangePreview.SetActive(true);
            }
        }
        if (attackRangePreview.activeSelf)
        {
            attackPoint = attackRangePreview.transform.position;
        }
        else
        {
            attackPoint = transform.position;
        }

        if (finalAttackAction.triggered)
        {
            attackRangePreview.SetActive(false);
            attackRangePreview.transform.parent = null;
            moveRange = 15;
            attackRangePreview.transform.localScale = new Vector3(moveRange * 2, attackRangePreview.transform.localScale.y, moveRange * 2);
        }

    }

    void FixedUpdate()
    {
        readVector = moveAction.ReadValue<Vector2>();
        moveVector = readVector.y * transform.forward + readVector.x * transform.right;

        spring.connectedAnchor = attackPoint;
        spring.maxDistance = moveRange + .5f;
        Debug.Log(spring.connectedAnchor);

        if (readVector.magnitude > 0.1)
        {
            if (attackRangePreview.activeSelf)
            {
                if (Vector3.Distance(attackPoint, transform.position) >= moveRange)
                {
                    if (Vector3.Distance(attackPoint, transform.position) > Vector3.Distance(attackPoint, transform.position + moveVector))
                    {
                        p_Rigidbody.linearVelocity = moveVector * moveSpeed;

                    }
                    else
                    {
                        p_Rigidbody.linearVelocity /= 1.2f;
                    }
                }
                else
                {
                    p_Rigidbody.linearVelocity = moveVector * moveSpeed;
                }
            }
            else
            {
                p_Rigidbody.linearVelocity = moveVector * moveSpeed;
            }
        }
        else
        {
            p_Rigidbody.linearVelocity /= 1.2f;
        }
    }
}
