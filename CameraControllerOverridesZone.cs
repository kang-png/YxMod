using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraControllerOverridesZone : MonoBehaviour
{
	private interface ICameraControllerOverride
	{
		void Apply();

		void Revert();
	}

	[Serializable]
	private abstract class CameraControllerOverride<T> : ICameraControllerOverride
	{
		[SerializeField]
		protected bool isEnabled;

		private Func<T, T> applyValueHandler;

		private Action<T> revertValueHandler;

		private T cachedValue;

		public bool IsEnabled
		{
			[CompilerGenerated]
			get
			{
				return isEnabled;
			}
		}

		protected abstract T Value { get; }

		public void Initialize(Func<T, T> applyValueHandler, Action<T> revertValueHandler)
		{
			this.applyValueHandler = applyValueHandler;
			this.revertValueHandler = revertValueHandler;
		}

		public void Apply()
		{
			if (IsEnabled || applyValueHandler == null)
			{
				cachedValue = applyValueHandler(Value);
			}
		}

		public void Revert()
		{
			if (IsEnabled || revertValueHandler == null)
			{
				revertValueHandler(cachedValue);
			}
		}
	}

	[Serializable]
	private class CameraControllerOverrideCameraMode : CameraControllerOverride<CameraMode>
	{
		[SerializeField]
		private CameraMode value;

		protected override CameraMode Value
		{
			[CompilerGenerated]
			get
			{
				return value;
			}
		}
	}

	[Serializable]
	private class CameraControllerOverrideVector3 : CameraControllerOverride<Vector3>
	{
		[SerializeField]
		private Vector3 value;

		protected override Vector3 Value
		{
			[CompilerGenerated]
			get
			{
				return value;
			}
		}
	}

	[SerializeField]
	private CameraControllerOverrideCameraMode cameraModeOverride;

	[SerializeField]
	private CameraControllerOverrideVector3 fpsTargetOffsetOverride;

	private ICameraControllerOverride[] overrides;

	private CameraController3 targetController;

	private void Start()
	{
		targetController = Camera.main?.GetComponentInParent<CameraController3>();
		InitializeOverrides();
	}

	private void InitializeOverrides()
	{
		if (targetController != null)
		{
			cameraModeOverride.Initialize(delegate(CameraMode nextValue)
			{
				CameraMode mode = targetController.mode;
				targetController.mode = nextValue;
				return mode;
			}, delegate(CameraMode nextValue)
			{
				targetController.mode = nextValue;
			});
			fpsTargetOffsetOverride.Initialize(delegate(Vector3 nextValue)
			{
				Vector3 fpsTargetOffset = targetController.fpsTargetOffset;
				targetController.fpsTargetOffset = nextValue;
				return fpsTargetOffset;
			}, delegate(Vector3 nextValue)
			{
				targetController.fpsTargetOffset = nextValue;
			});
			overrides = new ICameraControllerOverride[2] { cameraModeOverride, fpsTargetOffsetOverride };
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !(targetController == null))
		{
			ICameraControllerOverride[] array = overrides;
			foreach (ICameraControllerOverride cameraControllerOverride in array)
			{
				cameraControllerOverride.Apply();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && !(targetController == null))
		{
			ICameraControllerOverride[] array = overrides;
			foreach (ICameraControllerOverride cameraControllerOverride in array)
			{
				cameraControllerOverride.Revert();
			}
		}
	}
}
