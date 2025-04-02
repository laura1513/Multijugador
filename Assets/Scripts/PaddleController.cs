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
    private void Update()
    {
        if (IsLocalPlayer)
        {
            float vertical = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(0, vertical) * speed;
        }
    }
}
