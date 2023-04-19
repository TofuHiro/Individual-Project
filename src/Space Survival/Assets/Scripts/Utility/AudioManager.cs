using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioLowPassFilter))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    class Audio
    {
        public AudioClip audioClip;
        public float volume;
        public float pitch;
        public Vector2 range;
    }

    [System.Serializable]
    struct AudioType
    {
        public string tag;
        public List<Audio> audios;
    }
    
    [Tooltip("Prefab with an audiosource component attached. Remember to add lowpassfilter")]
    [SerializeField] GameObject audioSourcePrefab;
    [Tooltip("Audiosource of background music")]
    [SerializeField] AudioSource backGroundMusic;
    [Tooltip("Set audio clips with tags. A random audio clip will be chosen from a type")]
    [SerializeField] List<AudioType> audioTypes;

    PlayerController player;
    List<AudioSource> playingSources;

    bool useLowPass;

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        }
        Instance = this;

        playingSources = new List<AudioSource>();
    }

    void Start()
    {
        player = PlayerController.Instance;

        if (backGroundMusic != null) {
            backGroundMusic.Play();
        }
    }

    void OnEnable()
    {
        PlayerVitals.Instance.OnOxygenTriggerChange += ToggleInOxygenSetting;
    }

    void OnDisable()
    {
        PlayerVitals.Instance.OnOxygenTriggerChange -= ToggleInOxygenSetting;
    }

    void OnDestroy()
    {
        PlayerVitals.Instance.OnOxygenTriggerChange -= ToggleInOxygenSetting;
    }

    void Update()
    {
        //Check if no longer playing and pool
        if (playingSources.Count > 0) {
            foreach (AudioSource _source in playingSources) {
                if (!_source.isPlaying) {
                    ObjectPooler.PoolObject("Audio Source", _source.gameObject);
                    playingSources.Remove(_source);
                    break;
                }
            }
        }
    }

    Audio GetAudio(string _tag)
    {
        for (int i = 0; i < audioTypes.Count; i++) {
            if (audioTypes[i].tag == _tag) {
                int _index = Random.Range(0, audioTypes[i].audios.Count);
                return audioTypes[i].audios[_index];
            }
        }
        return null;
    }
    
    public void PlayClip(string _tag, bool _spatial)
    {
        player ??= PlayerController.Instance;

        AudioSource _source = ObjectPooler.SpawnObject("Audio Source", audioSourcePrefab, player.GetPlayerPosition(), Quaternion.identity).GetComponent<AudioSource>();
        Audio _audio = GetAudio(_tag);
        _source.clip = _audio.audioClip;
        _source.volume = _audio.volume;
        _source.pitch = _audio.pitch;
        _source.minDistance = _audio.range.x;
        _source.maxDistance = _audio.range.y;
        _source.spatialBlend = _spatial ? 1f : 0f;
        _source.PlayOneShot(_source.clip);

        if (useLowPass)
            _source.GetComponent<AudioLowPassFilter>().enabled = true;
        else
            _source.GetComponent<AudioLowPassFilter>().enabled = false;

        playingSources.Add(_source);
    }

    public void PlayClip(string _tag, Vector3 _position)
    {
        AudioSource _source = ObjectPooler.SpawnObject("Audio Source", audioSourcePrefab, _position, Quaternion.identity).GetComponent<AudioSource>();
        Audio _audio = GetAudio(_tag);
        _source.clip = _audio.audioClip;
        _source.volume = _audio.volume;
        _source.pitch = _audio.pitch;
        _source.minDistance = _audio.range.x;
        _source.maxDistance = _audio.range.y;
        _source.spatialBlend = 1f;
        _source.PlayOneShot(_source.clip);

        if (useLowPass)
            _source.GetComponent<AudioLowPassFilter>().enabled = true;
        else
            _source.GetComponent<AudioLowPassFilter>().enabled = false;

        playingSources.Add(_source);
    }

    void ToggleInOxygenSetting(bool _state)
    {
        if (_state)
            useLowPass = false;
        else 
            useLowPass = true;
    }
}
