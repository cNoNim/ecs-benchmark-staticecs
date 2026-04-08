using System;
using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct DamageSystem : ISystem, W.IQueryBlock.Write<AttackComponent>
	{
		public void Update() =>
			W.Query().WriteBlock<AttackComponent>().For(ref this);

		[MethodImpl(AggressiveInlining)]
		public void Invoke(uint count, W.EntityBlock entities, Block<AttackComponent> attackComponent)
		{
			for (uint i = 0; i < count; i++)
			{
				ref var attack = ref attackComponent[i].Value;
				if (attack.Ticks-- > 0)
					continue;

				if (attack.Target.TryUnpack<StaticWorld>(out var target) && !target.Has<DeadTag>())
				{
					ref var          health = ref target.Ref<HealthComponent>().Value;
					ref readonly var damage = ref target.Read<DamageComponent>().Value;
					ApplyDamageSequential(ref health, in damage, in attack);
				}

				entities[i].Destroy();
			}
		}
	}
}

}
