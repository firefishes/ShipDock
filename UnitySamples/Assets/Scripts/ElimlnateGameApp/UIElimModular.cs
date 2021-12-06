using Elimlnate;
using ShipDock.UI;

public class UIElimModular : UIEliminatePlayModular
{
    public override string ABName { get; } = ElimConsts.AB_ELIM_UI;
    public override string UIAssetName { get; protected set; } = ElimConsts.ASSET_ELIM_UI;
    public override string Name { get; protected set; } = ElimConsts.UIM_ELIM_UI;
    public override int UILayer { get; protected set; } = UILayerType.WINDOW;
}
