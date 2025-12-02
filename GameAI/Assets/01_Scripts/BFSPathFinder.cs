/* 
1. BFS란?
BFS는 그래프나 격자에서 시작점으로 가까운 노드 or 셀부터 차례대로 탐색하는 방식
큐 자료구조를 사용하여, 현재 레벨의 모든 노드를 확인한 뒤 그 다음 레벨로 넘어감
DFS가 "깊게 파고드는 방식이면", BFS는 "넓게 퍼져가나가는" 탐색을 함

2. Why BFS?
최단 경로 보장
BFS는 시작점에서 목표 지점까지 가는 가장 짧은 경로를 보장한다.
시작점에서 1, 2, 3칸 씩 차례대로 확장 하므로 탐색의 범위가 원형으로 퍼져나가는 구조를 직관적으로 볼 수 있음
미로에서 최단 탈출 경로 찾기
NPC가 가장 빠르게 플레이어에게 도달하는 AI 구현
*/

using System.Collections.Generic;
using UnityEngine;

public class BFSPathFinder : MonoBehaviour // BFS 알고리즘으로 시작점에서 도착점까지의 경로를 찾는 클래스
{
    GridManager gridManager = null; // 그리드 경계 / 통로 여부 / 타일 정보 조회

    // BFS 탐색을 위한 시작, 도착 지점 설정
    [SerializeField] Vector2Int startLocation = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endLocation = new Vector2Int(4, 4);

    // 탐색횟수 카운트
    public int expandCount;
    public int discoverCount;

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    public List<Vector2Int> GetBFSPath()
    {
        // 시작할 때 초기화
        expandCount = 0;
        discoverCount = 0;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(startLocation);
        visited.Add(startLocation);
        discoverCount++;   // 시작점도 발견한 노드 1개로 계산

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            expandCount++;   // 실제로 확장한 노드 수 증가

            if (current == endLocation) { return ReconstructPath(cameFrom, endLocation); }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsVaild(next) && !visited.Contains(next))
                {
                    queue.Enqueue(next); //큐에 추가
                    visited.Add(next); // 방문처리 
                    cameFrom[next] = current; // next의 부모는 current

                    discoverCount++;   // 새로 방문한 노드 수 증가
                }
            }
        }

        Debug.Log($"Expand: {expandCount}, Discover: {discoverCount}");
        return null; // 경로를 찾지 못한 경우 null 반환
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Vector2Int current = end;

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current); //지금칸을 경로에 추가
            current = cameFrom[current]; // 한 칸 이전 위치(부모)로 이동
        }

        path.Add(current);


        path.Reverse();

        return path;
    }

    bool IsVaild(Vector2Int pos)
    {
        if (!gridManager.IsInside(pos)) { return false; }
        return gridManager.IsWalkable(pos);
    }
}
