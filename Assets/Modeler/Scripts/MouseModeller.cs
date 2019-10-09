using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using GoogleARCore;

public class MouseModeller : MonoBehaviour {

    public PointController pointController;
    public GameObject selectedAnchor;
    public Material anchorSelectMaterial;
    public Material anchorUnSelectMaterial;
    public Text debugText;
    public Camera firstPersonCamera;

    //Pick and Delete Anchor
    private bool pickAnchorFlag;
    private bool deleteAnchorFlag;

    //Create Anchor
    private DetectedPlane detectedPlane;
    public GameObject myAnchor;
    public GameObject whiteBall;
    private bool createAnchorAirFlag;
    private bool createAnchorPlaneFlag;
    private const float k_ModelRotation = 180.0f;
    private bool m_IsQuitting;

    //Buttons
    public Button createAnchorPlaneBtn;
    public Button createAnchorAirBtn;
    public Button selectAnchorBtn;
    public Button deseletAnchorBtn;
    public Button deleteAnchorBtn;
    public Button modelAirBtn;
    public Button modelTouchBtn;

    //Model
    private bool modelAirFlag;
    private bool modelTouchFlag;
    public Text textModelAirBtn;
    public Text textModelTouchlBtn;

    private Vector3 saveWhiteBallPos;
    private bool saveWhiteBallPosFlag;

    // Use this for initialization
    void Start () {
        whiteBall.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem.currentSelectedGameObject != null) { 
            return;
        }

        if (Input.touchCount>0)
        {
            //Create Anchor
            createAnchorPlane();

            //Edit Anchor
            pickAnchor();
            deleteAnchor();
        }
        //Create Anchor
        createAnchorAir();

        //Model
        modelAir();
        modelAnchorAir();
        modelTouch();
        modelAnchorTouch();
    }

    void modelAir()
    {

        if (modelAirFlag && selectedAnchor == null && Input.touchCount>0 && !PointController.instance.fCP.gameObject.activeSelf)
        {
            PointController.instance.addPoint(whiteBall.transform.position);
        }

    }

    void modelAnchorAir()
    {
        if (modelAirFlag && selectedAnchor != null && Input.touchCount > 0 && !PointController.instance.fCP.gameObject.activeSelf)
        {
            //Anchor logic
            if (!saveWhiteBallPosFlag)
            {
                saveWhiteBallPos = whiteBall.transform.position;
                saveWhiteBallPosFlag = true;
            }
            Vector3 pos = (whiteBall.transform.position - saveWhiteBallPos) + selectedAnchor.transform.position;
            PointController.instance.addPoint(pos);
        }

        if (modelAirFlag && Input.touchCount==0)
        {
            saveWhiteBallPosFlag = false;
        }
    }

    private void modelTouch()
    {
        if (modelTouchFlag && selectedAnchor == null && Input.touchCount > 0 && !PointController.instance.fCP.gameObject.activeSelf)
        {
            Vector3 touchPos = firstPersonCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y,0.3f));
            PointController.instance.addPoint(touchPos);
        }
    }

    private void modelAnchorTouch()
    {
        if (modelTouchFlag && selectedAnchor != null && Input.touchCount > 0 && !PointController.instance.fCP.gameObject.activeSelf)
        {
            //Anchor logic
            if (!saveWhiteBallPosFlag)
            {
                saveWhiteBallPos = firstPersonCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.3f));
                saveWhiteBallPosFlag = true;
            }
            Vector3 pos = (firstPersonCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.3f)) - saveWhiteBallPos) + selectedAnchor.transform.position;
            PointController.instance.addPoint(pos);
        }

        if (modelTouchFlag && Input.touchCount == 0)
        {
            debugText.text = "Reset Pos Touch Model";
            saveWhiteBallPosFlag = false;
        }
    }

    void createAnchorPlane()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && createAnchorPlaneFlag)
        {
            createAnchorAirFlag = false;
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(firstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Choose the Andy model for the Trackable that got hit.
                GameObject prefab = myAnchor;

                // Instantiate Andy model at the hit pose.
                var anchorObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Compensate for the hitPose rotation facing away from the raycast (i.e.
                // camera).
                anchorObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make Andy model a child of the anchor.
                anchorObject.transform.parent = anchor.transform;

                debugText.text = "Create Ball by hit plane";
                createAnchorPlaneFlag = false;
            }
        }
    }

    void createAnchorAir()
    {
        if (createAnchorAirFlag && Input.touchCount > 0)
        {
            var anchorObject = Instantiate(myAnchor, whiteBall.transform.position, whiteBall.transform.rotation);

            // Compensate for the hitPose rotation facing away from the raycast (i.e.
            // camera).
            anchorObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

            // Create an anchor to allow ARCore to track the hitpoint as understanding of
            // the physical world evolves.
            var anchor = Session.CreateAnchor(new Pose(whiteBall.transform.position, whiteBall.transform.rotation));

            // Make Andy model a child of the anchor.
            anchorObject.transform.parent = anchor.transform;

            debugText.text = "Create Ball in Enviroment";
            createAnchorAirFlag = false;
            whiteBall.SetActive(false);
        }    
    }

    void deleteAnchor()
    {
        RaycastHit hit;

        if (deleteAnchorFlag && Physics.Raycast(firstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Anchor")
            {
                Destroy(hit.collider.gameObject);
            }
            deleteAnchorFlag = false;
        }
    }

    void pickAnchor()
    {
        RaycastHit hit;
        //Pick New Anchor
        if (pickAnchorFlag && Physics.Raycast(firstPersonCamera.ScreenPointToRay(Input.GetTouch(0).position), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Anchor")
            {
                Debug.Log("Did Hit");
                if (selectedAnchor != null)
                {
                    //The selected Anchor changes to unselected
                    selectedAnchor.GetComponent<Renderer>().material = anchorUnSelectMaterial;
                }

                Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;

                //Change the anchor
                selectedAnchor = hit.collider.gameObject;
                selectedAnchor.GetComponent<Renderer>().material = anchorSelectMaterial;
                debugText.text = "Anchor Picked" + selectedAnchor.name;
            }

            pickAnchorFlag = false;
        }
    }

    public void changeAction(string action)
    {
        //Create Anchors
        createAnchorAirFlag=false;
        createAnchorPlaneFlag = false;

        //Pick Delete Anchors
        pickAnchorFlag = false;
        deleteAnchorFlag = false;

        switch (action)
        {
            case "pickAnchorFlag":
                pickAnchorFlag = true;
                debugText.text = "Pick Anchor";
                break;

            case "deleteAnchorFlag":
                deleteAnchorFlag = true;
                debugText.text = "Delete Anchor";
                break;

            case "createAnchorAirFlag":
                createAnchorAirFlag = true;
                debugText.text = "Create Anchor Air";
                whiteBall.SetActive(true);
                break;

            case "createAnchorPlaneFlag":
                createAnchorPlaneFlag = true;
                debugText.text = "Create Anchor Plane";
                break;

            case "modelAirFlag":
                if (modelAirFlag)
                {
                    deactivateButtons(true);
                    modelTouchBtn.interactable = true;
                    modelAirFlag = false;
                    textModelAirBtn.text = "Model Air";
                    whiteBall.SetActive(false);
                    saveWhiteBallPosFlag = false;
                    PointController.instance.clearList();
                }
                else
                {
                    modelAirFlag = true;
                    whiteBall.SetActive(true);
                    textModelAirBtn.text = "Stop Model Air";
                    deactivateButtons(false);
                    modelTouchBtn.interactable = false;
                }
                
                debugText.text = "Model Air";
                
                break;

            case "modelTouchFlag":
                if (modelTouchFlag) {
                    textModelTouchlBtn.text = "Model Touch";
                    modelTouchFlag = false;
                    saveWhiteBallPosFlag = false;
                    deactivateButtons(true);
                    modelAirBtn.interactable = true;
                    PointController.instance.clearList();
                }
                else
                {
                    textModelTouchlBtn.text = "Stop Model Touch";
                    modelTouchFlag = true;
                    deactivateButtons(false);
                    modelAirBtn.interactable = false;
                }
                break;
        }
    }

    void deactivateButtons(bool state)
    {
        createAnchorPlaneBtn.interactable = state;
        createAnchorAirBtn.interactable = state;
        selectAnchorBtn.interactable = state;
        deleteAnchorBtn.interactable = state;
        deseletAnchorBtn.interactable = state;
    }



    public void deselectAnchor()
    {
        selectedAnchor.GetComponent<Renderer>().material = anchorUnSelectMaterial;
        selectedAnchor = null;
    }

    public void resetScene()
    {
        SceneManager.LoadScene(0);
    }
}
