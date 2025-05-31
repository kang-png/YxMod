public interface IGrabbableWithInfo
{
	void OnGrab(GrabManager grabbedBy);

	void OnRelease(GrabManager releasedBy);
}
