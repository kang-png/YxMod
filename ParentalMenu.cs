public class ParentalMenu : MenuTransition
{
	public MenuSelector parentalSelector;

	public MenuSelector pinSelector;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		parentalSelector.SelectIndex(Options.parental);
		pinSelector.SelectIndex(Options.usePINForParental);
	}

	public void ParentalChanged(int value)
	{
		Options.parental = value;
	}

	public void PinSettingChanged(int value)
	{
		Options.usePINForParental = value;
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<OptionsMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
