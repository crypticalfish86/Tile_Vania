using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float enemyVelocity = 1f;

    Rigidbody2D enemyRigidBody;
    Transform enemyPosition;


    // Start is called before the first frame update
    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        enemyPosition = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 enemyMovement = new Vector2 (enemyVelocity, 0);
        enemyRigidBody.velocity = enemyMovement;
    }

    //turn enemy when reaches edge of platform/wall(when box trigger exits collision with map)
    private void OnTriggerExit2D(Collider2D other) {
        enemyVelocity = -enemyVelocity;
        enemyPosition.localScale = new Vector2(Mathf.Sign(enemyVelocity), 1f);//1 when positive, -1 when negative
    }
}
