using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public Text player1ScoreText, player2ScoreText;
    private NetworkVariable<int> player1Score = new NetworkVariable<int>(0);
    private NetworkVariable<int> player2Score = new NetworkVariable<int>(0);

    void Start()
    {
        if (IsServer)
        {
            player1Score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
            player2Score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
        }
    }

    public void ScorePoint(int player)
    {
        if (IsServer)
        {
            if (player == 1)
                player1Score.Value++;
            else
                player2Score.Value++;

            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.Value.ToString();
        player2ScoreText.text = player2Score.Value.ToString();
    }
}
