using CLOVR_Plugin;
using UnityEngine;
using UnityEngine.UI;

public class CLOVRManager : CLOVRBehaviour
{
    public GameObject leftCam, rightCam;

    void Start()
    {
        Initialize();
        leftCam = GameObject.Find("Left Camera");
        rightCam = GameObject.Find("Right Camera");

        // Hardcoded Rotation and Interlens distance to remove distortion.
        leftCam.transform.localPosition = new Vector3(leftCam.transform.localPosition.x, leftCam.transform.localPosition.y, -0.04f);
        rightCam.transform.localPosition = new Vector3(rightCam.transform.localPosition.x, rightCam.transform.localPosition.y, 0.04f);

        leftCam.transform.localEulerAngles = new Vector3(0, -3.5f, 0);
        rightCam.transform.localEulerAngles = new Vector3(0, 3.5f, 0);
    }

    void Update()
    {
        CLOVRUpdate();
        //code your logic here

        transform.rotation = transform.rotation * Quaternion.Euler(CLOVRSensorManager.GetGyroValue[1] * 3.0f, -CLOVRSensorManager.GetGyroValue[0] * 3.0f, 0);
        float z = transform.eulerAngles.z;
        transform.Rotate(0, 0, -z);
    }

}
