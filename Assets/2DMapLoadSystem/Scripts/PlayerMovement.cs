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
        // ��ȡ���루-1 �� 1��
        input.x = Input.GetAxisRaw("Horizontal"); // A/D �� ��/��
        input.y = Input.GetAxisRaw("Vertical");   // W/S �� ��/��
        input = input.normalized; // ��ֹб�����ٶȼӳ�
    }

    void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }
}
