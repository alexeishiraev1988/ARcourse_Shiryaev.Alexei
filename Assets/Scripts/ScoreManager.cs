using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _score = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore()
    {
        _score++;
        _scoreText.text = "Монеток: " + _score;
    }

    public void ResetScore()
    {
        _score = 0;
        _scoreText.text = "Монеток: " + _score;
    }
}
