using System;
using System.Collections.Generic;

namespace Multiplayer;

public class NetFrames
{
	public object framesLock = new object();

	public List<FrameState> frameQueue = new List<FrameState>();

	public List<FrameState> eventQueue = new List<FrameState>();

	public bool AllowDiscontinuous;

	public void DropOldStates(int frameId)
	{
		while (frameQueue.Count > 0 && frameQueue[0].frameId < frameId)
		{
			frameQueue[0].stream.Release();
			frameQueue.RemoveAt(0);
		}
	}

	public void DropOldEvents(int frameId)
	{
		while (eventQueue.Count > 0 && eventQueue[0].frameId < frameId)
		{
			eventQueue[0].stream.Release();
			eventQueue.RemoveAt(0);
		}
	}

	public void PushState(int frameId, NetStream state)
	{
		int num = frameQueue.Count;
		while (num - 1 > 0 && frameQueue[num - 1].frameId > frameId)
		{
			num--;
		}
		frameQueue.Insert(num, new FrameState
		{
			frameId = frameId,
			stream = state
		});
	}

	public void PushEvents(int frameId, NetStream eventStream)
	{
		eventQueue.Add(new FrameState
		{
			frameId = frameId,
			stream = eventStream
		});
	}

	public void LimitHistory()
	{
		int num = 1024;
		if (frameQueue.Count > num)
		{
			DropOldStates(frameQueue[frameQueue.Count - num].frameId);
		}
		if (eventQueue.Count > num)
		{
			DropOldEvents(eventQueue[eventQueue.Count - num].frameId);
		}
	}

	public NetStream GetState(int frameId, bool rewind = false)
	{
		for (int i = 0; i < frameQueue.Count; i++)
		{
			if (frameQueue[i].frameId == frameId)
			{
				NetStream stream = frameQueue[i].stream;
				if (rewind)
				{
					stream.Seek(0);
				}
				return stream;
			}
		}
		return null;
	}

	public int TestForState(int frameId)
	{
		int count = frameQueue.Count;
		if (count == 0)
		{
			return -1;
		}
		int num = count - 1;
		int frameId2 = frameQueue[num].frameId;
		if (frameId >= frameId2)
		{
			return (frameId != frameId2) ? (-1) : num;
		}
		int frameId3 = frameQueue[0].frameId;
		if (frameId <= frameId3)
		{
			return (frameId != frameId3) ? (-1) : 0;
		}
		int num2 = 0;
		while (num2 < num)
		{
			int num3 = num + num2 >> 1;
			int frameId4 = frameQueue[num3].frameId;
			if (frameId == frameId4)
			{
				return num3;
			}
			if (frameId < frameId4)
			{
				num = num3 - 1;
			}
			else
			{
				num2 = num3 + 1;
			}
		}
		if (num2 == num && frameQueue[num2].frameId == frameId)
		{
			return num2;
		}
		return -1;
	}

	public bool GetState(int frame, float fraction, out int frame0id, out NetStream frame0, out NetStream frame1, out float mix)
	{
		frame0 = null;
		frame1 = null;
		frame0id = -1;
		try
		{
			for (int num = frameQueue.Count - 1; num >= 0; num--)
			{
				if (frameQueue[num].frameId <= frame)
				{
					frame0id = frameQueue[num].frameId;
					if (num < frameQueue.Count - 1)
					{
						int frameId = frameQueue[num + 1].frameId;
						if (AllowDiscontinuous && frameId >= frame0id + 60)
						{
							if ((float)(frameId - frame) <= 0.25f + fraction)
							{
								frame1 = NetStream.AllocStream(frameQueue[num + 1].stream);
								frame0 = NetStream.AllocStream(frame1);
								frame0id = frameId;
								mix = 1f;
								return true;
							}
							frame0 = NetStream.AllocStream(frameQueue[num].stream);
							frame1 = NetStream.AllocStream(frame0);
							mix = 0f;
							return true;
						}
						frame0 = NetStream.AllocStream(frameQueue[num].stream);
						frame1 = NetStream.AllocStream(frameQueue[num + 1].stream);
						mix = ((float)(frame - frame0id) + fraction) / (float)(frameId - frame0id);
						return true;
					}
					frame0 = NetStream.AllocStream(frameQueue[num].stream);
					frame1 = NetStream.AllocStream(frame0);
					mix = 0f;
					return true;
				}
			}
			mix = 0f;
			return false;
		}
		catch (Exception)
		{
			if (frame0 != null)
			{
				frame0 = frame0.Release();
			}
			if (frame1 != null)
			{
				frame1 = frame1.Release();
			}
			throw;
		}
	}
}
