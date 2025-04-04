using Unity.Netcode;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{
    public GameManager gameManager; // Referencia al GameManager
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost(); // Inicia el servidor
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient(); // Se une como cliente
    }
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown(); // Desconecta el cliente o servidor
        gameManager.Desconectar(); // Llama al m�todo de desconexi�n en el GameManager
    }
}
