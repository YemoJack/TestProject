using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 获取输入（-1 到 1）
        input.x = Input.GetAxisRaw("Horizontal"); // A/D 或 左/右
        input.y = Input.GetAxisRaw("Vertical");   // W/S 或 上/下
        input = input.normalized; // 防止斜方向速度加成
    }

    void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }
}
