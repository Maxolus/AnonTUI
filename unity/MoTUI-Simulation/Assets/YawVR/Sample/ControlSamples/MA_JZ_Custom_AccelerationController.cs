using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YawVR;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// Sets the YawTracker's orientation based on the GameObject's speed
/// </summary>
public class MA_JZ_Custom_Accelerationontroller : MonoBehaviour
{
    /*
     This script uses the gameObjects's rigidbody's velocity to control the YawTracker
  */
    YawController yawController;

    private Rigidbody rigid;


    [SerializeField]
    private Vector3 multiplier = new Vector3(50f, 1f, -1.8f);

    [SerializeField]
    private Vector3 axisSmoothingSpeed = new Vector3(2.5f, 2f, 1.2f); // X, Y, Z

    private Vector3 smoothedAccel;
    private Vector3 prevVelocity;


    private void Awake() {
        rigid = GetComponent<Rigidbody>();
    }


    private void Start() {
        yawController = YawController.Instance();
    }


    private void FixedUpdate()
    {
        Vector3 currentVel = transform.InverseTransformVector(rigid.linearVelocity);
        Vector3 rawAccel = (currentVel - prevVelocity) / Time.fixedDeltaTime;
        prevVelocity = currentVel;

        // Separate scaling for forward/backward (Z)
        float forwardZ = Mathf.Max(rawAccel.z, 0f) * multiplier.z;   // acceleration
        float brakeZ = Mathf.Min(rawAccel.z, 0f) * (multiplier.z * 0.2f); // braking 80% weaker
        rawAccel.z = forwardZ + brakeZ;

        // Scale X/Y normally
        rawAccel.x *= multiplier.x;
        rawAccel.y *= multiplier.y;

        // Scale X velocity
        float rawVelX = currentVel.x * multiplier.x;

        // Compute per-axis time-based smoothing factors
        Vector3 t = new Vector3(
            1f - Mathf.Exp(-axisSmoothingSpeed.x * Time.fixedDeltaTime),
            1f - Mathf.Exp(-axisSmoothingSpeed.y * Time.fixedDeltaTime),
            1f - Mathf.Exp(-axisSmoothingSpeed.z * Time.fixedDeltaTime)
        );


        // Double-pass smoothing on X
        smoothedAccel.x = Mathf.Lerp(smoothedAccel.x, rawVelX, t.x);            //  x components gets calculated with velocity for smoother course and to avoid over-steering

        // Normal smoothing on Y/Z
        smoothedAccel.y = Mathf.Lerp(smoothedAccel.y, rawAccel.y, t.y);
        smoothedAccel.z = Mathf.Lerp(smoothedAccel.z, rawAccel.z, t.z);

        // Apply result
        Vector3 v = new Vector3(smoothedAccel.z, 0f, smoothedAccel.x);
        yawController.TrackerObject.SetRotation(v);
    }

}
