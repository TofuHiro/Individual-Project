using UnityEngine;

public class BuildingTool : Weapon
{
    BuildingManager buildingManager;
    InterfaceManager interfaceManager;

    Buildable currentBlueprint;

    LayerMask layerMask;
    //To switch to blueprint layer and back
    int tempLayer;
    bool isBlueprinting, isRemoving;

    void Start()
    {
        buildingManager = BuildingManager.Instance;
        interfaceManager = InterfaceManager.Instance;
        semiAutomatic = true;

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
        buildingManager.CloseInterface();
    }

    /// <summary>
    /// Sets and displays a blueprint of the given buildable object
    /// </summary>
    /// <param name="_buildable">The buildable to set the blueprint to build to</param>
    public void SetBlueprint(Buildable _buildable)
    {
        if (_buildable == null) {
            CancelBuild();
            return;
        }

        currentBlueprint = ObjectPooler.SpawnObject(_buildable.ItemInfo.name + "_blueprint", _buildable.gameObject).GetComponent<Buildable>();

        //Save layer and switch to blueprint
        tempLayer = currentBlueprint.GetObject().layer;
        currentBlueprint.GetObject().layer = LayerMask.NameToLayer("Blueprint");
        currentBlueprint.StartBlueprint();
        isBlueprinting = true;
    }

    protected override void Update()
    {
        base.Update();
        //blueprint set
        if (isBlueprinting) {
            PositionBlueprint();
        }

        if (isRemoving) {
            GetTarget();
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

    /// <summary>
    /// Gets and highlights the buildable the player is looking at
    /// </summary>
    void GetTarget()
    {
        //Check for a surface within range
        RaycastHit _hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range, layerMask);
        if (_hit.transform != null) {
            if (_hit.transform.CompareTag("Buildable")) {
                //Highlight object
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();

        if (isBlueprinting) {
            BuildBlueprint();
        }

        if (isRemoving) {
            Remove();
        }
    }

    /// <summary>
    /// Finalizes the blueprint and creates a physical object of the buildable
    /// </summary>
    void BuildBlueprint()
    {
        //Finish transition for real pos
        currentBlueprint.SetPosition(currentBlueprint.GetTargetPos());

        //If overlapping
        if (!buildingManager.BuildObject(currentBlueprint))
            return;

        //Create instance
        Buildable _newBuildable = ObjectPooler.SpawnObject(
            currentBlueprint.ItemInfo.name,
            currentBlueprint.gameObject,
            currentBlueprint.GetTargetPos(),
            currentBlueprint.transform.rotation).GetComponent<Buildable>();
        _newBuildable.Build();
        //Reset layer
        _newBuildable.GetObject().layer = tempLayer;

        //Check with remaining items if can continue building
        if (!buildingManager.CheckIngriedients(currentBlueprint)) {
            SetBlueprint(null);
        }
    }

    /// <summary>
    /// Removes the targeted buildable
    /// </summary>
    void Remove()
    {
        RaycastHit _hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range, layerMask);
        if (_hit.transform != null) {
            Buildable _buildable = _hit.transform.GetComponentInParent<Buildable>();
            if (_buildable != null) {
                ObjectPooler.PoolObject(_buildable.ItemInfo.name, _buildable.gameObject);
                buildingManager.RemoveBuildable(_buildable);
            }
        }
    }

    protected override void SecondaryAttack()
    {
        base.SecondaryAttack();
        //Building
        if (isBlueprinting) {
            CancelBuild();
            return;
        }

        //Removing
        if (isRemoving) {
            CancelBuild();
            return;
        }

        //Toggle UI
        if (!isBlueprinting && !isRemoving) {
            if (!BuildingManager.IsEnabled)
                interfaceManager.OpenBuilding();
            else
                interfaceManager.CloseBuilding();
        }
    }

    protected override void Reload()
    {
        if (currentBlueprint == null)
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
        if (currentBlueprint != null) {
            currentBlueprint.GetObject().layer = tempLayer;
            ObjectPooler.PoolObject(currentBlueprint.ItemInfo.name + "_blueprint", currentBlueprint.gameObject);
            currentBlueprint = null;
        }

        tempLayer = 0;
        isBlueprinting = false;
        isRemoving = false;
    }

    /// <summary>
    /// Set the tool to remove objects
    /// </summary>
    public void StartRemoveMode()
    {
        isRemoving = true;
    }
}
