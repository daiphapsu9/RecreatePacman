using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint[] neighbors;
    public Vector2[] validDirections;
    public bool isPortal;
    public Waypoint targetPortal;

    // Start is called before the first frame update
    void Start()
    {
        validDirections = new Vector2[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            Waypoint neighbor = neighbors[i];
            Vector2 temp = neighbor.transform.localPosition - transform.localPosition;

            validDirections[i] = temp.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
