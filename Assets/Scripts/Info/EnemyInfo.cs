using UnityEngine;

public class EnemyInfo : MonoBehaviour
{

    [Header("ボスのライフ(部位の数も兼ねる)")][SerializeField] float Life = 1; // デフォルトは1
    [SerializeField] GameObject AfterDefeatEnemy;
    [SerializeField] BodyInfo[] bodyInfos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Life <= 0)
        {
            Instantiate(AfterDefeatEnemy, transform.position + Vector3.up, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    public void bodyRepaired()
    {
        Life--;
    }

}
