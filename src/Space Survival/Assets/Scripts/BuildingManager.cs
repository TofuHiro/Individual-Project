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
    class ItemBuildablePair
    {
        public Buildable buildable;
        public ItemScriptable itemInfo;
    }

    public static bool IsEnabled { get; private set; }

    [SerializeField] GameObject UIGameObject;
    [SerializeField] Transform bluePrintSlotsParents;
    [SerializeField] GameObject buildableSlotPrefab;
    [SerializeField] List<ItemBuildablePair> buildableCatalog;

    BuildingTool equippedTool;
    ItemDisplayUI itemDisplay;

    void Start()
    {
        itemDisplay = GetComponent<ItemDisplayUI>();

        foreach (ItemBuildablePair _item in buildableCatalog) {
            BuildableSlot _newSlot = Instantiate(buildableSlotPrefab, bluePrintSlotsParents).GetComponent<BuildableSlot>();
            _newSlot.Init(_item.buildable, _item.itemInfo);
        }

        CloseInterface();
    }

    public void SetTool(BuildingTool _newTool)
    {
        equippedTool = _newTool;
    }

    public void OpenInterface()
    {
        IsEnabled = true;
        UIGameObject.SetActive(true);
        ResetInterface();
    }

    public void CloseInterface()
    {
        IsEnabled = false;
        UIGameObject.SetActive(false);
    }

    public void DisplayBuildable(ItemScriptable _buildable)
    {
        itemDisplay.SetItem(_buildable);
    }

    public void SelectBuildable(Buildable _buildable)
    {
        equippedTool.SetBuildable(_buildable);
        CloseInterface();
    }

    public void CancelBuild()
    {
        equippedTool.SetBuildable(null);
        OpenInterface();
    }

    void ResetInterface()
    {
        itemDisplay.SetItem(null);
    }
}
