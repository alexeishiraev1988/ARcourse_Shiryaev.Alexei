using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    private Transform _target;


    private void Start()
    {
        _target = GameObject.Find("Sphere").transform;
    }

    private void Update()
    {
        if (_target == null) return;
        transform.position = Vector2.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Sphere")
        {
            HealthManager.Instance.TakeDamage();
            // Отбрасываем врага в случайную сторону чтобы не снимал жизни непрерывно
            transform.position = GetRandomSpawnPosition();
        }
    }

    public Vector3 GetRandomSpawnPosition()
    {
        // Спавним по краям экрана
        float x = Random.Range(-4f, 4f);
        float y = Random.value > 0.5f ? 4f : -4f;
        return new Vector3(x, y, 0);
    }
}
