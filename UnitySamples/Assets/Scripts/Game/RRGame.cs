using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShipDock;
using StaticConfig;

public class RRGame : ShipDockWechatGame
{
    protected override void InitConfigTypes()
    {
        base.InitConfigTypes();

        //添加配置表对应的数据类型
        mConfigHelper.AddHolderType<SampleConfig>("sample_config");
    }

    protected override void EnterGame()
    {
        base.EnterGame();

        //读取配置
        var table = "sample_config".GetConfigTable<SampleConfig>();
        Debug.Log("已读取表格 sample_config 的数据: " + table[1].answer);
        Debug.Log("游戏已启动");
    }
}
