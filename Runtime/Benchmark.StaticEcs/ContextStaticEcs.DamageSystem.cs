using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct DamageSystem
		: ISystem, World<StaticWorld>.IQueryBlock.Write<AttackComponent>
	{
		public void Update() =>
			W.Query()
			 .WriteBlock<AttackComponent>()
			 .For(ref this);

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(uint count, World<StaticWorld>.EntityBlock entities, Block<AttackComponent> attackComponent)
		{
			for (uint i = 0; i < count; i++)
			{
				ref var attack = ref attackComponent[i].Value;
				if (attack.Ticks-- > 0)
					continue;

				if (attack.Target.TryUnpack<StaticWorld>(out var target)
				 && !target.Has<DeadTag>())
				{
					ref var health = ref target.Ref<HealthComponent>()
											   .Value;
					ref readonly var damage = ref target.Read<DamageComponent>()
														.Value;
					ApplyDamageSequential(ref health, in damage, in attack);
				}

				entities[i]
				   .Destroy();
			}
		}
	}
}

}
