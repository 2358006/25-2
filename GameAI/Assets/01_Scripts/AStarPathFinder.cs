using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class AStarPathFinder : MonoBehaviour
{
    GridManager gridManager = null;

    [SerializeField] Vector2Int startLocation = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endLocation = new Vector2Int(19, 19);

    int aStarSearchCount = 0;
    Text aStarSearchCountText = null;

    Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.down,
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right
    };

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
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
                if (aStarSearchCountText != null) { aStarSearchCountText.text = "AStar: " + aStarSearchCount.ToString(); }
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

    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return 1;
    }

    // ⑤ 경로 복원 : cameFrom을 이용해 시작→도착 경로 만들기 
    //     - cameFrom 정보를 이용하여 end 지점에서 시작 지점까지 역추적한 뒤,
    //       "시작 → 도착" 순서의 경로 리스트를 만들어 반환한다.
    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // current를 도착 좌표(end)로 설정
        Vector2Int current = end;

        // end에서 시작 위치까지 부모를 따라 거슬러 올라감
        // 현재 칸이 cameFrom에 Key로 존재하는 동안 반복
        // 즉. " 이 칸이 어디에서 왔는지 기록이 남아 있는 동안 " 반복
        // while이 끝나면, 더 이상 부모 정보가 없는 칸에 도착 → 보통 시작 칸
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current); // 지금 칸을 경로에 추가

            current = cameFrom[current]; // 한 칸 이전 위치(부모)로 이동
        }

        // 마지막으로 시작 위치 추가
        // 이 시점에서 path는 end → ... → start 순서로 거꾸로 쌓여 있으므로
        path.Add(current); // 시작 지점도 경로에 추가

        // 현재는 "도착 → 시작" 순서이므로 반대로 뒤집음
        // 리스트를 뒤집이서 start → ... → end 순서로 정렬한다.
        path.Reverse();

        // 이 리스트를 반환하면, BFSVisualizer가 이 좌표들을 순서대로 읽으면서 타일 색을 바꿔줄 수 있다.
        return path;

    }

    // IsValid : 특정 좌표가 그리드 안 + 이동 가능한 칸인지 검사하는 메소드
    // 좌표가 그리드 내부이며, 이동 가능한 셀인지 검사하는 메소드

    bool IsValid(Vector2Int pos)
    {
        if (!gridManager.IsInside(pos)) //그리드 내부인지 확인
        {
            return false;
        }

        return gridManager.IsWalkable(pos); //이동 가능한 셀인지 확인(벽이 아니여야 함)
    }
}
