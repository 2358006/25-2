using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    GameManager gameManager;

    Rigidbody rigid;
    CapsuleCollider col;

    [Header("Move")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    float vInput;
    float hInput;

    [Header("Jump")]
    public float jumpVelocity = 5f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    bool isJumping = false;

    [Header("Bullet")]
    public GameObject bullet;
    public float bulletSpeed = 100f;
    bool isShooting;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);
        rigid.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        rigid.MoveRotation(rigid.rotation * angleRot);

        if (isJumping && IsGrounded())
        {
            rigid.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }
        isJumping = false;

        if (isShooting)
        {
            GameObject newBullet = Instantiate(bullet, this.transform.position + new Vector3(0, 0, 1), this.transform.rotation);
            Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = this.transform.forward * bulletSpeed;
        }
        isShooting = false;
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

        if (Keyboard.current.spaceKey.isPressed) { isJumping = true; }
        if (Keyboard.current.jKey.isPressed) { isShooting = true; }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Enemy")
        {
            gameManager.hp -= 1;
        }
    }

    bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);
        bool isGrunded = Physics.CheckCapsule(col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);

        return isGrunded;
    }
}
