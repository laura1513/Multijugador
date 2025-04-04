using Unity.Netcode;
using UnityEngine;

public class PaddleController : NetworkBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;
    public BallController ballController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        // El host mueve una raqueta y el cliente mueve la otra
        if (IsOwner)
        {
            float moveVertical = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(0.0f, moveVertical);
            rb.velocity = movement * speed;
            // Limitar la posición de la raqueta
            if (transform.position.y < -4f)
            {
                transform.position = new Vector2(transform.position.x, -4f);
            }
            if (transform.position.y > 4f)
            {
                transform.position = new Vector2(transform.position.x, 4f);
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            ballController.RebotarRaqueta();
        }
    }
}
