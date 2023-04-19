using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour, IDataPersistance
{
    public delegate void BuildAction();
    public BuildAction OnBuild;

    /// <summary>
    /// The item info for this buildables
    /// </summary>
    public ItemScriptable ItemInfo { get { return itemInfo; } private set { itemInfo = value; } }

    [Tooltip("The item scriptable object for this buildable")]
    [SerializeField] ItemScriptable itemInfo;
    [Tooltip("The physical model game object. Make sure that the desired layer is set on this object")]
    [SerializeField] GameObject modelObject;

    [Header("Building Settings")]
    [Tooltip("The size of this structure")]
    [SerializeField] Vector3 structureSize;
    [Tooltip("The type of building this buildable is")]
    [SerializeField] BuildableType type;
    [Tooltip("The grid size this building will snap to when being placed")]
    [SerializeField] Vector3 gridSize;
    [Tooltip("The offset of the model's center in the x,z axis from the actual game object's center. This is used to detect overlapping")]
    [SerializeField] Vector3 centerOffset;

    BuildingManager buildingManager;
    Renderer buildingRenderer;
    Material deletionMaterial;

    //Smoothing trans anim
    Vector3 velocity;
    Vector3 targetPos;
    float smoothTime;

    Material[] tempMats;
    Collider[] colliders;

    void Start()
    {
        buildingManager = BuildingManager.Instance;
        buildingRenderer = GetComponentInChildren<Renderer>();

        deletionMaterial = buildingManager.GetDeletionMaterial();
        smoothTime = buildingManager.GetBuildingSmoothTime();

        tempMats = buildingRenderer.materials;
    }

    /// <summary>
    /// Returns the structure size of this buildable
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSize()
    {
        return structureSize;
    }

    /// <summary>
    /// Returns the set grid size of this buildable
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGridSize()
    {
        return gridSize;
    }

    /// <summary>
    /// Returns the buildable type of this buildable
    /// </summary>
    /// <returns>The buildable type of this buildable</returns>
    public BuildableType GetBuildableType()
    {
        return type;
    }

    /// <summary>
    /// The position that this buildable is set to move to ignoreing the transitioning position
    /// </summary>
    /// <returns>The vector3 position where this building is moving to</returns>
    public Vector3 GetTargetPos()
    {
        return targetPos;
    }

    /// <summary>
    /// Set the position for this buildable to smooth to
    /// </summary>
    /// <param name="_pos"></param>
    public void SetTargetPos(Vector3 _pos)
    {
        targetPos = _pos;
    }

    /// <summary>
    /// Returns the center offset of this building
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenterOffset()
    {
        return centerOffset;
    }

    /// <summary>
    /// Returns the model game objects of the buildable
    /// </summary>
    /// <returns>Game object of the buildable model</returns>
    public GameObject GetObject()
    {
        return modelObject;
    }

    /// <summary>
    /// Set the position for this buildable to move to
    /// </summary>
    /// <param name="_pos">The vector3 position to move to</param>
    public void SetPosition(Vector3 _pos)
    {
        _pos.x = Mathf.RoundToInt(_pos.x / gridSize.x) * gridSize.x;
        _pos.y = Mathf.RoundToInt(_pos.y / gridSize.y) * gridSize.y;
        _pos.z = Mathf.RoundToInt(_pos.z / gridSize.z) * gridSize.z;

        targetPos = _pos;

        //smooth interpolation
        transform.position = Vector3.SmoothDamp(transform.position, _pos, ref velocity, smoothTime);
    }

    /// <summary>
    /// Sets the rotation of this buildable
    /// </summary>
    /// <param name="_rot">The rotation to set to</param>
    public void SetRotation(Quaternion _rot)
    {
        transform.rotation = _rot;
    }

    /// <summary>
    /// Turn this buildable into a blueprint to be placed
    /// </summary>
    public void StartBlueprint()
    {
        colliders ??= GetComponentsInChildren<Collider>();

        foreach (Collider _col in colliders) {
            _col.enabled = false;
        }
    }

    /// <summary>
    /// Finalizes this buildable
    /// </summary>
    public void Build()
    {
        colliders ??= GetComponentsInChildren<Collider>();

        foreach (Collider _col in colliders) {
            _col.enabled = true;
        }

        OnBuild?.Invoke();
    }

    /// <summary>
    /// Highlight this object to indicate it being selected for deletion
    /// </summary>
    /// <param name="_state"></param>
    public void HighlightDelete(bool _state)
    {
        if (_state == true) {
            Material[] _mats = new Material[tempMats.Length];
            for (int i = 0; i < _mats.Length; i++) {
                _mats[i] = deletionMaterial;
            }
            //Highlight object
            buildingRenderer.materials = _mats;
        }
        else {
            buildingRenderer.materials = tempMats;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (itemInfo.unique)
            return;

        BuildableData _buildableData = new BuildableData(itemInfo.name, transform.position, transform.rotation);
        _data.buildables.Add(_buildableData);
    }

    public void LoadData(GameData _data)
    {
        
    }
}
