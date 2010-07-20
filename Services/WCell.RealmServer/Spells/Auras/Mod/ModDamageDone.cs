/*************************************************************************
 *
 *   file		: ModDamageDone.cs
 *   copyright		: (C) The WCell Team
 *   email		: info@wcell.org
 *   last changed	: $LastChangedDate: 2010-01-25 18:19:39 +0100 (ma, 25 jan 2010) $
 *   last author	: $LastChangedBy: dominikseifert $
 *   revision		: $Rev: 1222 $
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *************************************************************************/

using WCell.RealmServer.Entities;

namespace WCell.RealmServer.Spells.Auras.Handlers
{
	public class ModDamageDoneHandler : AuraEffectHandler
	{
		protected internal override void Apply()
		{
			if (m_aura.Auras.Owner is Character)
			{
				((Character)m_aura.Auras.Owner).AddDamageMod(m_spellEffect.MiscBitSet, EffectValue);
			}
		}

		protected internal override void Remove(bool cancelled)
		{
			if (m_aura.Auras.Owner is Character)
			{
				((Character)m_aura.Auras.Owner).RemoveDamageMod(m_spellEffect.MiscBitSet, EffectValue);
			}
		}
	}
};