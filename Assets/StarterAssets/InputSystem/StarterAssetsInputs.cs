using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using Photon.Pun;

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviourPun
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			if (photonView != null && !photonView.IsMine) return;
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (photonView != null && !photonView.IsMine) return;
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			if (photonView != null && !photonView.IsMine) return;
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			if (photonView != null && !photonView.IsMine) return;
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			if (photonView != null && !photonView.IsMine) return;
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			if (photonView != null && !photonView.IsMine) return;
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			if (photonView != null && !photonView.IsMine) return;
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			if (photonView != null && !photonView.IsMine) return;
			sprint = newSprintState;
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