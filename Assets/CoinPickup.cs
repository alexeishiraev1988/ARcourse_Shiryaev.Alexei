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
            audioSource.Play();
            Debug.Log("Монетка собрана!");

            Destroy(gameObject, audioSource.clip.length);
        }
    }
}
