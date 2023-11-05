using System;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch;
		public bool ads;
		public bool fire;
		public bool silencer;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Gun Particle Pos")]
		[SerializeField] ParticleSystem MuzzleParticleSystem;
		[SerializeField] GameObject NoSilencerPos;
        [SerializeField] GameObject SilencerPos;

		[SerializeField]private GameObject SilencerObj;

		private bool CanFire;

        private PlayerAnimator playerAnim;

		
        private void Start()
        {
			CanFire = true;
			silencer = false;
            playerAnim = GetComponent<PlayerAnimator>();
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			if (!crouch) {
				JumpInput(value.isPressed);
			}
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnCrouch(InputValue value) 
		{
			CrouchInput(value.isPressed);
		}

		public void OnADS(InputValue value) 
		{
			ADSInput(value.isPressed);
		}

        public void OnFire(InputValue value)
        {
			FireInput(value.isPressed);
        }

		public void OnSilencer(InputValue value) 
		{
			if (value.isPressed) { ToggleSilencer(); }
		}
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
        } 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void CrouchInput(bool newCrouchState) 
		{
			crouch = newCrouchState;

			if (newCrouchState)
			{
				if (ads)
				{
					StartCoroutine(playerAnim.CrouchADSAim());
				}
				else
				{
                    StartCoroutine(playerAnim.crouchADSUnaim());
                }
            }
			else
			{
				if (ads)
				{
                    StartCoroutine(playerAnim.ADSAnim());
				}
				else
				{
					playerAnim.UnSetArms();
				}
			}
        }

        private void ADSInput(bool newADSState)
        {
			ads = newADSState;
			if (newADSState)
			{
				if (crouch)
				{
					StartCoroutine(playerAnim.CrouchADSAim());
				}
				else 
				{
                    StartCoroutine(playerAnim.ADSAnim());
                }
                StartCoroutine(playerAnim.FOVZoomIn());
            }
			else
			{
				if (crouch)
				{
					StartCoroutine(playerAnim.crouchADSUnaim());
				}
				else
				{
					StartCoroutine(playerAnim.ADSUnaim());
				}

				StartCoroutine(playerAnim.FOVZoomOut());
			}
        }
        private void FireInput(bool newFireState)
        {
			fire = newFireState;
			if (fire)
			{
				if (!ads)
				{
					playerAnim.SetShoot();
				}
				StartCoroutine(playerAnim.ShootAnim(ads));
			}
        }

		public void ToggleSilencer() 
		{
			silencer = !silencer;
            SilencerObj.SetActive(silencer);
		}

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
    }
	
}