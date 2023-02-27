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
    public class BuildableRecipe
    {
        public GameObject GameObject;
        public ItemScriptable ItemInfo;
        public List<ItemScriptable> Ingredients;

        public Buildable GetBuildable()
        {
            return GameObject.GetComponent<Buildable>();
        }
        public void Clear()
        {
            GameObject = null;
            ItemInfo = null;
            Ingredients = null;
        }
    }

    public static bool IsEnabled { get; private set; }

    [Tooltip("The transition time when a blueprint snaps to another snap point")]
    [SerializeField] float buildingSmoothTime = .05f;
    [Tooltip("The game object holding the building UI")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("The parent transform holding all blueprints")]
    [SerializeField] Transform blueprintSlotsParents;
    [Tooltip("The prefab for a building slot")]
    [SerializeField] GameObject buildableSlotPrefab;
    [Tooltip("Set a catalog of all the buildable objects to be buildable and its required ingredients")]
    [SerializeField] List<BuildableRecipe> buildableCatalog;
    [Tooltip("Layer mask to ignore collision when detecting a surface for blueprints")]
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] Transform ingredientSlotsParent;

    InterfaceManager interfaceManager;
    PlayerInventory playerInventory;
    BuildingTool equippedTool;
    BuildableSlot hoveredSlot;
    ItemDisplayUI itemDisplay;
    SlotUI[] ingredientSlots;

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
        playerInventory = PlayerInventory.Instance;
        itemDisplay = GetComponent<ItemDisplayUI>();
        ingredientSlots = ingredientSlotsParent.GetComponentsInChildren<SlotUI>();

        foreach (BuildableRecipe _item in buildableCatalog) {
            BuildableSlot _newSlot = Instantiate(buildableSlotPrefab, blueprintSlotsParents).GetComponent<BuildableSlot>();
            _newSlot.Init(_item.GameObject, _item.ItemInfo, _item.Ingredients);
        }

        foreach (SlotUI _slot in ingredientSlots) {
            _slot.SetIcon(null);
            _slot.gameObject.SetActive(false);
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
            DisplayBuildable(hoveredSlot.BuildableRecipe);
        else
            DisplayBuildable(null);
    }

    //On UI click
    void SelectBuildable()
    {
        if (hoveredSlot == null)
            return;

        if (CheckIngriedients(hoveredSlot.BuildableRecipe)) {
            equippedTool.SetBuildable(hoveredSlot);
            interfaceManager.CloseBuilding();
        }
    }

    bool CheckIngriedients(BuildableRecipe _buildable)
    {
        List<ItemScriptable> _items = playerInventory.GetItems();
        bool[] _acquired = new bool[_buildable.Ingredients.Count];

        for (int i = 0; i < _items.Count; i++) {
            for (int j = 0; j < _buildable.Ingredients.Count; j++) {
                if (_items[i] == _buildable.Ingredients[j] && !_acquired[j]) {
                    _acquired[j] = true;
                    break;
                }
            }
        }

        foreach (bool _itemAcquired in _acquired) {
            if (!_itemAcquired) {
                return false;
            }
        }

        return true;
    }

    void DisplayBuildable(BuildableRecipe _buildable)
    {
        if (_buildable != null) {
            itemDisplay.SetItem(_buildable.ItemInfo);
            for (int i = 0; i < _buildable.Ingredients.Count; i++) {
                ingredientSlots[i].gameObject.SetActive(true);
                ingredientSlots[i].SetIcon(_buildable.Ingredients[i].icon);
            }
            for (int i = _buildable.Ingredients.Count; i < ingredientSlots.Length; i++) {
                ingredientSlots[i].SetIcon(null);
                ingredientSlots[i].gameObject.SetActive(false);
            }
        }
        else {
            itemDisplay.SetItem(null);
            foreach (SlotUI _slot in ingredientSlots) {
                _slot.SetIcon(null);
                _slot.gameObject.SetActive(false);
            }
        }
    }

    public void BuildObject(BuildableRecipe _buildable)
    {
        foreach (ItemScriptable _item in _buildable.Ingredients) {
            playerInventory.RemoveItem(_item);
        }

        if (!CheckIngriedients(_buildable)) 
            equippedTool.SetBuildable(null);
    }

    void ResetInterface()
    {
        hoveredSlot = null;
        DisplayBuildable(null);
    }
}
