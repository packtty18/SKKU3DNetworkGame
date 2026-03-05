using System;
using Photon.Pun;
using UnityEngine;

public class ScoreItem : MonoBehaviourPun
{
    private bool _isCollected;

    private void OnEnable()
    {
        _isCollected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected)
        {
            return;
        }

        if (!other.TryGetComponent(out PlayerController player) || player.IsDead)
        {
            return;
        }

        if (!player.PhotonView.IsMine || ScoreManager.Instance == null)
        {
            return;
        }

        _isCollected = true;
        ScoreManager.Instance.AddScore(100);
        ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
    }
}
