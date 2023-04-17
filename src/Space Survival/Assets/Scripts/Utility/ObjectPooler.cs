using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    public static Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// Returns the requested game object from a new or existing pool
    /// </summary>
    /// <param name="_tag">The tag used to store the game object</param>
    /// <param name="_object">The game object to instantiate in case a pool of that item is not existing</param>
    /// <returns>The game object from a pool with a tag or new instantiated object</returns>
    public static GameObject SpawnObject(string _tag, GameObject _object)
    {
        //Create new pool if a pool for that item did not exist
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }

        GameObject _newObject;
        //If object in pool exist
        if (objectPools[_tag].Count > 0) {
            //Return it
            _newObject = objectPools[_tag].Dequeue();
            _newObject.SetActive(true);
        }
        //Otherwise create a new instance of it
        else {
            _newObject = Object.Instantiate(_object);
        }

        return _newObject;
    }

    /// <summary>
    /// Returns the requested game object from a new or existing pool
    /// </summary>
    /// <param name="_tag">The tag used to store the game object</param>
    /// <param name="_object">The game object to instantiate in case a pool of that item is not existing</param>
    /// <param name="_position">The position to spawn the object at</param>
    /// <param name="_rotation">The rotation to spawn the object with</param>
    /// <returns></returns>
    public static GameObject SpawnObject(string _tag, GameObject _object, Vector3 _position, Quaternion _rotation)
    {
        //Create new pool if a pool for that item did not exist
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }

        GameObject _newObject;
        //If object in pool exist
        if (objectPools[_tag].Count > 0) {
            _newObject = objectPools[_tag].Dequeue();
            _newObject.transform.SetPositionAndRotation(_position, _rotation);
            _newObject.SetActive(true);
        }
        //Otherwise create a new instance of it
        else {
            _newObject = Object.Instantiate(_object);
            _newObject.transform.SetPositionAndRotation(_position, _rotation);
        }

        return _newObject;
    }

    /// <summary>
    /// Returns the requested game object from a new or existing pool
    /// </summary>
    /// <param name="_tag">The tag used to store the game object</param>
    /// <param name="_object">The game object to instantiate in case a pool of that item is not existing</param>
    /// <param name="_position">The position to spawn the object at</param>
    /// <param name="_rotation">The rotation to spawn the object with</param>
    /// <param name="_scale">The local scale to spawn the object with</param>
    public static GameObject SpawnObject(string _tag, GameObject _object, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        //Create new pool if a pool for that item did not exist
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }

        GameObject _newObject;
        //If object in pool exist
        if (objectPools[_tag].Count > 0) {
            _newObject = objectPools[_tag].Dequeue();
            _newObject.transform.SetPositionAndRotation(_position, _rotation);
            _newObject.transform.localScale = _scale;
            _newObject.SetActive(true);
        }
        //Otherwise create a new instance of it
        else {
            _newObject = Object.Instantiate(_object);
            _newObject.transform.SetPositionAndRotation(_position, _rotation);
            _newObject.transform.localScale = _scale;
        }

        return _newObject;
    }

    /// <summary>
    /// Pool the given object in a pool with a specified tag
    /// </summary>
    /// <param name="_tag">The tag to determine the pool to store in</param>
    /// <param name="_object">The object to pool</param>
    public static void PoolObject(string _tag, GameObject _object)
    {
        //Create new pool if a pool for that item did not exist
        if (!objectPools.ContainsKey(_tag)) {
            objectPools.Add(_tag, new Queue<GameObject>());
        }

        //Reset rigidbody
        Rigidbody _rigidbody = _object.GetComponent<Rigidbody>();
        if (_rigidbody != null) {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        //Pool
        _object.SetActive(false);
        objectPools[_tag].Enqueue(_object);
    }
}
