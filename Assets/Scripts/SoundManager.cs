using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;

    [Header("ここにStitchingフォルダから自由にSEを入れて試してみよう")]
    [SerializeField]AudioClip StitchSE;
    [SerializeField]AudioClip BossPunchSE;
    [SerializeField]AudioClip BossKickSE;    
    [SerializeField]AudioClip PlayerDamageSE;    
    [SerializeField]AudioClip TamadomeFinishSE;    
    [SerializeField]AudioClip BossDamageSE;    
    [SerializeField]AudioClip PlayerWalkSE;
    [SerializeField]AudioClip BossWalkSE;
    

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
    public void PlayPlayerDamageSE()
    {
        audioSource.PlayOneShot(PlayerDamageSE);
    }
    public void PlayTamadomeFinishSE()
    {
        audioSource.PlayOneShot(TamadomeFinishSE);
    }
    public void PlayBossDamageSE()
    {
        audioSource.PlayOneShot(BossDamageSE);
    }
    public void PlayPlayerWalkSE()
    {
        audioSource.PlayOneShot(PlayerWalkSE);
    }
    public void PlayBossWalkSE()
    {
        audioSource.PlayOneShot(BossWalkSE);
    }



}
