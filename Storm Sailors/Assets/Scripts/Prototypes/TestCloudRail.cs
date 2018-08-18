using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCloudRail : MonoBehaviour
{
    public float railRadius = 2;
    public float rotRate = 0.1f;                // The time in sec of each rotation about the center
    private Vector2 rotVel = new Vector2(0, 0); // The current velocity of the rotation about the center
	
	// Rotate about the center
    public void RotateCenter(Vector2 strtVect, Vector2 endVect)
    {
        // Vector3 strtVect3 = new Vector3(strtVect.x, 0, strtVect.y);
        // Vector3 endVect3 = new Vector3(endVect.x, 0, endVect.y);
        float rotAmt = Vector2.SignedAngle(strtVect, endVect);
        transform.Rotate(Vector3.forward * Time.deltaTime * rotAmt);
    }
}
