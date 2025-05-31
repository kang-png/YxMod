using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HumanAPI;

[AddNodeMenuItem]
[AddComponentMenu("Human/Signals/SignalTime", 10)]
public class SignalTime : Node, IReset
{
	public NodeOutput output;

	private NodeOutput[] extraTriggerOutputs = new NodeOutput[0];

	public NodeOutput maxValueReached;

	public NodeInput input;

	public bool startTimerOnSignal;

	public bool pauseTimerOnSignalOff;

	public bool resetTimerOnSignalOff;

	public float[] extraTriggerTimes = new float[0];

	public float maxValue;

	public bool resetAtMaxValue;

	[Tooltip("If resetAtMaxValue is true and startTimerOnSignal is false, how long to hold maxValueReached at 1.0 after timer loops.")]
	public float holdTime = 0.25f;

	private float prevInput;

	private bool timerRunning;

	private float initialOutput;

	public override string Title => "Time: Max " + maxValue;

	protected override void CollectAllSockets(List<NodeSocket> sockets)
	{
		base.CollectAllSockets(sockets);
		if (extraTriggerTimes == null)
		{
			return;
		}
		if (extraTriggerOutputs == null || extraTriggerTimes.Length != extraTriggerOutputs.Length)
		{
			extraTriggerOutputs = new NodeOutput[extraTriggerTimes.Length];
			for (int i = 0; i < extraTriggerTimes.Length; i++)
			{
				extraTriggerOutputs[i] = extraTriggerOutputs[i] ?? new NodeOutput();
			}
		}
		for (int j = 0; j < extraTriggerTimes.Length; j++)
		{
			extraTriggerOutputs[j].node = this;
			extraTriggerOutputs[j].name = extraTriggerTimes[j].ToString("F2", CultureInfo.InvariantCulture) + " reached";
			sockets.Add(extraTriggerOutputs[j]);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		initialOutput = output.value;
		if (startTimerOnSignal)
		{
			timerRunning = false;
		}
		else
		{
			timerRunning = true;
		}
	}

	public override void Process()
	{
		base.Process();
		if (startTimerOnSignal && input.value >= 0.5f && prevInput < 0.5f)
		{
			timerRunning = true;
		}
		if ((resetTimerOnSignalOff || pauseTimerOnSignalOff) && input.value < 0.5f && prevInput >= 0.5f)
		{
			if (startTimerOnSignal || pauseTimerOnSignalOff)
			{
				timerRunning = false;
			}
			else
			{
				timerRunning = true;
			}
			if (resetTimerOnSignalOff)
			{
				output.SetValue(0f);
				maxValueReached.SetValue(0f);
				if (extraTriggerOutputs != null)
				{
					NodeOutput[] array = extraTriggerOutputs;
					foreach (NodeOutput nodeOutput in array)
					{
						nodeOutput.SetValue(0f);
					}
				}
			}
		}
		prevInput = input.value;
	}

	public void ResetMaxValueReached()
	{
		maxValueReached.SetValue(0f);
	}

	private void FixedUpdate()
	{
		if (!timerRunning)
		{
			return;
		}
		float num = output.value + Time.fixedDeltaTime;
		if (maxValue > 0f && num >= maxValue)
		{
			if (resetAtMaxValue)
			{
				if (startTimerOnSignal)
				{
					timerRunning = false;
				}
				else
				{
					timerRunning = true;
				}
				num = 0f;
			}
			else
			{
				num = maxValue;
			}
			maxValueReached.SetValue(1f);
		}
		else if (num >= holdTime || startTimerOnSignal || !resetAtMaxValue)
		{
			maxValueReached.SetValue(0f);
		}
		output.SetValue(num);
		if (extraTriggerTimes != null)
		{
			for (int i = 0; i < extraTriggerTimes.Length; i++)
			{
				extraTriggerOutputs[i].SetValue((!(num >= extraTriggerTimes[i])) ? 0f : 1f);
			}
		}
	}

	public void ResetState(int checkpoint, int subObjectives)
	{
		output.SetValue(initialOutput);
		maxValueReached.SetValue(0f);
		if (extraTriggerOutputs != null)
		{
			NodeOutput[] array = extraTriggerOutputs;
			foreach (NodeOutput nodeOutput in array)
			{
				nodeOutput.SetValue(0f);
			}
		}
		if (startTimerOnSignal)
		{
			timerRunning = false;
		}
		else
		{
			timerRunning = true;
		}
		prevInput = 0f;
	}
}
