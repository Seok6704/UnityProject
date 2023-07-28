using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
몬스터 스폰 관련 스크립트, 몬스터의 능력치도 이곳에서 추가 가능.
*/

public class Spawner : MonoBehaviour
{
    public GameObject enemy; // 복제할 prefab -> Assets 폴더의 Enemy 사용.
    public EnemyData[] enemyData; // 몬스터 데이터 배열
    BoxCollider spawnArea; // 스폰 구역(플레이어 이동에 따라 맞춰서 이동함)
    public int nowSpawn = 0; // 현재 스폰 개체 수
    Coroutine spawnCoroutine;
    bool flag; // Update문 반복 방지용 플래그

    List<GameObject> eGameObject = new List<GameObject>(); // 복제 몬스터가 추가 될 리스트

    void Awake()
    {
        spawnArea = GetComponent<BoxCollider>(); // 스폰 구역 범위 정의(Spawner 스크립트가 들어있는 오브젝트의 BoxCollider 범위를 스폰 구역으로 설정)
        flag = true;
    }

    void Update() // 현재 스폰 양 검사용 Update문
    {
        if(nowSpawn >= enemyData[0].maxSpawn && !flag) // 현재 스폰양이 최대 스폰양 이상이 될 경우
        {
            StopCoroutine(spawnCoroutine); // 코루틴 정지
            flag = true;
        }
        else if(nowSpawn < enemyData[0].maxSpawn && flag) // 현재 스폰양이 최대 스폰양보다 적을 경우
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine()); // 코루틴 시작
            flag = false;
        }
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSecondsRealtime(enemyData[0].spawnRate); // enemyData[0](지금은 거미 하나만 있으므로, 0으로 직접 지정함)의 spawnRate만큼 지연 시킨 후, 동작
        Spawn();
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    Vector3 GetEnemyPosition() // 소환 위치 리턴하는 함수
    {
        Vector3 size = spawnArea.size; // Spawner의 BoxCollider의 사이즈 만큼 범위 설정

        float posX = transform.position.x + Random.Range(-size.x/2f, size.x/2f); // x축 사이즈 값의 절반을 각각 음의 축, 양의 축 범위 안에서 x축 값 설정(사이즈 총 크기의 -와 + 방향이므로, 절반으로 나누어 주어야 본래의 크기 범위가 됨)
        float posY = 0; // y축은 현재 몬스터 스폰 높이인데, 필요하지 않으므로 0 고정.
        float posZ = transform.position.z + Random.Range(-size.z/2f, size.z/2f); // x축 계산과 동일

        Vector3 spawnPos = new Vector3(posX, posY, posZ); // 설정된 x, y, z 축 값을 하나로 합침.

        if(spawnPos == transform.position) GetEnemyPosition(); // 캐릭터와 겹치는 위치에 스폰될 경우, 위치 다시 지정

        return spawnPos;
    }

    void Spawn()
    {
        Vector3 spawnPos = GetEnemyPosition(); // 스폰 위치 설정

        GameObject spawnEnemy = Instantiate(enemy, spawnPos, Quaternion.identity); // 오브젝트 생성 (enemy prefab, 위치, 회전값 지정. 회전값은 기본 값으로 설정)
        spawnEnemy.GetComponent<EnemyBase>().SetData(enemyData[0]); // 생성된 오브젝트의 EnemyBase 스크립트 값에 enemyData[0]의 값 적용.
        eGameObject.Add(spawnEnemy); // 리스트 배열에 생성된 오브젝트 추가. 해당 부분이 있어야 각각 생성된 오브젝트를 관리 할 수 있게 됨.
    }
}

[System.Serializable]
public class EnemyData // 몬스터 스탯 정보, 데이터를 자유롭게 변경하고, 추후 다양한 몬스터 데이터를 추가 하게 될 경우 위와 같이 Serializable하게 생성하여 여러 데이터를 관리하는것이 더 편하기 때문에 이용.
{
    [Header("몬스터 능력치 설정")]
    [Header("스폰 주기")]
    public int spawnRate; // 스폰 주기
    [Header("최대 스폰 수")]
    public int maxSpawn; // 스폰 최대 개체 수
    [Header("몬스터 체력")]
    public int hp; // 체력
    [Header("몬스터 공격력")]
    public int attack; // 공격력
    [Header("몬스터 공격속도")]
    public float rapid; // 공격속도
    [Header("몬스터 공격 사거리")]
    public int range; // 공격 사거리
    [Header("몬스터 처치 경험치")]
    public int exp; // 지급 경험치
    [Header("몬스터 이동 속도")]
    public int moveSpeed; // 이동 속도
}
