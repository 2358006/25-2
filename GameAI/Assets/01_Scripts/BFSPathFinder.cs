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

// BFS 알고리즘으로 시작점에서 도착점까지의 경로를 찾는 클래스
public class BFSPathFinder : MonoBehaviour
{
    GridManager gridManager = null; // 그리드 경계 / 통로 여부 / 타일 정보 조회

    // BFS 탐색을 위한 시작, 도착 지점 설정
    [SerializeField] Vector2Int startLocation = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endLocation = new Vector2Int(4, 4);

    // 유니티 실행주기 함수중 하나로, 게임이 시작 할때, 컴포넌트가 준비되는 순간 한번만 호출됨
    // GetComponent<GridManager>() 는 같은 게임 옵제 에 붙어있는 그리드 매니저 컴포넌트 찾아오는 메소드
    // 빈 오브젝트인 그리드루트에 그리드매니저와 패스파인더를 같이 붙였기에 이 한줄의 코드로 오브젝트의 해당 기능 추가 할수 있음
    void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    // BFS로 실제 경로를 찾는 메소드
    // BFS알고리즘 이용해 시작점 > 도착점 까지 경로 찾음
    // 시작 > 도착 점까지 최단 경로를 List<Vector2Int> 로 반환, 없으면 null 반환
    public List<Vector2Int> GetBFSPath()
    {
        // BFS에 필요한 자료구조 만들기

        // 방문 집합 : 이미 방문한 노드 기록
        // visited
        //  - 이미 체크해본 타일목록
        //  - 같은 곳을 여러번 보지 않게 해서 무한루프 방지 및 성능 확보
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // 부모 추적 : 특정 노드의 직전 위치 기록 > 경로 복원할 떄 사용
        // cameFrom 
        //  - "어떤 타일이, 어디 타일에서 왔는지" 체인정보 저장
        //  - ex. (2, 1)은 (1, 1)서 왔다.
        //  - 나중에 도착지점에서 거꾸로 따라가며 경로 복원 할때 사용 
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // 탐색 큐 : BFS 핵심 자료구조, FIFO 원칙으로 노드 탐색
        // Queue
        // - BFS의 핵심
        // - 먼저 들어온 좌표부터 꺼내 처리하는 줄서기 자료구조
        // - 초기 설정 : queue.enqueue(startLocation);
        //  > 시작 조교포를 제일 처음 탐색 대상으로 줄세운다.
        // - visited.Add(startLocation)
        //  > 시작 좌표는 이미 방분했다 표시
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 초기 설정 : 시작점을 큐에 넣고 방문 처리
        queue.Enqueue(startLocation);
        visited.Add(startLocation);

        // 탐색 방향 : 상화좌우 네 방향으로 이동
        // -현재 타일에서 4방향을 한번에 처리하기 위한 배열
        // - 이 배열덕에 foreach를 사용해 이웃칸을 간단하게 탐색 가능
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            // 1. current 꺼내기 : 현재 노드 큐에서 꺼낸 가장 오래된 노드
            // - Vector2Int current = queue.Dequeue();
            // - 큐에서 가장 먼저 들어왔던 자표를 꺼낸다.
            // - BFS는 "가까운 거리 순서대로" 큐에 쌓이기 때문에 항상 거리 1 > 2 > 3 순서로 탐색해 나간다.
            Vector2Int current = queue.Dequeue();

            // 2. 도착 지점에 도달했는지 확인
            // - 종료 조건 : 도착지점에 도달하면 경로 복원
            // - 현재 칸이 우리가 찾고 있던 도착좌표라면, 더이상 탐색할 필요없이 바로 경로 복원 단계로 넘어감
            // - ReconstructPath는 "출구에 도착했으니 이제 지나온 길을 역으로 따라가 경로 리스트 만들어라"는 의미
            if (current == endLocation) { return ReconstructPath(cameFrom, endLocation); }

            // 3. 이웃탐색 : 상하좌우 4방향
            // - BFS알고리즘의 탐색 실행
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsVaild(next) && !visited.Contains(next))
                {
                    queue.Enqueue(next); //큐에 추가
                    visited.Add(next); // 방문처리 
                    cameFrom[next] = current; // next의 부모는 current
                }
            }
        }

        // 4. 큐가 공백일 때 까지 도착점을 못찾은 경우
        // - while (queue.Count > 0) 루프가 끝났다는 것은, 더이상 탐색할 좌표가 없는데도 도착지점에 도달하지 못한것을 의미함
        // - 이때 return null;로 "갈수 있는 길이 없다"는 결과를 돌려주어야함
        return null; // 경로를 찾지 못한 경우 null 반환
    }

    // BFS탐색 결과를 이용해 "실제 경로 리스트"를 만드는 메소드
    // 부모 추적 정보를 사용해 시작점에서 도착점 까지의 경로를 복원하는 메소드
    // Dictionary로 부모 정보를 저장하는 이유는
    //     - cameFrom Dictionary 에는 Key : 자식노드 좌표, value : 부모노드 좌표 형태로 
    //                                     "어떤칸이 어디서 왔는지"가 줄줄이 연결된 상태로 저장됨
    //     - Ex. (1, 0)은 (0, 0)서 왔고, 
    //           (2, 0)은 (1, 0)서 왔고, 
    //           (2, 1)은 (2, 0)서 왔다...
    //     이런 식으로 체인처럼 이어져 있음
    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        //current를 도착좌표로 설정
        Vector2Int current = end;

        // end에서 시작위치까지 부모를 따라 거슬러 올라감
        // 현재 칸이 cameFrom에 Key로 존재하는 동안 반복
        // 즉, "이 칸이 어디서 왔는지 기록이 남아있는 동안" 반복
        // while이 끝나면 더이상 부모 정보가 없는 칸에 도착 > 보통 시작 칸
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current); //지금칸을 경로에 추가
            current = cameFrom[current]; // 한 칸 이전 위치(부모)로 이동
        }

        // 마지막으로 시작 위치 추가
        // 이 시점에서 path는 end > ... > start 순서로 거꾸로 쌓여있음
        path.Add(current);

        // 현재는 도착> 시작 순서라서 반대로 뒤집음
        // 리스트를 뒤집어 start > ... > end 순서로 정렬
        path.Reverse();

        // 이 리스트를 반환하면 BFSVisualizer가 이 좌표들을 순서대로 읽으면서 타일색을 바꿔줄 수 있음
        return path;
    }

    // 특정 좌표가 그리드 안 + 이동 가능한 칸인지 검사하는 메소드
    bool IsVaild(Vector2Int pos)
    {
        if (!gridManager.IsInside(pos)) { return false; }
        return gridManager.IsWalkable(pos);
    }
}
