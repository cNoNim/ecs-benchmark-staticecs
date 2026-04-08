using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct DamageSystem : ISystem
	{
		public void Update() =>
			W.Query().For(static (World<StaticWorld>.Entity entity, ref AttackComponent attackComponent) =>
			{
				ref var attack = ref attackComponent.Value;
				if (attack.Ticks-- > 0)
					return;

				if (attack.Target.TryUnpack<StaticWorld>(out var target)
				 && !target.Has<DeadTag>())
				{
					ref var          health = ref target.Ref<HealthComponent>().Value;
					ref readonly var damage = ref target.Read<DamageComponent>().Value;
					ApplyDamageSequential(ref health, in damage, in attack);
				}

				entity.Destroy();
			});
	}
}

}
