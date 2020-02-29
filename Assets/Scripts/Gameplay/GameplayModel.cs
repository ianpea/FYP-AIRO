using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayModel
{
    public bool isLevelComplete = false;

    public const float GRAVITY = -19.0f;
}

public class GameTime
{
    public const float startTime = 0.0f;
    public static float currentTime;

}

public class AirTime
{
    public static float currentAirTime = 0.0f;
    public const float MIN_AIR_DIST = 1.0f;
    public const float MAX_AIR_DIST = 50.0f;
}