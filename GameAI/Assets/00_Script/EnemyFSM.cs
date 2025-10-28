using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

/*
1. FSM (Finite State Machine, 유한상태머신)
   - 한 번에 하나의 상태만 가지며, 특정 조건에 따라 다른 상태로 전환되는 구조
     · 상태(State) : Idle, Attack, Patrol, Chase, Die
        public Enum State
        {
            Idle
            Attack
            Patrol
            Chase
            Die
        };
     · Transition : 상태 변경 조건(거리, 시간, 이벤트 등)
     · Entry / Exit : 상태 진입 시 동작 / 상태 탈출 시 동작
     · Update : 상태 지속 중 반복 처리(예, 추격 중 이동)

2. FSM을 사용하는 이유
    - 상태 수정이 용이
     · 디자이너나 혹은 기획자 분들이 이 상태를 수정을 하고 싶을 때 좀 더 용이하게 수정을 할 수 있게끔 처리를 해 줄 수 있는 방법
    - 간단하면서도 명확한 딕셔너리 구조
     · 업데이트 라이프 사이클 루프에서 현재 상태에 맞는 행동을 실행을 해주면서 조건에 따라서 각각의 상태를 전환할 수 있도록 처리함
    - 테스트 디버깅 용이
    - 상태 별 책임 분리가 쉬움
    - 모든 AI 로직의 기반
     · FSM은 AI 캐릭터가 어떤 상태에 있는지. 그리고 언제 어떻게 상태를바꾸는지를 설계하는 틀
     · FSM은 계산 모델이자, 제어 흐름을 정의하는 알고리즘 구조로 간주될 수 있음
     · 게임 개발 관점 : FSM은 설계 구조(디자인 구조, 상태 처리 프레임워크)

3. FSM 단점
    - 상태 수가 많아질수록 복잡도가 급격히 증가
     · 예를들어, 상태가 3~4개일 때 문제가 없음, 상태가 10개 이상이라면 상태 전이 조건을 일일이 관리하기 어렵다.
     · n개의 상태가 있으면 전이 조건은 최대 n² 개가 될 수 있음
     · 그러므로 상태폭발(State Explosion) 문제가 있음
    - 행동이 고정적이라 유연성이 부족함
     · 상태마다 동작이 고정되어 있으므로 상황에 따라 AI가 전략적으로 판단하는 행동을 만들기가 어렵다
      (예, 체력이 50% 이하면 도망 이것을 여러 상태에서 구현하면 -> 중복 코드, 설계가 어렵다)
    - 조건 분기가 많아질수록 관리가 힘들다
     · 조건문이 뒤엉키기 시작하면서 디버깅이 매우 어려워지고 상태간의 연결관계 시각화도 상당히 어려워짐
    - 재사용성과 확장성이 낮음
     · 상태가 늘어날수록 기존 코드와 강하게 결합이 되어 있어서 새로운 상태를 넣거나 뺄 때 전체 구조를 뜯어 고쳐야 할 수도 있음
    - FSM은 간단하고 직관적인 구조이지만, 상태가 많아질수록 전위 조건과 분기 처리가 굉장히 복잡해지고
    AI가 복잡한 전략을 판단하는 데는 한계가 있기 때문에 이후에 사용하는 BT(행동트리), GOAP(목표 지향적 행동 계획) 같은 새로운 AI 알고리즘이 등장했음
*/

public class EnemyFSM : MonoBehaviour
{
    public enum State
    {
        Idle,  // 대기 : 대기화면 상태
        Chase, // 추적
        Patrol, // 순찰
        Attack // 공격
    }
    public State MonsterCurrentState = State.Idle; // 몬스터 현재 상태 변수 선언, 초기값은 'Idle(대기)'로 설정
    Animator animatorMonsterState;                 // 상태별 애니메이터

    // 몬스터가 NPC(캐릭터) 추적할 스피드 값 저장 변수
    public float fMonsterSpeed = 3.0f;       // 몬스터가 NPC(캐릭터) 추적할 스피드 값 저장 변수

    [SerializeField]
    private float idleStateTimer = 0f;

    private bool isPatrolling = false;


    public Transform characterTarget = null; // 캐릭터 목표대상 오브젝트 변수
    public float fChaseRange = 5.0f;         // 추적할 수 있는 거리 변수, 초기값은 5m
    public float fAttackRange = 1.5f;        // 공격할 수 있는 거리 변수, 추적변수와 초기값은 달라야 함. 초기값 1.5m



    void Start()
    {


        animatorMonsterState = GetComponent<Animator>();
    }

    void Update()
    {

        switch (MonsterCurrentState)
        {
            case State.Idle:    // 대기상태
                f_MonsterIdleState();
                break;

            case State.Chase:   // 추적
                f_MonsterChaseState();
                break;

            case State.Patrol:
                f_MonsterPatrolState();
                break;

            case State.Attack:  // 공격
                f_MonsterAttackState();
                break;
        }

        f_MonsterTransitionCheck(); // 몬스터 상태전환 체크 매서드 호출
    }




    private void f_MonsterIdleState()       // 대기상태 처리 매서드
    {
        MonsterAnimatorStateChange("IDLE");
        
        
    }


    private void f_MonsterPatrolState()       // 활보 상태
    {
        MonsterAnimatorStateChange("CHASE");
        //랜덤 방향 이동
        if (!isPatrolling)
        {
            StartCoroutine("PatrolMove");
        }
    }



    private void f_MonsterChaseState()     // 추격상태 처리 매서드
    {
        transform.position = Vector3.MoveTowards(transform.position, characterTarget.position, Time.deltaTime * fMonsterSpeed);

        f_MonsterRotate(); // 몬스터가 NPC를 볼 수 있도록 Rotation 변경

        MonsterAnimatorStateChange("CHASE");
    }

    private void f_MonsterAttackState()    // 공격상태 처리 매서드
    {
        MonsterAnimatorStateChange("ATTACK");
    }

    // 몬스터 Animator 상태 변경 매서드
    private void MonsterAnimatorStateChange(string strState)
    {
        // 애니메이터상태 false 로 초기화
        animatorMonsterState.SetBool("IDLE", false);
        animatorMonsterState.SetBool("CHASE", false);
        animatorMonsterState.SetBool("ATTACK", false);

        // 매개변수로 전달 받은 애니메이터 상태만 true 로 변경
        animatorMonsterState.SetBool(strState, true);

    }

    // FSM 상태 전환 체크 매서드
    private void f_MonsterTransitionCheck()
    {
        // 몬스터에서 공격 물체까지의 거리 계산
        float fDistance = Vector3.Distance(transform.position, characterTarget.position);

        if (fDistance <= fAttackRange) // 몬스터에서 캐릭터거리가 공격 범위에 들어오면 공격 상태 전환
        {
            MonsterCurrentState = State.Attack;
        }
        else if (fDistance <= fChaseRange) // 몬스터에서 캐릭터 거리가 추적 범위에 들어오면 추적 상태 전환
        {
            MonsterCurrentState = State.Chase;
        }
        else // 그렇지 않으면 애니메이션 상태를 대기로 전환
        {
            // Patrol 상태일 땐 상태 전환을 막는다
            if (MonsterCurrentState == State.Patrol)
            return;

            if (MonsterCurrentState == State.Idle)
            {

                idleStateTimer += Time.deltaTime;
                if (idleStateTimer >= 3f)
                {
                    MonsterCurrentState = State.Patrol;
                    return;
                }
            }
            else
            {
                MonsterCurrentState = State.Idle;
            }


        }
        
    }

    

    private void f_MonsterRotate()
    {
        Vector3 vectorDirection = (characterTarget.position - transform.position).normalized;
        vectorDirection.y = 0.0f;

        transform.forward = vectorDirection;
    }




    IEnumerator PatrolMove()
    {
        
        isPatrolling = true;
        float fRandDir = Random.Range(0, 360f);
        transform.rotation = Quaternion.Euler(0f, fRandDir, 0f);

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * fMonsterSpeed;


        // 일정 시간 동안 앞으로 이동
        
        float elapsedTime = 0f;    //이동하는 현재 시간
        float moveDuration = 1.5f; // 이동 시간

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fMonsterSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        MonsterCurrentState = State.Idle;
        idleStateTimer = 0f;
        isPatrolling = false;
    }
}
