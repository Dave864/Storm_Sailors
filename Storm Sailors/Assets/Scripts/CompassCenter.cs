using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassCenter : MonoBehaviour
{
    public float compassRadius = 2;                    // Adjustable radius of cloud rail; to be matched with the cloud rail sprite
    public float rotRate = 0.1f;                    // The time in sec of each rotation about the center
    public Vector2 strtPos = new Vector2(0, 1);     // Intial orientation of the center of the cloud rail

    // Holds rotations corresponding to the positions that the wizard can be at on the cloud rail
    private Dictionary<Vector2, float> cardinalPositions = new Dictionary<Vector2, float>();

    private void Awake()
    {
        // Initialize the cardinalPositions dictionary
        cardinalPositions.Add(new Vector2(0, 1), 0.0f);     // position N
        cardinalPositions.Add(new Vector2(1, 1), 45.0f);    // position NE
        cardinalPositions.Add(new Vector2(1, 0), 90.0f);    // position E
        cardinalPositions.Add(new Vector2(1, -1), 135.0f);  // position SE
        cardinalPositions.Add(new Vector2(0, -1), 180.0f);  // position S
        cardinalPositions.Add(new Vector2(-1, -1), 225.0f); // position SW
        cardinalPositions.Add(new Vector2(-1, 0), 270.0f);  // position W
        cardinalPositions.Add(new Vector2(-1, 1), 315.0f);  // position NW

        // Set the center rotation to be at position N to allow for proper orientation of wizard
        //transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), transform.up);

        // Set the center rotation to be aligned with start position
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, cardinalPositions[strtPos], 0)), 1.0f);
    }

    // Returns the angle of the position with respect to position N
    public float PositionAngle(Vector2 position)
    {
        if (cardinalPositions.ContainsKey(position))
        {
            return cardinalPositions[position];
        }
        else
        {
            return 0.0f;
        }
    }
}
