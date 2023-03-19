using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetDummy : MonoBehaviour, IDamage
{
    [SerializeField] Animator anim;

    public void TakeDamage(float damage)
    {
        anim.SetTrigger("Damage");
    }
}
