/*************************************************************************
 *
 *   file		: AddModifierFlat.cs
 *   copyright		: (C) The WCell Team
 *   email		: info@wcell.org
 *   last changed	: $LastChangedDate: 2010-01-02 06:18:34 +0800 (Sat, 02 Jan 2010) $
 *   last author	: $LastChangedBy: dominikseifert $
 *   revision		: $Rev: 1164 $
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *************************************************************************/

using System.Collections.Generic;
using WCell.Constants.Spells;
using WCell.RealmServer.Entities;

namespace WCell.RealmServer.Spells.Auras.Handlers
{
	public class AddModifierEffectHandler : AuraEffectHandler
	{
		/// <summary>
		/// The amount of remaining charges or 0 if it doesn't need any
		/// </summary>
		public int Charges;
	}

	/// <summary>
	/// All kinds of different Talent modifiers (mostly caused by talents)
	/// </summary>
	public class AddModifierFlatHandler : AddModifierEffectHandler
	{
		protected internal override void Apply()
		{
			var owner = m_aura.Auras.Owner as Character;
			if (owner != null)
			{
				Charges = m_spellEffect.Spell.ProcCharges;
				if (Charges > 0)
				{
					owner.PlayerSpells.ModifiersWithCharges++;
				}
				owner.PlayerSpells.SpellModifiersFlat.Add(this);
				AuraHandler.SendModifierUpdate(owner, m_spellEffect, false);
			}
		}

		protected internal override void Remove(bool cancelled)
		{
			var owner = m_aura.Auras.Owner as Character;
			if (owner != null)
			{
				if (m_spellEffect.Spell.ProcCharges > 0)
				{
					owner.PlayerSpells.ModifiersWithCharges--;
				}
				owner.PlayerSpells.SpellModifiersFlat.Remove(this);
				AuraHandler.SendModifierUpdate(owner, m_spellEffect, false);
			}
		}
	}

	public class AddModifierPercentHandler : AddModifierEffectHandler
	{
		protected internal override void Apply()
		{
			var owner = m_aura.Auras.Owner as Character;
			if (owner != null)
			{
				Charges = m_spellEffect.Spell.ProcCharges;
				if (Charges > 0)
				{
					owner.PlayerSpells.ModifiersWithCharges += 1;
				}
				owner.PlayerSpells.SpellModifiersPct.Add(this);
				AuraHandler.SendModifierUpdate(owner, m_spellEffect, true);
			}
		}

		protected internal override void Remove(bool cancelled)
		{
			var owner = m_aura.Auras.Owner as Character;
			if (owner != null)
			{
				if (m_spellEffect.Spell.ProcCharges > 0)
				{
					owner.PlayerSpells.ModifiersWithCharges -= 1;
				}
				owner.PlayerSpells.SpellModifiersPct.Remove(this);
				AuraHandler.SendModifierUpdate(owner, m_spellEffect, true);
			}
		}
	}
};