using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class PlayerVoiceAbility : PlayerAbility
{
    [SerializeField] private GameObject _speakingIcon;
    [SerializeField] private PhotonVoiceView _voiceView;

    private Recorder _recorder;

    private void Start()
    {
        InitializeRecorder();
    }

    private void Update()
    {
        if (_recorder == null)
        {
            InitializeRecorder();
        }

        bool isSpeaking = false;

        if (_owner.PhotonView.IsMine)
        {
            if (_recorder != null)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    _recorder.TransmitEnabled = !_recorder.TransmitEnabled;
                    Debug.Log($"TransmitEnabled : {_recorder.TransmitEnabled}");
                }

                isSpeaking = _recorder.TransmitEnabled && _recorder.IsCurrentlyTransmitting;
            }
        }
        else
        {
            if (_voiceView != null)
            {
                isSpeaking = _voiceView.IsSpeaking;
            }
        }

        if (_speakingIcon != null)
        {
            _speakingIcon.SetActive(isSpeaking);
        }
    }

    private void InitializeRecorder()
    {
        if (_recorder != null)
        {
            return;
        }

        if (_voiceView != null && _voiceView.RecorderInUse != null)
        {
            _recorder = _voiceView.RecorderInUse;
        }

        if (_recorder == null)
        {
            _recorder = FindAnyObjectByType<Recorder>();
        }

        if (_recorder == null)
        {
            Debug.LogWarning("[PlayerVoiceAbility] Recorder를 찾지 못했습니다.");
            return;
        }

        _recorder.VoiceDetection = true;
        _recorder.VoiceDetectionThreshold = 0.01f;
        _recorder.VoiceDetectionDelayMs = 300;

        Debug.Log($"[PlayerVoiceAbility] Recorder 찾음: {_recorder.name}");
    }
}