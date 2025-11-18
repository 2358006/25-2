/*
하나의 타일을 표현하는 스크립트
좌표와 색상은 외부에서 접근 불가, 초기화 & 색상 설정 메서드 제공
Grid Manager를 통해 타일 생성및 관리
*/

using UnityEngine;

public class Tile : MonoBehaviour
{
    Vector2Int gridPosition = Vector2Int.zero; // 자신의 그리드 좌표
    SpriteRenderer spriteRenderer = null; // 색상 변경을 위한 스프라이트 렌더러


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Vector2Int gridPos, Color initialColor)
    {
        gridPosition = gridPos;
        SetColor(initialColor);
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null) { spriteRenderer.color = color; }
    }
}
