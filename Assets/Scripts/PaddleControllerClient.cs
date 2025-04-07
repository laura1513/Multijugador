using Unity.Netcode;
using UnityEngine;

public class PaddleControllerClient : NetworkBehaviour
{
    public float speed = 10f;
    public BallController ballController;

    private void FixedUpdate()
    {
        if (IsHost) { return; }
        float moveVertical = Input.GetAxis("Vertical");
        if (moveVertical != 0)
        {
            MovimientoVerderServerRpc(moveVertical);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void MovimientoVerderServerRpc(float moveVertical)
    {
        ActualizarMovimientoClientRpc(moveVertical);
    }
    [ClientRpc]
    public void ActualizarMovimientoClientRpc(float moveVertical)
    {
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            ballController.RebotarRaqueta();
        }
    }
}
