using I2.Loc;
using InControl;
using Multiplayer;
using UnityEngine;

public class TutorialRepositoryItem : MonoBehaviour
{
	[Multiline]
	public string tutorialText;

	[Multiline]
	public string controllerText;

	public string term;

	public string keyboardTerm;

	public int level;

	public int checkpoint;

	public bool showInPause = true;

	private string ReplaceKeyboardControls(string input)
	{
		string text = ((Options.keyboardBindings.Forward.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.Forward.Bindings[0].Name);
		string text2 = ((Options.keyboardBindings.Left.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.Left.Bindings[0].Name);
		string text3 = ((Options.keyboardBindings.Back.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.Back.Bindings[0].Name);
		string text4 = ((Options.keyboardBindings.Right.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.Right.Bindings[0].Name);
		string text5 = ((Options.keyboardBindings.LeftHand.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.LeftHand.Bindings[0].Name);
		if (text5 == "LeftButton")
		{
			text5 = "LMB";
		}
		string text6 = ((Options.keyboardBindings.RightHand.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.RightHand.Bindings[0].Name);
		if (text6 == "RightButton")
		{
			text6 = "RMB";
		}
		string text7 = ((Options.keyboardBindings.Jump.Bindings.Count <= 0) ? string.Empty : Options.keyboardBindings.Jump.Bindings[0].Name);
		string newValue = text + "," + text2 + "," + text3 + "," + text4;
		string text8 = input.Replace("W,A,S,D", newValue);
		text8 = text8.Replace("W, A, S, D", newValue);
		text8 = text8.Replace("LMB", text5);
		text8 = text8.Replace("RMB", text6);
		return text8.Replace("<b>Space</b>", "<b>" + text7 + "</b>");
	}

	public string GetText()
	{
		bool flag = false;
		for (int i = 0; i < NetGame.instance.players.Count; i++)
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			flag |= NetGame.instance.players[i].controls.ControllerJumpPressed() || (activeDevice.MenuWasPressed && !Input.GetKeyDown(KeyCode.Escape));
		}
		if (flag && !string.IsNullOrEmpty(term))
		{
			return ScriptLocalization.Get(term);
		}
		if (!flag && !string.IsNullOrEmpty(keyboardTerm))
		{
			return ReplaceKeyboardControls(ScriptLocalization.Get(keyboardTerm));
		}
		return null;
	}
}
