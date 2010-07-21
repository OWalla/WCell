using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCell.Constants.Spells;
using WCell.Core.Initialization;
using WCell.RealmServer.Misc;
using WCell.RealmServer.Spells;
using WCell.RealmServer.Spells.Auras;
using WCell.RealmServer.Spells.Auras.Misc;
using WCell.RealmServer.Spells.Effects;

namespace WCell.Addons.Default.Spells.Warrior
{
	public static class WarriorProtectionFixes
	{
		[Initialization(InitializationPass.Second)]
		public static void FixIt()
		{

			// Shield Spec is proc'ed when owner dodges, blocks or parries
			SpellLineId.WarriorProtectionShieldSpecialization.Apply(spell =>
			{
				spell.AddCasterProcHandler(new TriggerSpellProcHandler(
					ProcTriggerFlags.MeleeAttack | ProcTriggerFlags.RangedAttack,
					ProcHandler.DodgeBlockOrParryValidator,
					SpellHandler.Get(SpellId.EffectShieldSpecializationRank1),
					spell.ProcChance
					));
			});

			// Gag Order needs a custom proc trigger and the correct auratype
			SpellLineId.WarriorProtectionGagOrder.Apply(spell =>
			{
				spell.AddCasterProcSpells(SpellLineId.WarriorShieldBash, SpellLineId.WarriorHeroicThrow);
				var effect = spell.GetAuraEffect(AuraType.Dummy);
				if (effect != null)
				{
					effect.AuraType = AuraType.ProcTriggerSpell;
				}
			});


			// Concussion Blow deals AP based school damage
			SpellLineId.WarriorProtectionConcussionBlow.Apply(spell =>
			{
				var effect = spell.GetEffect(SpellEffectType.Dummy);
				if (effect != null)
				{
					effect.SpellEffectHandlerCreator = (cast, eff) => new SchoolDamageByAPPctEffectHandler(cast, eff);
				}
				effect = spell.GetEffect(SpellEffectType.SchoolDamage);
				if (effect != null)
				{
					// dont need this one
					effect.IsUsed = false;
				}
			});

			// Last Stand has a Dummy instead of applying an Aura
			SpellEffect lastStandEffect = null;
			SpellLineId.WarriorProtectionLastStand.Apply(spell =>
			{
				lastStandEffect = spell.GetEffect(SpellEffectType.Dummy);
				if (lastStandEffect != null)
				{
					lastStandEffect.EffectType = SpellEffectType.TriggerSpell;
					lastStandEffect.TriggerSpellId = SpellId.ClassSkillLastStand;
				}
			});
			SpellHandler.Apply(spell =>
			{
				var effect = spell.GetEffect(AuraType.ModIncreaseHealth);
				if (effect != null && lastStandEffect != null)
				{
					effect.AuraEffectHandlerCreator = () => new AddMaxHealthPctToHealthHandler();
					effect.BasePoints = lastStandEffect.BasePoints;
					effect.DiceSides = lastStandEffect.DiceSides;
				}
			}, SpellId.ClassSkillLastStand);

			// Safe Guard should only affect Intervene
			SpellLineId.WarriorProtectionSafeguard.Apply(spell =>
			{
				spell.Effects.First().AffectMask = SpellHandler.Get(SpellId.ClassSkillIntervene).SpellClassMask;
			});

			// S&B should only affect Devastate and Revenge
			SpellLineId.WarriorProtectionSwordAndBoard.Apply(spell =>
			{
				var effect = spell.GetEffect(AuraType.ProcTriggerSpell);
				effect.AddToAffectMask(SpellLineId.WarriorRevenge, SpellLineId.WarriorProtectionDevastate);
			});

			// Shockwave needs to deal damage in % of AP
			SpellLineId.WarriorProtectionShockwave.Apply(spell =>
			{
				var stunEffect = spell.GetEffect(AuraType.ModStun);
				var effect = spell.GetEffect(SpellEffectType.Dummy);
				effect.ImplicitTargetA = stunEffect.ImplicitTargetA;
				effect.Radius = stunEffect.Radius;

				effect.SpellEffectHandlerCreator = (cast, effct) => new SchoolDamageByAPPctEffectHandler(cast, effct);
			});

			// Damage Shield should reflect damage on block
			SpellLineId.WarriorProtectionDamageShield.Apply(spell =>
			{
				var effect = spell.GetEffect(AuraType.Dummy);
				effect.AuraEffectHandlerCreator = () => new DamageShieldHandler();
			});
		}

		public class AddMaxHealthPctToHealthHandler : AuraEffectHandler
		{
			private int health;

			protected override void Apply()
			{
				health = ((Owner.MaxHealth * EffectValue) + 50) / 100;	//rounded
				Owner.Health += health;
			}

			protected override void Remove(bool cancelled)
			{
				Owner.Health -= health;
			}
		}

		class DamageShieldHandler : AttackEventEffectHandler
		{

			public override void OnBeforeAttack(DamageAction action)
			{
				// do nothing
			}

			public override void OnAttack(DamageAction action)
			{
				// do nothing
			}

			public override void OnDefend(DamageAction action)
			{
				// inflict damage upon block
				if (action.Blocked > 0)
				{
					var dmg = (action.Blocked * EffectValue + 50) / 100;
					action.Victim.AddMessage(() =>
					{
						if (action.Attacker.IsInWorld && action.Victim.MayAttack(action.Attacker))
						{
							// TODO: Add mods to damage?
							action.Attacker.DoSpellDamage(action.Victim, SpellEffect, dmg);
						}
					});
				}
			}
		}
	}
}
