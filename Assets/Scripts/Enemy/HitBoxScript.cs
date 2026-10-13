using UnityEngine;

public class HitBoxScript : MonoBehaviour
{
    /// <summary>
    /// テストパワー
    /// </summary>
    [SerializeField] float power = 16f;

    /// <summary>
    /// ダメージ量
    /// </summary>
    [SerializeField] int AmountDamage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Playerが当たったらPlayerMoveControllerにある関数を呼ぶ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMoveController playerMoveController = other.gameObject.GetComponent<PlayerMoveController>();
            if (playerMoveController != null)
            {
                // 
                playerMoveController.OnKnockBack(gameObject.transform, power);
                Debug.Log("nnna-");
                PlayerInfo playerinfo = other.GetComponent<PlayerInfo>();
                if (playerinfo != null){
                    playerinfo.Damage(AmountDamage);
                    Debug.Log("nnni-");
                }
                Debug.Log("nnnu-");
            }
            
        }
    }
}
