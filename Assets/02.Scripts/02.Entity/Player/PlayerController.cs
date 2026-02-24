using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float STAMINA_REGEN_DELAY = 3f;

    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    public PhotonView PhotonView;
    public EntityStat Stat;

    public PlayerInputs Inputs;

    public ConsumableStat Health;
    public ConsumableStat Stamina;

    public bool Exhausted { get; set; }

    private float _staminaRegenCooldown = 0f;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Inputs = new PlayerInputs();
        Health = new ConsumableStat(Stat.MaxHealth, Stat.MaxHealth, Stat.RegenerateHealth);
        Stamina = new ConsumableStat(Stat.MaxStamina, Stat.MaxStamina, Stat.RegenerateStamina);
        Exhausted = false;
    }

    private void Update()
    {
        if (PhotonView == null || !PhotonView.IsMine || Inputs == null)
        {
            return;
        }

        Inputs.MoveHorizontalInput = Input.GetAxisRaw("Horizontal");
        Inputs.MoveVerticalInput = Input.GetAxisRaw("Vertical");
        Inputs.DashPressed = Input.GetKey(KeyCode.LeftShift);
        Inputs.JumpPressed = Input.GetKeyDown(KeyCode.Space);

        Inputs.LookInputX = Input.GetAxis("Mouse X");
        Inputs.LookInputY = Input.GetAxis("Mouse Y");

        Inputs.AttackPressed = Input.GetMouseButton(0);

        Inputs.Skill1Pressed = Input.GetKeyDown(KeyCode.Alpha1);
        Inputs.Skill2Pressed = Input.GetKeyDown(KeyCode.Alpha2);
        Inputs.Skill3Pressed = Input.GetKeyDown(KeyCode.Alpha3);

        if (_staminaRegenCooldown > 0f)
        {
            _staminaRegenCooldown -= Time.deltaTime;
            return;
        }

        Stamina?.TryRegenerate(Time.deltaTime);

        if (Exhausted && Stamina != null && Stamina.Ratio >= Stat.ExhaustRecoveryRatio)
        {
            Exhausted = false;
        }
    }

    public bool TryUseStamina(float amount)
    {
        if (Stamina == null)
        {
            return false;
        }

        if (Exhausted)
        {
            return false;
        }

        if (Stamina.TryConsume(amount))
        {
            _staminaRegenCooldown = STAMINA_REGEN_DELAY;

            if (Stamina.IsEmpty)
            {
                Exhausted = true;
            }

            return true;
        }

        return false;
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;
            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}를 {gameObject.name}에서 찾을 수 없습니다.");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Health.Current);
            stream.SendNext(Stamina.Current);
        }
        else if (stream.IsReading)
        {
            Health.SetCurrent((float)stream.ReceiveNext());
            Stamina.SetCurrent((float)stream.ReceiveNext());
        }
    }
}
