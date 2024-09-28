using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected internal PlayerAnimation Animation;

    private void Start()
    {
        Animation = GetComponentInChildren<PlayerAnimation>();
    }
}
