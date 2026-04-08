using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct MovementSystem : ISystem
	{
		public void Update() =>
			W.Query<None<DeadTag>>().For(static (ref PositionComponent position, in VelocityComponent velocity) =>
			{
				MovementSystemForEach(ref position.Value, in velocity.Value);
			});
	}
}

}
