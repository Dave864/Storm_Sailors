using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassCenter : MonoBehaviour
{
    public float compassRadius = 2;                 // Adjustable radius of cloud rail; to be matched with the cloud rail sprite
    public float rotRate = 0.1f;                    // The time in sec of each rotation about the center
    public float shiftRate = 0.1f;                  // The time in sec for the wizard to shift to a new mode
    public Vector2 strtPos = new Vector2(0, 1);     // Intial orientation of the center of the cloud rail
    public float stormHeight = 0;                   // Position wizard takes when in storm mode

    // Holds rotations corresponding to the positions that the wizard can be at on the cloud rail
    private Dictionary<Vector2, Quaternion> cardinalRot = new Dictionary<Vector2, Quaternion>();

    private void Awake()
    {
        // Create entries in the dictionary to hold rotations of compass positions
        cardinalRot.Add(new Vector2(0, 1), transform.rotation);     // position N
        cardinalRot.Add(new Vector2(1, 1), transform.rotation);     // position NE
        cardinalRot.Add(new Vector2(1, 0), transform.rotation);     // position E
        cardinalRot.Add(new Vector2(1, -1), transform.rotation);    // position SE
        cardinalRot.Add(new Vector2(0, -1), transform.rotation);    // position S
        cardinalRot.Add(new Vector2(-1, -1), transform.rotation);   // position SW
        cardinalRot.Add(new Vector2(-1, 0), transform.rotation);    // position W
        cardinalRot.Add(new Vector2(-1, 1), transform.rotation);    // position NW

        List<Vector2> cardRotIndex = new List<Vector2>(cardinalRot.Keys);

        // Initialize the rotations of the compass positions
        for (int i = 0; i < cardRotIndex.Count; i++)
        {
            cardinalRot[cardRotIndex[i]] = Quaternion.Euler(new Vector3(0, (45.0f * i), 0) + transform.rotation.eulerAngles);
        }
    }

    private void Start()
    {
        // Set the center rotation to be aligned with start position
        transform.rotation = cardinalRot[strtPos];
    }

    // Returns rotation of position
    public Quaternion PositionRot(Vector2 position)
    {
        if (cardinalRot.ContainsKey(position))
        {
            return cardinalRot[position];
        }
        else
        {
            return transform.rotation;
        }
    }
}
