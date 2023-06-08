using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInteraction : MonoBehaviour
{
    
    public LayerMask layerMaskCollider;

    void Update()
    {
        RaycastHit2D collider = Physics2D.Raycast(transform.position, Vector2.right, 0.01F, layerMaskCollider);

        if (collider)
        {
            Destroy(gameObject);
        }
    }
}
