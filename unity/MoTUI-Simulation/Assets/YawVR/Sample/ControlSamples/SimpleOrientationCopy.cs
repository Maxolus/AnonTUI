using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YawVR;
/// <summary>
/// Sets the YawTracker's orientation based on the GameObject's orientation
/// </summary>
public class SimpleOrientationCopy : MonoBehaviour
{
    /*
       This script simply copies this gameObject's rotation, and sends it to the YawTracker
    */
    YawController yawController; // reference to YawController
    MotionCompensation motionCompensation;
    Vector3 initialLocalEulerAngles;
            public GameObject volo;

    private void Start() {
        yawController = YawController.Instance();
        //yawController.DiscoverDevices();
        motionCompensation = yawController.gameObject.GetComponent<MotionCompensation>();
       // initialLocalEulerAngles = new Vector3(-6.837f,31.275f,0f);
        initialLocalEulerAngles = new Vector3(0.00f,103.86f,5.65f);

    }
    private void FixedUpdate() 
    {
        if (motionCompensation?.GetDevice() == MotionCompensation.enumYawPitchRollDevice.YawVRController)
        {
//           print("test1"+ (transform.localEulerAngles-initialLocalEulerAngles)+ transform.localEulerAngles+yawController.TrackerObject.transform.rotation);

          yawController.TrackerObject.SetRotation(transform.localEulerAngles-initialLocalEulerAngles);
           // yawController.TrackerObject.SetRotation(transform.localEulerAngles);
        }
        else if (motionCompensation?.GetDevice() == MotionCompensation.enumYawPitchRollDevice.LeftController
              || motionCompensation?.GetDevice() == MotionCompensation.enumYawPitchRollDevice.RightController) 
        {
            Vector3 eulerAngles = new Vector3();

            try
            {
                eulerAngles = motionCompensation.GetOpenXRControllerTransform().localEulerAngles;
            }
            catch (Exception ex) 
            {
                ex.ToString();
            }

            if (null != eulerAngles) 
            {
                yawController.TrackerObject.SetRotation(eulerAngles-initialLocalEulerAngles);
               // yawController.TrackerObject.SetRotation(eulerAngles);
           print("test2"+(eulerAngles));

            }
        }
            Vector3 pos = new Vector3(-1003.828f, 337.0166f, 199.3668f);
        Vector3 pos2 = new Vector3(-994.7218f, 344.9991f, 216.3505f);
     /*   if (volo.transform.position.x >= pos.x && volo.transform.position.y >= pos.y && volo.transform.position.x <= pos2.x && volo.transform.position.y <= pos2.y)
        {
        initialLocalEulerAngles = new Vector3(353.16f,31.28f,0f);
        yawController.TrackerObject.SetRotation(initialLocalEulerAngles);
        print("initialeuer");

          
        }*/
    }
}
