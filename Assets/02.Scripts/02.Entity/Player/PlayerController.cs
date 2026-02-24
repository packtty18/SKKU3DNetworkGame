using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

//플레이어 대표로써 외부와의 소통 또는 어빌리티 관리
public class PlayerController : MonoBehaviour
{
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    
    public PhotonView PhotonView;
    public EntityStat Stat;

    //도메인 스텟
    public ConsumableStat Health;
    public ConsumableStat Stamina;
    
    //플래그
    public bool Exhausted { get; set; }  //스테미너를 모두 소모한경우 일시적으로 스테미너 사용 기능 금지
    
    
    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Health = new ConsumableStat(Stat.MaxHealth,  Stat.MaxHealth, Stat.RegenerateHealth);
        Stamina = new ConsumableStat(Stat.MaxStamina,  Stat.MaxStamina, Stat.RegenerateStamina);
        Exhausted = false;
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
        //                    필요할때만 하는.. 뒤로 미루는 기법
        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;

            return ability as T;
        }
        
        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //송신
            stream.SendNext(Health.Current);
            stream.SendNext(Stamina.Current);
        }
        else if (stream.IsReading)
        {
            //수신(송신한 순서대로 object로 전송(박싱언박싱 발생 -> 자원소모))
            //JSON을 사용했을때 비용과 박싱언박싱 비용을 잘 판단해서 적절한 것 사용
            Health.SetCurrent((float)stream.ReceiveNext());
            Stamina.SetCurrent((float)stream.ReceiveNext());
        }
        
    }
}