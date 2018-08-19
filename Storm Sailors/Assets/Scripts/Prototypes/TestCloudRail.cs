using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCloudRail : MonoBehaviour
{
    public float railRadius = 2;                // Adjustable radius of cloud rail; to be matched with the cloud rail sprite
    public float rotRate = 0.1f;                // The time in sec of each rotation about the center
    private Vector2 rotVel = new Vector2(0, 0); // The current velocity of the rotation about the center

    // Holds rotations corresponding to the positions that the wizard can be at on the cloud rail
    private Dictionary<Vector2, float> cardinalPositions = new Dictionary<Vector2, float>();

    private void Awake()
    {
        // Set the initial center rotation to be at position N
        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), transform.up);
        
        // Initialize the cardinalPositions dictionary
        cardinalPositions.Add(new Vector2(0, 1), 0.0f);     // position N
        cardinalPositions.Add(new Vector2(1, 1), 45.0f);   // position NE
        cardinalPositions.Add(new Vector2(1, 0), 90.0f);   // position E
        cardinalPositions.Add(new Vector2(1, -1), 135.0f); // position SE
        cardinalPositions.Add(new Vector2(0, -1), 180.0f);  // position S
        cardinalPositions.Add(new Vector2(-1, -1), 225.0f);  // position SW
        cardinalPositions.Add(new Vector2(-1, 0), 270.0f);   // position W
        cardinalPositions.Add(new Vector2(-1, 1), 315.0f);   // position NW
    }

    // Returrns the angle of the position with respect to position N
    public float PositionAngle(Vector2 position)
    {
        if (cardinalPositions.ContainsKey(position))
        {
            return cardinalPositions[position];
        }
        else
        {
            Debug.Log("Invaild key " + position);
            return 0.0f;
        }
    }

    // Rotate about the center
    public void RotateCenter(Vector2 strtVect, Vector2 endVect)
    {
        // Vector3 strtVect3 = new Vector3(strtVect.x, 0, strtVect.y);
        // Vector3 endVect3 = new Vector3(endVect.x, 0, endVect.y);
        float rotAmt = Vector2.SignedAngle(strtVect, endVect);
        transform.Rotate(Vector3.forward * Time.deltaTime * rotAmt);
    }
}
