using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

    Color pipeColor;
    List<Vector3> pointList = new List<Vector3>();


    public Pipe(List<Vector3> list, Color color)
    {
        pointList = list;
        pipeColor = color;

		
	}

    public void setPipe(List<Vector3> list, Color color)
    {
        pointList = list;
        pipeColor = color;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
