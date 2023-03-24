using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [SerializeField] int sensHor;
    [SerializeField] int sensVer;

    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    [SerializeField] bool invertX;


    [SerializeField] float recoilAngleModifier;
    float xRotation;

    bool recoilUp = false;
    bool recoilDown = false;
    private bool webGL = false;
    float recoilSpeed = 0.0f;
    float remainingRecoilAngle = 0.0f;
    float recoilAngle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            webGL = true;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVer;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

        if (webGL) {
            mouseX *= 0.1f;
            mouseY *= 0.1f;
        }

        if ( invertX )
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        doRecoil();

        rotateVertical();

        //rotate the player on its T-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    void rotateVertical()
    {
        //clamp the camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

        //rotatate the camera on the X-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
    public void startRecoil(float _force)
    {
        recoilSpeed = _force * -recoilAngleModifier;
        recoilUp = true;
        recoilDown = false;
        recoilAngle = recoilSpeed / recoilAngleModifier;
        remainingRecoilAngle = recoilAngle;
    }

    void doRecoil()
    {
        // If the gun is recoiling, rotate the camera up or down accordingly
        if (recoilUp)
        {
            xRotation += recoilSpeed * Time.deltaTime;
            remainingRecoilAngle -= recoilSpeed * Time.deltaTime;

            if (remainingRecoilAngle >= 0.0f)
            {
                recoilUp = false;
                recoilDown = true;
                remainingRecoilAngle = -recoilAngle / 2.0f;
                recoilSpeed /= 1.5f;
            }
        }
        else if (recoilDown)
        {
            xRotation -= recoilSpeed * Time.deltaTime;
            remainingRecoilAngle += recoilSpeed * Time.deltaTime;
            if (remainingRecoilAngle <= 0.0f)
            {
                recoilDown = false;
            }
        }
    }
    #region Accessors
    public int Sensitivity_Horizontal { get { return sensHor; } set { sensHor = value; } }
    public int Sensitivity_Vertical { get { return sensVer; } set { sensVer = value; } }
    public bool Invert_X { get { return invertX; } set { invertX = value; } }
    #endregion
}
