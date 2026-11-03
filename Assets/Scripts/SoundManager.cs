using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;

    public static SoundManager playSound { get; private set; }

    [Header("ここにStitchingフォルダから自由にSEを入れて試してみよう")]
    [SerializeField] AudioClip StitchSE;
    [SerializeField] AudioClip BossPunchSE;
    [SerializeField] AudioClip BossKickSE;
    [SerializeField] AudioClip PlayerDamageSE;
    [SerializeField] AudioClip TamadomeFinishSE;
    [SerializeField] AudioClip BossDamageSE;
    [SerializeField] AudioClip PlayerWalkSE;
    [SerializeField] AudioClip BossWalkSE;
    [SerializeField] AudioClip BossChargeSE;
    [SerializeField] AudioClip TamadomeStartSE;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        playSound = this;
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (playSound == this)
            playSound = null;
    }

    /// <summary>
    /// SEを鳴らす。nullならば何もしない
    /// </summary>
    private void Play(AudioClip clip)
    {
        if (clip == null)
            return;
        audioSource.PlayOneShot(clip);
    }

    public void PlayAttackSE()
    {
        Play(StitchSE);
    }

    public void PlayBossPunchSE()
    {
        Play(BossPunchSE);
    }

    public void PlayBossKickSE()
    {
        Play(BossKickSE);
    }
    public void PlayPlayerDamageSE()
    {
        Play(PlayerDamageSE);
    }
    public void PlayTamadomeFinishSE()
    {
        Play(TamadomeFinishSE);
    }
    public void PlayBossDamageSE()
    {
        Play(BossDamageSE);
    }
    public void PlayPlayerWalkSE()
    {
        Play(PlayerWalkSE);
    }
    public void PlayBossWalkSE()
    {
        Play(BossWalkSE);
        Debug.Log("aruiteru");
    }
    public void PlayBossChargeSE()
    {
        Play(BossChargeSE);
    }
    public void PlayTamadomeStartSE()
    {
        Play(BossWalkSE);
    }

    



}
