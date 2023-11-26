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

        //������ñ��Ӧ����������
        mConfigHelper.AddHolderType<SampleConfig>("sample_config");
    }

    protected override void EnterGame()
    {
        base.EnterGame();

        //��ȡ����
        var table = "sample_config".GetConfigTable<SampleConfig>();
        Debug.Log("�Ѷ�ȡ��� sample_config ������: " + table[1].answer);
        Debug.Log("��Ϸ������");
    }
}
