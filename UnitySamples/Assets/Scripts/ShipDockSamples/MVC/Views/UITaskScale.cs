using ShipDock;
using UnityEngine;
using UnityEngine.UI;

public class UITaskScale : UISubgroup
{
    public override string ChangerTaskName { get; protected set; } = "ImageScale";
    public override float ChangerTaskerDuring { get; protected set; } = 0.5f;

    [SerializeField]
    private string m_TaskName;

    private Vector2 mTarget;
    private UIWindow mParentNode;

    protected override void Start()
    {
        base.Start();

        mParentNode = m_UIOwner as UIWindow;
    }

    protected override void OnUIHandler(INoticeBase<int> param)
    {
        base.OnUIHandler(param);

        switch (param.Name)
        {
            case UIWindowModular.UIM_WINDOW_PIC_RESHOW:
                mTarget = (param as IParamNotice<Vector2>).ParamValue;
                break;
        }
    }

    protected override void TaskerChange(UI ui, TimeGapper timeGapper)
    {
        base.TaskerChange(ui, timeGapper);

        if (timeGapper.IsFinised)
        {
            ui.Dispatch(UIWindowModular.UIM_WINDOW_PIC_RESHOW_FINISH);
        }
        else
        {
            string name = "Pic";
            Image image = mParentNode.UINodes.GetSceneNode(ref name).image;
            Vector2 current = image.rectTransform.sizeDelta;
            image.rectTransform.sizeDelta = Vector2.Lerp(current, mTarget, timeGapper.Progress);
        }
    }

    private void Update()
    {
        m_TaskName = ChangerTaskName;
    }
}
