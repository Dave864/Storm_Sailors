﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderhead : MonoBehaviour
{
    // Strength level of the thunderhead gale
    private int galeLvl = 1;

    // Direction the thunderhead blows the wind
    private Vector3 galeVector = new Vector3(0, 0, 0);

    public Vector3 GaleVector
    {
        get { return galeVector; }
    }

    private void Awake()
    {
        // Initialize gale vector
        GameObject compassCenter = GameObject.Find("Compass Center");
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        galeVector = compassCenter.transform.position - transform.position;
    }

    // Convert gale strength level to actual float value
    public float GaleStrength()
    {
        return galeLvl; // TODO: figure out strength calculation
    }
}
