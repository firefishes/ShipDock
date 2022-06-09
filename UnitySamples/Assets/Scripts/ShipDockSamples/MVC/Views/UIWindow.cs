using ShipDock.Notices;
using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow : UIContainer
{
    protected override void Purge()
    {
        base.Purge();
    }

    protected override void Awake()
    {
        base.Awake();

        NodesControl.AddClickHandler("ClickMe", OnClickMeClickHandler);
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnClickMeClickHandler()
    {
        NodesControl.GetImage("Pic", out Image image);
        image.rectTransform.sizeDelta = new Vector2(1f, 1f);

        ParamNotice<Vector2> notice = new ParamNotice<Vector2>();
        notice.SetNoticeName(UIWindowModular.UIM_WINDOW_PIC_RESHOW);
        notice.ParamValue = new Vector2(800f, 800f);

        UpdateSubgroup("ImageScale", notice);
    }
}
