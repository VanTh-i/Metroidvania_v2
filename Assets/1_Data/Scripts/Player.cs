using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected PlayerAnimation Animation;
    protected bool attacking;
    protected float xAxis, yAxis;
    public bool Grounded { get; set; }

    protected virtual void Start()
    {
        Animation = GetComponentInChildren<PlayerAnimation>();
    }
    protected virtual void Update()
    {
        GetInput();
    }

    protected virtual void GetInput()
    {
        attacking = Input.GetKeyDown(KeyCode.Mouse0);
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }
}
