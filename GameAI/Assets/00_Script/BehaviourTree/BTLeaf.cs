public class BTLeaf : BTNode
{
    System.Func<BTNodeStatus> action;

    public BTLeaf(System.Func<BTNodeStatus> action)
    {
        this.action = action;
    }

    public override BTNodeStatus Evaluate()
    {
        return action();
    }
}
