using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private GameObject _gameOverScreen;

    private int _health = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateText();
        _gameOverScreen.SetActive(false);
    }

    public void TakeDamage()
    {
        _health--;
        UpdateText();
        if (_health <= 0)
        {
            _gameOverScreen.SetActive(true);
            Time.timeScale = 0; // останавливаем игру
        }
    }

    private void UpdateText()
    {
        _healthText.text = "Жизни: " + _health;
    }

    public void ResetHealth()
    {
        _health = 5;
        UpdateText();
    }
}
