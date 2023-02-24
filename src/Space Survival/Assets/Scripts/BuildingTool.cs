using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTool : Weapon
{
    BuildingManager buildingManager;
    InterfaceManager interfaceManager;

    Buildable selectedBuildable;

    void Start()
    {
        buildingManager = BuildingManager.Instance;
        interfaceManager = InterfaceManager.Instance;
    }

    public override void Equip(Transform _parent)
    {
        base.Equip(_parent);
        buildingManager.SetTool(this);
    }

    public override void Holster()
    {
        base.Holster();
        buildingManager.SetTool(null);
    }

    public void SetBuildable(Buildable _buildable)
    {
        selectedBuildable = _buildable;
        selectedBuildable.StartBluePrint();
    }

    protected override void Attack()
    {
        base.Attack();
        if (selectedBuildable != null) {
            selectedBuildable.Build();
            selectedBuildable = null;
        }
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        if (selectedBuildable != null)
            buildingManager.CancelBuild();

        interfaceManager.ToggleBuildingInterface();

        //Single click
        SetSecondaryAttack(false);
    }

    protected override void Reload()
    {
        base.Reload();
        if (selectedBuildable != null)
            selectedBuildable.SetRotation(Quaternion.Euler(selectedBuildable.transform.rotation.eulerAngles + new Vector3(0f, 90f, 0f)));
    }
}
