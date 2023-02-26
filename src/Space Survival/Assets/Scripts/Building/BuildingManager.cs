using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDisplayUI))]
public class BuildingManager : MonoBehaviour
{
    #region Singleton
    public static BuildingManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else 
            Instance = this;
    }
    #endregion

    [System.Serializable]
    public class ItemBuildablePair
    {
        public GameObject GameObject;
        public ItemScriptable ItemInfo;

        public Buildable GetBuildable()
        {
            return GameObject.GetComponent<Buildable>();
        }
        public void Clear()
        {
            GameObject = null;
            ItemInfo = null;
        }
    }

    public static bool IsEnabled { get; private set; }

    [SerializeField] float buildingSmoothTime = .05f;
    [SerializeField] GameObject UIGameObject;
    [SerializeField] Transform blueprintSlotsParents;
    [SerializeField] GameObject buildableSlotPrefab;
    [SerializeField] List<ItemBuildablePair> buildableCatalog;
    [SerializeField] LayerMask ignoreLayers;

    InterfaceManager interfaceManager;
    BuildingTool equippedTool;
    ItemDisplayUI itemDisplay;
    BuildableSlot hoveredSlot;

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
        itemDisplay = GetComponent<ItemDisplayUI>();

        foreach (ItemBuildablePair _item in buildableCatalog) {
            BuildableSlot _newSlot = Instantiate(buildableSlotPrefab, blueprintSlotsParents).GetComponent<BuildableSlot>();
            _newSlot.Init(_item.GameObject, _item.ItemInfo);
        }

        CloseInterface();
    }

    public float GetBuildingSmoothTime()
    {
        return buildingSmoothTime;
    }

    public LayerMask GetBuildingMasks()
    {
        return ignoreLayers;
    }

    public void SetTool(BuildingTool _newTool)
    {
        equippedTool = _newTool;
    }

    public void OpenInterface()
    {
        UIGameObject.SetActive(true);
        IsEnabled = true;
        PlayerController.OnUIClickStarted += SelectBuildable;
    }

    public void CloseInterface()
    {
        IsEnabled = false;
        PlayerController.OnUIClickStarted -= SelectBuildable;
        ResetInterface();
        UIGameObject.SetActive(false);
    }

    public void HoverBuildable(BuildableSlot _slot)
    {
        hoveredSlot = _slot;
        if (hoveredSlot != null)
            DisplayBuildable(hoveredSlot.ItemScriptable);
        else
            DisplayBuildable(null);
    }

    //On UI click
    void SelectBuildable()
    {
        if (hoveredSlot == null)
            return;

        equippedTool.SetBuildable(hoveredSlot);
        interfaceManager.CloseBuilding();
    }

    public void CancelBuild()
    {
        equippedTool.SetBuildable(null);
    }

    void DisplayBuildable(ItemScriptable _buildable)
    {
        itemDisplay.SetItem(_buildable);
    }

    void ResetInterface()
    {
        hoveredSlot = null;
        DisplayBuildable(null);
    }
}
