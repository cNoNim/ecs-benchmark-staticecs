using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct UpdateVelocitySystem : ISystem
	{
		public void Update() =>
			W.Query<None<DeadTag>>()
			 .For(static (ref VelocityComponent velocity, ref UnitComponent unit, in DataComponent data,
						  in PositionComponent position) =>
			  {
				  UpdateVelocitySystemForEach(ref velocity.Value, ref unit.Value, in data.Value, in position.Value);
			  });
	}
}

}
