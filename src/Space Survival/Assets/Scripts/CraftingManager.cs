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

    [System.Serializable] 
    class CraftingTypeSet
    {
        public CraftingStationType StationType;
        public List<ItemRecipe> Recipes;
    }

    public bool IsEnabled { get; private set; }

    public delegate void CraftingActions();
    public static event CraftingActions OnCraftingOpen, OnCraftingClose;

    [SerializeField] GameObject interfaceGameObject;
    [SerializeField] Transform standardRecipeScrollBox, weaponRecipeScrollBox;
    [SerializeField] CraftingRecipeBlock recipeBlockPrefab;
    [SerializeField] List<CraftingTypeSet> recipeSets;

    PlayerInventory playerInventory;
    ObjectPooler objectPooler;
    List<CraftingRecipeBlock> recipeBlocks;
    ItemScriptable[] playerItems;

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        objectPooler = ObjectPooler.Instance;
        recipeBlocks = new List<CraftingRecipeBlock>();
        InitRecipeBlocks();
        CloseInterface();
    }

    void OnEnable()
    {
        PlayerController.OnInventoryToggle += CloseInterface;
        PlayerInventory.OnItemChange += CheckPlayerItems;
    }

    void OnDisable()
    {
        PlayerController.OnInventoryToggle -= CloseInterface;
        PlayerInventory.OnItemChange -= CheckPlayerItems;
    }

    void InitRecipeBlocks()
    {
        //Create all recipe blocks at corresponding recipe scroll box
        foreach (CraftingTypeSet _set in recipeSets) {
            Transform _scrollbox = GetRecipeScrollBox(_set.StationType);

            foreach (ItemRecipe _recipe in _set.Recipes) {
                CraftingRecipeBlock _recipeBlock = Instantiate(recipeBlockPrefab, _scrollbox).GetComponent<CraftingRecipeBlock>();
                recipeBlocks.Add(_recipeBlock);
                _recipeBlock.Init(_recipe);
            }
        }
    }

    public void AddRecipeToSet(CraftingStationType _type, ItemRecipe _recipe)
    {
        Transform _scrollbox = GetRecipeScrollBox(_type);
        CraftingRecipeBlock _recipeBlock = Instantiate(recipeBlockPrefab, _scrollbox).GetComponent<CraftingRecipeBlock>();
        recipeBlocks.Add(_recipeBlock);
        _recipeBlock.Init(_recipe);
    }

    public void ToggleCraftingInterface(CraftingStationType _type)
    {
        OpenInterface();
        GetRecipeScrollBox(_type).gameObject.SetActive(true);
    }
    
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

    void OpenInterface()
    {
        interfaceGameObject.SetActive(true);
        IsEnabled = true;
        CheckPlayerItems();
        OnCraftingOpen?.Invoke();
    }
    
    void CloseInterface()
    {
        standardRecipeScrollBox.gameObject.SetActive(false);
        weaponRecipeScrollBox.gameObject.SetActive(false);

        interfaceGameObject.SetActive(false);
        IsEnabled = false;
        OnCraftingClose?.Invoke();
    }

    void CheckPlayerItems()
    {
        if (!IsEnabled)
            return;

        //Get all player items
        List<ItemScriptable> _items = playerInventory.GetItems();
        playerItems = new ItemScriptable[_items.Count];
        for (int i = 0; i < _items.Count; i++) {
            playerItems[i] = _items[i];
        }

        foreach (CraftingRecipeBlock _recipeBlock in recipeBlocks) {
            _recipeBlock.ResetBlock();
        }

        foreach (ItemScriptable _item in playerItems) {
            foreach (CraftingRecipeBlock _recipeBlock in recipeBlocks) {
                _recipeBlock.CheckIngredients(_item);
            }
        }
    }

    public void CraftRecipe(ItemRecipe _recipe)
    {
        foreach (ItemScriptable _item in _recipe.ingredientItems) {
            playerInventory.RemoveItem(_item);
        }

        GameObject _newObject = objectPooler.GetObject(_recipe.productItem.name, _recipe.productGameObject);
        Item _newItem = _newObject.GetComponent<Item>();
        objectPooler.PoolObject(_newItem.ItemScriptableObject.name, _newObject);
        playerInventory.AddItem(_newItem);

        CheckPlayerItems();
    }
}
