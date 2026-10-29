using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject clearText;
    [SerializeField] FadeOut fadeOut;

    [Header("クリア後に読み込むシーンの名前（書いていなければ今のシーンをやり直す）")]
    [SerializeField] string nextSceneName;

    [Header("クリアしてから次へ進むまでの秒数(仮)")]
    [SerializeField] float waitTime = 3f;

    void Start()
    {
        if (clearText != null)
            clearText.SetActive(false);
    }

    void Update()
    {
        
    }

    public void StageClear()
    {
        Debug.Log("STAGE CLEAR");
        if (clearText != null)
            clearText.SetActive(true);

        Invoke(nameof(LoadNextScene), waitTime);
    }

    void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}
