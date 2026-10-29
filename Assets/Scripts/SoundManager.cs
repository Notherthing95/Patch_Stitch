using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;

    [Header("ここにStitchingフォルダから自由にSEを入れて試してみよう")]
    [SerializeField]AudioClip StitchSE;
    [SerializeField]AudioClip BossPunchSE;
    [SerializeField]AudioClip BossKickSE;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAttackSE()
    {
        audioSource.PlayOneShot(StitchSE);
    }

    public void PlayBossPunchSE()
    {
        audioSource.PlayOneShot(BossPunchSE);
    }

    public void PlayBossKickSE()
    {
        audioSource.PlayOneShot(BossKickSE);
    }



}
