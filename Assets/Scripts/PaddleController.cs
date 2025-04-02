using Unity.Netcode;
using UnityEngine;

public class PaddleController : NetworkBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!IsOwner) return; // Solo el dueño del objeto puede controlarlo

        float move = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(0, move * speed);
    }
}
