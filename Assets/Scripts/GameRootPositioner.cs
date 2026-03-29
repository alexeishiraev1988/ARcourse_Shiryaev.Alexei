using UnityEngine;

public class GameRootPositioner : MonoBehaviour
{
    [SerializeField] private Transform _gameRoot;
    [SerializeField] private Vector3 _position = Vector3.zero;
    [SerializeField] private Vector3 _rotation = Vector3.zero;
    [SerializeField] private float _scale = 1f;

    void Update()
    {
        if (_gameRoot != null)
        {
            _gameRoot.position = _position;
            _gameRoot.rotation = Quaternion.Euler(_rotation);
            _gameRoot.localScale = Vector3.one * _scale;
            _gameRoot.gameObject.SetActive(true);
        }
    }
}
