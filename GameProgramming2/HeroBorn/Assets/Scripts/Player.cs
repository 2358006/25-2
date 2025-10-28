using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    Rigidbody rigid;

    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;

    float vInput;
    float hInput;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);
        rigid.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        rigid.MoveRotation(rigid.rotation * angleRot);
    }

    void Update()
    {
        // 키보드가 없는 경우 방지
        if (Keyboard.current == null) return;

        // 새 Input System 방식으로 입력 읽기
        vInput = 0f;
        hInput = 0f;

        if (Keyboard.current.wKey.isPressed) vInput += moveSpeed;

        if (Keyboard.current.sKey.isPressed) vInput -= moveSpeed;

        if (Keyboard.current.aKey.isPressed) hInput -= rotateSpeed;

        if (Keyboard.current.dKey.isPressed) hInput += rotateSpeed;
    }
}
