using UnityEngine;

namespace ShipDock.UI
{
    public interface IUIRoot
    {
        Canvas MainCanvas { get; }
        Camera UICamera { get; }
        RectTransform Widgets { get; }
        RectTransform Popups { get; }
        RectTransform Windows { get; }
        float MatchWidthOrHeight { get; }
        float ScaleRatio { get; }
        float FOVRatio { get; }
        int ScreenW { get; }
        int ScreenH { get; }
    }
}
