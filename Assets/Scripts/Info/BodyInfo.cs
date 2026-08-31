using Unity.Scripting.LifecycleManagement;
using Unity.VisualScripting;
using UnityEngine;

public class BodyInfo : MonoBehaviour
{
    [SerializeField] EnemyInfo enemyInfo;
    /// <summary>
    /// 修繕した身体に付けるマテリアル
    /// </summary>
    [SerializeField] Material repairedMaterial;
    /// <summary>
    /// 
    /// </summary>
    public float Life = 5;  // デフォルト
    
    public float attackCount = 0;
    public float stringRadius = 10;

    public bool isRepaired = false;
    public bool isJoint = false;

    private bool _checkFlag = false;

    [SerializeField] bool isHit = false;             // デバッグ用
    [SerializeField] bool isFinish = false;         // デバッグ用
    [SerializeField] bool isThreadBreak = false;   // デバッグ用


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

        //Hitのデバッグ用
        if (isHit)
        {
            Hit();
            isHit = false;
        }

        //Finishのデバッグ用
        if (isFinish)
        {
            Finish();
            isFinish = false;
        }

        //Finishのデバッグ用
        if (isThreadBreak)
        {
            ThreadBreak();
            isThreadBreak = false;
        }
    }

    public void Hit()
    {
        if (!isJoint)
        {
            isJoint = true;
            return;
        }

        if (isJoint)
        {
            attackCount++;
        }
    }

    public void Finish()
    {
        if (isJoint)
        {
            Life -= attackCount;
            attackCount = 0;
            isJoint = false;
        }
    }

    public void ThreadBreak()
    {
        attackCount = 0;
        isJoint = false;
    }
}
