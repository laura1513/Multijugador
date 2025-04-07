using Unity.Netcode;
using UnityEngine;

public class PaddleController : NetworkBehaviour
{
    public float speed = 10f;
    public BallController ballController;

    private void FixedUpdate()
    {
        // El host mueve una raqueta y el cliente mueve la otra
        if (IsOwner)
        {
            float moveVertical = Input.GetAxis("Vertical");
            transform.position = new Vector2(transform.position.x, transform.position.y + moveVertical * speed * Time.fixedDeltaTime);

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
