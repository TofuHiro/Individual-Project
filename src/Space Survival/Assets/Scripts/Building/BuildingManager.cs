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

        /// <summary>
        /// Returns the Buildable component of the attached game object
        /// </summary>
        /// <returns></returns>
        public Buildable GetBuildable()
        {
            return GameObject.GetComponent<Buildable>();
        }

        /// <summary>
        /// Clears/resets this class
        /// </summary>
        public void Clear()
        {
            GameObject = null;
            ItemInfo = null;
            Ingredients = null;
        }
    }

    public static bool IsEnabled { get; private set; }

    [Tooltip("The game object holding the building UI")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("The parent transform holding all blueprints")]
    [SerializeField] Transform blueprintSlotsParents;
    [Tooltip("The prefab for a building slot")]
    [SerializeField] GameObject buildableSlotPrefab;
    [Tooltip("The parents transform holding all the slots that displays the ingredients for a buildable")]
    [SerializeField] Transform ingredientSlotsParent;

    [Header("Building Settings")]
    [Tooltip("The transition time when a blueprint snaps to another snap point")]
    [SerializeField] float buildingSmoothTime = .05f;
    [Tooltip("Layer mask to ignore collision when detecting a surface for blueprints")]
    [SerializeField] LayerMask ignoreLayers;
    [Tooltip("The default size of each unit grid used for building")]
    [SerializeField] int gridSize = 4;

    [Header("Catalog Setup")]
    [Tooltip("Set a catalog of all the buildable objects to be buildable and its required ingredients")]
    [SerializeField] List<BuildableRecipe> buildableCatalog;

    InterfaceManager interfaceManager;
    PlayerInventory playerInventory;
    BuildingTool equippedTool;
    BuildableSlot hoveredSlot;
    ItemDisplayUI itemDisplay;
    SlotUI[] ingredientSlots;

    BuildingGrid buildingGrid;

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
        playerInventory = PlayerInventory.Instance;
        itemDisplay = GetComponent<ItemDisplayUI>();
        ingredientSlots = ingredientSlotsParent.GetComponentsInChildren<SlotUI>();

        //Create a buildable recipe block for each buildable set in the catalog
        foreach (BuildableRecipe _item in buildableCatalog) {
            BuildableSlot _newSlot = Instantiate(buildableSlotPrefab, blueprintSlotsParents).GetComponent<BuildableSlot>();
            _newSlot.Init(_item.GameObject, _item.ItemInfo, _item.Ingredients);
        }

        //Disable/hide all ingredient ui slot
        foreach (SlotUI _slot in ingredientSlots) {
            _slot.SetIcon(null);
            _slot.gameObject.SetActive(false);
        }

        buildingGrid = new BuildingGrid(gridSize);

        //Close after init
        CloseInterface();
    }

    /// <summary>
    /// Returns the preset building smoothing transition time
    /// </summary>
    /// <returns>The float time for the transition time</returns>
    public float GetBuildingSmoothTime()
    {
        return buildingSmoothTime;
    }

    /// <summary>
    /// Returns the preset building masks for when placing blueprints
    /// </summary>
    /// <returns>The layers to ignore when building</returns>
    public LayerMask GetBuildingMasks()
    {
        return ignoreLayers;
    }

    /// <summary>
    /// Set the given building tool to become the active building tool
    /// </summary>
    /// <param name="_newTool">The building tool to set as active</param>
    public void SetTool(BuildingTool _newTool)
    {
        equippedTool = _newTool;
    }

    /// <summary>
    /// Shows the building interface
    /// </summary>
    public void OpenInterface()
    {
        UIGameObject.SetActive(true);
        IsEnabled = true;
        PlayerController.OnUIClickStarted += SelectBuildable;
    }

    /// <summary>
    /// Hides the building interface
    /// </summary>
    public void CloseInterface()
    {
        IsEnabled = false;
        PlayerController.OnUIClickStarted -= SelectBuildable;
        ResetInterface();
        UIGameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the given buildable in the building interface with its info and ingredients
    /// </summary>
    /// <param name="_slot">The building slot containing the buildable and its information</param>
    public void HoverBuildable(BuildableSlot _slot)
    {
        hoveredSlot = _slot;
        if (hoveredSlot != null)
            DisplayBuildable(hoveredSlot.BuildableRecipe);
        else
            DisplayBuildable(null);
    }

    //On UI click
    /// <summary>
    /// Sets the currently hovered slot to become the active selected one
    /// </summary>
    void SelectBuildable()
    {
        if (hoveredSlot == null)
            return;

        if (CheckIngriedients(hoveredSlot.BuildableRecipe)) {
            equippedTool.SetBlueprint(hoveredSlot);
            interfaceManager.CloseBuilding();
        }
    }

    /// <summary>
    /// Checks to see if the given recipe can be built based on the player's current items
    /// </summary>
    /// <param name="_buildable">The recipe to check</param>
    /// <returns>Returns true if the player is able to craft this recipe</returns>
    public bool CheckIngriedients(BuildableRecipe _buildable)
    {
        List<ItemScriptable> _items = playerInventory.GetItems();
        //Array to keep track of all the required items and if they are acquired
        bool[] _acquired = new bool[_buildable.Ingredients.Count];

        //For each item, check if they are a valid item
        for (int i = 0; i < _items.Count; i++) {
            for (int j = 0; j < _buildable.Ingredients.Count; j++) {
                //If so, mark it as acquired
                if (_items[i] == _buildable.Ingredients[j] && !_acquired[j]) {
                    _acquired[j] = true;
                    break;
                }
            }
        }

        //Check if all items are acquired
        foreach (bool _itemAcquired in _acquired) {
            if (!_itemAcquired) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Displays the buildable and its info along with all the ingredients required in the UI
    /// </summary>
    /// <param name="_buildable">The slot with the buildable and its info</param>
    void DisplayBuildable(BuildableRecipe _buildable)
    {
        if (_buildable != null) {
            itemDisplay.SetItem(_buildable.ItemInfo);
            //Display all ingredients in icon
            for (int i = 0; i < _buildable.Ingredients.Count; i++) {
                ingredientSlots[i].gameObject.SetActive(true);
                ingredientSlots[i].SetIcon(_buildable.Ingredients[i].icon);
            }
            //And hide all other icons
            for (int i = _buildable.Ingredients.Count; i < ingredientSlots.Length; i++) {
                ingredientSlots[i].SetIcon(null);
                ingredientSlots[i].gameObject.SetActive(false);
            }
        }
        else {
            //Hide UI
            itemDisplay.SetItem(null);
            foreach (SlotUI _slot in ingredientSlots) {
                _slot.SetIcon(null);
                _slot.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Adds the buildable to the grid and removes the ingredient items from the player inventory
    /// </summary>
    /// <param name="_buildableRecipe">The recipe that has been built</param>
    /// <returns>Returns true if the structure is not overlapping another in the grid</returns>
    public bool BuildObject(BuildableRecipe _buildableRecipe)
    {
        if (!buildingGrid.AddStructure(_buildableRecipe.GetBuildable())) 
            return false;

        //Remove items
        foreach (ItemScriptable _item in _buildableRecipe.Ingredients) {
            playerInventory.RemoveItem(_item);
        }

        return true;
    }

    /// <summary>
    /// Clears the buildable display
    /// </summary>
    void ResetInterface()
    {
        hoveredSlot = null;
        DisplayBuildable(null);
    }
}
