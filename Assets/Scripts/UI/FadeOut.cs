using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeOutCanvas;
    private bool _isFadeOut;

    [Tooltip("フェードアウトにかかる時間")][SerializeField] float FadeOutTime = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFadeOut && fadeOutCanvas.alpha < 1)
        {
            fadeOutCanvas.alpha += Time.deltaTime / FadeOutTime;
        }
    }

    public void StartFadeOut()
    {
       _isFadeOut = true;
    }
}
