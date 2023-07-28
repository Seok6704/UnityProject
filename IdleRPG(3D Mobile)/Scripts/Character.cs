using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
메인 캐릭터의 스탯 및 동작 관련 스크립트
*/

public class Character : MonoBehaviour
{
    [Header("캐릭터 능력치 설정")]
    [Header("최대 체력")]
    public int maxHp; // 최대 체력
    [Header("캐릭터 현재 체력")]
    public int hp; // 체력
    [Header("캐릭터 공격력")]
    public int attack; // 공격력
    [Header("캐릭터 공격 속도")]
    public float rapid; // 공격속도
    [Header("캐릭터 공격 사거리")]
    public int range; // 공격 사거리
    [Header("캐릭터 광역 공격 범위")]
    public int skillRange; // 스킬2 공격 범위
    [Header("캐릭터 이동 속도")]
    public float moveSpeed; // 이동 속도
    [Header("캐릭터 레벨")]
    public int level; // 플레이어 레벨
    [Header("캐릭터 현재 경험치")]

    public int nowExp; // 현재 경험치
    public Animator cAnim; // 캐릭터 애니메이션 변수
    HpBarManager hpBar; // 공격3 사용 시, 체력 회복으로 hp bar 변동을 이용하기 위한 변수
    Transform target; // 타겟의 트랜스폼.
    EnemyBase eBase; // 추적 대상의 스탯
    Spawner nowSpawn; // 현재 스폰양
    float distance; // 추적 대상과의 거리
    bool flag; // Update문 중복 동작 방지를 위한 플래그
    bool engage; // 공격 범위 내 적 검출 용 플래그
    Vector3 enemyPos; // 추적 대상 위치
    int atk; // 공격1, 2, 3 구분 용 변수( 0 = 공격1(단일 공격), 1 = 공격2(광역 공격), 2 = 공격3(회복))
    Quaternion eRotation; // 적 쿼터리언 변수
    Vector3 goalVector; // 목표 벡터값

    void Start()
    {
        cAnim = GetComponent<Animator>();
        hpBar = GameObject.Find("Hp").GetComponent<HpBarManager>();
        nowSpawn = GameObject.Find("SpawnPoint").GetComponent<Spawner>();
        flag = false;
        engage = false;
        InvokeRepeating("UpdateTarget", 0, 1f); // 타겟 검색 시작
    }

    public void UpdateTarget() // 추적 할 타겟 설정하는 함수
    {
        Collider[] cols;
        if ( IsEngage() ) // 현재 공격 범위 내에 적이 있는 경우
        {
            cols = Physics.OverlapSphere(transform.position, range);
        }
        else // 현재 공격 범위 내에 적이 없는 경우
        {
            cols = Physics.OverlapSphere(transform.position, 10f);
            // Collider 배열 cols에 OverlapSphere를 이용하여 현재 캐릭터 위치에서 10의 반지름을 가지는 크기의 구 안의 있는 콜라이더 검출. 추후, 맵의 기울기가 생기는 것을 감안하여 Sphere(구)형태 사용
        }

        if(cols.Length > 0) // 배열의 메소드가 1개 이상 있을 경우
        {
            for(int i = 0; i < cols.Length; i++) // 배열의 총 길이만큼 반복
            {
                if(cols[i].tag == "Enemy") // 배열 안의 메소드 중, 태그가 Enemy인 것을 찾음.
                {
                    target = cols[i].gameObject.transform; // 타겟 트랜스폼 배정
                    eBase = cols[i].gameObject.GetComponent<EnemyBase>(); // 타겟의 능력치 배정
                    if(eBase.hp > 0) break; 
                    else if(eBase.hp <= 0) target = null;
                    /* cols 배열 안에 Enemy 태그를 지닌 대상이 여럿 있을 경우, 대상이 계속 변경되는 것을 막기 위해, 가장 먼저 찾아진 대상으로 타겟을 설정 한 뒤, 반복문 탈출. 해당 break가 존재 하지 않을 경우
                    캐릭터가 대상을 제대로 쳐다 보지 않는 등의 문제가 발생함(target에 따라 바라보는 방향을 지정해주는데, Update문의 동작 타이밍에 따라 target이 변화하는 경우가 발생하는 듯 함).
                    */
                }
            }
        }
    }

    public bool IsEngage() // 공격 범위 내에 적이 있는지 검사하는 함수 해당 함수가 없을 경우 캐릭터 공격 범위 내에 적이 있음에도 불구하고 멀리 있는 적을 때리러 가는 경우가 발생함.
    {
        engage = false;
        Collider[] cols = Physics.OverlapSphere(transform.position, range);
        // 캐릭터 공격 범위 내에서만 메소드 검출

        if(cols.Length > 0)
        {
            for(int i = 0; i < cols.Length; i++)
            {
                if(cols[i].tag == "Enemy")
                {
                    eBase = cols[i].gameObject.GetComponent<EnemyBase>();
                    if(eBase.hp > 0)
                    {
                        engage = true; // 범위 내 살아있는 적이 있음
                        break; 
                    } 
                }
            }
        }
        return engage;
    }

    void Update()
    {
        if(target != null) // 타겟이 존재 할 경우, 추적 대상의 위치를 타겟의 위치로 설정.
        {
            CancelInvoke("UpdateTarget"); // 타겟 검색 중지
            enemyPos = target.position;
        }
        else 
        {
            enemyPos = transform.position; // 타겟이 존재하지 않을 경우, 추적 대상의 위치를 캐릭터의 위치로 설정(맵 안에 몬스터가 존재하지 않을 경우, 제자리에 서있음).
        }

        distance = Vector3.Distance(transform.position, enemyPos);

        if(distance != 0) // 대상과의 거리가 0이 아닐 경우(적이 존재 할 경우)
        {
            goalVector = enemyPos - transform.position; // 목표 벡터 값 설정
            eRotation = Quaternion.LookRotation(goalVector, Vector3.up); // 캐릭터가 쳐다봐야할 적 Quaternion 값 설정. 기준 y축 
            if (distance > range && !flag) // 추적 대상과의 거리가 캐릭터의 사거리보다 멀 경우, 캐릭터 이동(flag는 Update문의 반복 실행을 막기 위함).
            {
                Move();
            }
            else if(distance <= range && !flag) // 추적 대상과의 거리가 캐릭터의 사거리보다 가까울 경우, 공격 시작.
            {
                cAnim.SetBool("isMove", false);

                transform.rotation = Quaternion.Slerp(transform.rotation, eRotation, rapid * 10 *Time.deltaTime);
                /*
                회전을 최대한 부드럽게 구현하기 위해 구형 선형 보간 Slerp 사용, Lerp(선형 보간)을 사용하여도 육안으로 큰 차이를 느낄수는 없을것으로 예상되나, 3D인점을 감안하여 구형 선형 보간을 사용하였음.
                */
                
                if(transform.rotation == Quaternion.LookRotation(goalVector)) // 캐릭터의 현재 회전값이 목표하였던 회전값과 같을 경우
                {
                    InvokeRepeating("Attack", 0, 1/rapid); //Attack 함수를 지연 없이 캐릭터의 초당 공격 횟수만큼 반복 실행.
                }
                /*
                해당 부분에서 캐릭터는 몬스터를 정확하게 쳐다보고 있으나, 값은 일치하지 않는 경우가 발생함.
                축 대칭된 값으로 나타나는것을 보아, Slerp 과정에서 기준이 되는 축의 차이로 인하여 발생하는것 같으나, 이를 해결할 마땅한 방법이 떠오르지 않아, 쳐다보는 방향은 일단 동일함으로, else문을 통해 강제로 값을 지정해줌.
                */
                else
                {
                    transform.rotation = Quaternion.LookRotation(goalVector); // 타겟 강제 조정
                }
           }
        }
        else 
        {
            cAnim.SetBool("isMove", false); // 대상과의 거리가 0일 경우(적이 존재하지 않을 경우) 이동 X.
            CancelInvoke("Attack");
        }
    }

    void Move() // 캐릭터 이동 함수
    {
        cAnim.SetBool("isMove", true); // 애니메이션 트리거 isMove 동작  
        transform.rotation = Quaternion.Slerp(transform.rotation, eRotation, rapid * 10 * Time.deltaTime); // 목표 바라보게 설정
        transform.position = Vector3.MoveTowards(transform.position, enemyPos, moveSpeed * Time.deltaTime); // 캐릭터의 위치를 추적 대상의 위치로 매 프레임마다 캐릭터 이동속도만큼 전진.
    }

    void Attack() // 캐릭터 공격 함수
    {
        if(eBase.hp <= 0) // 적의 체력이 0 이하인지 체크
        {
            CancelInvoke("Attack"); // 공격 중지
            InvokeRepeating("UpdateTarget", 0, 1f); // 타겟 검색 시작
            flag = false;
        }
        else if(eBase.hp > 0 && distance <= range) // 적의 체력이 0이 아닐경우
        {
            if(atk == 0) Attack1(); // 공격 1
            else if(atk ==1) Attack2(); // 공격 2
            else if(atk ==2) Attack3(); // 공격 3
        }
        else 
        {
            CancelInvoke("Attack");
            flag = false;
        }
    }

    void Attack1() // 단일 공격
    {
        flag = true;
        cAnim.SetBool("isMove", false); // 이동 애니메이션 재생 중지
        cAnim.SetTrigger("isAttack");  // 공격1 공격 애니메이션 재생
        atk++; // 공격 값(0 = 공격1, 1 = 공격2, 2 = 공격3) 증가
    }

    void Attack2() // 광역 공격
    {
        flag = true;
        cAnim.SetBool("isMove", false);
        cAnim.SetTrigger("isAttack2"); // 공격2 공격 애니메이션 재생
        atk++;
    }

    void Attack3()
    {
        flag = true;
        cAnim.SetBool("isMove", false);
        cAnim.SetTrigger("isAttack3"); // 공격3 공격 애니메이션 재생(따로 힐과 관련된 애니메이션이 존재하지 않아, Idle모션으로 대체)
        hp = Mathf.Clamp(hp + attack, 0, maxHp); // 체력 초과 회복 방지
        hpBar.HpUpdate(); // 체력바 상태 업데이트
        atk = 0; // 공격 1 준비
    }

    void OnAttack1Trigger() // 공격1 애니메이션 트리거 동작 함수. 해당 트리거 삽입 위치에 따라 데미지가 들어가는 타이밍이 변동됨. 빠른 공격속도에 대응하기 위해, 애니메이션 트리거 타이밍을 수정하였음. 기존 0:15 -> 0:01
    {
        eBase.hp -= attack; // 현재 공격중인 적의 체력을 캐릭터 공격력 만큼 감소.
    }

    void OnAttack2Trigger() // 공격2 애니메이션 트리거 동작 함수.
    {
        Collider[] cols = Physics.OverlapSphere(eBase.transform.position, skillRange); // 공격2의 스킬 범위를 반지름으로 가지는 구 범위 만큼 공격 대상 주변 적 탐색. 원형 범위 몬스터에 피해이므로 Sphere(구) 사용.

        if(cols.Length > 0)
        {
            for(int i = 0; i < cols.Length; i++)
            {
                if(cols[i].tag == "Enemy")
                {
                    cols[i].gameObject.GetComponent<EnemyBase>().hp -= attack; // Enemy 태그를 가지고 있는 배열 내 메소드 모두에게 현재 체력 - 캐릭터 공격력 만큼의 체력 변동 발생.
                }
            }
        }
    }
}
