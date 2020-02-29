using System.IO.Ports;
using System.Collections.Generic;

/*
 * Model/data for each Locomotion Controller (LC), left (L) and right (R).
 */
public class LCModel
{
    public string PortName = "NONAME";
    public SerialPort stream;

    public float angleX = 0;
    public float angleY = 0;
    public float angleZ = 0;

    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;
    /// <summary>
    /// If leg is up, then the other leg cannot walk.
    /// </summary>
    public bool isUp = false;
    /// <summary>
    /// If this leg jumped, then the other other leg can jump.
    /// </summary>
    public bool isJump = false;
}

public class LC_CONST
{
    /// <summary>
    /// Select port and baudrate for your locomotion controller's receiver.
    /// </summary>
    public const float overallFactor = 10.0f; // Increase the speed/influence rotation
    public const int baudrate = 9600;

    /// <summary>
    /// Time out for reading data from respective LC.
    /// </summary>
    public const int readTimeout = 25;

    /// <summary>
    /// Enable rotation/translation of the object. <CURRENTLT NOT USED>
    /// </summary>
    public bool enableRotation;
    public bool enableTranslation;

    /// <summary>
    /// Normalize accelerometer readings.
    /// </summary>
    public const float accNormalizer = 0.00025f;

    /// <summary>
    /// Normalize gyroscope readings, 32768 is the max value captured during test on IMU.
    /// </summary>
    public const float gyroNormalizer = 1.0f / 32768.0f;


    /// <summary>
    /// Noise cancelling factor, increasing this requires increase in use physical movement (User must move more abruptly, stronger, or wider movements.
    /// </summary>
    public const float noiseCancel = 0.075f;

    /// <summary>
    /// Physical right controller is offset by extra +1.1f on the Z-axis (gravity axis).
    /// </summary>
    public const float RIGHT_CONTROLLER_OFFSET_Z = 1.1f;

    /// <summary>
    /// Minimum value for accelerometer value to trigger a forward movement.
    /// </summary>
    public const float FORWARD_MINIMUM_MOVEMENT = 0.0f;

    /// <summary>
    /// Minimum value for accelerometer value to trigger a jump.
    /// </summary>
    public const float JUMP_MINIMUM_MOVEMENT = 5.0f;

    /// <summary>
    /// Thread mutex.
    /// </summary>
    public static object LOCKER = new object();

    public static float lastStepTime = 0.0f;

    public const float RESET_TIME = 1.5f;
}
