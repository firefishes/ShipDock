using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 数据层
/// 
/// 根据控制层的调用更新数据，并根据数据的变化通知控制层
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
    /// 修改数据
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
    /// 封装现有的数据修改方法，提供外部调用
    /// </summary>
    public void CurrentDataAdavance()
    {
        SetCurrentData(CurrentData + 1);
    }
}
