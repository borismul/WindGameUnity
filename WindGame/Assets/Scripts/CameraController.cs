using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    [Header("Camera Speeds")]
    public float maxTranslationSpeed = 50;
    public float minTranslationSpeed = 3f;
    public float camSpeedRot = 30;
    public float scrollSpeed = 1000;
    public float baseZoomSpeed = 1;
    public float maxZoomSpeed = 40;

    [Header("Translation Formula variable")]
    public float translationFormulaPow = 0.8f;

    [Header("Camera lerp speeds")]
    public float camLerpTrans = 10;
    public float camLerpScroll = 8;
    public float camLerpRot = 10;

    [Header("Camera position boundaries")]
    public float maxHeight = 200;
    public float minHeight = 50;

    [Header("Automatic tilt variables")]
    public float maxTilt = 70;
    public float minTilt = 40;
    public float maxTiltHeight = 200;
    public float tiltFormulaPow = 0.6f;

    [Header("Start Variables")]
    public float camLerpScrollStart;

    [Header("Camera Boundaries")]
    public float minX = 1400;
    public float maxX = 2600;
    public float minZ = 1400;
    public float maxZ = 2600;

    Vector3 targetPos;
    Vector3 targetRot;
    float prevHitPointY;
    float hitPointDiff;
    public float camHeight;
    float tiltDeg;

    float xInput;
    float zInput;
    float xMouse;

    float scrollInput;
    bool middleMouse;
    bool overRuleCam;
    bool setStartPos;
    public bool hasStarted;
    //[HideInInspector]
    public bool haveControl;

    TerrainController terrain;
    float camScrollLerpSet;

    Vector3 rotatePoint;
    float hitDistance;
    public LayerMask groundLayer;
    // Unity Methods //

    // Determine initial conditions


    void Start()
    {
        terrain = TerrainController.thisTerrainController;
        CameraStart();

    }

    // Update inputs and process
    void Update()
    {
        if (!terrain.levelLoaded)
            return;

        if (!setStartPos)
        {
            StartGame();
        }

        if(!hasStarted)
            CheckStart();

        if (!haveControl)
            return;

        UpdateCamHeight();
        GetInput();
        targetPos = ProcessInput();
    }

    // Update camera position and rotation
    void LateUpdate()
    {
        if (!terrain.levelLoaded)
            return;


        if (haveControl)
            transform.rotation = UpdateRotation();

        transform.position = UpdatePosition();

    }

    // Own Methods //
    void CameraStart()
    {
        float xPos = terrain.length / 2;
        float zPos = terrain.width / 2;
        float yPos = terrain.width / Mathf.Atan(Camera.main.fieldOfView);
        transform.position = new Vector3(xPos, yPos, zPos);
        transform.rotation = Quaternion.Euler(90, 0, 0);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = terrain.width / 2;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        targetPos = transform.position;
    }

    void StartGame()
    {
        Camera.main.orthographic = false;
        UpdateCamHeight();

        targetPos = new Vector3(terrain.width/2, transform.position.y - camHeight + maxHeight, terrain.length/2 - terrain.tileSize * 5);
        camScrollLerpSet = camLerpScrollStart;
        setStartPos = true;
    }

    void CheckStart()
    {
        if(terrain.levelLoaded && !hasStarted)
        {
            transform.LookAt(new Vector3(terrain.width/2, 0, terrain.length/2));
            if (Vector3.Distance(transform.position, targetPos) < 5)
            {
                hasStarted = true;
                haveControl = true;
                camScrollLerpSet = camLerpScroll;
            }
        }
    }

    // Method determines the user inputs required for the camera
    void GetInput()
    {
        // Determine horizontal, vertical input, the scrollwheel of the mouse and if the middle mouse button is clicked
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");

        // Check if the mouse hovers over a menu which can be scrolled, only allow camera scrolling when this is not the case
        if (!PointerInfo.inScrollableArea)
        {
            scrollInput = -Input.GetAxis("Mouse ScrollWheel");
        }
        else
        {
            scrollInput = 0;
        }
        middleMouse = Input.GetMouseButton(2);

        if (Input.GetMouseButtonDown(2))
        {
            RaycastHit hit;
            Physics.Raycast(targetPos, Camera.main.transform.forward, out hit, Mathf.Infinity);
            rotatePoint = hit.point + ProcessInput() - targetPos;
            hitDistance = Vector3.Distance(rotatePoint, transform.position);

            // Set overrule to true so the camera is moved wrt mouse movement
            overRuleCam = true;
        }

        // Only track the mouse movement if the middle mouse button is being clicked
        if (middleMouse)
        {
            xMouse = Input.GetAxis("Mouse X");
            rotatePoint += ProcessInput() - targetPos;

            //hitDistance += hitPointDiff * Mathf.Cos(Mathf.Deg2Rad * tiltDeg);
        }
        else
        {
            xMouse = 0;
            overRuleCam = false;
        }
    }

    // Method the processes the camera inputs and sets the target position of the camera
    Vector3 ProcessInput()
    {
        // Determine a translation step for the vertical and horizontal input
        float dX = TranslationInputStrength() * xInput * Time.deltaTime;
        float dZ = TranslationInputStrength() * zInput * Time.deltaTime;

        // Determine a target position in the y axis for the camera
        float targetY = TargetY();

        // create a forward and right vector, only containing the horizontal plane of the camera wrt the ground (
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        Vector3 right = transform.right;
        right.y = 0;
        right = Vector3.Normalize(right);

        // Add the input to the previous position
        Vector3 Horizon = targetPos + forward * dZ + right * dX;
        Horizon.y = 0;

        Vector3 targetPosTry = Horizon + new Vector3(0, targetY, 0);

        if (targetPosTry.x < minX)
        {
            targetPosTry.x = minX;
        } 
        if(targetPosTry.x > maxX)
        {
            targetPosTry.x = maxX;
        }
        if(targetPosTry.z < minZ)
        {
            targetPosTry.z = minZ;
        }
        if(targetPosTry.z > maxZ)
        {
            targetPosTry.z = maxZ;
        }

        return targetPosTry;
    }

    // Method determines the strength of the input in horizontal plane. Bigger camHeight = faster movement, lower camHeight = lower movement
    float TranslationInputStrength()
    {
        // variable that measure how high the camera is wrt to the maximum height above which the camera movement speed stays constant
        float heightPercentage;

        // If the camera height is below the maximum camera speed zone, calculate the percentage of how high the camera is wrt the highest point where the camera movement speed stays constant
        if (camHeight < maxTiltHeight)
            heightPercentage = (camHeight - minHeight) / (maxTiltHeight - minHeight);
        // Else set this to 1 (maximum movement speed)
        else
            heightPercentage = 1;

        // If the camera glitches through the ground this will help filtering the NaN errors
        if (heightPercentage < 0)
            heightPercentage = 0;

        // Determine the strength based on this formula: variableRange * heightPercentage^translationFormulaPow + minTilt
        float strength = (maxTranslationSpeed - minTranslationSpeed) * Mathf.Pow(heightPercentage, translationFormulaPow) + minTranslationSpeed;

        return strength;
    }

    // This method determines the target height of the camera 
    float TargetY()
    {
        // initialize the camera height to 0
        float targetY = 0;

        // Determine a factor that makes the camera go slower per scroll step, if the camera is lower to the ground
        float zoomHeightFactor = (((camHeight - minHeight) / maxHeight - minHeight));

        // Limit the zoomHeightFactor to a maximum value and minimum value so the camera will not shoot of in space or go to slow.
        zoomHeightFactor = Mathf.Clamp(zoomHeightFactor, baseZoomSpeed, maxZoomSpeed);

        // Determine a y movement of the camera based on the input the scrollspeed setting the previously calculated zoomHeightFactor.
        // Also move the camera up and down based on how the terrain change underneath due to horizontal camera movement (hitPointDiff)
        float y = scrollInput * scrollSpeed * zoomHeightFactor * Time.deltaTime + hitPointDiff;

        // Calculate the difference in height of the ground wrt to the absolute axis system
        float heightDiff = transform.position.y - camHeight;

        // Set the targetHeight of the camera wrt the ground (with diffHeight)
        float targetHeight = targetPos.y - heightDiff + y;

        // If this is in between the set ranges set the camera target to the global axis system (without diffHeight)
        if (targetHeight < maxHeight && targetHeight > minHeight)
            targetY = targetPos.y + y;

        // Else if the targetHeight goes above the set maximum boundary
        else if (targetHeight >= maxHeight)
        {
            // if the camera input is downward accept it
            if (y <= 0)
                targetY = targetPos.y + y;
            // Else set to maximum camera height
            else
                targetY = maxHeight + heightDiff;
        }
        // Else if the targetHeight goes below the set minimum boundary
        else if (targetHeight <= minHeight)
        {
            // if the camera input is upward accept it
            if (y >= 0)
                targetY = targetPos.y + y;
            // Else set the minimum camera height
            else
                targetY = minHeight + heightDiff;
        }
        return targetY;
    }

    // Method updates the position of the camera based on the current position and the target position
    Vector3 UpdatePosition()
    {
        Vector3 heightPos;
        if (!hasStarted)
        {
            camScrollLerpSet *= 1.001f;
            heightPos = Vector3.MoveTowards(new Vector3(0, transform.position.y, 0), new Vector3(0, targetPos.y, 0), camScrollLerpSet * Time.deltaTime);

        }
        else
        {
            // Move the camera towards the target by small steps, going slower if it gets closer to the target (Lerp function)
            heightPos = Vector3.Lerp(new Vector3(0, transform.position.y, 0), new Vector3(0, targetPos.y, 0), camScrollLerpSet * Time.deltaTime);
        }
        Vector3 transPos = Vector3.Lerp(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPos.x, 0, targetPos.z), camLerpTrans * Time.deltaTime);

        // Combine the height and translational movement to a current position
        Vector3 curPos = heightPos + transPos;
        return curPos;
    }

    // Update the Rotation of the camera, based on the current rotation and the target rotation
    Quaternion UpdateRotation()
    {
        // If the camera is not overruled tilt it automatically
        if (!overRuleCam)
        {
            // Determine the automatic tilt angle
            targetRot = AutoTilt();
        }
        // Else, let the camera be moved manually
        else
        {
            // determine the camera rotation based the mouse input of the user
            ManualRot(rotatePoint);
            //targetRot = transform.rotation.eulerAngles;
        }

        // Set a camera position and move it slowly towards the target position (Lerp function)
        Quaternion camRot = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), camLerpRot * Time.deltaTime);

        return camRot;
    }

    // Method that determines the tilt angle of the camera based on the height of the camera
    Vector3 AutoTilt()
    {
        // Determine the tilt angle of the camera
        tiltDeg = Tilt();

        // set it as the x euler angle (pitch) and keep the y eulerangle the same (yaw), while keeping the z eulerangle (roll) always 0
        Vector3 newTargetRot = new Vector3(tiltDeg, targetRot.y, 0f);
        return newTargetRot;
    }

    // Method determine the tilt angle based on the camera height
    float Tilt()
    {

        // Calculate a percentage of how high the camera is in the variable tilt range
        float heightPercentage = 0;
        if (camHeight < maxTiltHeight)
            heightPercentage = (camHeight - minHeight) / (maxTiltHeight - minHeight);
        // if not in the variable tilt range, set heightPercentage to 1
        else
            heightPercentage = 1;

        // Resolve NaN problems on camera glitches
        if (heightPercentage < 0)
            heightPercentage = 0;

        // Tilt the camera based on the heightPercentage with this formula: variableRange * heightPercentage^tiltFormulaPow + minTilt
        float tilt = (maxTilt - minTilt) * Mathf.Pow(heightPercentage, tiltFormulaPow) + minTilt;

        return tilt;
    }

    // Method determines the rotation of the camera in x and y axis based on user mouse input
    void ManualRot(Vector3 hit)
    {
        float movement = xMouse * camSpeedRot * Time.deltaTime;

        targetRot += new Vector3(0, movement, 0);
        targetPos = Quaternion.Euler(targetRot) * new Vector3(0, 0, -hitDistance) + hit;
        targetPos.y += hitPointDiff;
        // Determine the camera rotation in x and y axis speed based on mouse user input
        targetPos = targetPos + Camera.main.transform.right * movement;

    }

    // Method determines the camera height and saves some values for next run
    void UpdateCamHeight()
    {
        // Hit a raycast down on the ground
        RaycastHit hit;

        // TEMPORARY if the ground can't be hit set the camera back to the previous location (camera went of terrain) TEMPORARY //
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            //targetPos = previousPos;
            hitPointDiff = 0;
            return;
        }

        // determine the camera height
        camHeight = hit.distance;        

        // Determine the difference in ground level wrt last iteration
        hitPointDiff = hit.point.y - prevHitPointY;

        // save the current hitpoint as the previous
        prevHitPointY = hit.point.y;
    }

    public void SetHaveControl(bool haveControl)
    {
        this.haveControl = haveControl;
    }

    public bool GetHaveControl()
    {
        return haveControl;
    }

}
