using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
몬스터 동작 관련 기본 스크립트입니다.
*/

public class EnemyBase : MonoBehaviour
{
    Character cBase; // 캐릭터 능력치
    Spawner nowSpawn; // 현재 스폰양
    LevelManager lvM; // 습득 경험치 관리용 변수
    HpBarManager cHpBar; // 캐릭터 체력 관리용 변수
    public Animator eAnim; // 몬스터 애니메이션
    [Header("값 입력 필요X, 해당 값은 SpawnPoint 내 Spawner -> EnemyData에서 관리함")]
    public int hp, attack, range, moveSpeed, exp; // 체력, 공격력, 공격 사거리, 이동속도, 지급 경험치
    public float rapid; // 공격 속도
    Transform cPos; // 캐릭터 위치
    float distance; // 캐릭터와의 거리
    bool flag = false; // Update문 반복 동작을 방지하기 위한 플래그
    bool deadFlag; // 사망 확인용 플래그

    void Start()
    {
        cPos = GameObject.FindGameObjectWithTag("Character").transform;
        cBase = GameObject.Find("Character").GetComponent<Character>();
        nowSpawn = GameObject.Find("SpawnPoint").GetComponent<Spawner>();
        lvM = GameObject.Find("UIMain").GetComponent<LevelManager>();
        cHpBar = GameObject.Find("Hp").GetComponent<HpBarManager>();
        nowSpawn.nowSpawn += 1; // 현재 스폰양 증가
        deadFlag = false;

    }

    public void SetData(EnemyData data)
    /* 몬스터 능력치 정보 업데이트, 추후 다양한 몬스터를 스폰해야 하는 경우를 대비해서 위와 같이 구현함. SpawnPoint 내에서 EnemyData를 여러개 만들얻고 이를 나중에 몬스터 개별 ID 식별 코드를 만든 후, ID코드에 맞는 정보를 넣어주면
    간단하게 다양한 몬스터 스폰 구현이 가능하기 때문
    */
    {
        hp = data.hp;
        attack = data.attack;
        rapid = data.rapid;
        range = data.range;
        moveSpeed = data.moveSpeed;
        exp = data.exp;
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, cPos.position); // 캐릭터와의 현재 거리 검사

        if (distance > range && !flag && !deadFlag) // 캐릭터와의 거리가 공격 범위보다 멀 경우
        {
            Move(); // 이동
        }
        else if(distance <= range && !flag && !deadFlag) // 캐릭터와의 거리가 공격 범위보다 가까울 경우
        {
            transform.LookAt(cPos.position); // 캐릭터 방향을 쳐다봄
            InvokeRepeating("Attack", 0, 1/rapid); // 공격
        }

        if( hp <= 0 && !deadFlag) // 체력이 0보다 낮아졌을 경우(사망)
        {
            lvM.ExpUp(exp); // 경험치 지급
            eAnim.SetTrigger("isDead"); // 사망 애니메이션 재생
            nowSpawn.nowSpawn -= 1; // 현재 스폰양 감소
            deadFlag = true;
            Destroy(this.gameObject, 1.5f); // 오브젝트 파괴
        }
    }

    void Move() // 몬스터 이동 함수
    {
        eAnim.SetBool("isMove", true); // 이동 애니메이션 재생
        transform.LookAt(cPos.position); // 캐릭터 방향 쳐다봄
        transform.position = Vector3.MoveTowards(transform.position, cPos.position, moveSpeed * Time.deltaTime); // 현재 위치에서 캐릭터 방향으로 이동속도만큼 지속적으로 이동.
    }

    void Attack() // 몬스터 공격 함수
    {
        if(distance > range && flag)
        {
            CancelInvoke("Attack");
            flag = false;
        }
        else if (distance <= range)
        {
            flag = true;
            eAnim.SetBool("isMove", false);
            eAnim.SetTrigger("isAttack");
        }    
    }

    void OnAttack1Trigger()
    {
        cBase.hp = Mathf.Clamp(cBase.hp - attack, 0, cBase.maxHp); // 캐릭터 체력이 0 밑으로 내려가는 것 방지
        cHpBar.HpUpdate();
    }
}
