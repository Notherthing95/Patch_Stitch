using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] GameOverManager gameOverManager;
    private bool _isDead = false;

    
    [Header("ÉvÉåÉCÉÑÅ[ÇÃHP")]public int PlayerLife = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int AmountDamage)
    {
        if (_isDead)
            return;

        PlayerLife -= AmountDamage;

        if (PlayerLife <= 0)
        {
            PlayerLife = 0;
            _isDead = true;
            gameOverManager.GameOver();
        }
    }
}
