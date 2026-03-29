using UnityEngine;
public class Coin1 : MonoBehaviour
{
    [SerializeField] private float _destroyY = -6f; // граница внизу экрана

    private void Update()
    {
        // Уничтожаем монетку если она упала ниже экрана
        if (transform.position.y < _destroyY)
        {
            CoinSpawner.Instance.SpawnCoin();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Sphere")
        {
            ScoreManager.Instance.AddScore();
            CoinSpawner.Instance.SpawnCoin();
            Destroy(gameObject);
        }
    }
}
