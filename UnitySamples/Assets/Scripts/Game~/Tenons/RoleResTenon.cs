using ShipDock;
using UnityEngine;

public class RoleResTenon : Tenon
{
    public RoleRes RoleRes { get; private set; }

    protected override void Purge()
    {
    }

    public void BindRes(GameObject target)
    {
        RoleRes = target.GetComponent<RoleRes>();
    }
}
