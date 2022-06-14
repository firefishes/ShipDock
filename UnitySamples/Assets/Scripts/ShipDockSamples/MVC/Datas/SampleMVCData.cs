using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// ���ݲ�
/// 
/// ���ݿ��Ʋ�ĵ��ø������ݣ����������ݵı仯֪ͨ���Ʋ�
/// 
/// </summary>
public class SampleMVCData : DataProxy
{
    private const int STATE_MAX = 3;

    public int CurrentData { get; private set; }

    public SampleMVCData() : base(SampleConsts.D_SAMPLE_MVC)
    {
    }

    /// <summary>
    /// �޸�����
    /// </summary>
    /// <param name="value"></param>
    public void SetCurrentData(int value)
    {
        CurrentData = value;
        if (CurrentData > STATE_MAX)
        {
            CurrentData = 2;
        }
        else { }

        DataNotify(SampleConsts.DN_SAMPLE_MVC_DATA_CHANGED);
    }

    /// <summary>
    /// ��װ���е������޸ķ������ṩ�ⲿ����
    /// </summary>
    public void CurrentDataAdavance()
    {
        SetCurrentData(CurrentData + 1);
    }
}
