using UnityEngine;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private Transform _sphere; // перетащи Sphere
    [SerializeField] private Vector3 _sphereStartPos = new Vector3(0, 2.56f, 0);

    public void Restart()
    {
        // Сбрасываем время
        Time.timeScale = 1;

        // Прячем Game Over
        _gameOverScreen.SetActive(false);

        // Возвращаем шар на место
        _sphere.position = _sphereStartPos;
        var rb = _sphere.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        // Сбрасываем счёт и жизни
        ScoreManager.Instance.ResetScore();
        HealthManager.Instance.ResetHealth();

        // Уничтожаем всех врагов и спавним заново
        foreach (var enemy in FindObjectsOfType<Enemy>())
            Destroy(enemy.gameObject);
        FindObjectOfType<EnemySpawner>().Spawn();
    }
}
