using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{

    [Header("ゲームオーバーの際に読み込むシーンの名前")][SerializeField] string loadSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 一旦アクティブのシーンの名前を読み込む。他のシーンに遷移するようになったら消すこと
        loadSceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameOver()
    {
        Debug.Log("GAME OVER");
        Invoke(nameof(Restart), 2f);      // 2秒後にRestart()を呼ぶ
    }

    void Restart()
    {
        SceneManager.LoadScene(loadSceneName);
    }
}
