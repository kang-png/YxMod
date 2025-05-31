using System;

namespace I2.Loc;

[Serializable]
public struct PlatformSpecificLocalization
{
	public Platforms currentPlatform;

	public string Term;
}
