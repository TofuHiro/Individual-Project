using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCatalog : MonoBehaviour
{
    #region Singleton
    public static PrefabCatalog Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;
    }
    #endregion

    [System.Serializable]
    class StorageName
    {
        public string tag;
        public GameObject prefab;
    }

    [SerializeField] List<Buildable> buildablePrefabs;
    [SerializeField] List<Item> itemPrefabs;
    [SerializeField] List<StorageName> storagePrefabs;

    public GameObject GetBuildableObject(string _tag)
    {
        return buildablePrefabs.Find(x => x.ItemInfo.name == _tag).gameObject;
    }

    public GameObject GetItemObject(string _tag)
    {
        Debug.Log(_tag);
        return itemPrefabs.Find(x => x.ItemScriptableObject.name == _tag).gameObject;
    }

    public Item GetItem(string _tag)
    {
        return itemPrefabs.Find(x => x.ItemScriptableObject.name == _tag);
    }

    public GameObject GetStorage(string _tag)
    {
        return storagePrefabs.Find(x => x.tag == _tag).prefab;
    }
}
