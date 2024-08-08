using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinSoundEffect;
    private bool isPickingUpCoin; //bool to prevent double pickup of same coin

    private void Start() {
        isPickingUpCoin = false;
    }

    //picks up coin, plays sound effect, adds to score and then destroys coin
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !isPickingUpCoin){
            isPickingUpCoin = true;
            AudioSource.PlayClipAtPoint(coinSoundEffect, Camera.main.transform.position);
            FindFirstObjectByType<GameSessionController>().addToScore(100);
            Destroy(gameObject);
        }
    }
}
