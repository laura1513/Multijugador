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
            //Aumentar la velocidad horizontal de la pelota hasta un máximo
            if (dir.x < 0)
            {
                dir.x -= rebote * Time.deltaTime;
            }
            else
            {
                dir.x += rebote * Time.deltaTime;
            }

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
