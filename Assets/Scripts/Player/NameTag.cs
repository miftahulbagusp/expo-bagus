using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : MonoBehaviour
{
    private bool _isMuted = true;
    private bool _isMine;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image voiceIcon;
    [SerializeField] private Sprite[] voiceSprite;

    [SerializeField] private float updateStep = 0.3f;
    [SerializeField] private int sampleDataLength = 1024;
    private AudioSource _voiceAudioSource;
    private float _currentUpdateTime;
    private float[] _clipSampleData;
    
    private Transform _lookAtTarget;

    private void Start()
    {
        SetTarget(NonVRPlayerController.Instance.GetComponentInChildren<Camera>().transform);
    }

    private void Update()
    {
        LookAtLocalPlayer();
        GetAudioLoudness();
    }

    public void Init(string playerName, AudioSource audioSource = null, bool isMine = false)
    {
        nameText.text = playerName;
        _voiceAudioSource = audioSource;
        _isMine = isMine;
    }

    public void UpdateVoiceIcon(bool isTransmitting)
    {
        if (isTransmitting)
        {
            voiceIcon.enabled = false;
            voiceIcon.sprite = voiceSprite[1];
            _isMuted = false;
        }
        else
        {
            voiceIcon.enabled = true;
            voiceIcon.sprite = voiceSprite[0];
            _isMuted = true;
        }
    }
    
    private void LookAtLocalPlayer()
    {
        transform.LookAt(_lookAtTarget);

        var newRotation = transform.localRotation.eulerAngles;
        newRotation.x = -newRotation.x;
        newRotation.y += 180;
        newRotation.z = 0;
        transform.localRotation = Quaternion.Euler(newRotation);
    }

    private void SetTarget(Transform lookAtTarget)
    {
        _lookAtTarget = lookAtTarget;
    }

    /// <summary>
    /// Show speaker icon when user is speaking
    /// </summary>
    private void GetAudioLoudness()
    {
        if (_isMuted) return;

        if (_isMine)
        {
            // voiceIcon.enabled = VoiceManager.Instance.recorder.VoiceDetector.Detected;
        }
        else
        {
            if (_voiceAudioSource.clip == null) return;
            if (_clipSampleData == null)
            {
                _clipSampleData = new float[sampleDataLength];
            }

            _currentUpdateTime += Time.deltaTime;
            if (_currentUpdateTime >= updateStep)
            {
                _currentUpdateTime = 0f;
                _voiceAudioSource.clip.GetData(_clipSampleData, _voiceAudioSource.timeSamples);
                float clipLoudness = 0f;
                foreach (var sample in _clipSampleData)
                {
                    clipLoudness += Mathf.Abs(sample);
                }

                clipLoudness /= sampleDataLength;

                voiceIcon.enabled = clipLoudness >= 0.001f;
            }
        }
    }
}