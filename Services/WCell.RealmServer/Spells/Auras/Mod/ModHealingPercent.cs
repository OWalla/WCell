using WCell.Constants;
using WCell.RealmServer.Entities;
using WCell.RealmServer.Modifiers;

namespace WCell.RealmServer.Spells.Auras.Handlers
{
    /// <summary>
    /// Increases healing done by %
    /// </summary>
    public class ModHealingTakenPctHandler : AuraEffectHandler
    {
        protected internal override void Apply()
        {
        	var owner = Owner as Character;
			if (owner != null)
			{
				owner.HealingTakenModPct += EffectValue;
			}
        }

        protected internal override void Remove(bool cancelled)
		{
			var owner = Owner as Character;
			if (owner != null)
			{
				owner.HealingTakenModPct -= EffectValue;
			}
        }
    }
};