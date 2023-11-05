using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(StarterAssetsInputs))]
public class PlayerAnimator : MonoBehaviour
{
    enum EAnimState {ADS, UnADS, CrouchADS, CrouchUnADS, Shoot, UnsetShoot, None }
    enum EZoomState { ZoomIn, ZoomOut, None}
    [Header("Left Shoulder")]
    [SerializeField] private Transform LeftShoulderDefault;
    [SerializeField] private Transform LeftShoulderADS;
    [SerializeField] private Transform LeftShoulderCrouchADS;
    [SerializeField] private GameObject LeftShoulder;

    [Header("Right Shoulder")]
    [SerializeField] private Transform RightShoulderDefault;
    [SerializeField] private Transform RightShoulderADS;
    [SerializeField] private Transform RightShoulderCrouchADS;
    [SerializeField] private GameObject rightShoulder;

    [Header("Right Hand")]
    [SerializeField] private GameObject RightHand;
    [SerializeField] private GameObject RightHandDefaultPos;
    [SerializeField] private GameObject RightHandShootingPos;

    [Header("Left Hand")]
    [SerializeField] private GameObject LeftHand;
    [SerializeField] private GameObject LeftHandDefaultPos;
    [SerializeField] private GameObject LeftHandShootingPos;

    [Header("Camera")]
    [SerializeField] private GameObject PlayerCam;
    [SerializeField] private float DefaultFOV;
    [SerializeField] private float ADSFOV;

    [Header("Gun")]
    [SerializeField] private GameObject Gun;
    [SerializeField] private Transform DefaultGunPos;
    [SerializeField] private GameObject Detail;
    [SerializeField] private Transform DefaultDetailPos;
    [SerializeField] private Transform ShootingDetailPos;
    [Range(0, 10)][SerializeField] private float GunAnimSpeed;

    [Range(0, 100)][SerializeField] private float AimSpeed = 50;

    private EAnimState CurrentAnimState;
    private EZoomState ZoomState;
    private bool ADSNoArms;

    private List<bool> fireList;
    List<bool> recoil;

    private void Start()
    {
        ZoomState = EZoomState.None;
        CurrentAnimState = EAnimState.None;
        ADSNoArms = false;
        fireList = new List<bool>();
        recoil = new List<bool>();
    }

    public void SetShoot()
    {
        ADSNoArms = true;
        LeftShoulder.transform.rotation = LeftShoulderADS.rotation;
        rightShoulder.transform.rotation = RightShoulderADS.rotation;
    }

    public void UnSetArms()
    {
        LeftShoulder.transform.rotation = LeftShoulderDefault.rotation;
        rightShoulder.transform.rotation = RightShoulderDefault.rotation;
    }

    public void setArmCrouch()
    {
        LeftShoulder.transform.rotation = LeftShoulderADS.rotation;
        rightShoulder.transform.rotation = RightShoulderADS.rotation;
    }

    public IEnumerator ADSAnim() 
    {
        bool exiting = false;
        CurrentAnimState = EAnimState.ADS;
        float rateLeft = 1.0f / Quaternion.Angle(LeftShoulder.transform.rotation, LeftShoulderADS.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, RightShoulderADS.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            if (CurrentAnimState != EAnimState.ADS) { exiting = true;  break; }
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            LeftShoulder.transform.rotation = Quaternion.Lerp(LeftShoulder.transform.rotation, LeftShoulderADS.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, RightShoulderADS.rotation, tRight);
            yield return null;
        }
        if (!exiting) {
            CurrentAnimState = EAnimState.None;
            yield return null;
        }
        
        yield return null;
    }

    public IEnumerator ADSUnaim()
    {
        bool exiting = false;
        CurrentAnimState = EAnimState.UnADS;
        float rateLeft = 1.0f / Quaternion.Angle(LeftShoulder.transform.rotation, LeftShoulderDefault.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, RightShoulderDefault.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            if (CurrentAnimState != EAnimState.UnADS) { exiting = true; break; }
            if (ADSNoArms) { ADSNoArms = false; break; }

            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            LeftShoulder.transform.rotation = Quaternion.Lerp(LeftShoulder.transform.rotation, LeftShoulderDefault.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, RightShoulderDefault.rotation, tRight);
            yield return null;
        }

        if (!exiting)
        {
            Gun.transform.position = DefaultGunPos.position;
            Gun.transform.rotation = DefaultGunPos.rotation;
            CurrentAnimState = EAnimState.None;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator CrouchADSAim() 
    {
        bool exiting = false;
        CurrentAnimState = EAnimState.CrouchADS;
        float rateLeft = 1.0f / Quaternion.Angle(LeftShoulder.transform.rotation, LeftShoulderCrouchADS.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, RightShoulderCrouchADS.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            if (CurrentAnimState != EAnimState.CrouchADS) { break; }
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            LeftShoulder.transform.rotation = Quaternion.Lerp(LeftShoulder.transform.rotation, LeftShoulderCrouchADS.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, RightShoulderCrouchADS.rotation, tRight);
            yield return null;
        }
        if (!exiting) 
        {
            CurrentAnimState = EAnimState.None;
            yield return null;
        }

        yield return null;
    }

    public IEnumerator crouchADSUnaim()
    {
        bool exiting = false;
        CurrentAnimState = EAnimState.CrouchUnADS;
        float rateLeft = 1.0f / Quaternion.Angle(LeftShoulder.transform.rotation, LeftShoulderADS.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, RightShoulderADS.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            if (CurrentAnimState != EAnimState.CrouchUnADS) { exiting = true;  break; }
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            LeftShoulder.transform.rotation = Quaternion.Lerp(LeftShoulder.transform.rotation, LeftShoulderADS.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, RightShoulderADS.rotation, tRight);
            yield return null;
        }
        if (!exiting)
        {
            CurrentAnimState = EAnimState.None;
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

        recoil.Add(true);
        Debug.Log(recoil.Count);
        bool exiting = false;

        float rateChamber = 1.0f / Vector3.Distance(Detail.transform.position, ShootingDetailPos.position) * GunAnimSpeed;
        float rateLeftHand = 1.0f / Quaternion.Angle(LeftHand.transform.rotation, LeftHandShootingPos.transform.rotation) * 300;
        float rateRightHand = 1.0f / Quaternion.Angle(RightHand.transform.rotation, RightHandShootingPos.transform.rotation) * 300;

        float tChamber = 0.0f;
        float tLeftHand = 0.0f;
        float tRightHand = 0.0f;

        while (tChamber < 1.0f || tLeftHand < 1.0f || tRightHand < 1.0f)
        {
            if (recoil.Count > 1) { exiting = true; break; }
            tChamber += Time.deltaTime * rateChamber;
            tLeftHand += Time.deltaTime * rateLeftHand;
            tRightHand += Time.deltaTime * rateRightHand;

            Detail.transform.position = Vector3.Lerp(Detail.transform.position, ShootingDetailPos.position, tChamber);
            LeftHand.transform.rotation = Quaternion.Lerp(LeftHand.transform.rotation, LeftHandShootingPos.transform.rotation, tLeftHand);
            RightHand.transform.rotation = Quaternion.Lerp(RightHand.transform.rotation, RightHandShootingPos.transform.rotation, tRightHand);
            yield return null;
        }

        if (!exiting) {
            rateChamber = 1.0f / Vector3.Distance(Detail.transform.position, DefaultDetailPos.position) * GunAnimSpeed;
            rateLeftHand = 1.0f / Quaternion.Angle(LeftHand.transform.rotation, LeftHandDefaultPos.transform.rotation) * 300;
            rateRightHand = 1.0f / Quaternion.Angle(RightHand.transform.rotation, RightHandDefaultPos.transform.rotation) * 300;

            tChamber = 0.0f;
            tLeftHand = 0.0f;
            tRightHand = 0.0f;

            while (tChamber < 1.0f || tLeftHand < 1.0f || tRightHand < 1.0f)
            {
                if (recoil.Count > 1) { break; }
                tChamber += Time.deltaTime * rateChamber;
                tLeftHand += Time.deltaTime * rateLeftHand;
                tRightHand += Time.deltaTime * rateRightHand;

                Detail.transform.position = Vector3.Lerp(Detail.transform.position, DefaultDetailPos.position, tChamber);
                LeftHand.transform.rotation = Quaternion.Lerp(LeftHand.transform.rotation, LeftHandDefaultPos.transform.rotation, tLeftHand);
                RightHand.transform.rotation = Quaternion.Lerp(RightHand.transform.rotation, RightHandDefaultPos.transform.rotation, tRightHand);
                yield return null;
            }
        }

        recoil.RemoveAt(0);

        yield return new WaitForSeconds(0.5f);

        //Preferably, I wouldn't have to check for that last thing, but if I don't do this the arms go down while player is zoomed in.
        //Also, no need to pass literally all of the player object, this is a one time thing.
        if (fireList.Count <= 1 && !ADS && !gameObject.GetComponent<StarterAssetsInputs>().ads)
        {
            yield return UnsetShootAnim();
        }

        if (fireList.Count > 0)
        {
            fireList.RemoveAt(fireList.Count - 1);
        }

        yield return null;
    }

    public IEnumerator UnsetShootAnim()
    {
        bool exiting = false;

        CurrentAnimState = EAnimState.UnsetShoot;
        float rateLeft = 1.0f / Quaternion.Angle(LeftShoulder.transform.rotation, LeftShoulderDefault.rotation) * AimSpeed;
        float rateRight = 1.0f / Quaternion.Angle(rightShoulder.transform.rotation, RightShoulderDefault.rotation) * AimSpeed;

        float tLeft = 0.0f;
        float tRight = 0.0f;
        ADSNoArms = false;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            if (CurrentAnimState != EAnimState.UnsetShoot) { exiting = true; break; }
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            LeftShoulder.transform.rotation = Quaternion.Lerp(LeftShoulder.transform.rotation, LeftShoulderDefault.rotation, tLeft);
            rightShoulder.transform.rotation = Quaternion.Lerp(rightShoulder.transform.rotation, RightShoulderDefault.rotation, tRight);

            yield return null;
        }

        if (!exiting) 
        {
            CurrentAnimState = EAnimState.None;
            yield return null;
        }

        yield return null;
    }

    public IEnumerator FOVZoomIn() 
    {
        bool exiting = false;
        ZoomState = EZoomState.ZoomIn;
        float rateFOV = 1.0f / Mathf.Abs(ADSFOV - DefaultFOV) * AimSpeed;

        float tFOV = 0.0f;

        while (tFOV < 1.0f)
        {
            if (ZoomState != EZoomState.ZoomIn) { exiting = true; break; }
            tFOV += Time.deltaTime * rateFOV;

            PlayerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = Mathf.Lerp(PlayerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, ADSFOV, tFOV);
            yield return null;
        }
        if (!exiting)
        {
            ZoomState = EZoomState.None;
        }
        yield return null;
    }

    public IEnumerator FOVZoomOut()
    {
        bool exiting = false;
        ZoomState = EZoomState.ZoomOut;
        float rateFOV = 1.0f / Mathf.Abs(ADSFOV - DefaultFOV) * AimSpeed;

        float tFOV = 0.0f;

        while (tFOV < 1.0f)
        {
            if (ZoomState != EZoomState.ZoomOut) { exiting = true; break; }
            tFOV += Time.deltaTime * rateFOV;

            PlayerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = Mathf.Lerp(PlayerCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView, DefaultFOV, tFOV);
            yield return null;
        }
        if (!exiting)
        {
            ZoomState = EZoomState.None;
        }
        yield return null;
    }
}
