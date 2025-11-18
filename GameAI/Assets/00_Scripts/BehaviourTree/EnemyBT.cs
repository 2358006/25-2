using UnityEngine;
using System.Collections.Generic;

public class EnemyBT : MonoBehaviour
{
    public Transform[] waypoints;
    int waypointIndex = 0;

    BTNode root = null;              // BT 루트 노드 : 모든 Evaluate() 호출이 시작되는 진입점
    Animator animatorMonsterState = null;    // 애니메이터
    public Transform characterTarget = null; // 추적 대상

    public float chaseRange = 5.0f;         // 추적할 수 있는 거리 변수, 초기값은 5m
    public float attackRange = 1.5f;        // 공격할 수 있는 거리 변수, 추적변수와 초기값은 달라야 함. 초기값 1.5m
    public float monsterSpeed = 2.0f;       // 몬스터가 NPC(캐릭터) 추적할 스피드 값 저장 변수
    public float patrolRange = 5.0f;

    void Awake()
    {
        animatorMonsterState = GetComponent<Animator>();
    }

    void Start()
    {
        // Root : Selector
        root = new BTSelector
        (new List<BTNode>
        {
            new BTSequence(new List<BTNode>          // 공격 시퀀스  : [공격 범위?] -> [공격]
            {
               new BTLeaf(CheckPlayerAttackRange), // 공격 조건 Leaf
               new BTLeaf(AttackPlayer)              // 행동 Leaf
            }),
            new BTSequence(new List<BTNode>
            {
               new BTLeaf(CheckPlayerChaseRange), // 추적 조건 Leaf
               new BTLeaf(ChasePlayer)               // 행동 Leaf
            }),

            new BTSequence(new List<BTNode>
            {
               new BTLeaf(CheckPlayerPatrolRange), // 순찰 조건 Leaf
               new BTLeaf(PatrolPlayer)               // 행동 Leaf
            }),

            new BTLeaf(IdlePlayer)                   // 아무 조건도 충족하지 못하면 Idle
        });

    }

    // 입력받은 range 값과 플레이어와의 실제 거리를 비교하여 플레이어가 공격 범위 안에 있으면 Success, 밖에 있으면 Failure 반환
    BTNodeStatus CheckPlayerAttackRange() // 플레이어가 공격 범위 내에 있음?
    {
        float monsterCharacterDist = Vector3.Distance(transform.position, characterTarget.position);

        return (monsterCharacterDist <= attackRange) ? BTNodeStatus.Success : BTNodeStatus.Failure;
    }

    BTNodeStatus CheckPlayerChaseRange() // 플레이어가 추적 범위 내에 있음?
    {
        float monsterCharacterDist = Vector3.Distance(transform.position, characterTarget.position);

        return (monsterCharacterDist <= chaseRange) ? BTNodeStatus.Success : BTNodeStatus.Failure;
    }

    BTNodeStatus CheckPlayerPatrolRange() // 플레이어가 순찰 범위 내에 있음? 
    {
        // 거리보단 웨이포인트 위주로

        float monsterCharacterDist = Vector3.Distance(transform.position, characterTarget.position);
        return (monsterCharacterDist > patrolRange) ? BTNodeStatus.Success : BTNodeStatus.Failure;
    }

    BTNodeStatus IdlePlayer() // 대기
    {
        Rotate();
        MonsterAnimatorStateChange("IDLE");
        return BTNodeStatus.Success;
    }

    BTNodeStatus AttackPlayer() // 공격
    {
        Rotate();
        MonsterAnimatorStateChange("ATTACK");
        return BTNodeStatus.Success;
    }

    BTNodeStatus ChasePlayer() // 추적
    {
        transform.position = Vector3.MoveTowards(transform.position, characterTarget.position, Time.deltaTime * monsterSpeed);
        Rotate();
        MonsterAnimatorStateChange("CHASE");
        return BTNodeStatus.Running;
    }

    BTNodeStatus PatrolPlayer() // 순찰
    {
        MonsterPatrol();       // 실제 순찰 동작
        MonsterAnimatorStateChange("PATROL");
        return BTNodeStatus.Running;
    }

    void MonsterAnimatorStateChange(string state)
    {
        // 애니메이터 상태 초기화(false)
        animatorMonsterState.SetBool("IDLE", false);
        animatorMonsterState.SetBool("CHASE", false);
        animatorMonsterState.SetBool("ATTACK", false);
        animatorMonsterState.SetBool("PATROL", false);

        // 매개변수로 전달 받은 애니메이터만 true 로 변경
        animatorMonsterState.SetBool(state, true);

        Debug.Log($"{state} : {animatorMonsterState.GetBool(state)}");
    }

    void Rotate()
    {
        Vector3 vector3Direction = (characterTarget.position - transform.position).normalized;
        vector3Direction.y = 0.0f;
        transform.forward = vector3Direction;
    }

    void MonsterPatrol()
    {
        // 안전 체크: waypoints 세팅 확인
        if (waypoints == null || waypoints.Length == 0) { return; }

        Transform waypoint = waypoints[waypointIndex]; // 현재 웨이포인트 설정

        // 단순 이동 함수 (아래 MonsterMoveToWaypoint가 있으면 대체 가능)
        float step = monsterSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, waypoint.position, step);

        // 도착 판정
        if (Vector3.Distance(transform.position, waypoint.position) < 1.0f)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }

        // 회전(플레이어 추적용 Rotate가 있긴 하지만, 순찰 시에는 목표 방향으로 향하게)
        Vector3 dir = (waypoint.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f) { transform.forward = dir.normalized; }
    }

    // root.Evaluate()를 매 프레임 호출해 트리 갱신
    // Start()에서 설계한 행동 트리 전체가 최상위 노드부터 시작하여 매 프레임마다 자신의 상태를 평가하고 적절한 행동을 수행
    void Update()
    {
        // 루트의 Evaluate() 메서드를 호출하여 하위 로직을 일괄 수행
        // Sequence, Selector 노드가 AND/OR, Leaf 노드가 실제 동작을 수행하도록 Update
        root.Evaluate();
    }
}
