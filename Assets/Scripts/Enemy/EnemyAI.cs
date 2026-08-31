using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float enemySpeed = 20.0f;
    [SerializeField] GameObject Player;
    
    public float distance = 0;
    public float attackRange = 10;
    public float trackPlayerRange = 50;
    float distanceX, distanceZ; // 二乗したPlayer-のx,y座標の意

    private NavMeshAgent navMeshAgent;

    // Debug用、仮攻撃処理用変数
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {


        // 距離の更新, 攻撃判定
        distanceX = Mathf.Abs(Player.transform.position.x - gameObject.transform.position.x);
        distanceZ = Mathf.Abs(Player.transform.position.z - gameObject.transform.position.z);
        distance = Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceZ,2));
        Debug.Log("Distance: " + distance);

        // 攻撃判定
        if(distance < attackRange)
        {
            Debug.Log("Attack!");
        }
        else if(distance < trackPlayerRange)    // 移動
        {
            navMeshAgent.destination = Player.transform.position;
        }

    }
}
