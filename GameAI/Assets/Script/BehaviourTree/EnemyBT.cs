using UnityEngine;
using System.Collections.Generic;

public class EnemyBT : MonoBehaviour
{
    BTNode root;
    Animator animatorMonsterState;
    public Transform characterTarget;

    float monsterSpeed = 2f;
    float chaseRange = 5.0f;
    float attackRange = 1.5f;

    /*
        루트 = Selector
        자식 1 : 공격 시퀀스 = [공격 범위?] > [공격]
        자식 2 : 추적 시퀀스 = [추적 범위?] > [추적]
        자식 3 : 대기 시퀀스 = [기본상태] > idle(리프)
        우선순위는 리스트 순서로 구현(앞에 있을수록 먼저 평가)
    */

    void Awake()
    {
        animatorMonsterState = GetComponent<Animator>();
    }

    void Start()
    {
        root = new BTSelector
        (new List<BTNode>
        {
new BTSequence(new List<BTNode>
{
    new BTLeaf(CheckPlayerAttackRange),
    new BTLeaf(AttackPlayer)
}),

new BTSequence(new List<BTNode>
{
    new BTLeaf(CheckPlayerChaseRange),
    new BTLeaf(ChasePlayer)
}),
    new BTLeaf(IdlePlayer)


        });
    }

    BTNodeStatus CheckPlayerAttackRange()
    {
        float monsterCharacterDist = Vector3.Distance(transform.position, characterTarget.position);
        return (monsterCharacterDist < attackRange) ? BTNodeStatus.Success : BTNodeStatus.Failure;
    }

    BTNodeStatus CheckPlayerChaseRange()
    {
        float monsterCharacterDist = Vector3.Distance(transform.position, characterTarget.position);
        return (monsterCharacterDist < chaseRange) ? BTNodeStatus.Success : BTNodeStatus.Failure;
    }

    BTNodeStatus IdlePlayer()
    {
        return BTNodeStatus.Success;
    }

    BTNodeStatus AttackPlayer()
    {
        return BTNodeStatus.Running;
    }

    BTNodeStatus ChasePlayer()
    {
        return BTNodeStatus.Running;
    }

    void MonsterAnimationStateChange(string state)
    {
        animatorMonsterState.SetBool("IDLE", false);
        // animatorMonsterState.SetBool("PATROL", false);
        animatorMonsterState.SetBool("CHASE", false);
        animatorMonsterState.SetBool("ATTACK", false);

        animatorMonsterState.SetBool(state, true);
    }
}
