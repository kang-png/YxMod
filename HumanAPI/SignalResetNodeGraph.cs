namespace HumanAPI;

[AddNodeMenuItem]
public class SignalResetNodeGraph : Node
{
	public NodeInput input;

	public override void Process()
	{
		base.Process();
		if (input.value > 0.5f)
		{
			IReset[] componentsInChildren = GetComponentsInChildren<IReset>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ResetState(Game.instance.currentCheckpointNumber, Game.instance.currentCheckpointSubObjectives);
			}
			IPostReset[] componentsInChildren2 = GetComponentsInChildren<IPostReset>(includeInactive: true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].PostResetState(Game.instance.currentCheckpointNumber);
			}
		}
	}
}
