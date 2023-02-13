using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    Dictionary<string, Queue<GameObject>> objectPools;

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }

        objectPools = new Dictionary<string, Queue<GameObject>>();
    }

    public GameObject SpawnObject(string _tag, GameObject _object)
    {
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }
        
        if (objectPools[_tag].Count > 0) {
            GameObject _newObject = objectPools[_tag].Dequeue();
            _newObject.SetActive(true);
            return _newObject;
        }
        else {
            GameObject _newObject = Instantiate(_object);
            return _newObject;
        }
    }

    public GameObject SpawnObject(string _tag, GameObject _object, Vector3 _position, Quaternion _rotation)
    {
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }

        if (objectPools[_tag].Count > 0) {
            GameObject _newObject = objectPools[_tag].Dequeue();
            _newObject.transform.SetPositionAndRotation(_position, _rotation);
            _newObject.SetActive(true);
            return _newObject;
        }
        else {
            GameObject _newObject = Instantiate(_object);
            _newObject.transform.SetPositionAndRotation(_position, _rotation);
            return _newObject;
        }
    }


    public void PoolObject(string _tag, GameObject _object)
    {
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }

        _object.SetActive(false);
        objectPools[_tag].Enqueue(_object);
    }
}
