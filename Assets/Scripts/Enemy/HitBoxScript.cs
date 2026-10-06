using UnityEngine;

public class HitBoxScript : MonoBehaviour
{
    /// <summary>
    /// テストパワー
    /// </summary>
    [SerializeField] float power = 16f;

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
                playerMoveController.OnKnockBack(gameObject.transform, power);
        }
    }
}
