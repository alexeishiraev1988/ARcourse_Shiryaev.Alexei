
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance;

    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private RectTransform _spawnArea; // перетащи сюда RawImage

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnCoin();
    }

    public void SpawnCoin()
    {
        // ѕолучаем случайную позицию внутри RawImage
        Rect rect = _spawnArea.rect;
        float x = Random.Range(rect.xMin * 0.8f, rect.xMax * 0.8f);
        float y = Random.Range(rect.yMin * 0.8f, rect.yMax * 0.8f);

        // ѕереводим локальные координаты RawImage в мировые
        Vector3 localPos = new Vector3(x, y, 0);
        Vector3 worldPos = _spawnArea.TransformPoint(localPos);

        Instantiate(_coinPrefab, worldPos, Quaternion.identity);
    }
}
