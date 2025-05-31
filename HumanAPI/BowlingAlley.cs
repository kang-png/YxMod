using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI;

public class BowlingAlley : Node, IReset
{
	private enum BowlingState
	{
		BowlingState_InitialSetup,
		BowlingState_Ready,
		BowlingState_BallInPins,
		BowlingState_TidyingPins,
		BowlingState_Complete
	}

	public NodeInput ballInPinsArea;

	public NodeOutput bowlingComplete;

	public NodeOutput spareMode;

	public NodeOutput[] pinsDownOutputs;

	public BowlingPin[] bowlingPins;

	private BowlingState currentState;

	private bool inSpareMode;

	private float initialSetupTimer;

	private const float initialSetupDuration = 3f;

	private float pinsKnockDownTimer;

	private const float pinsKnockDownDuration = 10f;

	private bool initialSetup;

	private int pinsDown;

	protected override void CollectAllSockets(List<NodeSocket> sockets)
	{
		ballInPinsArea.name = "BallInPins";
		ballInPinsArea.node = this;
		sockets.Add(ballInPinsArea);
		bowlingComplete.name = "BowlingComplete";
		bowlingComplete.node = this;
		sockets.Add(bowlingComplete);
		spareMode.name = "SpareMode";
		spareMode.node = this;
		sockets.Add(spareMode);
		if (pinsDownOutputs == null || pinsDownOutputs.Length == 0)
		{
			Debug.LogError("Combine node has no inputs!");
			return;
		}
		for (int i = 0; i < pinsDownOutputs.Length; i++)
		{
			pinsDownOutputs[i].node = this;
			pinsDownOutputs[i].name = "Pin down " + i;
			sockets.Add(pinsDownOutputs[i]);
		}
	}

	private void Start()
	{
		if (bowlingPins.Length != pinsDownOutputs.Length)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		for (int i = 0; i < bowlingPins.Length; i++)
		{
			if (!bowlingPins[i].IsInPlace())
			{
				pinsDownOutputs[i].SetValue(1f);
			}
			else
			{
				pinsDownOutputs[i].SetValue(0f);
			}
		}
		switch (currentState)
		{
		case BowlingState.BowlingState_InitialSetup:
			initialSetupTimer += Time.deltaTime;
			if (initialSetupTimer >= 3f)
			{
				for (int n = 0; n < bowlingPins.Length; n++)
				{
					bowlingPins[n].Show();
				}
				inSpareMode = false;
				spareMode.SetValue(0f);
				currentState = BowlingState.BowlingState_Ready;
			}
			break;
		case BowlingState.BowlingState_Ready:
			if (ballInPinsArea.value > 0.5f)
			{
				currentState = BowlingState.BowlingState_BallInPins;
				pinsKnockDownTimer = 0f;
			}
			break;
		case BowlingState.BowlingState_BallInPins:
		{
			pinsKnockDownTimer += Time.deltaTime;
			pinsDown = 0;
			for (int l = 0; l < bowlingPins.Length; l++)
			{
				if (!bowlingPins[l].IsInPlace())
				{
					pinsDown++;
				}
			}
			if (pinsDown == bowlingPins.Length)
			{
				for (int m = 0; m < bowlingPins.Length; m++)
				{
					bowlingPins[m].Hide();
				}
				bowlingComplete.SetValue(1f);
				currentState = BowlingState.BowlingState_Complete;
			}
			else if (pinsKnockDownTimer >= 10f)
			{
				currentState = BowlingState.BowlingState_TidyingPins;
			}
			break;
		}
		case BowlingState.BowlingState_TidyingPins:
			if (inSpareMode)
			{
				for (int j = 0; j < bowlingPins.Length; j++)
				{
					bowlingPins[j].Show();
				}
				inSpareMode = false;
				spareMode.SetValue(0f);
			}
			else
			{
				for (int k = 0; k < bowlingPins.Length; k++)
				{
					if (!bowlingPins[k].IsInPlace())
					{
						bowlingPins[k].Hide();
					}
				}
				if (pinsDown != 0)
				{
					inSpareMode = true;
					spareMode.SetValue(1f);
				}
			}
			currentState = BowlingState.BowlingState_Ready;
			break;
		case BowlingState.BowlingState_Complete:
			break;
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		bowlingComplete.SetValue(0f);
		spareMode.SetValue(0f);
		initialSetupTimer = 0f;
		currentState = BowlingState.BowlingState_InitialSetup;
	}
}
