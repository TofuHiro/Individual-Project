using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayer : MonoBehaviour
{
    #region Singleton
    public static HealthDisplayer Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }
    #endregion

    [Tooltip("The prefab displaying the health bar for targets")]
    [SerializeField] GameObject healthBarPrefab;
    [Tooltip("The time the health bar will be displayed before disapearing")]
    [SerializeField] float displayTime;
    [Tooltip("The transform to focus the display to")]
    [SerializeField] Transform playerHead;
    [Tooltip("The height to display the health bar above the target")]
    [SerializeField] float displayHeight;

    Dictionary<Transform, Transform> targetHealthBarKVP;

    void Start()
    {
        targetHealthBarKVP = new Dictionary<Transform, Transform>();
    }

    void LateUpdate()
    {
        //Billboard effect
        if (targetHealthBarKVP.Count > 0) {
            foreach (KeyValuePair<Transform, Transform> _bar in targetHealthBarKVP) {
                _bar.Value.LookAt(playerHead.position - playerHead.forward);
                _bar.Value.position = _bar.Key.position + (Vector3.up * displayHeight);
            }
        }
    }

    public void ShowHealthBar(Transform _parent, float _maxHealth, float _health)
    {
        //Update displayed healthbar
        if (targetHealthBarKVP.ContainsKey(_parent)) {
            Slider _slider = targetHealthBarKVP[_parent].GetComponentInChildren<Slider>();
            _slider.maxValue = _maxHealth;
            _slider.value = _health;
        }

        //Create new health bar
        else {
            GameObject _healthBar = ObjectPooler.SpawnObject("Health Bar", healthBarPrefab);
            _healthBar.transform.SetParent(_parent);
            targetHealthBarKVP.Add(_parent, _healthBar.transform);

            //Slider values
            Slider _slider = _healthBar.GetComponentInChildren<Slider>();
            _slider.maxValue = _maxHealth;
            _slider.value = _health;

            StartCoroutine(Despawn(_healthBar.gameObject, _parent));
        }
    }

    IEnumerator Despawn(GameObject _healthBar, Transform _parent)
    {
        yield return new WaitForSeconds(displayTime);

        targetHealthBarKVP.Remove(_parent);
        ObjectPooler.PoolObject("Health Bar", _healthBar);
    }
}
