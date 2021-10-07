using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jackie;
using fs;

/// <summary>
/// 启动脚本
/// </summary>
public class GameMain:DnotMonoSingleton<GameMain>
{
    private void Awake()
    {
        OnInit();
        OnStart();

       
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
     
    }

    private void LateUpdate()
    {
       

    }

    /// <summary>
    /// 初始化框架
    /// </summary>
    private void OnInit() 
    {
        this.gameObject.AddComponent<Log2FileScript>();
        this.gameObject.AddComponent<AudioTool>();

        TableManager.instance.OnInit();
    }

    /// <summary>
    /// 开始创建游戏
    /// </summary>
    private void OnStart() 
    {
        SysManage.GetSys<GameSys>();
    }

    private IEnumerator LoadRes()
    {
        yield return 0;

        //对比版本



        //加载资源


    }
}
