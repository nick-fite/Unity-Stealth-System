using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Left Shoulder")]
    [SerializeField] private Transform leftShoulderDefault;
    [SerializeField] private Transform leftShoulderADS;
    [SerializeField] private Transform leftShoulderCrouchADS;
    [SerializeField] private GameObject leftShoulder;

    [Header("Right Shoulder")]
    [SerializeField] private Transform rightShoulderDefault;
    [SerializeField] private Transform rightShoulderADS;
    [SerializeField] private Transform rightShoulderCrouchADS;
    [SerializeField] private GameObject rightShoulder;

    [Header("Camera")]
    [SerializeField] private GameObject playerCam;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float ADSFOV;

    [Header("Gun")]
    [SerializeField] private GameObject gun;
    [SerializeField] private Transform DefaultGunPos;
    [SerializeField] private GameObject detail;
    [SerializeField] private Transform defaultDetailPos;
    [SerializeField] private Transform shootingDetailPos;
    [Range(0, 10)][SerializeField] private float gunAnimSpeed;

    [Range(0, 100)][SerializeField] private float AimSpeed = 50;
    
    private List<bool> fireList;

    private void Start()
    {
        fireList = new List<bool>();
    }

    public void SetShoot()
    {
        leftShoulder.transform.rotation = leftShoulderADS.rotation;
        rightShoulder.transform.rotation = rightShoulderADS.rotation;
    }

    public void UnSetArms()
    {
        leftShoulder.transform.rotation = leftShoulderDefault.rotation;
        rightShoulder.transform.rotation = rightShoulderDefault.rotation;
    }

    public void setArmCrouch()
    {
        leftShoulder.transform.rotation = leftShoulderADS.rotation;
        rightShoulder.transform.rotation = rightShoulderADS.rotation;
    }

    public IEnumerator ADSAnim() 
    {
        float rateLeft = 1.0f / Quaternion.Angle(leftShoulder.transform.rotation, leftShoulderADS.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, rightShoulderADS.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            leftShoulder.transform.rotation = Quaternion.Lerp(leftShoulder.transform.rotation, leftShoulderADS.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, rightShoulderADS.rotation, tRight);
            yield return null;
        }
        yield return null;
    }

    public IEnumerator UnsetShootAnim()
    {
            float rateLeft = 1.0f / Quaternion.Angle(leftShoulder.transform.rotation, leftShoulderDefault.rotation) * AimSpeed;
            float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, rightShoulderDefault.rotation) * AimSpeed;

            float tLeft = 0.0f;
            float tRight = 0.0f;

            while (tLeft < 1.0f || tRight < 1.0f )
            {
                tLeft += Time.deltaTime * rateLeft;
                tRight += Time.deltaTime * rateRight;

                leftShoulder.transform.rotation = Quaternion.Lerp(leftShoulder.transform.rotation, leftShoulderDefault.rotation, tLeft);
                rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, rightShoulderDefault.rotation, tRight);
                yield return null;
            }

        yield return null;
    }

    public IEnumerator ADSUnaim()
    {
        float rateLeft = 1.0f / Quaternion.Angle(leftShoulder.transform.rotation, leftShoulderDefault.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, rightShoulderDefault.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            leftShoulder.transform.rotation = Quaternion.Lerp(leftShoulder.transform.rotation, leftShoulderDefault.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, rightShoulderDefault.rotation, tRight);
            yield return null;
        }

        gun.transform.position = DefaultGunPos.position;
        gun.transform.rotation = DefaultGunPos.rotation;
        yield return null;
    }

    public IEnumerator crouchADSAim() 
    {
        float rateLeft = 1.0f / Quaternion.Angle(leftShoulder.transform.rotation, leftShoulderCrouchADS.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, rightShoulderCrouchADS.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            leftShoulder.transform.rotation = Quaternion.Lerp(leftShoulder.transform.rotation, leftShoulderCrouchADS.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, rightShoulderCrouchADS.rotation, tRight);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator crouchADSUnaim()
    {
        float rateLeft = 1.0f / Quaternion.Angle(leftShoulder.transform.rotation, leftShoulderADS.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, rightShoulderADS.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            leftShoulder.transform.rotation = Quaternion.Lerp(leftShoulder.transform.rotation, leftShoulderADS.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, rightShoulderADS.rotation, tRight);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator ShootAnim(bool ADS)
    {
        if (!ADS)
        {
            fireList.Add(true);
        }

        float rate = 1.0f / Vector3.Distance(detail.transform.position, shootingDetailPos.position) * gunAnimSpeed;

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            detail.transform.position = Vector3.Lerp(detail.transform.position, shootingDetailPos.position, t);
            yield return null;
        }

        rate = 1.0f / Vector3.Distance(detail.transform.position, defaultDetailPos.position) * gunAnimSpeed;

        t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            detail.transform.position = Vector3.Lerp(detail.transform.position, defaultDetailPos.position, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log(fireList.Count);
        if (fireList.Count <= 1 && !ADS)
        {
            yield return UnsetShootAnim();
        }
        
        if (fireList.Count > 0)
        {
            fireList.RemoveAt(fireList.Count - 1);
        }

        yield return null;
    }

    public IEnumerator FOVZoomIn() 
    {
        float rateFOV = 1.0f / Mathf.Abs(ADSFOV - defaultFOV) * AimSpeed;

        float tFOV = 0.0f;

        while (tFOV < 1.0f)
        {

            tFOV += Time.deltaTime * rateFOV;

            playerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = Mathf.Lerp(playerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, ADSFOV, tFOV);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator FOVZoomOut()
    {
        float rateFOV = 1.0f / Mathf.Abs(ADSFOV - defaultFOV) * AimSpeed;

        float tFOV = 0.0f;

        while (tFOV < 1.0f)
        {

            tFOV += Time.deltaTime * rateFOV;

            playerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = Mathf.Lerp(playerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, defaultFOV, tFOV);
            yield return null;
        }

        yield return null;
    }
}
