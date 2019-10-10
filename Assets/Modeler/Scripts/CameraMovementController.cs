using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour {

    public GameObject pointModeler;
    private Vector3 savePosition;
    public PointController pointController;
    private float distanceTreshold = 0.01f;
    public bool modelFlag=false;
    public GameObject snap;
    private Vector3 saveLastPos;

	// Use this for initialization
	void Start () {
        
        savePosition = pointModeler.transform.position;
	}

    
	
	// Update is called once per frame
	void Update () {

        Vector3 pointModelerPosition = pointModeler.transform.position;
        if (Vector3.Distance(savePosition, pointModelerPosition) > distanceTreshold && modelFlag)
        {
            pointController.addPoint(snapPostionChange(pointModelerPosition));
        }
        savePosition = pointModelerPosition;
    }


    public Vector3 snapPostionChange(Vector3 realPos)
    {
        Vector3 changedPos;
        changedPos = realPos;

        return changedPos;
    }

}

