using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct MovementSystem : ISystem, W.IQueryBlock.Write<PositionComponent>.Read<VelocityComponent>
	{
		public void Update() =>
			W.Query<None<DeadTag>>().WriteBlock<PositionComponent>().Read<VelocityComponent>().For(ref this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(uint count, World<StaticWorld>.EntityBlock entities,
						   Block<PositionComponent> position,
						   BlockR<VelocityComponent> velocity)
		{
			for (uint i = 0; i < count; i++)
			{
				MovementSystemForEach(ref position[i].Value, in velocity[i].Value);
			}
		}

	}
}

}
