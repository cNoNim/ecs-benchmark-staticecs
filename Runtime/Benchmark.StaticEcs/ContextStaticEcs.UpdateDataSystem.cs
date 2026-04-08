using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct UpdateDataSystem : ISystem
	{
		public void Update() =>
			W.Query().For(static (ref DataComponent data) => UpdateDataSystemForEach(ref data.Value));
	}
}

}
