using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void RunAnimation(bool value)
    {
        anim.SetBool("Run", value);
    }
    public void JumpAnimation(bool value)
    {
        anim.SetBool("Jump", value);
    }
    public void FallAnimation(bool value)
    {
        anim.SetBool("Fall", value);
    }
}
