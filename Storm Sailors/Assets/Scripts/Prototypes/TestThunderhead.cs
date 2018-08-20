using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThunderhead : MonoBehaviour
{
    // Direction the thunderhead blows the wind
    private Vector3 galeVector = new Vector3(0, 0, 0);

    public Vector3 GaleVector
    {
        get
        {
            return galeVector;
        }
    }

    private void Awake()
    {
        // Initialize gale vector
        GameObject railCenter = GameObject.Find("Rail Center");
        galeVector = railCenter.transform.position - transform.position;
    }
}
