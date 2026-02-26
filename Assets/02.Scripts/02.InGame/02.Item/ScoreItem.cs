using System;
using Photon.Pun;
using UnityEngine;

public class ScoreItem : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController controller) && !controller.IsDead)
        {
            controller.GetCoins(this);
        }
    }
}
