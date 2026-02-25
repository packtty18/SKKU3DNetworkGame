using Photon.Pun;
using UnityEngine;

public class PlayerNetworkSyncAbility : PlayerAbility, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (_owner == null)
        {
            return;
        }

        if (stream.IsWriting)
        {
            float health = 0f;
            float stamina = 0f;
            _owner.TryGetNetworkResourceState(out health, out stamina);

            stream.SendNext(health);
            stream.SendNext(stamina);
            return;
        }

        if (stream.IsReading)
        {
            float health = (float)stream.ReceiveNext();
            float stamina = (float)stream.ReceiveNext();
            _owner.ApplyNetworkResourceState(health, stamina);
        }
    }
}
