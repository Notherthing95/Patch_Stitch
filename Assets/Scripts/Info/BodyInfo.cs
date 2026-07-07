using Unity.VisualScripting;
using UnityEngine;

public class BodyInfo : MonoBehaviour
{
    [SerializeField] EnemyInfo enemyInfo;
    [SerializeField] Material repairedMaterial;

    public float Life = 5;  // デフォルト
    public bool isRepaired = false;
    public bool isJoint = false;

    private bool _checkFlag = false;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Life <= 0)
            isRepaired = true;

        if (isRepaired && !_checkFlag)
        {
            _checkFlag = true;
            enemyInfo.bodyRepaired();
            
        }
    }

    public void isHit()
    {
        if (!isJoint)
        {
            isJoint = true;
            return;
        }

        if (isJoint)
        {
            Life--;
        }
    }
}
