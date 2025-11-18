public enum BTNodeStatus
{
    Success, // 성공 : 상위노드가 다음 단계로 넘어갈 상태
    Failure, // 실패 : 상위 노드에서 다른 분기를 시도하게 하는 상태
    Running  // 진행중 : 다음 프레임 에서도 계속해서 이 노드를 다시 평가해야함을 나타내는 상태
}

public abstract class BTNode
{
    // 현재 노드의 로직을 1프레임 동안 수행하고, 결과를 반환하는 메서드
    public abstract BTNodeStatus Evaluate();

}
