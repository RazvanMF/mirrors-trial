///SCRIPT THAT DEALS WITH PLAYER STATE VARIABLES
///HANDLES: IFRAME, SHOOTING, DASHING, TIMER, HEALTH, ROOM STATE, SCORE (AVAILABILITY)
///ACCESSIBLE: EVERYTHING IS PUBLIC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool canBeHit = true;

    public bool canShoot = true;

    public bool canDash = true;
    public float dashTimer = 1.5f;

    public int health = 3;

    public int maxRoundSeconds = 40;
    public int currentRoundSeconds = 0;
    public int maxCooldownSeconds = 10;
    public int currentCooldownSeconds = 0;
    public bool insideRound = true;

    public static int score = 0;
}
