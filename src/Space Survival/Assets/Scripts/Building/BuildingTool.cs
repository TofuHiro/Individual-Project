using UnityEngine;

public class BuildingTool : Weapon
{
    [Tooltip("The position to play attack effects at when building/deleting")] 
    [SerializeField] Transform attackPoint;
    [Tooltip("Visual effects to play on build")]
    [SerializeField] string[] buildEffects;
    [Tooltip("Visual effects to play on building when built")]
    [SerializeField] string[] buildImpactEffects;
    [Tooltip("Visual effects to play on delete")]
    [SerializeField] string[] deleteEffects;
    [Tooltip("Visual effects to play on building when deleted")]
    [SerializeField] string[] deleteImpactEffects;
    [Tooltip("Sound effects to play on build")]
    [SerializeField] string[] buildSounds;
    [Tooltip("Sound effects to play on delete")]
    [SerializeField] string[] deleteSounds;

    BuildingManager buildingManager;
    InterfaceManager interfaceManager;

    Buildable currentBlueprint, hoveredBuild;
    
    //To switch to blueprint layer and back
    int tempLayer;
    bool isBlueprinting, isRemoving;
    LayerMask mask;

    protected override void Start()
    {
        base.Start();
        buildingManager = BuildingManager.Instance;
        interfaceManager = InterfaceManager.Instance;
        semiAutomatic = true;
        mask = ~0;//All
    }

    public override void Equip(Transform _parent)
    {
        base.Equip(_parent);
        buildingManager ??= BuildingManager.Instance;
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
        currentBlueprint.GetObject().layer = LayerMask.NameToLayer("Ignore Raycast");
        currentBlueprint.StartBlueprint();
        isBlueprinting = true;
    }

    protected override void Update()
    {
        base.Update();
        if (isBlueprinting) {
            PositionBlueprint();
        }
        else if (isRemoving) {
            GetDeleteTarget();
        }
    }

    /// <summary>
    /// Positions the blueprint
    /// </summary>
    void PositionBlueprint()
    {
        //Check for a surface within range
        RaycastHit _hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range);
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
    void GetDeleteTarget()
    {
        //Check for a surface within range
        RaycastHit _hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range + 5f, mask, QueryTriggerInteraction.Ignore);

        //If look at another object
        if (hoveredBuild != null) {
            if (_hit.transform != hoveredBuild.transform) {
                //Restore mats to other object if was looking at one
                hoveredBuild.HighlightDelete(false);
                hoveredBuild = null;
                return;
            }
        }

        //If look at nothing
        if (_hit.transform == null) { 
            return;
        }
        //Ignore if looking at same object
        if (hoveredBuild != null)
            if (_hit.transform == hoveredBuild.transform)
                return;

        //If look at something that isnt another buildable
        hoveredBuild = _hit.transform.GetComponent<Buildable>();
        if (hoveredBuild == null) {
            return;
        }

        hoveredBuild.HighlightDelete(true);
    }

    protected override void Attack()
    {
        base.Attack();

        if (isBlueprinting) {
            BuildBlueprint();
        }
        else if (isRemoving) {
            Remove();
        }
        else {
            return;
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

        //Sound
        foreach (string _audio in buildSounds) {
            audioManager.PlayClip(_audio, transform.position);
        }
        //Weapon attack effects
        foreach (string _effect in buildEffects) {
            effectsManager.PlayEffect(_effect, attackPoint.position, attackPoint.rotation);
        }
        //On buildable effects
        foreach (string _effect in buildImpactEffects) {
            effectsManager.PlayEffect(_effect, currentBlueprint.GetTargetPos(), currentBlueprint.transform.rotation);
        }

        //Create new instance
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
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, range + 5f, mask, QueryTriggerInteraction.Ignore);
        if (_hit.transform != null) {
            Storage _storage = _hit.transform.GetComponent<Storage>();
            Buildable _buildable = _hit.transform.GetComponentInParent<Buildable>();

            //Check if removing a non empty storage
            if (_storage != null && _buildable != null) {
                if (!_storage.IsEmpty) {
                    //notify?
                    return;
                }
            } 

            if (_buildable != null) {
                //Sound
                foreach (string _audio in deleteSounds) {
                    audioManager.PlayClip(_audio, transform.position);
                }
                //shoot effects
                foreach (string _effect in deleteEffects) {
                    effectsManager.PlayEffect(_effect, attackPoint.position, attackPoint.rotation);
                }
                //impact effects
                foreach (string _effect in deleteImpactEffects) {
                    effectsManager.PlayEffect(_effect, _hit.point, _buildable.transform.rotation);
                }

                hoveredBuild.HighlightDelete(false);
                hoveredBuild = null;

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
        //Rotate 90 degrees
        currentBlueprint.SetRotation(Quaternion.Euler(currentBlueprint.transform.rotation.eulerAngles + new Vector3(0f, 90, 0f)));
    }

    /// <summary>
    /// Stops building the current blueprint
    /// </summary>
    void CancelBuild()
    {
        buildingManager ??= BuildingManager.Instance;
            
        if (currentBlueprint != null) {
            currentBlueprint.GetObject().layer = tempLayer;
            ObjectPooler.PoolObject(currentBlueprint.ItemInfo.name + "_blueprint", currentBlueprint.gameObject);
            currentBlueprint = null;
        }

        if (hoveredBuild != null) {
            hoveredBuild.HighlightDelete(false);
            hoveredBuild = null;
        } 

        tempLayer = 0;
        isBlueprinting = false;
        isRemoving = false;
        buildingManager.CancelBuild();
    }

    /// <summary>
    /// Set the tool to remove objects
    /// </summary>
    public void StartRemoveMode()
    {
        isRemoving = true;
    }
}
