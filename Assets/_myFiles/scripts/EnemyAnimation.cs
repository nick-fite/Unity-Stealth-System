using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    [Range(1, 10)] [SerializeField] private float AnimSpeed;

    [Header("Left Arm")]
    [SerializeField] GameObject LeftArm;
    [SerializeField] Transform LeftArmDefaultPos;
    [SerializeField] Transform LeftArmShootingPos;

    [Header("Right Arm")]
    [SerializeField] GameObject RightArm;
    [SerializeField] Transform RightArmDefaultPos;
    [SerializeField] Transform RightArmShootingPos;

    [Header("Shooting")]
    [SerializeField] GameObject StartingPos;
    [SerializeField] ParticleSystem MuzzleFlash;
    [SerializeField] AudioSource sound;

    private bool ContinueShooting;
    private bool armsRaised;

    private void Start()
    {
        MuzzleFlash.Stop();
        var main = MuzzleFlash.main;
        main.duration = 1f;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(this.GetComponent<CapsuleCollider>().center), FootstepAudioVolume);
            }
        }
    }

    public IEnumerator RaiseArms()
    {
        if (!armsRaised)
        {
            armsRaised = true;
            float rateLeft = 1.0f / Quaternion.Angle(LeftArm.transform.rotation, LeftArmShootingPos.rotation) * 50;
            float rateRight = 1.0f / Quaternion.Angle(RightArm.transform.rotation, RightArmShootingPos.rotation) * 50;

            float tLeft = 0.0f;
            float tRight = 0.0f;

            while (tLeft < 1.0f || tRight < 1.0f)
            {
                tLeft += Time.deltaTime * rateLeft;
                tRight += Time.deltaTime * rateRight;

                LeftArm.transform.rotation = Quaternion.Lerp(LeftArm.transform.rotation, LeftArmShootingPos.rotation, tLeft);
                RightArm.transform.rotation = Quaternion.Lerp(RightArm.transform.rotation, RightArmShootingPos.rotation, tRight);
                yield return null;
            }
            ContinueShooting = true;
            yield return Shooting();
        }
    }

    public IEnumerator LowerArms()
    {
        armsRaised = false;
        ContinueShooting = false;
        float rateLeft = 1.0f / Quaternion.Angle(LeftArm.transform.rotation, LeftArmDefaultPos.rotation) * 50;
        float rateRight = 1.0f / Quaternion.Angle(RightArm.transform.rotation, RightArmDefaultPos.rotation) * 50;

        float tLeft = 0.0f;
        float tRight = 0.0f;

        while (tLeft < 1.0f || tRight < 1.0f)
        {
            tLeft += Time.deltaTime * rateLeft;
            tRight += Time.deltaTime * rateRight;

            LeftArm.transform.rotation = Quaternion.Lerp(LeftArm.transform.rotation, LeftArmDefaultPos.rotation, tLeft);
            RightArm.transform.rotation = Quaternion.Lerp(RightArm.transform.rotation, RightArmDefaultPos.rotation, tRight);
            yield return null;
        }
    }

    public IEnumerator Shooting() 
    {
        while (ContinueShooting)
        {
            Ray ray = new Ray(StartingPos.transform.position, transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == GameManager.m_Instance.GetPlayer())
                {
                    ThirdPersonController playerController;
                    if (hit.collider.gameObject.TryGetComponent(out playerController))
                    {
                        playerController.SetHealth(playerController.GetHealth() - 2f);
                    }
                }
            }
            sound.Play();
            MuzzleFlash.Play();
            yield return new WaitForSeconds(0.1f);
            MuzzleFlash.Stop();

            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    public void SetContinueShooting(bool newState) { ContinueShooting = newState; }
}
