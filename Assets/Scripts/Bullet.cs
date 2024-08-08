using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    [SerializeField] float bulletSpeed;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        
        Vector2 playerScale = FindFirstObjectByType<PlayerInput>().transform.localScale; //get player scale
        transform.localScale = playerScale; //change x direction of bullet based on player scale
        float bulletVelocity = bulletSpeed * playerScale.x; //multiply speed by x scale to shoot bullet the right way
        myRigidBody.velocity = new Vector2 (bulletVelocity, 0);
    }

    //Destroy itself on impact with anything, If bullet hits enemy destroy the enemy too (also ensure the object isnt a tilemap)
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<TilemapCollider2D>() == null){
            FindFirstObjectByType<GameSessionController>().addToScore(50);
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }
}
