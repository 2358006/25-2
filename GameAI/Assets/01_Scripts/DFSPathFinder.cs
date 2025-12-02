using System.Collections.Generic;
using UnityEngine;

public class DFSPathFinder : MonoBehaviour
{
    GridManager gridManager = null; // 그리드 경계 / 통로 여부 / 타일 정보 조회

    // DFS 탐색을 위한 시작, 도착 지점 설정
    [SerializeField] Vector2Int startLocation = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endLocation = new Vector2Int(4, 4);

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    // 시작 / 도착 좌표를 코드로 변경
    // 필요시 다른 시작, 도착 지점 동적으로 변경 가능
    // start : 시작점, end : 도착점
    public void SetStartLocation(Vector2Int start) // 시작지점 설정
    {
        startLocation = start;
    }


    public void SetEndLocation(Vector2Int end) // 도착지점 설정
    {
        endLocation = end;
    }

    // DFS 탐색 실행 메소드
    //  - HashSet을 사용하여 방문한 좌표를 기록 → 중복 방문 방지 + 빠른 탐색
    //  - 최종적으로 경로 리스트(List<Vector2Int>)를 반환
    public List<Vector2Int> GetDFSPath()
    {
        // [HashSet] : 중복된 값을 허용하지 않는 집합

        HashSet<Vector2Int> visitedLocation = new HashSet<Vector2Int>();
        return DFS(startLocation, endLocation, visitedLocation);
    }


    /* DFS 재귀 탐색 로직 만들기
    [탐색 순서]
    현재 좌표가 유효한지 검사 (f_IsValid) > 
    이미 방문한 좌표라면 중단 > 
    현재 좌표가 도착점이면 리스트에 담아 반환 > 
    상·하·좌·우 순서로 재귀 탐색 진행 > 
    경로 발견 시 현재 좌표를 맨 앞에 추가
    */
    List<Vector2Int> DFS(Vector2Int current, Vector2Int end, HashSet<Vector2Int> visited)
    {
        if (!IsValid(current) || visited.Contains(current))
        {
            return null;
        }

        visited.Add(current); //현재 좌표를 방문 목록에 등록해 중복 방문을 방지한다.

        if (current == end) //도착지점에 도달했다면
        {
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

        /*
         * 각 방향으로 한 칸 이동해 다음 좌표(vNeighbor)를 계산
         * 그 좌표를 시작점으로 재귀적으로 f_DFS 호출
         */
        foreach (Vector2Int dir in vDirections)
        {
            //현재 위치에서 dir 방향으로 한 칸 이동한 이웃 좌표
            Vector2Int neighbor = current + dir;

            /*
             * 이웃 좌표에서 도착지점까지의 경로를 탐색
             * - 경로가 발견되면 현재 좌표를 경로의 맨 앞에 추가하고 반환
             * - path에는 vNeighbor → ... → end 순서로 좌표가 들어있다
             * 경로가 없으면 null이 반환되고, 다음 방향을 탐색한다
             */
            List<Vector2Int> path = DFS(neighbor, end, visited);

            /*
             * 경로가 발견되었다면, 현재 좌표를 경로의 맨 앞에 추가하고 반환
             * current → vNeighbor → ... → end 순서로 경로가 구성된다
             * 경로가 하나라도 발견되면 즉시 반환하므로, 가장 먼저 발견된 경로가 선택된다(DFS 특성)
             */
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
