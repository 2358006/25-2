using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DFSVisualizer : MonoBehaviour
{
    DFSPathFinder dFSPathFinder = null; // 경로 탐색
    GridManager gridManager = null; // 그리드 경계 / 통로여부 / 타일정보 조회
    public float stepDelaySeconds = 0.5f; // 경로 표시 간격

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
        dFSPathFinder = GetComponent<DFSPathFinder>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(ShowPathRoutine());
        }
    }

    /* 코루틴 ShowPathRoutine()
    코루틴이란? → 실행 도중 yield return으로 일시 중단/재개 가능한 함수
    한 칸씩 색칠하고 일정 시간 대기하는 방식으로 경로 시각화를 위해 사용함
    코루틴: IEnumerator를 반환하고, yield return을 사용하여 실행을 일시 중지할 수 있는 함수
    경로를 하나씩 초록색으로 칠해가며 보여주는 코루틴
    */
    IEnumerator ShowPathRoutine()
    {
        List<Vector2Int> path = dFSPathFinder.GetDFSPath();

        // 예외처리 : 경로가 없으면 아무것도 하지 않고 종료
        if (path == null || path.Count == 0) { yield break; }

        // 각 좌표를 순회하며 해당 타일을 초록색으로 칠함
        foreach (Vector2Int pos in path)
        {
            Tile tile = gridManager.GetTileBounds(pos);

            if (tile != null) { tile.SetColor(Color.green); } // 타일을 녹색으로 칠함

            yield return new WaitForSeconds(stepDelaySeconds);
        }

    }
}