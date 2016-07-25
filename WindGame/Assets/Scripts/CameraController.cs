using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float maxTranslationSpeed = 50;
    public float minTranslationSpeed = 3f;

    public float scrollSpeed = 50;

    public float camSpeedTrans = 10;
    public float camSpeedScroll = 8;
    public float camSpeedRot = 10;
    public float camMouseSpeed = 30;

    public float maxHeight = 200;
    public float minHeight = 50;

    public float maxTilt = 70;
    public float minTilt = 40;
    public float maxTiltHeight = 200;

    public float zoomFactor = 2;
    public float baseZoomSpeed = 1;
    public float maxZoomSpeed = 40;

    Vector3 previousPos;

    Vector3 targetPos;
    Quaternion targetRot;
    float prevHitPointY;
    float hitPointDiff;
    float camHeight;

    float xInput;
    float zInput;
    float xMouse;
    float yMouse;

    float scrollInput;
    bool middleMouse;
    bool overRuleCam;
	
    void Start()
    {
        targetPos = transform.position;
        previousPos = transform.position;
    }

    void Update ()
    {
        GetInput();
        ProcessInput();
    }

    void FixedUpdate()
    {
        UpdateCamHeight();
    }

    void LateUpdate()
    {
        transform.position = UpdatePosition();
        transform.rotation = UpdateRotation();
    }

    void GetInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        scrollInput = -Input.GetAxis("Mouse ScrollWheel");
        middleMouse = Input.GetMouseButton(2);

        if (middleMouse)
        {
            xMouse = Input.GetAxis("Mouse X");
            yMouse = Input.GetAxis("Mouse Y");

            overRuleCam = true;
        }
        else
        {
            xMouse = 0;
            yMouse = 0;
        }

        if (scrollInput != 0)
            overRuleCam = false;
    }

    void ProcessInput()
    {
        float targetX = TranslationInputStrength() * xInput * Time.deltaTime;
        float targetZ =TranslationInputStrength() * zInput * Time.deltaTime;
        float targetY = TargetY();

        Vector3 temp = targetPos + transform.forward * targetZ + transform.right * targetX;

        temp.y = 0;
        targetPos =  temp + new Vector3(0, targetY, 0);
    }

    float TranslationInputStrength()
    {
        float upper = maxTiltHeight - minHeight;
        float heightPercentage = 0;

        if (camHeight < maxTiltHeight)
            heightPercentage = (camHeight - minHeight) / upper;
        else
            heightPercentage = (maxTiltHeight - minHeight) / upper;

        if (heightPercentage < 0)
            heightPercentage = 0;

        float strength = (maxTranslationSpeed - minTranslationSpeed) * Mathf.Pow(heightPercentage, 0.8f) + minTranslationSpeed;

        return strength;
    }

    float TargetY()
    {
        float targetY = 0;
        float zoomHeightFactor = (((camHeight - minHeight) / (maxHeight - minHeight)) * zoomFactor) + baseZoomSpeed;
        if (zoomHeightFactor > maxZoomSpeed)
        {
            zoomHeightFactor = maxZoomSpeed;
        }
        float y = scrollInput * scrollSpeed * zoomHeightFactor + hitPointDiff;
        float heightDiff = transform.position.y - camHeight;
        float targetHeight = targetPos.y - heightDiff + y;

        if (targetHeight < maxHeight && targetHeight > minHeight)
            targetY = targetPos.y + y;
        else if (targetHeight >= maxHeight)
        {
            if (y < 0)
                targetY = targetPos.y + y;
            else
                targetY = maxHeight + heightDiff;
        }
        else if (targetHeight <= minHeight)
        {
            if (y > 0)
                targetY = targetPos.y + y;
            else
                targetY = minHeight + heightDiff;
        }

        return targetY;
    }

    Vector3 UpdatePosition()
    {
        Vector3 heightPos = Vector3.Lerp(new Vector3(0, transform.position.y, 0), new Vector3(0, targetPos.y, 0), camSpeedScroll * Time.deltaTime);
        Vector3 transPos = Vector3.Lerp(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPos.x, 0, targetPos.z), camSpeedTrans * Time.deltaTime);

        Vector3 curPos = heightPos + transPos;
        return curPos;
    }

    Quaternion UpdateRotation()
    {
        if (!overRuleCam)
        {
            float tiltDeg = Tilt();
            targetRot = Quaternion.Euler(new Vector3(tiltDeg, transform.rotation.eulerAngles.y, 0f));
            Quaternion camRot = Quaternion.Lerp(transform.rotation, targetRot, camSpeedRot * Time.deltaTime);

            return camRot;
        }
        else
        {
            float eulX = -yMouse * camMouseSpeed * Time.deltaTime;
            float eulY = xMouse * camMouseSpeed * Time.deltaTime;

            targetRot = Quaternion.Euler(targetRot.eulerAngles + new Vector3(eulX, eulY, 0));
            Quaternion camRot = Quaternion.Lerp(transform.rotation, targetRot, camSpeedRot * Time.deltaTime);

            return camRot;
        }
    }

    float Tilt()
    {
        float upper = maxTiltHeight - minHeight;
        float heightPercentage = 0;

        if (camHeight < maxTiltHeight)
            heightPercentage = (camHeight - minHeight) / upper;
        else
            heightPercentage = (maxTiltHeight - minHeight) / upper;

        if (heightPercentage < 0)
            heightPercentage = 0;

        float tilt = (maxTilt - minTilt) * Mathf.Pow(heightPercentage, 0.6f)+ minTilt;

        return tilt;
    }

    void UpdateCamHeight()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            targetPos = previousPos;
        else
            previousPos = transform.position;
        camHeight = hit.distance;
        if (camHeight == 0)
            camHeight = minHeight;
        hitPointDiff = hit.point.y - prevHitPointY;
        prevHitPointY = hit.point.y;
    }
}
