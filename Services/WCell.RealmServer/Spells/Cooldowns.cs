using System;
using Castle.ActiveRecord;

namespace WCell.RealmServer.Spells
{
	public interface ICooldown
	{
		uint Identifier { get; }
		DateTime Until { get; set; }
		IConsistentCooldown AsConsistent();
	}

	public interface IConsistentCooldown : ICooldown
	{
		uint CharId { get; set; }

		void SaveAndFlush();
		void UpdateAndFlush();
		void CreateAndFlush();
		void DeleteAndFlush();
	}

	public interface ISpellIdCooldown : ICooldown
	{
		uint SpellId { get; set; }
		uint ItemId { get; set; }
	}

	public interface ISpellCategoryCooldown : ICooldown
	{
		uint SpellId { get; set; }
		uint CategoryId { get; set; }
		uint ItemId { get; set; }
	}

	public class SpellIdCooldown : ISpellIdCooldown
	{
		public DateTime Until
		{
			get;
			set;
		}
		public uint SpellId
		{
			get;
			set;
		}
		public uint ItemId
		{
			get;
			set;
		}

		public uint Identifier
		{
			get { return SpellId; }
		}

		public IConsistentCooldown AsConsistent()
		{
			return new ConsistentSpellIdCooldown {
				Until = Until,
				SpellId = SpellId,
				ItemId = ItemId
			};
		}
	}

	public class SpellCategoryCooldown : ISpellCategoryCooldown
	{
		public DateTime Until
		{
			get;
			set;
		}
		public uint SpellId
		{
			get;
			set;
		}
		public uint CategoryId
		{
			get;
			set;
		}
		public uint ItemId
		{
			get;
			set;
		}

		public uint Identifier
		{
			get { return CategoryId; }
		}

		public IConsistentCooldown AsConsistent()
		{
			return new ConsistentSpellCategoryCooldown {
				Until = Until,
				CategoryId = CategoryId,
				ItemId = ItemId
			};
		}
	}

	[ActiveRecord("SpellIdCooldown", Access = PropertyAccess.Property)]
	public class ConsistentSpellIdCooldown : ActiveRecordBase<ConsistentSpellIdCooldown>, ISpellIdCooldown, IConsistentCooldown
	{
		[Field("SpellId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _spellId;
		[Field("ItemId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _itemId;
		[Field("CharId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _charId;

		[PrimaryKey(PrimaryKeyType.Increment)]
		public long Id
		{
			get;
			set;
		}

		public uint Identifier
		{
			get { return (uint)_spellId; }
		}

		public uint CharId
		{
			get
			{
				return (uint)_charId;
			}
			set
			{
				_charId = (int)value;
			}
		}

		[Property]
		public DateTime Until
		{
			get;
			set;
		}

		public uint SpellId
		{
			get
			{
				return (uint)_spellId;
			}
			set
			{
				_spellId = (int)value;
			}
		}

		public uint ItemId
		{
			get
			{
				return (uint)_itemId;
			}
			set
			{
				_itemId = (int)value;
			}
		}

		public IConsistentCooldown AsConsistent()
		{
			return this;
		}
	}

	[ActiveRecord("SpellCategoryCooldown", Access = PropertyAccess.Property)]
	public class ConsistentSpellCategoryCooldown : ActiveRecordBase<ConsistentSpellCategoryCooldown>, ISpellCategoryCooldown, IConsistentCooldown
	{
		[Field("CatId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _catId;
		[Field("ItemId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _itemId;
		[Field("CharId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _charId;
		[Field("SpellId", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		private int _spellId;

		[PrimaryKey(PrimaryKeyType.Increment)]
		public long Id
		{
			get;
			set;
		}

		public uint Identifier
		{
			get { return (uint)_catId; }
		}

		public uint SpellId
		{
			get
			{
				return (uint)_spellId;
			}
			set
			{
				_spellId = (int)value;
			}
		}

		public uint CharId
		{
			get
			{
				return (uint)_charId;
			}
			set
			{
				_charId = (int)value;
			}
		}

		[Property]
		public DateTime Until
		{
			get;
			set;
		}

		public uint CategoryId
		{
			get
			{
				return (uint)_catId;
			}
			set
			{
				_catId = (int)value;
			}
		}

		public uint ItemId
		{
			get
			{
				return (uint)_itemId;
			}
			set
			{
				_itemId = (int)value;
			}
		}

		public IConsistentCooldown AsConsistent()
		{
			return this;
		}
	}
}