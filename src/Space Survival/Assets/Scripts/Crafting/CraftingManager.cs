using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDisplayUI))]
public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;
    
    //Key value for sets of recipes for a station type
    [System.Serializable] 
    class CraftingTypeSet
    {
        public CraftingStationType StationType;
        public List<ItemRecipe> Recipes;
    }

    public static bool IsEnabled { get; private set; }

    [Tooltip("Game object holding the crafting user interface")]
    [SerializeField] GameObject UIGameObject;
    [Tooltip("Root gameobject for armory scroll")]
    [SerializeField] GameObject armoryScrollBase;
    [Tooltip("Parent holding armory recipe blocks")]
    [SerializeField] RectTransform armoryBlocksParent;
    [Tooltip("Root gameobject for armory scroll")]
    [SerializeField] GameObject manufacturerScrollBase;
    [Tooltip("Parent holding manufacturer recipe blocks")]
    [SerializeField] RectTransform manufacturerBlocksParent;
    [Tooltip("Root gameobject for armory scroll")]
    [SerializeField] GameObject smelteryScrollBase;
    [Tooltip("Parent holding smeltery recipe blocks")]
    [SerializeField] RectTransform smelteryBlocksParent;
    [Tooltip("Root gameobject for armory scroll")]
    [SerializeField] GameObject cookingScrollBase;
    [Tooltip("Parent holding cooking recipe blocks")]
    [SerializeField] RectTransform cookingBlocksParent;
    [Tooltip("Root gameobject for armory scroll")]
    [SerializeField] GameObject starterScrollBase;
    [Tooltip("Parent holding starter recipe blocks")]
    [SerializeField] RectTransform starterBlocksParent;
    [Tooltip("Prefab for a UI recipe block that will be instantiated to display a recipe")]
    [SerializeField] CraftingRecipeBlock recipeBlockPrefab;
    [Tooltip("Sets of recipes for a given station type. Make sure there is only one of each type")]
    [SerializeField] List<CraftingTypeSet> recipeSets;
    [Tooltip("Sound effects to play on craft")]
    [SerializeField] string[] craftSounds;

    AudioManager audioManager;
    PlayerInventory playerInventory;
    ItemDisplayUI itemDisplay;

    bool useIngredients;

    void OnDisable()
    {
        IsEnabled = false;
    }

    void OnDestroy()
    {
        IsEnabled = false;
    }

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;

        itemDisplay = GetComponent<ItemDisplayUI>();
    }

    void Start()
    {
        //Cache ref
        audioManager = AudioManager.Instance;
        playerInventory = PlayerInventory.Instance;
    }

    /// <summary>
    /// /// <summary>
    /// Initializes all set recipe blocks to corresponding scroll boxes
    /// </summary>
    /// </summary>
    /// <param name="_useIngredients">Display ingredients for each recipe</param>
    public void InitRecipes(bool _useIngredients)
    {
        useIngredients = _useIngredients;

        foreach (CraftingTypeSet _set in recipeSets) {
            //Create all set recipe blocks for that type set
            foreach (ItemRecipe _recipe in _set.Recipes) {
                AddRecipe(_set.StationType, _recipe, _useIngredients);
            }
        }

        //After init, disable UI
        CloseInterface();
    }

    /// <summary>
    /// Adds a recipe block to a crafting set
    /// </summary>
    /// <param name="_stationType">The station type to add to</param>
    /// <param name="_recipe">The recipe to add</param>
    /// <param name="_useIngredients">Display ingredients or not</param>
    void AddRecipe(CraftingStationType _stationType, ItemRecipe _recipe, bool _useIngredients)
    {
        CraftingRecipeBlock _newRecipe = Instantiate(recipeBlockPrefab, GetRecipeParent(_stationType)).GetComponent<CraftingRecipeBlock>();
        _newRecipe.Init(_recipe, _useIngredients);
    }

    /// <summary>
    /// Opens a crafting interface with recipe blocks
    /// </summary>
    /// <param name="_type">The type of set of recipes to displays</param>
    public void OpenCraftingInterface(CraftingStationType _type)
    {
        OpenInterface();
        GetScrollBox(_type).gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Returns the corresponding parent holding all recipe blocks of a type
    /// </summary>
    /// <param name="_stationType">The recipe type to get the parent of</param>
    /// <returns>The parent transform for the recipe blocks of a type</returns>
    RectTransform GetRecipeParent(CraftingStationType _stationType) 
    {
        switch (_stationType) {
            case CraftingStationType.Armory:
                return armoryBlocksParent;
            case CraftingStationType.Manufacturer:
                return manufacturerBlocksParent;
            case CraftingStationType.Smelter:
                return smelteryBlocksParent;
            case CraftingStationType.Cooking:
                return cookingBlocksParent;
            default:
                return starterBlocksParent;
        }
    }

    /// <summary>
    /// Returns the corresponding scroll box for a crafting station
    /// </summary>
    /// <param name="_stationType"></param>
    /// <returns></returns>
    GameObject GetScrollBox(CraftingStationType _stationType)
    {
        switch (_stationType) {
            case CraftingStationType.Armory:
                return armoryScrollBase;
            case CraftingStationType.Manufacturer:
                return manufacturerScrollBase;
            case CraftingStationType.Smelter:
                return smelteryScrollBase;
            case CraftingStationType.Cooking:
                return cookingScrollBase;
            default:
                return starterScrollBase;
        }
    }

    /// <summary>
    /// Enables and opens interface
    /// </summary>
    void OpenInterface()
    {
        IsEnabled = true;
        UIGameObject.SetActive(true);
    }
    
    /// <summary>
    /// Disables and closes interface
    /// </summary>
    public void CloseInterface()
    {
        //Hide all recipe scroll boxes
        armoryScrollBase.SetActive(false);
        manufacturerScrollBase.SetActive(false);
        smelteryScrollBase.SetActive(false);
        cookingScrollBase.SetActive(false);
        starterScrollBase.SetActive(false);

        IsEnabled = false;
        UIGameObject.SetActive(false);
        itemDisplay.SetItem(null);
    }

    /// <summary>
    /// Checks if the player can craft a given recipe based on their inventory and the requirements for the recipe
    /// </summary>
    /// <param name="_recipe">The recipe to check if the player can craft</param>
    /// <returns>Returns true if the player is able to craft the recipe</returns>
    bool CheckRecipe(ItemRecipe _recipe)
    {
        if (!useIngredients) 
            return true;

        List<Item> _items = playerInventory.GetItems();
        bool[] _acquired = new bool[_recipe.ingredientItems.Length];

        for (int i = 0; i < _items.Count; i++) {
            for (int j = 0; j < _recipe.ingredientItems.Length; j++) {
                if (_items[i].ItemScriptableObject == _recipe.ingredientItems[j].ItemScriptableObject && !_acquired[j]) {
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

    /// <summary>
    /// Gives the player the product item and removing the ingredients from the player inventory
    /// </summary>
    /// <param name="_recipe">The recipe to craft</param>
    public void CraftRecipe(ItemRecipe _recipe)
    {
        if (!CheckRecipe(_recipe)) 
            return;

        //Sound
        foreach (string _audio in craftSounds) {
            audioManager.PlayClip(_audio, false);
        }

        if (useIngredients) {
            //Remove all ingredient item from player inventory
            foreach (Item _item in _recipe.ingredientItems) {
                playerInventory.RemoveItem(_item);
            }
        }

        //Create and give new item to player
        if (_recipe.productItem.ItemScriptableObject.unique) {
            GameObject _newObject = Instantiate(_recipe.productItem.gameObject);
            Item _newItem = _newObject.GetComponent<Item>();
            playerInventory.AddItem(_newItem);
            _newObject.SetActive(false);
        }
        else {
            GameObject _newObject = ObjectPooler.SpawnObject(_recipe.productItem.ItemScriptableObject.name, _recipe.productItem.gameObject);
            Item _newItem = _newObject.GetComponent<Item>();
            playerInventory.AddItem(_newItem);
            ObjectPooler.PoolObject(_newItem.ItemScriptableObject.name, _newObject);
        }
    }

    public void DisplayItem(ItemScriptable _item)
    {
        itemDisplay.SetItem(_item);
    }
}
