using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EffectsManager : MonoBehaviour
{
    #region Singleton
    public static EffectsManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    [System.Serializable]
    class EffectKeyValue
    {
        public string tag;
        public GameObject effect;
    }

    [SerializeField] List<EffectKeyValue> effects;

    public void PlayEffect(string _tag, Vector3 _pos, Quaternion _rot)
    {
        foreach (EffectKeyValue _effect in effects) {
            if (_effect.tag == _tag) {
                VisualEffect _newEffect = ObjectPooler.SpawnObject(_tag, _effect.effect, _pos, _rot).GetComponent<VisualEffect>();
                _newEffect.Play();
                StartCoroutine(DespawnEffect(_tag, _newEffect.gameObject));
                break;
            }
        }
    }

    IEnumerator DespawnEffect(string _tag, GameObject _effect)
    {
        yield return new WaitForSeconds(5f);

        ObjectPooler.PoolObject(_tag, _effect);
    }
}
