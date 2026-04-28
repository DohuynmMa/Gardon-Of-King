using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogingNPC : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
}
