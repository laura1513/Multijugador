using System.Collections;
using Unity.Netcode;
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
    public GameObject obj;
    public GameObject desconectar;

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
        obj.SetActive(false);
        desconectar.SetActive(true);
    }

    public void Desconectar()
    {
        Application.Quit();
    }
}
