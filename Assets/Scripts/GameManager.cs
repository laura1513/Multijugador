using System.Collections;
using System.Net.Sockets;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public Text player1ScoreText, player2ScoreText;
    private NetworkVariable<int> player1Score = new NetworkVariable<int>(0);
    private NetworkVariable<int> player2Score = new NetworkVariable<int>(0);

    public GameObject startButton;
    private float tiempoRestante = 3f;

    public GameObject timeText;
    public BallController ballController;
    public GameObject winnerText;
    public GameObject objCanva;
    public GameObject ball;
    public GameObject obj;
    public GameObject desconectar;
    public GameObject objCanvaHost;

    [Header("El inputField para asignar manualmente la ip a la conectarse")]
    [SerializeField] private TMP_InputField ipHostLocal;

    [Header("Cartel que muestra nuestra ip local")]
    [SerializeField] private TMP_Text ipLocalText;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }
    private void OnClientConnected(ulong id)
    {
        //Si ya hay 2 clientes conectados, no se permite la conexión
        if (NetworkManager.Singleton.ConnectedClients.Count > 2)
        {
            NetworkManager.Singleton.DisconnectClient(id);
            return;        }
        if (IsHost)
        {
            // Si el cliente es el host, se le asigna la puntuación inicial
            player1Score.Value = 0;
            player2Score.Value = 0;
            ClienteConectadoClientRpc();
        }
        else
        {
            // Si el cliente no es el host, se le asigna la puntuación inicial
            player1Score.Value = 0;
            player2Score.Value = 0;
            ClienteConectadoClientRpc();
        }
    }
    private void OnClientDisconnected(ulong id)
    {
        // Aquí puedes manejar la desconexión del cliente si es necesario
        Debug.Log("Cliente desconectado: " + id);
    }
    void Start()
    {
        // Escuchamos cambios en ambas puntuaciones en todos los clientes
        player1Score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
        player2Score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.Value.ToString();
        player2ScoreText.text = player2Score.Value.ToString();
    }

    public void AparecenBotones()
    {
        startButton.SetActive(true);
        timeText.SetActive(true);
    }

    public void StartGame()
    {
        ActualizarTiempo(tiempoRestante);
        ActualizarTiempoClientRpc(tiempoRestante);
        StartCoroutine(CuentaAtras());
    }

    IEnumerator CuentaAtras()
    {
        startButton.SetActive(false);

        while (tiempoRestante > 0)
        {
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
            ActualizarTiempo(tiempoRestante);
            ActualizarTiempoClientRpc(tiempoRestante);
        }

        ballController.LaunchBall();
        startButton.SetActive(false);
        QuitarTextoClientRpc();
        QuitarBotonesClientRpc();
    }
    [ClientRpc]
    public void QuitarBotonesClientRpc()
    {
        QuitarBotones();
    }
    public void QuitarBotones()
    {
        objCanvaHost.SetActive(false);
    }

    [ClientRpc]
    public void QuitarTextoClientRpc()
    {
        timeText.SetActive(false);
    }

    [ClientRpc]
    public void ActualizarTiempoClientRpc(float tiempoRestante)
    {
        timeText.SetActive(true);
        ActualizarTiempo(tiempoRestante);
    }

    private void ActualizarTiempo(float tiempoRestante)
    {
        timeText.GetComponent<Text>().text = "Cuenta atrás: " + Mathf.CeilToInt(tiempoRestante).ToString();
    }

    [ClientRpc]
    public void ScorePointClientRpc(int player)
    {
        ScorePoint(player);
    }

    public void ScorePoint(int player)
    {

        if (player == 1)
        {
            Debug.Log("Punto para el jugador 1");
            player1Score.Value++;
        }
        else if (player == 2)
        {
            Debug.Log("Punto para el jugador 2");
            player2Score.Value++;
        }

        CheckWinnerClientRpc();
    }
    [ClientRpc]
    public void CheckWinnerClientRpc()
    {
        CheckWinner();
    }
    private void CheckWinner()
    {
        if (player1Score.Value >= 5)
        {
            MostrarResultadoClientRpc("¡El jugador 1 ha ganado!");
        }
        else if (player2Score.Value >= 5)
        {
            MostrarResultadoClientRpc("¡El jugador 2 ha ganado!");
        }
    }
    [ClientRpc]
    public void MostrarResultadoClientRpc(string msg)
    {
        MostrarResultado(msg);
    }

    private void MostrarResultado(string mensaje)
    {
        winnerText.SetActive(true);
        winnerText.GetComponent<Text>().text = mensaje;
        objCanva.SetActive(false);
        ball.SetActive(false);
        obj.SetActive(false);
        desconectar.SetActive(true);
    }
    public void ClienteConectado()
    {
        timeText.SetActive(true);
        timeText.GetComponent<Text>().text = "Esperando a que el host inicie la partida";
    }
    [ClientRpc]
    public void ClienteConectadoClientRpc()
    {
        ClienteConectado();
    }

    public string GetLocalIPAddress()
    {
        string ipLocal;
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipLocal = ip.ToString();
                ipLocalText.text = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    public void AsignarIpNetworkManager()
    {
        //asigno la ip local al networkManager
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(GetLocalIPAddress(), (ushort)7777);
    }
    public void AsignarIpManual()
    {
        //asigno la ip que el usuario a puesto en el ipHostLocal al networkManager
        if (ipHostLocal.text != "")
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipHostLocal.text, (ushort)7777);
            Debug.Log("IP asignada: " + ipHostLocal.text);
        }
        else
        {
            Debug.Log("No se ha asignado ninguna IP");
        }
    }
}
