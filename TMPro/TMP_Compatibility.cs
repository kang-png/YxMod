namespace TMPro;

public static class TMP_Compatibility
{
	public enum AnchorPositions
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight,
		BaseLine,
		None
	}

	public static TextAlignmentOptions ConvertTextAlignmentEnumValues(TextAlignmentOptions oldValue)
	{
		return (int)oldValue switch
		{
			0 => TextAlignmentOptions.TopLeft, 
			1 => TextAlignmentOptions.Top, 
			2 => TextAlignmentOptions.TopRight, 
			3 => TextAlignmentOptions.TopJustified, 
			4 => TextAlignmentOptions.Left, 
			5 => TextAlignmentOptions.Center, 
			6 => TextAlignmentOptions.Right, 
			7 => TextAlignmentOptions.Justified, 
			8 => TextAlignmentOptions.BottomLeft, 
			9 => TextAlignmentOptions.Bottom, 
			10 => TextAlignmentOptions.BottomRight, 
			11 => TextAlignmentOptions.BottomJustified, 
			12 => TextAlignmentOptions.BaselineLeft, 
			13 => TextAlignmentOptions.Baseline, 
			14 => TextAlignmentOptions.BaselineRight, 
			15 => TextAlignmentOptions.BaselineJustified, 
			16 => TextAlignmentOptions.MidlineLeft, 
			17 => TextAlignmentOptions.Midline, 
			18 => TextAlignmentOptions.MidlineRight, 
			19 => TextAlignmentOptions.MidlineJustified, 
			20 => TextAlignmentOptions.CaplineLeft, 
			21 => TextAlignmentOptions.Capline, 
			22 => TextAlignmentOptions.CaplineRight, 
			23 => TextAlignmentOptions.CaplineJustified, 
			_ => TextAlignmentOptions.TopLeft, 
		};
	}
}
