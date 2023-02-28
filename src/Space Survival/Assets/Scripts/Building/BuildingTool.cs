using UnityEngine;

public class BuildingTool : Weapon
{
    BuildingManager buildingManager;
    InterfaceManager interfaceManager;

    //current building ref
    BuildingManager.BuildableRecipe currentBuildable;
    Buildable currentBlueprint;

    BuildingSnapPoint currentSnapPoint;
    LayerMask layerMask;
    bool canBuild;
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
        tempLayer = currentBuildable.GameObject.layer;
        currentBuildable.GameObject.layer = LayerMask.NameToLayer("Blueprint");
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
    /// Positions the blueprint based on what the player is looking at and determines the buildable state
    /// </summary>
    void PositionBlueprint()
    {
        //Check for a surface within range
        RaycastHit _hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range, layerMask);
        //Surface detected
        if (_hit.transform != null) {
            //If surface is a snap point
            if (_hit.transform.CompareTag("SnapPoint") && currentBlueprint.UseSnapPoints) {
                //Cache
                BuildingSnapPoint _snapPoint = _hit.transform.GetComponent<BuildingSnapPoint>();
                BuildableType _currentBuildableType = currentBlueprint.GetBuildableType();

                //Check if snap point has a set point for the current buildable type
                if (_snapPoint.CheckForType(_currentBuildableType)) {
                    Vector3 _pos = _snapPoint.GetSnapPosition(_currentBuildableType);
                    currentBlueprint.SetPosition(_pos, true);
                    currentBlueprint.SetRotation(_snapPoint.GetSnapRotation(_currentBuildableType));
                    currentSnapPoint = _snapPoint;
                    canBuild = true;
                }
                //No snap point, snap to grid with surface as normal
                else {
                    currentBlueprint.SetPosition(_hit.point, true);
                    currentSnapPoint = null;
                    canBuild = currentBlueprint.CanPlaceWithoutSnaps;
                }
            }
            else {
                //Snap to grid with surface
                currentBlueprint.SetPosition(_hit.point, true);
                currentSnapPoint = null;
                canBuild = currentBlueprint.CanPlaceWithoutSnaps;
            }
        }
        //No surface
        else {
            //Follow mouse, dont snap
            currentBlueprint.SetPosition(transform.position + (transform.forward * (range - .5f)), false);
            canBuild = false;
        }
    }

    protected override void Attack()
    {
        if (currentBuildable.GameObject == null)
            return;
        if (!canBuild)
            return;
        //If snap point already occupied
        if (currentSnapPoint != null && currentSnapPoint.GetPointOccupied(currentBlueprint.GetBuildableType()) == true)
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
        _newBuildable.gameObject.layer = tempLayer;
        
        buildingManager.BuildObject(currentBuildable);
        currentSnapPoint.SetPointOccupied(currentBlueprint.GetBuildableType(), true);
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
        base.Reload();
        //Rotate
        if (currentBuildable.GameObject != null)
            currentBlueprint.SetRotation(Quaternion.Euler(currentBlueprint.transform.rotation.eulerAngles + new Vector3(0f, 90f, 0f)));
    }

    /// <summary>
    /// Stops building the current blueprint
    /// </summary>
    void CancelBuild()
    {
        if (currentBuildable.GameObject != null) {
            currentBuildable.GameObject.layer = tempLayer;
            ObjectPooler.PoolObject(currentBuildable.ItemInfo.name + "_blueprint", currentBuildable.GameObject);
            currentBuildable.Clear();
            currentBlueprint = null;
        }
        tempLayer = 0;
    }
}
