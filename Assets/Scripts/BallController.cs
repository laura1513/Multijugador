using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float speed = 5f;
    public float rebote = 10f;
    public float maxSpeed = 15f; // Velocidad máxima de la pelota
    private ContactPoint2D[] collisionPoints;
    private bool movimiento = false;
    private Vector2 dir;
    public GameManager gameManager;

    public void LaunchBall()
    {
        //Numero random para la dirección inicial
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        dir = new Vector2(randomX, randomY).normalized;
        movimiento = true;
    }
    private void Update()
    {
        if (movimiento)
        {
            // Mover la pelota hacia adelante
            transform.Translate(dir * speed * Time.deltaTime);
        }
        if (transform.position.y < -4.5 || transform.position.y > 4.5)
        {
            dir.y *= -1;
        }
        if (transform.position.x < -8 || transform.position.x > 8)
        {
            //Sumar puntos al jugador que anota
            if (transform.position.x < -8)
            {
                gameManager.ScorePointClientRpc(2);
            }
            else if (transform.position.x > 8)
            {
                gameManager.ScorePointClientRpc(1);
            }
            ResetBallServerRpc();
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Goal1"))
        {
            gameManager.ScorePointClientRpc(2);
            ResetBallServerRpc();
        } else if (collision.gameObject.CompareTag("Goal2"))
        {
            gameManager.ScorePointClientRpc(1);
            ResetBallServerRpc();
        }
        else
        {
            collisionPoints = collision.contacts;

            //Calcular la dirección de rebote
            Vector2 dir = (collisionPoints[0].point - (Vector2)transform.position).normalized;
        }
        
        
    }
    public void RebotarRaqueta()
    {
        dir.x *= -1;
    }
    [ServerRpc]
    void ResetBallServerRpc()
    {
        transform.position = Vector2.zero;
        LaunchBall();
    }
}
