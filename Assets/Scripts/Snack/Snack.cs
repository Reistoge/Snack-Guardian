using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snack : MonoBehaviour
{
    [SerializeField] Fall fall;
    [SerializeField] bool touchingPlayer = false;
    SnackAnimator animatorHandler;

    



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            touchingPlayer = true;
            fall.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }

 

}
