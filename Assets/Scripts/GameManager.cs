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
    private float tiempoRestante = 3f; // Tiempo total en segundos
    public GameObject timeText;
    public BallController ballController;
    public GameObject winnerText;
    public GameObject objCanva;
    public GameObject obj;
    public GameObject desconectar;

    void Start()
    {
        if (IsServer)
        {
            player1Score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
            
        } else
        {
            player2Score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
        }

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
        timeText.GetComponent<Text>().text = "Cuenta atrás: " + tiempoRestante.ToString();
    }
    [ClientRpc]
    public void ScorePointClientRpc(int player)
    {
        ScorePoint(player);
    }

    public void ScorePoint(int player)
    {
        if (!IsServer) { return; }
        //Si el host anota, aumenta el puntaje del jugador 1
        if (player == 1)
        {
            Debug.Log("Punto para el jugador 1");
            player1Score.Value++;
            if (player1Score.Value >= 5)
            {
                winnerText.SetActive(true);
                winnerText.GetComponent<Text>().text = "¡Jugador 1, has ganado!";
                objCanva.SetActive(false);
                obj.SetActive(false);
                desconectar.SetActive(true);
            } 
            if (player2Score.Value >= 5)
            {
                winnerText.SetActive(true);
                winnerText.GetComponent<Text>().text = "¡Jugador 1, has perdido!";
                objCanva.SetActive(false);
                obj.SetActive(false);
                desconectar.SetActive(true);
            }
        }
        else if (player == 2)
        {
            Debug.Log("Punto para el jugador 2");
            //Si el cliente anota, aumenta el puntaje del jugador 2
            player2Score.Value++;
            if (player2Score.Value >= 5)
            {
                winnerText.SetActive(true);
                winnerText.GetComponent<Text>().text = "Jugador 2, has ganado";
                objCanva.SetActive(false);
                obj.SetActive(false);
                desconectar.SetActive(true);
            } 
            if (player1Score.Value >= 5)
            {
                winnerText.SetActive(true);
                winnerText.GetComponent<Text>().text = "Jugador 2, has perdido";
                objCanva.SetActive(false);
                obj.SetActive(false);
                desconectar.SetActive(true);
            }
        }
        ActualizarPuntuacionClientRpc();
    }

    void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.Value.ToString();
        player2ScoreText.text = player2Score.Value.ToString();
    }
    [ClientRpc]
    public void ActualizarPuntuacionClientRpc()
    {
        UpdateScoreUI();
    }
    public void Desconectar()
    {
        Application.Quit();
    }
}
