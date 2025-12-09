using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class AStarVisualizer : MonoBehaviour
{
    AStarPathFinder aStarPathFinder = null;
    GridManager gridManager = null;
    [SerializeField]
    float stepDelaySeconds = 0.5f;

    void Awake()
    {
        aStarPathFinder = GetComponent<AStarPathFinder>();
        gridManager = GetComponent<GridManager>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame)
        {
            StartCoroutine(ShowAStarPath());
        }
    }

    // ShowBFSPath 코루틴 : 경로를 한 칸씩 색칠하는 시각화 로직
    IEnumerator ShowAStarPath()
    {
        // BFSPathFinder에서 경로 가져오기
        // GetBFSPath() 는 시작 지점에서 도착 지점까지의 최단 경로를 List<Vector2Int> 형태로 반환합니다.
        // 이 path 리스트 안에는 예를 들어, (0,0) → (0,1) → (1,1) → ... → (19,19) 과 같은 형태로 타일 좌표들이 순서대로 들어 있습니다.
        var path = aStarPathFinder.GetAStarPath();

        // foreach – 경로 리스트를 순서대로 순회
        foreach (Vector2Int pos in path) //경로의 각 위치에 대해 반복
        {
            // 해당 위치의 타일을 가져오기
            //  - GridManager에게 “이 좌표에 해당하는 타일 오브젝트를 달라”고 요청합니다.
            //  - GridManager 내부에서는 2차원 배열을 통해 해당 좌표의 Tile을 찾아 반환합니다.
            Tile tile = gridManager.GetTileBounds(pos);

            // 이 좌표에 타일이 없다면 tile이 null일 수 있기 때문에, 널 체크 후에 색을 변경합니다.
            if (tile != null)
            {
                // Tile 스크립트에 정의된 메소드로, 해당 타일의 SpriteRenderer 색상을 파랑색으로 바꿉니다.
                tile.SetColor(Color.yellow);
            }

            // 지정된 시간만큼 대기
            // fStepDelaySeconds 에 지정된 시간 동안 기다렸다가, 다음 좌표로 넘어가 경로를 계속 칠합니다.
            // “한 번에 모든 타일이 바뀌는 것”이 아니라 한 칸씩, 차례대로 색이 칠해지는 애니메이션 효과를 얻을 수 있습니다.
            yield return new WaitForSeconds(stepDelaySeconds);
        }
    }
}

