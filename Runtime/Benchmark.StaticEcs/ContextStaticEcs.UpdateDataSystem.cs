using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct UpdateDataSystem
		: ISystem, World<StaticWorld>.IQueryBlock.Write<DataComponent>
	{
		public void Update() =>
			W.Query()
			 .WriteBlock<DataComponent>()
			 .For(ref this);

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(uint count, World<StaticWorld>.EntityBlock _, Block<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
				UpdateDataSystemForEach(ref data[i].Value);
		}
	}
}

}
