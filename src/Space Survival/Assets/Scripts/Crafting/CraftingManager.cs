using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    #region Singleton
    public static CraftingManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        else {
            Instance = this;
        }
    }
    #endregion

    //Key value for sets of recipes for a station type
    [System.Serializable] 
    class CraftingTypeSet
    {
        public CraftingStationType StationType;
        public List<ItemRecipe> Recipes;
    }

    public bool IsEnabled { get; private set; }

    [Tooltip("Game object holding the crafting user interface")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("Scroll box holding standard recipe blocks")]
    [SerializeField] Transform standardRecipeScrollBox;
    [Tooltip("Scroll box holding weapon recipe blocks")]
    [SerializeField] Transform weaponRecipeScrollBox;
    [Tooltip("Prefab for a UI recipe block that will be instantiated to display a recipe")]
    [SerializeField] CraftingRecipeBlock recipeBlockPrefab;
    [Tooltip("Sets of recipes for a given station type. Make sure there is only one of each type")]
    [SerializeField] List<CraftingTypeSet> recipeSets;

    PlayerInventory playerInventory;

    List<CraftingRecipeBlock> recipeBlocks;
    Item[] playerItems;

    void Start()
    {
        //Cache ref
        playerInventory = PlayerInventory.Instance;
        
        recipeBlocks = new List<CraftingRecipeBlock>();
        
        //Init all set recipe blocks to corresponding scroll boxes
        InitRecipeBlocks();

        //After init, disable UI
        CloseInterface();
    }

    void OnEnable()
    {
        PlayerInventory.OnItemChange += CheckPlayerItems;
    }

    void OnDisable()
    {
        PlayerInventory.OnItemChange -= CheckPlayerItems;
    }

    /// <summary>
    /// Initializes all set recipe blocks to corresponding scroll boxes
    /// </summary>
    void InitRecipeBlocks()
    {
        foreach (CraftingTypeSet _set in recipeSets) {
            //Get correct parent to instantiate to
            Transform _scrollbox = GetRecipeScrollBox(_set.StationType);

            //Create all set recipe blocks for that type set
            foreach (ItemRecipe _recipe in _set.Recipes) {
                CraftingRecipeBlock _recipeBlock = Instantiate(recipeBlockPrefab, _scrollbox).GetComponent<CraftingRecipeBlock>();
                recipeBlocks.Add(_recipeBlock);
                //Set block to hold a recipe
                _recipeBlock.Init(_recipe);
            }
        }
    }

    /// <summary>
    /// Adds a recipe block to a station set
    /// </summary>
    /// <param name="_type">Recipe set to add to</param>
    /// <param name="_recipe">Item recipe to apply to block and add</param>
    public void AddRecipeToSet(CraftingStationType _type, ItemRecipe _recipe)
    {
        Transform _scrollbox = GetRecipeScrollBox(_type);
        CraftingRecipeBlock _recipeBlock = Instantiate(recipeBlockPrefab, _scrollbox).GetComponent<CraftingRecipeBlock>();
        recipeBlocks.Add(_recipeBlock);
        _recipeBlock.Init(_recipe);
    }

    /// <summary>
    /// Opens a crafting interface with recipe blocks
    /// </summary>
    /// <param name="_type">The type of set of recipes to displays</param>
    public void OpenCraftingInterface(CraftingStationType _type)
    {
        OpenInterface();
        GetRecipeScrollBox(_type).gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Return the corresponding parent holding all recipe blocks of a type
    /// </summary>
    /// <param name="_stationType">The recipe type to get the parent of</param>
    /// <returns>The parent transform for the recipe blocks of a type</returns>
    Transform GetRecipeScrollBox(CraftingStationType _stationType) 
    {
        switch (_stationType) {
            case CraftingStationType.Standard:
                return standardRecipeScrollBox;
            case CraftingStationType.Weapons:
                return weaponRecipeScrollBox;
            default:
                return null;
        }
    }

    /// <summary>
    /// Enables and opens interface
    /// </summary>
    void OpenInterface()
    {
        IsEnabled = true;
        UIGameObject.SetActive(true);
        CheckPlayerItems();
    }
    
    /// <summary>
    /// Disables and closes interface
    /// </summary>
    public void CloseInterface()
    {
        IsEnabled = false;
        UIGameObject.SetActive(false);
        
        //Hide all recipe scroll boxes
        standardRecipeScrollBox.gameObject.SetActive(false);
        weaponRecipeScrollBox.gameObject.SetActive(false);
    }

    /// <summary>
    /// Gets all player items and checks all recipes for possiblities to craft
    /// </summary>
    void CheckPlayerItems()
    {
        if (!IsEnabled)
            return;

        //Get all player items
        List<Item> _items = playerInventory.GetItems();
        playerItems = new Item[_items.Count];
        for (int i = 0; i < _items.Count; i++) {
            playerItems[i] = _items[i];
        }

        //Resets block from previous checks
        foreach (CraftingRecipeBlock _recipeBlock in recipeBlocks) {
            _recipeBlock.ResetBlock();
        }

        //Check off item for each recipe with all items
        foreach (Item _item in playerItems) {
            foreach (CraftingRecipeBlock _recipeBlock in recipeBlocks) {
                _recipeBlock.CheckIngredients(_item.ItemScriptableObject);
            }
        }
    }

    /// <summary>
    /// Gives the player the product item and removing the ingredients from the player inventory
    /// </summary>
    /// <param name="_recipe">Recipe to give and remove from</param>
    public void CraftRecipe(ItemRecipe _recipe)
    {
        //Remove all ingredient item from player inventory
        foreach (Item _item in _recipe.ingredientItems) {
            playerInventory.RemoveItem(_item);
        }

        //Create and give new item to player
        GameObject _newObject = ObjectPooler.SpawnObject(_recipe.productItem.ItemScriptableObject.name, _recipe.productItem.gameObject);
        Item _newItem = _newObject.GetComponent<Item>();
        ObjectPooler.PoolObject(_newItem.ItemScriptableObject.name, _newObject);
        playerInventory.AddItem(_newItem);

        //Reset recipe blocks
        CheckPlayerItems();
    }
}
