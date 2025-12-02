/* 
그리드와 타일생성을 담당하는 매니저 스크립트 
내부 데이터 비공개, 외부는 메소드를 통해서만 접근 가능
*/

using UnityEngine;
public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab = null; // 각 셀 표현할 타일 프리펩

    readonly int[,] gridData = new int[,]
    {
        {0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0},
        {0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1},
        {1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1},
        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}
    };

    Tile[,] tiles = null;

    void Start()
    {
        GenerateGrid(); // 게임 시작시 그리드 생성
    }

    // 미로 생성
    void GenerateGrid()
    {
        int rows = GetHeight();
        int cols = GetWidth();
        tiles = new Tile[rows, cols];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                // 씬 좌표 배치: x는 오른쪽, y는 아래로 증가하도록 -y 사용
                Vector3 spawnPos = new Vector3(x, -y, 0.0f);   // 타일 위치
                Quaternion rot = Quaternion.identity;          // 회전 없음

                GameObject tileObj = Instantiate(tilePrefab, spawnPos, rot); // 타일 프리팹 생성
                Tile tile = tileObj.GetComponent<Tile>();

                // 색상: 벽(1)은 회색, 길(0)은 흰색
                Color initial = (gridData[y, x] == 1) ? Color.gray : Color.white;

                //Tile의 초기화 메소드 호출로 좌표/색상 설정
                tile.Initialize(new Vector2Int(x, y), initial);

                //좌표에 해당하는 타일을 2차원 배열에 저장
                tiles[y, x] = tile;
            }
        }
    }

    // 현재 미로의 가로 길이 반환
    public int GetWidth()
    {
        return gridData.GetLength(1);
    }

    // 현재 미로의 세로 길이 반환
    public int GetHeight()
    {
        return gridData.GetLength(0);
    }

    public bool IsInside(Vector2Int pos)
    {
        int width = GetWidth();
        int height = GetHeight();
        bool isInside = (pos.x >= 0) && (pos.x < width) &&
        (pos.y >= 0) && (pos.y < height);

        return isInside;
    }

    public bool IsWalkable(Vector2Int pos)
    {
        if (!IsInside(pos))
        {
            return false;
        }
        return gridData[pos.y, pos.x] == 0;
    }

    public Tile GetTileBounds(Vector2Int position)
    {
        if (!IsInside(position)) { return null; }
        return tiles[position.y, position.x];
    }
}