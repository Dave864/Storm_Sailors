using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThunderhead : MonoBehaviour
{
    // Direction the thunderhead blows the wind
    [HideInInspector] public Vector3 galeVector = new Vector3(0, 0, 0);

    // Initialize thunderhead
    private void Start()
    {
        galeVector = transform.rotation.eulerAngles;
    }
}
