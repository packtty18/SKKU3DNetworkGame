using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreItem : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent( out PlayerController player) && !player.IsDead)
        {
            Debug.Log("아이템 획득");
            player.Score += 100;
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
