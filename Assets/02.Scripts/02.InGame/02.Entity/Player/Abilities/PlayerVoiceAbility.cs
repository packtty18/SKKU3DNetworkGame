using System;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class PlayerVoiceAbility : PlayerAbility
{
    [SerializeField]  private GameObject _speakingIcon;
    [SerializeField] private PhotonVoiceView _voiceView;
    private Recorder _recorder;

    private void Start()
    {
        _recorder = FindAnyObjectByType<Recorder>();

        _recorder.VoiceDetection = true;
        _recorder.VoiceDetectionThreshold = 0.01f;
        _recorder.VoiceDetectionDelayMs = 300;
    }

    private void Update()
    {
        bool isSpeaking = false;
        if (_owner.PhotonView.IsMine)
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                _recorder.TransmitEnabled = !_recorder.TransmitEnabled;
            }
            isSpeaking = _recorder.IsCurrentlyTransmitting;
        }
        else
        {
            isSpeaking = _voiceView.IsSpeaking;
        }
        
        _speakingIcon.gameObject.SetActive(isSpeaking);
        
    }
}
