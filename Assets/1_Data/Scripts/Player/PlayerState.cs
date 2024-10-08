using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool IsFacingRight;
    public bool IsInGround;
    public bool IsWallSliding;

    public bool CanDoubleJump;

    public bool recoilingX, recoilingY;

    public int AirJumpCounter = 0;
}
