using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct UpdateDataSystem : ISystem, W.IQueryBlock.Write<DataComponent>
	{
		public void Update() =>
			W.Query()
			 .WriteBlock<DataComponent>()
			 .For(ref this);

		[MethodImpl(AggressiveInlining)]
		public void Invoke(uint count, W.EntityBlock _, Block<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
			{
				UpdateDataSystemForEach(ref data[i].Value);
			}
		}
	}
}

}
