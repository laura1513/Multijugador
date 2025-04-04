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
    private float tiempoRestante = 10f; // Tiempo total en segundos
    public GameObject timeText;
    public BallController ballController;

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
        StartCoroutine(CuentaAtras());
        
    }
    IEnumerator CuentaAtras()
    {
        while (tiempoRestante > 0)
        {
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
            ActualizarTiempo(tiempoRestante);
        }
        ballController.LaunchBall();
        startButton.SetActive(false);
        timeText.SetActive(false);
    }
    private void ActualizarTiempo(float tiempoRestante)
    {
        timeText.GetComponent<Text>().text = "Cuenta atrás: " + tiempoRestante.ToString();
    }


    public void ScorePoint()
    {
        if (IsServer)
        {
            player1Score.Value++;
        } else
        {
            player2Score.Value++;
        }
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.Value.ToString();
        player2ScoreText.text = player2Score.Value.ToString();
    }
}
