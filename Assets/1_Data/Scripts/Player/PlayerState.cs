using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool IsFacingRight;
    public bool IsInGround;
    public bool IsWallSliding;

    public bool CanDoubleJump;

    [HideInInspector] public bool recoilingX, recoilingY;

    [HideInInspector] public int AirJumpCounter = 0;
    [HideInInspector] public float GravityScale;
}
