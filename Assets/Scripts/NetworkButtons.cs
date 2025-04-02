using Unity.Netcode;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost(); // Inicia el servidor
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient(); // Se une como cliente
    }
}
