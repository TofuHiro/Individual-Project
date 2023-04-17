using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public GameData(int _difficulty)
    {
        difficulty = _difficulty;

        buildables = new List<BuildableData>();
        spawners = new List<BuildableData>();
        items = new List<ItemData>();
        weapons = new List<WeaponData>();
        storages = new List<StorageData>();
    }

    //0 to 4
    public int difficulty;

    //Player stats
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public float playerHealth;
    public float playerShields;
    public float playerWater;
    public float playerFood;
    public float playerOxygen;

    //Player Inventory
    public InventoryData inventoryData;

    //Placed/dropped items and buildings
    public List<BuildableData> buildables;
    public List<BuildableData> spawners;
    public int activeSpawner;
    public List<ItemData> items;
    public List<WeaponData> weapons;

    //Storage Inventory
    public List<StorageData> storages;

    //Story
    public bool fuseAcquired;
    public bool cubeAcquired;
    public bool coreAcquired;
    public bool fusePlaced;
    public bool cubePlaced;
    public bool corePlaced;
    public bool buttonPressed;
    public bool storyComplete;

    //Misc
    public PetData petData;
}

[System.Serializable]
public struct BuildableData
{
    public string tag;
    public Vector3 position;
    public Quaternion rotation;

    public BuildableData(string _tag, Vector3 _position, Quaternion _rotation)
    {
        tag = _tag;
        position = _position;
        rotation = _rotation;
    }
}

[System.Serializable]
public class InventoryData
{
    public string[] inventItems;
    public WeaponData[] inventWeapons;
    public string[] shields;
    public string[] upgrades;
    public WeaponData[] slotWeapons;

    public InventoryData(string[] _items, WeaponData[] _inventWeapons, string[] _shields, string[] _upgrades, WeaponData[] _slotWeapons)
    {
        inventItems = _items;
        inventWeapons = _inventWeapons;
        shields = _shields;
        upgrades = _upgrades;
        slotWeapons = _slotWeapons;
    }
}

[System.Serializable]
public class ItemData
{
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
    public bool isActive;

    /// <summary>
    /// Constructor for poolable items that do not need an instance for initialization, e.g. weapon
    /// </summary>
    /// <param name="_tag"></param>
    public ItemData(string _tag)
    {
        tag = _tag;
    }

    /// <summary>
    /// Constructor for active item in the scene
    /// </summary>
    /// <param name="_tag"></param>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    /// <param name="_isActive"></param>
    public ItemData(string _tag, Vector3 _position, Quaternion _rotation, bool _isActive)
    {
        tag = _tag;
        position = _position;
        rotation = _rotation;
        isActive = _isActive;
    }
}

[System.Serializable]
public class WeaponData : ItemData
{
    public int ammoCount;
    public int clipCount;
    public int durability;

    public WeaponData(string _tag, Vector3 _position, Quaternion _rotation, int _ammo, int _clip, bool _isActive) : base(_tag, _position, _rotation, _isActive)
    {
        ammoCount = _ammo;
        clipCount = _clip;
    }

    public WeaponData(string _tag, Vector3 _position, Quaternion _rotation, int _durability, bool _isActive) : base(_tag, _position, _rotation, _isActive)
    {
        durability = _durability;
    }
}

[System.Serializable]
public class StorageData
{
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
    public string[] items;
    public WeaponData[] weapons;

    public StorageData(string _tag, Vector3 _position, Quaternion _rotation, string[] _items, WeaponData[] _weapons)
    {
        tag = _tag;
        position = _position;
        rotation = _rotation;
        items = _items;
        weapons = _weapons;
    }
}

[System.Serializable]
public class PetData
{
    public bool acquired;
    public string[] itemTags;
    public WeaponData[] weapons;

    public PetData(bool _acquired, string[] _itemTags, WeaponData[] _weapons)
    {
        acquired = _acquired;
        itemTags = _itemTags;
        weapons = _weapons;
    }
}