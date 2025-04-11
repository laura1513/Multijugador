using Unity.Netcode;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{
    public GameManager gameManager; // Referencia al GameManager
    public void StartHost()
    {
        gameManager.AsignarIpNetworkManager(); // Asigna la IP al NetworkManager
        NetworkManager.Singleton.StartHost(); // Inicia el servidor
    }

    public void StartClient()
    {
        gameManager.AsignarIpManual();
        NetworkManager.Singleton.StartClient(); // Se une como cliente
    }
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown(); // Desconecta el cliente o servidor
        Application.Quit();                                     
    }
}
