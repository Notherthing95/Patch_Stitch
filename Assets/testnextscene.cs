using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class testnextscene : MonoBehaviour
{
    float time = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
            if(time > 5)
            SceneManager.LoadScene("Stage1");
    }
}
