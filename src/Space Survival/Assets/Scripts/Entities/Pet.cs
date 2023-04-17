using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGame;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(AICamera))]
public class Pet : MonoBehaviour, IInteractable, IDataPersistance
{
    //One pet for now
    public static bool IsActive { get; set; } = false;

    [SerializeField] float maxDistanceFromPlayer;
    [SerializeField] float moveDistanceThreshold;
    [SerializeField] float stopDistance;
    [SerializeField] string[] activateSounds;
    [SerializeField] Storage storage;

    PlayerController player;
    PlayerMotor motor;
    AICamera orientation;
    AudioManager audioManager;

    public InteractionType GetInteractionType()
    {
        if (IsActive)
            return InteractionType.None;
        else 
            return InteractionType.Use;
    }

    public void SetActive(bool _state)
    {
        IsActive = _state;

        if (IsActive) {
            GameManager.OnPlayerRespawn += TeleportToPlayer;
            PlayerController.OnSpeedUpStart += StartSpeedUp;
            PlayerController.OnSpeedUpCancel += StopSpeedUp;
        }
        else {
            GameManager.OnPlayerRespawn -= TeleportToPlayer;
            PlayerController.OnSpeedUpStart -= StartSpeedUp;
            PlayerController.OnSpeedUpCancel -= StopSpeedUp;
        }
    }

    float GetPlayerDistance()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    Vector3 GetDirFromPlayer()
    {
        return (player.transform.position - new Vector3(0f, .2f, 0f) - transform.position).normalized;
    }

    void OnDisable()
    {
        GameManager.OnPlayerRespawn -= TeleportToPlayer;
        PlayerController.OnSpeedUpStart -= StartSpeedUp;
        PlayerController.OnSpeedUpCancel -= StopSpeedUp;
        IsActive = false;
    }

    void OnDestroy()
    {
        GameManager.OnPlayerRespawn -= TeleportToPlayer;
        PlayerController.OnSpeedUpStart -= StartSpeedUp;
        PlayerController.OnSpeedUpCancel -= StopSpeedUp;
        IsActive = false;
    }

    void Start()
    {
        player = PlayerController.Instance;
        motor = GetComponent<PlayerMotor>();
        orientation = GetComponent<AICamera>();
        audioManager = AudioManager.Instance;
    }

    void Update()
    {
        if (!IsActive)
            return;

        float _distanceFromPlayer = GetPlayerDistance();

        if (_distanceFromPlayer <= stopDistance) {
            StopRotation();
            StopMovement();
            return;
        }

        if (_distanceFromPlayer > maxDistanceFromPlayer) {
            TeleportToPlayer();
        } 

        if (_distanceFromPlayer > moveDistanceThreshold) {
            LookAtPlayer();
            Move(orientation.GetOrientation().transform.forward);
        }
    }

    public void Interact()
    {
        if (!IsActive) {
            //Sounds
            foreach (string _sound in activateSounds) {
                audioManager.PlayClip(_sound, transform.position);
            }

            SetActive(true);
        }
    }

    void TeleportToPlayer()
    {
        transform.position = player.GetPlayerPosition() - (player.transform.forward * 2f);
    }

    void Move(Vector3 _dir)
    {
        motor.SetDirection(_dir);
    }

    void LookAtPlayer()
    {
        orientation.SetRotation(Quaternion.LookRotation(GetDirFromPlayer()), motor.IsFloating);
    }

    void StopRotation()
    {
        orientation.SetRotation(orientation.GetOrientation().rotation, motor.IsFloating);
    }

    void StopMovement()
    {
        Move(Vector3.zero);
    }

    void StartSpeedUp()
    {
        motor.SpeedUp(true);
    }

    void StopSpeedUp()
    {
        motor.SpeedUp(false);
    }

    public void SaveData(ref GameData _data)
    {
        Item[] _items = storage.GetStorage();
        //Save all item name as tags
        string[] _itemTags = new string[_items.Length];
        for (int i = 0; i < _items.Length; i++) {
            if (_items[i] == null)
                _itemTags[i] = "";
            else if (_items[i].GetItemType() == ItemType.Weapon)
                _itemTags[i] = "";
            else
                _itemTags[i] = _items[i].ItemScriptableObject.name;
        }

        //Weapons
        WeaponData[] _weapons = new WeaponData[_items.Length];
        for (int i = 0; i < _items.Length; i++) {
            if (_items[i] == null)
                continue;
            if (_items[i].GetItemType() != ItemType.Weapon) 
                continue;

            RayWeapon _rayWep = _items[i].GetComponent<RayWeapon>();
            if (_rayWep != null) {
                _weapons[i] = new WeaponData(_items[i].ItemScriptableObject.name, _items[i].transform.position, _items[i].transform.rotation, _rayWep.CurrentAmmo, _rayWep.CurrentClip, _rayWep.gameObject.activeSelf);
                continue;
            }

            ProjectileWeapon _projectileWep = _items[i].GetComponent<ProjectileWeapon>();
            if (_projectileWep != null) {
                _weapons[i] = new WeaponData(_items[i].ItemScriptableObject.name, _items[i].transform.position, _items[i].transform.rotation, _projectileWep.CurrentAmmo, _projectileWep.CurrentClip, _projectileWep.gameObject.activeSelf);
                continue;
            }

            MeleeWeapon _meleeWep = _items[i].GetComponent<MeleeWeapon>();
            if (_meleeWep != null) {
                _weapons[i] = new WeaponData(_items[i].ItemScriptableObject.name, _items[i].transform.position, _items[i].transform.rotation, _meleeWep.Durability, _meleeWep.gameObject.activeSelf);
                continue;
            }

            BuildingTool _tool = _items[i].GetComponent<BuildingTool>();
            if (_tool != null) {
                _weapons[i] = new WeaponData(_items[i].ItemScriptableObject.name, _items[i].transform.position, _items[i].transform.rotation, 0, _tool.gameObject.activeSelf);
                continue;
            }
        }

        _data.petData = new PetData(IsActive, _itemTags, _weapons);
    }

    public void LoadData(GameData _data)
    {
        PrefabCatalog _prefabs = PrefabCatalog.Instance;

        if (_data.petData.acquired) {
            IsActive = true;

            Item[] _items = new Item[_data.petData.itemTags.Length];

            //Assign items
            for (int i = 0; i < _items.Length; i++) {
                if (_data.petData.itemTags[i] == "")
                    continue;

                _items[i] = _prefabs.GetItem(_data.petData.itemTags[i]);
            }

            //Assign initialized weapon
            for (int i = 0; i < _items.Length; i++) {
                if (_data.petData.weapons[i].tag == "")
                    continue;

                Item _weapon = Instantiate(_prefabs.GetItem(_data.petData.weapons[i].tag), _data.petData.weapons[i].position, _data.petData.weapons[i].rotation);
                
                RayWeapon _rayWeapon = _weapon.GetComponent<RayWeapon>();
                if (_rayWeapon != null) {
                    _rayWeapon.CurrentAmmo = _data.petData.weapons[i].ammoCount;
                    _rayWeapon.CurrentAmmo = _data.petData.weapons[i].clipCount;
                    _rayWeapon.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }

                ProjectileWeapon _projectileWeapon = _weapon.GetComponent<ProjectileWeapon>();
                if (_projectileWeapon != null) {
                    _projectileWeapon.CurrentAmmo = _data.petData.weapons[i].ammoCount;
                    _projectileWeapon.CurrentAmmo = _data.petData.weapons[i].clipCount;
                    _projectileWeapon.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }

                MeleeWeapon _meleeWeapon = _weapon.GetComponent<MeleeWeapon>();
                if (_meleeWeapon != null) {
                    _meleeWeapon.Durability = _data.petData.weapons[i].durability;
                    _meleeWeapon.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }

                BuildingTool _tool = _weapon.GetComponent<BuildingTool>();
                if (_tool != null) {
                    _tool.gameObject.SetActive(false);
                    _items[i] = _weapon;
                    continue;
                }
            }

            storage.SetStorage(_items);
        }
    }
}
