using ShipDock.Notices;
using ShipDock.Tools;
using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// 显示层
/// 
/// 根据控制层的调用进行响应，并根据输入调用控制层
/// 
/// </summary>
public class UIWindow : UIContainer
{
    private Vector2 mSizeWillShow;
    private Vector2 mPosWillMove;

    protected override void Purge()
    {
        base.Purge();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        NodesControl.ReferenceButton("ClickMe", out Button button);
        button.gameObject.SetActive(false);

        NodesControl.AddClickHandler("ClickMe", OnClickMeClickHandler);

        this.Dispatch(UIWindowModular.UIM_WINDOW_PIC_START_SHOW);
    }

    private void OnClickMeClickHandler()
    {
        NodesControl.ReferenceButton("ClickMe", out Button button);
        button.gameObject.SetActive(false);

        ParamNotice<Vector2> notice = new ParamNotice<Vector2>();
        notice.SetNoticeName(UIWindowModular.UIM_WINDOW_PIC_RESHOW);

        if (mSizeWillShow != Vector2.zero)
        {
            notice.ParamValue = mSizeWillShow;
            UpdateSubgroup("ImageScale", notice);
        }
        else { }

        if (mPosWillMove != Vector2.zero)
        {
            notice.ParamValue = mPosWillMove;
            UpdateSubgroup("ImagePosition", notice);
        }
        else { }

        mSizeWillShow = Vector2.zero;
        mPosWillMove = Vector2.zero;
    }

    public void ShowByData(int value)
    {
        NodesControl.ReferenceButton("ClickMe", out Button button);
        button.gameObject.SetActive(true);

        switch (value)
        {
            case 1:
                NodesControl.GetImage("Pic", out Image image);
                image.rectTransform.sizeDelta = new Vector2(1f, 1f);

                mSizeWillShow = new Vector2(800f, 800f);
                break;

            case 2:
                SetSizeWillShow();
                break;

            case 3:
                SetSizeWillShow();
                SetPosWIllMov();
                break;
        }
    }

    private void SetSizeWillShow()
    {
        float randX = Utils.UnityRangeRandom(100f, 1500f);
        float randY = Utils.UnityRangeRandom(100f, 1500f);
        mSizeWillShow = new Vector2(randX, randY);
    }

    private void SetPosWIllMov()
    {
        float randX = Utils.UnityRangeRandom(50f, 500f);
        float randY = Utils.UnityRangeRandom(50f, 500f);
        mPosWillMove = new Vector2(randX, randY);
    }
}
