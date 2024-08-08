using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinSoundEffect;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player"){
            AudioSource.PlayClipAtPoint(coinSoundEffect, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
