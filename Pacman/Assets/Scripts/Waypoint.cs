using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool isTouched;
    private DateTime touchedTime;
    public float duration = 3f;

    public Waypoint[] neighbors;
    public Vector2[] validDirections;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan time = DateTime.Now - touchedTime;

        if (time.Seconds >= duration)
        {
            isTouched = false;
        }
    }

    public void Touch()
    {
        touchedTime = DateTime.Now;
        isTouched = true;
    }
}
