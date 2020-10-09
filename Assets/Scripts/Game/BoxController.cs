using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public Animator animator;


    public void DespawnBlock()
    {
        animator.SetBool("Despawn", true);
        Destroy(gameObject, 1.05f);
    }

    public void DespawnGhostBlock()
    {
        Destroy(gameObject);
    }
}
