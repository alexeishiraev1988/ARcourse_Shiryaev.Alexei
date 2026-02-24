using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private AudioSource audioSource;
        void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Монетка собрана!");
            GameManager.Instance.AddScore(1);
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
            Destroy(gameObject);
        }
    }
}
