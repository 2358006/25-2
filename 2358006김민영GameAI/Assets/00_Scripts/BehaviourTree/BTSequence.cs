using System.Collections.Generic;
public class BTSequence : BTNode
{
    List<BTNode> listChildren; // Sequence가 관리할 자식 노드들을 담는 리스트

    // 생성자 : 외부에서 자식 노드 리스트를 받아 내부에 저장
    public BTSequence(List<BTNode> arglstChildren)
    {
        this.listChildren = arglstChildren;
    }

    public override BTNodeStatus Evaluate()
    {
        foreach (BTNode node in listChildren)
        {
            BTNodeStatus status = node.Evaluate();

            if (status == BTNodeStatus.Failure) // 하나라도 실패했으면 Sequence는 즉시 실패
            {
                return BTNodeStatus.Failure;
            }

            if (status == BTNodeStatus.Running) // 어떤 자식이든 진행중이라면, 아직 진행중 (다음 프레임에서 다시 평가 진행)
            {
                return BTNodeStatus.Running;
            }
        }

        return BTNodeStatus.Success; // 위의 반복에서 성공 / 진행중을 만나지 못하면 모든 자식이 실패
    }
}