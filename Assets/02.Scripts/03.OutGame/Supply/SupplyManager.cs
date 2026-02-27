using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SupplyManager : MonoBehaviourPun
{
    //이 기능은 마스터만이 사용한다.
    // Nav의 랜덤한 지점 + y+10 랜덤한 위치를 구한다
    // 일정 시간마다 아이템을 생성한다

    [SerializeField] private float _supplyTime = 3f;
    [SerializeField]private float _supplyTimer = 0;
    [SerializeField] private Vector3 _spawnOffset = new Vector3(0, 10, 0);
    private NavMeshTriangulation _navData;
    
    private void Awake()
    {
        GetTriangulation();
    }

    private void Start()
    {
        _supplyTimer = 0;
    }


    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        _supplyTimer += Time.deltaTime;
        if (_supplyTimer >= _supplyTime)
        {
            _supplyTimer = 0;
            TrySupplySpawn();
        }
    }
    
    private void GetTriangulation()
    {
        //Nav 의 삼각형들을 모아 캐싱한다.
        _navData = NavMesh.CalculateTriangulation();
    }
    
    public Vector3 GetRandomPoint()
    {
        if (_navData.vertices == null || _navData.vertices.Length == 0)
        {
            throw new ArgumentException("Nav 데이터가 올바르지 않습니다.");
        }
        
        //랜덤한 삼각형을 하나 뽑아낸다.
        //indices는 3개씩 묶여서 삼각형을 구성하는 인덱스 배열이다.
        //_navData.indices.Length / 3  = 전체 삼각형의 개수
        //다시 *3을 하는 이유는 해당 삼각형의 첫번째 정점을 알아내기 위함
        int triangleIndex = Random.Range(0, _navData.indices.Length / 3) * 3;
        
        //주어진 삼각형의 정점들을 뽑아내기
        Vector3 a = _navData.vertices[_navData.indices[triangleIndex]];
        Vector3 b = _navData.vertices[_navData.indices[triangleIndex + 1]];
        Vector3 c = _navData.vertices[_navData.indices[triangleIndex + 2]];
        
        //0~1의 난수를 생성
        float r1 = Random.value;
        float r2 = Random.value;
        
        //만약 난수 두개의 합이 1보다 크면 각 난수 반전
                         //        //두개의 합이 1보다 크게되면 삼각형의 범위를 나가게된다.
                         //        //이를 반전함으로 다시 삼각형의 영역 안으로 들어오게된다.
        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }
        
        //a정점에서 시작해서 ab방향으로 r1만큼 이동, ac방향으로 r2만큼 이동
        return a + r1 * (b - a) + r2 * (c - a);
    }
    
    
    private void TrySupplySpawn()
    {
        Vector3 targetPosition = GetRandomPoint() + _spawnOffset;
        ItemObjectFactory.Instance.RequestSpawnCoins(targetPosition);
    }
}
