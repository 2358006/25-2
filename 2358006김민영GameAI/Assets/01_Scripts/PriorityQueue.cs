using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class PriorityQueue<TItem>
{
    // 1. 멤버변수 정의
    List<(TItem item, int priority)> listElements = new List<(TItem item, int priority)>();

    // 2. 현재 큐 안에 저장된 요소의 개수 관리하는 메소드
    public int Count
    {
        get { return listElements.Count; }
    }
    // 3. 새로운 요소를 우선순위와 함꼐 큐에 추가하는 메소드
    public void Enqueue(TItem newItem, int newPriority)
    {
        listElements.Add((newItem, newPriority));
    }

    // 4. 큐 안에 우선순위가 가장 낮은 요소를 찾아 꺼낸 뒤
    public TItem Dequeue()
    {
        int bestInedx = 0;

        for (int i = 1; i < listElements.Count; i++)
        {
            if (listElements[i].priority < listElements[bestInedx].priority)
            {
                bestInedx = i;
            }
        }

        TItem bestItem = listElements[bestInedx].item;
        listElements.RemoveAt(bestInedx);
        return bestItem;
    }

}
