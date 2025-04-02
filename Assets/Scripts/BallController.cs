using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Solo el servidor controla la pelota
        {
            LaunchBall();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void LaunchBall()
    {
        float xDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        float yDirection = Random.Range(-1f, 1f);
        rb.velocity = new Vector2(xDirection, yDirection).normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal") && IsServer)
        {
            ResetBallServerRpc();
        }
    }

    [ServerRpc]
    void ResetBallServerRpc()
    {
        transform.position = Vector3.zero;
        LaunchBall();
    }
}
