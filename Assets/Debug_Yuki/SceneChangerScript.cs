using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Debug_Yuki");
    }
}