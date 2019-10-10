using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class PointController : MonoBehaviour {

    public static PointController instance;
    public GameObject myAnchor;
    public GameObject whiteBall;
    private List<Vector3> pointArray= new List<Vector3>();
    private List<Pipe> pipeList = new List<Pipe>();
    
    //Color
    public FlexibleColorPicker fCP;
    public Text fCPText;
    public Image colorInButton;

    //TrailRenderer
    public GameObject trail;
    public GameObject currentTrail;

    /// <summary>
    /// The rotation in degrees need to apply to model when the Andy model is placed.
    /// </summary>
    private const float k_ModelRotation = 180.0f;

    // Use this for initialization
    void Start () {
        if (instance==null){
            instance = this;
        }
        currentTrail.name = "NewTrail";
        
        Debug.Log("Awake Point Controller");
        fCP.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(fCP.color!= colorInButton.color)
        {
            colorInButton.color = fCP.color;
        }

        if (fCP.gameObject.activeSelf){
            colorInButton.color = fCP.color;
        }
    }


    public void toggleColorPicker()
    {
        if (fCP.gameObject.activeSelf) {
            fCP.gameObject.SetActive(false);
            fCPText.text = "Pick Color";
            currentTrail.gameObject.GetComponent<Material>().color = fCP.color;
        }
        else {
            fCP.gameObject.SetActive(true);
            fCPText.text = "Close Pick Color";
        }
    }

    public void addPoint(Vector3 position){

        if (currentTrail == null)
        {
            currentTrail = Instantiate(trail, position, whiteBall.transform.rotation);
            currentTrail.gameObject.GetComponent<Renderer>().material.SetColor("_Color", fCP.color);
        }


        //var anchorObject = Instantiate(myAnchor, position, whiteBall.transform.rotation);

        // Compensate for the hitPose rotation facing away from the raycast (i.e.
        // camera).
        //anchorObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

        // Create an anchor to allow ARCore to track the hitpoint as understanding of
        // the physical world evolves.
        
        var anchor = Session.CreateAnchor(new Pose(position, whiteBall.transform.rotation));

        // Make Andy model a child of the anchor.
        //anchorObject.transform.parent = anchor.transform;
        //anchorObject.GetComponent<Renderer>().material.SetColor("_Color", fCP.color);
        //pointArray.Add(anchorObject.transform.position);

        currentTrail.transform.position = anchor.transform.position;
    }

    public void clearList()
    {
        currentTrail = null;
        //Debug.Log("Pipe list:"+ pipeList.Count);
        //pipeList.Add(new Pipe(pointArray, fCP.color));
        //pointArray.Clear();
    }
}
