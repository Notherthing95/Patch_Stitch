using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class CameraController_test : MonoBehaviour
{
    InputAction cameraAction;
    InputAction cameraResetAction;
    [SerializeField] GameObject playerObj;
    float cameraDistance;
    Ray cameraRay;
    float cameraAngleHorizontal;
    float cameraAngleVertical;
    Vector2 readVector2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraAction = InputSystem.actions.FindAction("Camera");
        cameraResetAction = InputSystem.actions.FindAction("CameraReset");
        cameraDistance = Vector3.Distance(playerObj.transform.position, Camera.main.transform.position);
        cameraRay.origin = playerObj.transform.position;
        cameraRay.direction = Camera.main.transform.position - playerObj.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        readVector2 = cameraAction.ReadValue<Vector2>();

        cameraRay.origin = playerObj.transform.position;

        cameraAngleHorizontal += readVector2.x * 0.01f;
        cameraAngleHorizontal %= 2 * Mathf.PI;

        cameraAngleVertical += readVector2.y * 0.01f;

        //xはコサイン zはサイン yは直入れ
        cameraRay.direction = new Vector3(Mathf.Cos(cameraAngleHorizontal), cameraAngleVertical, Mathf.Sin(cameraAngleHorizontal));

        if (cameraResetAction.triggered)
        {
            cameraAngleVertical = 0;
        }

        //Debug.Log("cos" + Mathf.Cos(cameraRay.direction.x) + "sin" + Mathf.Sin(cameraRay.direction.x));
        //Debug.Log(cameraAngleVertical);

        Debug.DrawRay(cameraRay.origin, cameraRay.direction * cameraDistance, Color.blue);

        RaycastHit hit;
        Physics.Raycast(cameraRay, out hit, cameraDistance);
        if (hit.collider != null)
        {
            Camera.main.transform.position = hit.point - Vector3.Normalize(Camera.main.transform.position - playerObj.transform.position) * 0.1f;
        }
        else
        {
            Camera.main.transform.position = cameraRay.origin + cameraRay.direction * cameraDistance;
        }

        Camera.main.transform.LookAt(playerObj.transform.position);
    }
}
