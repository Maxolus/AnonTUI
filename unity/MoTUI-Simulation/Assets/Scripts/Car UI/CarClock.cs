using UnityEngine;
using TMPro;   // Make sure you have TextMeshPro imported
using System;

public class CarClock : MonoBehaviour
{
    public TextMeshPro clockText3D;   // For 3D TextMeshPro

    void Update()
    {
        string currentTime = DateTime.Now.ToString("HH:mm");
        if (clockText3D != null)
            clockText3D.text = currentTime;
    }
}