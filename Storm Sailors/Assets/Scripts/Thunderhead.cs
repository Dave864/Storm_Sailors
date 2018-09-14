using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderhead : MonoBehaviour
{
    // Flag indicating if the tunderhead is being held
    [HideInInspector] public bool isHeld = false;

    // Strength level of the thunderhead gale
    public int GaleLvl { get; set; }

    // Direction the thunderhead blows the wind
    private Vector3 galeVector = new Vector3(0, 0, 0);
    public Vector3 GaleVector
    {
        get { return galeVector; }
        set { galeVector = value; }
    }

    // Reference to game objects
    private GameObject compassCenter;

    // Initializes the thuderhead
    private void Awake()
    {
        // Initialize gale vector
        compassCenter = GameObject.Find("Compass Center");
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        galeVector = compassCenter.transform.position - transform.position;
        GaleLvl = 1;
    }

    // Called once per frame
    private void Update()
    {
        if (isHeld)
        {
            transform.LookAt(compassCenter.transform.position, Vector3.up);
        }
    }

    // Convert gale strength level to some float value
    public float GaleStrength()
    {
        return GaleLvl; // TODO: figure out strength calculation
    }

    // Merge thunderheads, the thunderhead that is passed into the function is destroyed
    public void Merge(GameObject thunderheadToMerge)
    {
        Thunderhead thunderheadObj = thunderheadToMerge.GetComponent<Thunderhead>();
        if(thunderheadObj != null)
        {
            GaleLvl += thunderheadObj.GaleLvl;
            Destroy(thunderheadToMerge);
        }
    }
}
