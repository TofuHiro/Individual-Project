using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ItemDisplayUI))]

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;   

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
    [Tooltip("The material to switch to when outlining an object to delete")]
    [SerializeField] Material deletionMaterial;
    [Tooltip("The transition time when a blueprint snaps to another snap point")]
    [SerializeField] float buildingSmoothTime = .05f;

    [Header("Catalog Setup")]
    [Tooltip("Set a catalog of all the buildable objects to be buildable and its required ingredients")]
    [SerializeField] List<BuildingRecipe> buildableCatalog;

    InterfaceManager interfaceManager;
    PlayerInventory playerInventory;
    BuildingGrid buildingGrid;
    PromptDisplay promptDisplay;

    BuildingTool equippedTool;
    BuildableSlot hoveredSlot;

    ItemDisplayUI itemDisplay;
    SlotUI[] ingredientSlots;

    bool useIngredients;

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;

        itemDisplay = GetComponent<ItemDisplayUI>();
        ingredientSlots = ingredientSlotsParent.GetComponentsInChildren<SlotUI>();
    }

    void Start()
    {
        interfaceManager = InterfaceManager.Instance;
        playerInventory = PlayerInventory.Instance;
        buildingGrid = BuildingGrid.Instance;
        promptDisplay = PromptDisplay.Instance;
    }

    void OnDisable()
    {
        PlayerController.OnUIClickStarted -= SelectBuildable;
        IsEnabled = false;
    }

    void OnDestroy()
    {
        PlayerController.OnUIClickStarted -= SelectBuildable;
        IsEnabled = false;
    }

    public void InitRecipes(bool _useIngredients)
    {
        useIngredients = _useIngredients;

        //Create a buildable recipe block for each buildable set in the catalog
        foreach (BuildingRecipe _item in buildableCatalog) {
            BuildableSlot _newSlot = Instantiate(buildableSlotPrefab, blueprintSlotsParents).GetComponent<BuildableSlot>();
            if (_useIngredients) {
                _newSlot.Init(_item.productItem, _item.ingredientItems);
            }
            else {
                _newSlot.Init(_item.productItem, new List<Item>());
            }
        }

        //Disable/hide all ingredient ui slot
        foreach (SlotUI _slot in ingredientSlots) {
            _slot.SetIcon(null);
            _slot.gameObject.SetActive(false);
        }

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
    /// Returns the set material that outlines objects for deletion
    /// </summary>
    /// <returns></returns>
    public Material GetDeletionMaterial()
    {
        return deletionMaterial;
    }

    /// <summary>
    /// Set the given building tool to become the active building tool
    /// </summary>
    /// <param name="_newTool">The building tool to set as active</param>
    public void SetTool(BuildingTool _newTool)
    {
        equippedTool = _newTool;

        if (equippedTool != null)
            promptDisplay.ShowOpenInterface();
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

        if (CheckIngriedients(hoveredSlot.BuildableRecipe.productItem)) {
            promptDisplay.ShowBuildPrompt();
            equippedTool.SetBlueprint(hoveredSlot.BuildableRecipe.productItem);
            interfaceManager.CloseBuilding();
        }
    }

    //UI Button
    /// <summary>
    /// Sets the current building tool to remove objects
    /// </summary>
    public void StartRemoveMode()
    {
        promptDisplay.ShowDeletePrompt();
        equippedTool.StartRemoveMode();
        interfaceManager.CloseBuilding();
    }

    public void CancelBuild()
    {
        promptDisplay.HideBuildPrompt();
    }

    /// <summary>
    /// Checks to see if the given buildable can be built based on the player's current items
    /// </summary>
    /// <param name="_buildable">The recipe of the buildable to check</param>
    /// <returns>Returns true if the player is able to craft this recipe</returns>
    public bool CheckIngriedients(Buildable _buildable)
    {
        if (!useIngredients) {
            return true;
        }

        List<Item> _ingredients = GetIngredients(_buildable);
        List<Item> _playerItems = playerInventory.GetItems();

        //Array to keep track of all the required items and if they are acquired
        bool[] _acquired = new bool[_ingredients.Count];

        //For each item, check if they are a valid item
        for (int i = 0; i < _playerItems.Count; i++) {
            for (int j = 0; j < _ingredients.Count; j++) {
                //If so, mark it as acquired
                if (_playerItems[i].ItemScriptableObject == _ingredients[j].ItemScriptableObject && !_acquired[j]) {
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
    /// Get the list of ingredients required to build the given buildable
    /// </summary>
    /// <param name="_buildable">The buildable's ingredients to get</param>
    /// <returns>Returns a list of item objects</returns>
    List<Item> GetIngredients(Buildable _buildable)
    {
        return buildableCatalog[GetBuildableRecipeIndex(_buildable)].ingredientItems;
    }

    /// <summary>
    /// Returns the index of the buildable in the recipe catalog
    /// </summary>
    /// <param name="_buildable"></param>
    /// <returns></returns>
    int GetBuildableRecipeIndex(Buildable _buildable)
    {
        for (int i = 0; i < buildableCatalog.Count; i++) {
            if (buildableCatalog[i].productItem.ItemInfo.name == _buildable.ItemInfo.name) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Displays the buildable and its info along with all the ingredients required in the UI
    /// </summary>
    /// <param name="_recipe">The recipe to display</param>
    void DisplayBuildable(BuildingRecipe _recipe)
    {
        if (_recipe != null) {
            itemDisplay.SetItem(_recipe.productItem.ItemInfo);
            //Display all ingredients in icon
            for (int i = 0; i < _recipe.ingredientItems.Count; i++) {
                ingredientSlots[i].gameObject.SetActive(true);
                ingredientSlots[i].SetIcon(_recipe.ingredientItems[i].ItemScriptableObject.icon);
            }
            //And hide all other icons
            for (int i = _recipe.ingredientItems.Count; i < ingredientSlots.Length; i++) {
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
    /// <param name="_buildable">The buildable that has been built</param>
    /// <returns>Returns true if the structure is not overlapping another in the grid</returns>
    public bool BuildObject(Buildable _buildable)
    {
        if (!buildingGrid.AddStructure(_buildable)) 
            return false;

        List<Item> _ingredients = GetIngredients(_buildable);

        if (useIngredients) {
            //Remove items
            foreach (Item _item in _ingredients) {
                playerInventory.RemoveItem(_item);
            }
        }

        return true;
    }

    public void LoadObject(Buildable _buildable)
    {
        buildingGrid ??= BuildingGrid.Instance;

        _buildable.SetTargetPos(_buildable.transform.position);
        buildingGrid.AddStructure(_buildable);
    }

    /// <summary>
    /// Removes the buidable from the building grid and refunds items to player
    /// </summary>
    /// <param name="_buildable">The buildable that has been removed</param>
    public void RemoveBuildable(Buildable _buildable)
    {
        buildingGrid.RemoveStructure(_buildable);

        if (useIngredients) {
            //Give items
            List<Item> _ingredients = GetIngredients(_buildable);

            foreach (Item _item in _ingredients) {
                GameObject _newObject = ObjectPooler.SpawnObject(_item.ItemScriptableObject.name, _item.gameObject);
                Item _newItem = _newObject.GetComponent<Item>();
                ObjectPooler.PoolObject(_newItem.ItemScriptableObject.name, _newObject);
                playerInventory.AddItem(_newItem);
            }
        }
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
