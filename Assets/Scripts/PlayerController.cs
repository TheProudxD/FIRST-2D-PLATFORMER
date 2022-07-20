using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;
    private float movementSpeed = 10.5f;
    private float jumpForce = 16.0f;
    private bool isFacingRight = true;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    private void Start()
    {
        // TODO: CHANGE NAME
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
    }
    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}
