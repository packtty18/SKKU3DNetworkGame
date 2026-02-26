using System;
using Photon.Pun;
using UnityEngine;

public class ScoreItem : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent( out PlayerController player) && !player.IsDead)
        {
            player.Score += 100;
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
