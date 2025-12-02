using System.Collections.Generic;
using UnityEngine;

public class DFSPathFinder : MonoBehaviour
{
    GridManager gridManager = null; // 그리드 경계 / 통로 여부 / 타일 정보 조회

    // DFS 탐색을 위한 시작, 도착 지점 설정
    [SerializeField] Vector2Int startLocation = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endLocation = new Vector2Int(4, 4);

    // 탐색 횟수 카운트
    public int expandCount;
    public int discoverCount;

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    public void SetStartLocation(Vector2Int start) // 시작지점 설정
    {
        startLocation = start;
    }

    public void SetEndLocation(Vector2Int end) // 도착지점 설정
    {
        endLocation = end;
    }

    public List<Vector2Int> GetDFSPath()
    {
        expandCount = 0;
        discoverCount = 0;

        // [HashSet] : 중복된 값을 허용하지 않는 집합
        HashSet<Vector2Int> visitedLocation = new HashSet<Vector2Int>();
        return DFS(startLocation, endLocation, visitedLocation);
    }

    List<Vector2Int> DFS(Vector2Int current, Vector2Int end, HashSet<Vector2Int> visited)
    {
        if (!IsValid(current) || visited.Contains(current))
        {
            return null;
        }

        visited.Add(current); //현재 좌표를 방문 목록에 등록해 중복 방문을 방지한다.
        expandCount++;  // 실제로 방문한 노드 수 증가

        if (current == end) //도착지점에 도달했다면
        {
            Debug.Log($"DFS Expand: {expandCount}, Discover: {discoverCount}");
            return new List<Vector2Int> { current };
        }

        /*
         * 상하좌우 네 방향을 순서대로 탐색
         * 이 순서가 바뀌면 DFS 특성상 "처음 발견되는 경로"가 달라질 수 있다.
         */
        Vector2Int[] vDirections = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in vDirections)
        {
            Vector2Int neighbor = current + dir; //현재 위치에서 dir 방향으로 한 칸 이동한 이웃 좌표

            if (!visited.Contains(neighbor) && IsValid(neighbor))
            {
                discoverCount++;  // ⭐ 새로 발견한 노드 수 증가
            }

            List<Vector2Int> path = DFS(neighbor, end, visited);


            if (path != null)
            {
                path.Insert(0, current);
                return path;
            }
        }
        return null;
    }

    bool IsValid(Vector2Int pos)
    {
        if (!gridManager.IsInside(pos)) //그리드 내부인지 확인
        {
            return false;
        }

        return gridManager.IsWalkable(pos); //이동 가능한 셀인지 확인(벽이 아니여야 함)
    }
}
