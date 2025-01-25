using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    PlayerMovement movement;
    [SerializeField] float _knockbackAmount = 10f;

    private void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (movement == null)
            return;

        if (collision.transform.CompareTag("PlayerBubble"))
        {
            print("hit player bubble");
            Vector2 dir = transform.position - collision.transform.position;
            dir.Normalize();
            movement.Knockback(_knockbackAmount, dir);
        }
    }
}
