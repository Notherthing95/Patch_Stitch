using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_test : MonoBehaviour
{
    InputAction moveAction;
    InputAction attackAction;
    InputAction finalAttackAction;
    InputAction dodgeAction;

    SpringJoint spring;

    Rigidbody p_Rigidbody;

    [SerializeField] GameObject _attackRangePreview;
    GameObject attackRangePreview;
    BodyInfo sc_bodyInfo;

    Vector2 readVector;
    Vector3 moveVector;
    Vector3 attackPoint;

    [SerializeField] float normalMoveSpeed = 10;
    [SerializeField] float dodgeMoveSpeed = 30;
    float moveSpeed;

    bool IsDodge = false;
    float dodgeTimer;

    [SerializeField] float moveRange = 15;
    float rangeCoefficient;

    Color color;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        finalAttackAction = InputSystem.actions.FindAction("FinalAttack");
        dodgeAction = InputSystem.actions.FindAction("Dodge");
        p_Rigidbody = GetComponent<Rigidbody>();
        attackRangePreview = Instantiate(_attackRangePreview);
        attackRangePreview.SetActive(false);

        moveSpeed = normalMoveSpeed;
        attackPoint = transform.position;
        spring = gameObject.AddComponent<SpringJoint>();
        spring.connectedAnchor = transform.position;
        spring.spring = 1000;
        spring.damper = 0.2f;
        spring.maxDistance = 100;
        spring.tolerance = 0.0001f;
        spring.autoConfigureConnectedAnchor = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.SphereCast(transform.position, .5f, transform.forward, out hit, 1);
        Debug.DrawRay(transform.position + transform.forward.normalized * 0.5f, transform.forward.normalized * 1);
        if (finalAttackAction.triggered)
        {
            if (sc_bodyInfo != null) sc_bodyInfo.Finish();
            sc_bodyInfo = null;
            attackRangePreview.SetActive(false);
            attackRangePreview.transform.parent = null;
            moveRange = 15;
            attackRangePreview.transform.localScale = new Vector3(moveRange * 2, attackRangePreview.transform.localScale.y, moveRange * 2);
            color = new Color(0, 1, 0);
            attackRangePreview.gameObject.GetComponent<Renderer>().material.color = color;
        }
        else if (attackAction.triggered)
        {
            if (attackRangePreview.activeSelf)
            {
                if (hit.collider != null)
                {
                    moveRange--;
                    attackRangePreview.transform.localScale = new Vector3(moveRange * 2 * rangeCoefficient, attackRangePreview.transform.localScale.y, moveRange * 2 * rangeCoefficient);
                    color = new Color(1 - 1f / 15f * moveRange, 1f / 15f * moveRange, 0);
                    attackRangePreview.gameObject.GetComponent<Renderer>().material.color = color;
                    sc_bodyInfo.Hit();
                }
            }
            else if (hit.collider != null)
            {
                attackRangePreview.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
                attackRangePreview.transform.parent = hit.collider.transform;
                rangeCoefficient = 1 / hit.collider.transform.parent.transform.localScale.x / hit.collider.transform.localScale.x;  //親のscaleと子のscaleから、円の係数を算出
                attackRangePreview.SetActive(true);
                sc_bodyInfo = hit.collider.GetComponent<BodyInfo>();
                if (sc_bodyInfo != null) sc_bodyInfo.Hit();
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


        if (dodgeAction.triggered && !IsDodge)
        {
            IsDodge = true;
            moveSpeed = dodgeMoveSpeed;
        }
        if (IsDodge)
        {
            dodgeTimer += Time.deltaTime;

            moveSpeed = Mathf.Lerp(dodgeMoveSpeed, normalMoveSpeed, dodgeTimer * (1 / 0.33f));
            if (dodgeTimer >= 0.33f)
            {
                dodgeTimer = 0;
                IsDodge = false;
            }
        }

        //transform.forward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
    }

    void FixedUpdate()
    {
        readVector = moveAction.ReadValue<Vector2>();
        //moveVector = readVector.y * transform.forward + readVector.x * transform.right;
        moveVector = readVector.y * new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized + readVector.x * new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;

        spring.connectedAnchor = attackPoint;
        spring.maxDistance = moveRange + .5f;
        //Debug.Log(spring.connectedAnchor);

        if (readVector.magnitude > 0.1)
        {
            if (attackRangePreview.activeSelf)
            {
                if (Vector3.Distance(attackPoint, transform.position) >= moveRange)
                {
                    if (Vector3.Distance(attackPoint, transform.position) > Vector3.Distance(attackPoint, transform.position + moveVector))//糸の根元に近づく時
                    {
                        if (IsDodge)
                        {
                            //p_Rigidbody.linearVelocity = moveVector.normalized * moveSpeed;
                        }
                        else
                        {
                           // p_Rigidbody.linearVelocity = moveVector * moveSpeed;
                        }
                    }
                    else
                    {
                      //  p_Rigidbody.linearVelocity = new Vector3((p_Rigidbody.linearVelocity / 1.2f).x, p_Rigidbody.linearVelocity.y, (p_Rigidbody.linearVelocity / 1.2f).z);
                    }
                }
                else
                {
                    if (IsDodge)
                    {
                       // p_Rigidbody.linearVelocity = moveVector.normalized * moveSpeed;
                    }
                    else
                    {
                      //  p_Rigidbody.linearVelocity = moveVector * moveSpeed;
                    }
                }
            }
            else
            {
                if (IsDodge)
                {
                    //p_Rigidbody.linearVelocity = moveVector.normalized * moveSpeed;
                }
                else
                {
                    //p_Rigidbody.linearVelocity = moveVector * moveSpeed; //移動スピードは2段階らしいですよ
                }
            }
           // transform.forward = new Vector3(p_Rigidbody.linearVelocity.x, 0, p_Rigidbody.linearVelocity.z).normalized;
        }
        else
        {
           // p_Rigidbody.linearVelocity = new Vector3((p_Rigidbody.linearVelocity / 1.2f).x, p_Rigidbody.linearVelocity.y, (p_Rigidbody.linearVelocity / 1.2f).z);
        }
    }
}
