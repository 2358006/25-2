/*A* 알고리즘이란?
    A* 는 f(n) = g(n) + h(n) 공식을 기반으로 최적 경로를 찾는 탐색 알고리즘이다.
    - g(n) : 시작 노드에서 현재 노드 현재 노드까지의 실제 이동 비용 
    - h(n) : 현재 노드에서 목표 노드까지의 추정 비용 (휴리스틱)
    - 항상 f(n) 값이 가장 작은 노드를 먼저 탐색하여, 불필요한 확장을 줄이면서 목표 방향으로 빠르게 탐색한다.
    - 휴리스틱은 상/하/좌/우 격자에서 맨해튼 거리(|dx| + |dy|)를 h(n)으로 사용
*/

using UnityEngine;
using System.Collections.Generic; // List, Dictionary, HashSet 등 제네릭 자료구조 사용을 위해 필요하다.
using UnityEngine.UI;

public class AStarPathFinder : MonoBehaviour
{
    // 1 멤버 변수 정의
    GridManager gridManager = null; // 그리드 정보(맵 데이터, 벽/길 여부)를 제공하는 매니저

    [SerializeField] Vector2Int startLocation = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endLocation = new Vector2Int(19, 19);

    int aStarSearchCount = 0; // A* 탐색 중 실제로 꺼낸(Dequeue)된 노드 수
    Text aStarSearchCountText = null; // 탐색한 노드 수를 화면에 표시할 텍스트

    // 탐색 방향 : 상, 하, 좌, 우 네 방향으로 이동
    //    - 현재 타일에서 4방향(상/하/좌/우) 을 한번에 처리하기 위한 배열
    //    - 이 배열 덕분에 foreach를 사용해서 이웃 칸을 간단하게 탐색 할 수 있다.
    Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.down,  // 하
        Vector2Int.up,    // 상
        Vector2Int.left,  // 좌
        Vector2Int.right  // 우
    };

    // 2 초기화(Awake)
    void Awake()
    {
        gridManager = GetComponent<GridManager>(); // 같은 GameObject에 붙어 있는 GridManager 컴포넌트를 가져온다.
        aStarSearchCountText = GameObject.Find("AStar").GetComponent<Text>();
    }

    // ③ A* 메인 로직 : 경로 계산 
    //    - A* 알고리즘을 사용하여 startLocation → endLocation까지의 경로를 계산한다.
    //    - 경로가 존재하면 타일 좌표 리스트를 반환하고, 없으면 null을 반환한다.
    public List<Vector2Int> GetAStarPath()
    {
        // A* 오픈 리스트 : 아직 확정되지 않은 후보 노드들
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();

        // 각 노드의 "부모 노드"를 기록 (경로 복원용)
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // gScore[n] : 시작점에서 n까지의 실제 이동 비용
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();

        // fScore[n] : f(n) = g(n) + h(n) (총 예상 비용)
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        // 이미 최적 경로로 확정된 노드 집합
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        // 시작 노드 초기값 설정 : f(n) = g(n) + h(n)
        gScore[startLocation] = 0;
        fScore[startLocation] = gScore[startLocation] + Heuristic(startLocation, endLocation);

        // 시작 노드를 오픈 리스트에 등록 (우선순위 = fScore)
        openSet.Enqueue(startLocation, fScore[startLocation]);

        // 탐색 카운트 리셋
        aStarSearchCount = 0;

        // 오픈 리스트가 빌 때까지 반복
        while (openSet.Count > 0)
        {
            // fScore가 가장 작은 노드를 꺼낸다.
            Vector2Int current = openSet.Dequeue();

            // 탐색 카운트 누적
            aStarSearchCount++;

            // 도착 지점에 도달한 경우 → 경로 복원 후 반환
            if (current == endLocation)
            {
                // 탐색한 노드 수를 UI에 표시
                if (aStarSearchCountText != null)
                {
                    aStarSearchCountText.text = "A Star: " + aStarSearchCount.ToString();
                }

                return ReconstructPath(cameFrom, current);
            }

            // 현재 노드는 확정 집합에 추가
            closedSet.Add(current);

            // 상/하/좌/우 이웃 노드들 검사
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;

                // 그리드 밖이거나, 이미 확정된 노드면 건너뛴다.
                if (!IsValid(neighbor) || closedSet.Contains(neighbor)) { continue; }

                // current를 거쳐서 neighbor로 이동했을 때의 gScore 후보 값
                int tentative_gScore = gScore[current] + 1;  // 인접 타일까지 비용 1

                // 더 나은 경로인지 검사
                //   - 아직 gScore 정보가 없거나
                //   - 기존 gScore보다 더 작은 값인 경우에만 갱신
                bool isBetterPath =
                    !gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor];

                if (isBetterPath)
                {
                    // neighbor로 오는 최적의 경로는 current를 통해 왔다고 기록
                    cameFrom[neighbor] = current;

                    // gScore, fScore 갱신
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = tentative_gScore + Heuristic(neighbor, endLocation);

                    // 오픈 리스트에 후보로 추가 (우선순위 = fScore)
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        // 오픈 리스트가 모두 빌 때까지 도착 지점에 도달하지 못했다면 → 경로 없음
        return null;
    }

    // 4. 휴리스틱 계산 : 맨해튼 거리
    //      - 두 좌표 a,b 사이의 맨해튼(|dx| + |dy|) 거리 를 계산한다.
    //      - 상/하/좌/우로만 이동할 수 있는 격자에서 최소 이동 칸수를 의미한다.
    int Heuristic(Vector2Int a, Vector2Int b)
    {
        int deltaX = Mathf.Abs(a.x - b.x);
        int deltaY = Mathf.Abs(a.y - b.y);

        return deltaX + deltaY;
    }

    // ⑤ 경로 복원 : cameFrom을 이용해 시작→도착 경로 만들기 
    //     - cameFrom 정보를 이용하여 end 지점에서 시작 지점까지 역추적한 뒤,
    //       "시작 → 도착" 순서의 경로 리스트를 만들어 반환한다.
    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        // current의 부모가 존재하는 동안 계속 거슬러 올라간다.
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }

        // 마지막으로 시작 지점(current)을 추가
        path.Add(current);

        // 현재 리스트는 [도착 → ... → 시작] 순서이므로 뒤집어서 반환
        path.Reverse();

        return path;
    }

    // ⑥ 좌표 유효성 검사 : 맵 범위 + 이동 가능 여부 
    //     - 주어진 좌표가 그리드 내부이며, 이동 가능한 타일인지 검사한다.
    bool IsValid(Vector2Int pos)
    {
        // 그리드 범위 안인지 확인
        if (!gridManager.IsInside(pos)) { return false; }

        // 해당 위치가 이동 가능한 타일인지(벽이 아닌지) 확인
        return gridManager.IsWalkable(pos);
    }
}