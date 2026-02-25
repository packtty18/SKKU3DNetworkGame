using Photon.Pun;
using UnityEngine;

public class PlayerNetworkSyncAbility : PlayerAbility, IPunObservable
{
    private PlayerHealthAbility _healthAbility =>  _owner.GetComponent<PlayerHealthAbility>();
    private PlayerStaminaAbility _staminaAbility =>  _owner.GetComponent<PlayerStaminaAbility>();
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (_owner == null)
        {
            return;
        }

        if (stream.IsWriting)
        {
            float health = _healthAbility.Health != null ? _healthAbility.Health.Current : 0f;;
            float stamina = _staminaAbility.Stamina != null ? _staminaAbility.Stamina.Current : 0f;

            stream.SendNext(health);
            stream.SendNext(stamina);
            return;
        }

        if (stream.IsReading)
        {
            float health = (float)stream.ReceiveNext();
            float stamina = (float)stream.ReceiveNext();
            _healthAbility?.ApplyNetworkState(health);
            _staminaAbility?.ApplyNetworkState(stamina);
        }
    }
}
