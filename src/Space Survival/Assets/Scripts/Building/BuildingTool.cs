using UnityEngine;

public class BuildingTool : Weapon
{
    BuildingManager buildingManager;
    InterfaceManager interfaceManager;

    //current building ref
    BuildingManager.BuildableRecipe currentBuildable;
    Buildable currentBlueprint;

    LayerMask layerMask;
    //To switch to blueprint layer and back
    int tempLayer;

    void Start()
    {
        buildingManager = BuildingManager.Instance;
        interfaceManager = InterfaceManager.Instance;
        semiAutomatic = true;

        currentBuildable = new BuildingManager.BuildableRecipe();
        layerMask = buildingManager.GetBuildingMasks();
    }

    public override void Equip(Transform _parent)
    {
        base.Equip(_parent);
        buildingManager.SetTool(this);
    }

    public override void Holster()
    {
        base.Holster();
        CancelBuild();
        buildingManager.SetTool(null);
    }

    /// <summary>
    /// Sets and displays a blueprint of the given buildable object
    /// </summary>
    /// <param name="_buildable">The buildable to set the blueprint to build to</param>
    public void SetBlueprint(BuildableSlot _buildable)
    {
        if (_buildable == null) {
            CancelBuild();
            return;
        }

        //Set current buildable ref
        currentBuildable.GameObject = ObjectPooler.SpawnObject(_buildable.BuildableRecipe.ItemInfo.name + "_blueprint", _buildable.BuildableRecipe.GameObject);
        currentBuildable.ItemInfo = _buildable.BuildableRecipe.ItemInfo;
        currentBuildable.Ingredients = _buildable.BuildableRecipe.Ingredients;
        currentBlueprint = currentBuildable.GetBuildable();

        //Save layer and switch to blueprint
        tempLayer = currentBlueprint.GetObject().layer;
        currentBlueprint.GetObject().layer = LayerMask.NameToLayer("Blueprint");
        currentBlueprint.StartBlueprint();
    }

    protected override void Update()
    {
        base.Update();
        //blueprint set
        if (currentBuildable.GameObject != null) {
            PositionBlueprint();
        }
    }

    /// <summary>
    /// Positions the blueprint
    /// </summary>
    void PositionBlueprint()
    {
        //Check for a surface within range
        RaycastHit _hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range, layerMask);
        if (_hit.transform != null) {
            currentBlueprint.SetPosition(_hit.point);
        }
        else {
            currentBlueprint.SetPosition(transform.position + (transform.forward * range));
        }
    }

    protected override void Attack()
    {
        if (currentBuildable.GameObject == null)
            return;

        //Finish transition for real pos
        currentBlueprint.SetPosition(currentBlueprint.GetTargetPos());

        //If overlapping
        if (!buildingManager.BuildObject(currentBuildable))
            return;

        base.Attack();

        //Create instance
        Buildable _newBuildable = ObjectPooler.SpawnObject(
            currentBuildable.ItemInfo.name,
            currentBuildable.GameObject,
            currentBlueprint.GetTargetPos(),
            currentBlueprint.transform.rotation).GetComponent<Buildable>();
        _newBuildable.Build();
        //Reset layer
        _newBuildable.GetObject().layer = tempLayer;

        //Check with remaining items if can continue building
        if (!buildingManager.CheckIngriedients(currentBuildable)) {
            SetBlueprint(null);
        }
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        //If is building
        if (currentBuildable.GameObject != null) {
            CancelBuild();
        }
        else {
            if (!BuildingManager.IsEnabled)
                interfaceManager.OpenBuilding();
            else
                interfaceManager.CloseBuilding();
        }
    }

    protected override void Reload()
    {
        if (currentBuildable.GameObject == null)
            return;

        base.Reload();
        //Rotate
        currentBlueprint.SetRotation(Quaternion.Euler(currentBlueprint.transform.rotation.eulerAngles + new Vector3(0f, 90, 0f)));
    }

    /// <summary>
    /// Stops building the current blueprint
    /// </summary>
    void CancelBuild()
    {
        if (currentBuildable.GameObject != null) {
            currentBlueprint.GetObject().layer = tempLayer;
            ObjectPooler.PoolObject(currentBuildable.ItemInfo.name + "_blueprint", currentBuildable.GameObject);
            currentBuildable.Clear();
            currentBlueprint = null;
        }
        tempLayer = 0;
    }
}
