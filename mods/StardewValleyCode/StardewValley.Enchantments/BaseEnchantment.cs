using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace StardewValley.Enchantments
{
	[XmlInclude(typeof(BaseWeaponEnchantment))]
	[XmlInclude(typeof(ArtfulEnchantment))]
	[XmlInclude(typeof(BugKillerEnchantment))]
	[XmlInclude(typeof(CrusaderEnchantment))]
	[XmlInclude(typeof(HaymakerEnchantment))]
	[XmlInclude(typeof(MagicEnchantment))]
	[XmlInclude(typeof(VampiricEnchantment))]
	[XmlInclude(typeof(AxeEnchantment))]
	[XmlInclude(typeof(HoeEnchantment))]
	[XmlInclude(typeof(MilkPailEnchantment))]
	[XmlInclude(typeof(PanEnchantment))]
	[XmlInclude(typeof(PickaxeEnchantment))]
	[XmlInclude(typeof(ShearsEnchantment))]
	[XmlInclude(typeof(WateringCanEnchantment))]
	[XmlInclude(typeof(ArchaeologistEnchantment))]
	[XmlInclude(typeof(AutoHookEnchantment))]
	[XmlInclude(typeof(BottomlessEnchantment))]
	[XmlInclude(typeof(EfficientToolEnchantment))]
	[XmlInclude(typeof(GenerousEnchantment))]
	[XmlInclude(typeof(MasterEnchantment))]
	[XmlInclude(typeof(PowerfulEnchantment))]
	[XmlInclude(typeof(PreservingEnchantment))]
	[XmlInclude(typeof(ReachingToolEnchantment))]
	[XmlInclude(typeof(ShavingEnchantment))]
	[XmlInclude(typeof(SwiftToolEnchantment))]
	[XmlInclude(typeof(FisherEnchantment))]
	[XmlInclude(typeof(AmethystEnchantment))]
	[XmlInclude(typeof(AquamarineEnchantment))]
	[XmlInclude(typeof(DiamondEnchantment))]
	[XmlInclude(typeof(EmeraldEnchantment))]
	[XmlInclude(typeof(JadeEnchantment))]
	[XmlInclude(typeof(RubyEnchantment))]
	[XmlInclude(typeof(TopazEnchantment))]
	[XmlInclude(typeof(AttackEnchantment))]
	[XmlInclude(typeof(DefenseEnchantment))]
	[XmlInclude(typeof(SlimeSlayerEnchantment))]
	[XmlInclude(typeof(CritEnchantment))]
	[XmlInclude(typeof(WeaponSpeedEnchantment))]
	[XmlInclude(typeof(CritPowerEnchantment))]
	[XmlInclude(typeof(LightweightEnchantment))]
	[XmlInclude(typeof(SlimeGathererEnchantment))]
	[XmlInclude(typeof(GalaxySoulEnchantment))]
	public class BaseEnchantment : INetObject<NetFields>
	{
		[XmlIgnore]
		protected string _displayName;

		[XmlIgnore]
		protected bool _applied;

		[XmlIgnore]
		[InstancedStatic]
		public static bool hideEnchantmentName;

		[XmlIgnore]
		[InstancedStatic]
		public static bool hideSecondaryEnchantName;

		protected static List<BaseEnchantment> _enchantments;

		protected readonly NetInt level = new NetInt(1);

		[XmlIgnore]
		public NetFields NetFields { get; } = new NetFields("BaseEnchantment");


		[XmlElement("level")]
		public int Level
		{
			get
			{
				return level.Value;
			}
			set
			{
				level.Value = value;
			}
		}

		public BaseEnchantment()
		{
			InitializeNetFields();
		}

		public static BaseEnchantment GetEnchantmentFromItem(Item base_item, Item item)
		{
			if (base_item == null || (base_item is MeleeWeapon w && !w.isScythe()))
			{
				string text = item?.QualifiedItemId;
				if (text != null)
				{
					switch (text.Length)
					{
					case 5:
						switch (text[4])
						{
						case '0':
							if (!(text == "(O)60"))
							{
								if (!(text == "(O)70"))
								{
									break;
								}
								return new JadeEnchantment();
							}
							return new EmeraldEnchantment();
						case '2':
							if (!(text == "(O)62"))
							{
								if (!(text == "(O)72"))
								{
									break;
								}
								return new DiamondEnchantment();
							}
							return new AquamarineEnchantment();
						case '4':
							if (!(text == "(O)64"))
							{
								break;
							}
							return new RubyEnchantment();
						case '6':
							if (!(text == "(O)66"))
							{
								break;
							}
							return new AmethystEnchantment();
						case '8':
							if (!(text == "(O)68"))
							{
								break;
							}
							return new TopazEnchantment();
						}
						break;
					case 6:
						if (!(text == "(O)896") || !((base_item as MeleeWeapon)?.isGalaxyWeapon() ?? false))
						{
							break;
						}
						return new GalaxySoulEnchantment();
					}
				}
			}
			if (item?.QualifiedItemId == "(O)74")
			{
				return Utility.CreateRandom(Game1.stats.Get("timesEnchanted"), Game1.uniqueIDForThisGame, Game1.player.UniqueMultiplayerID).ChooseFrom(GetAvailableEnchantmentsForItem(base_item as Tool));
			}
			return null;
		}

		public static List<BaseEnchantment> GetAvailableEnchantmentsForItem(Tool item)
		{
			List<BaseEnchantment> item_enchantments = new List<BaseEnchantment>();
			if (item == null)
			{
				return GetAvailableEnchantments();
			}
			List<BaseEnchantment> enchantments = GetAvailableEnchantments();
			HashSet<Type> applied_enchantments = new HashSet<Type>();
			foreach (BaseEnchantment enchantment2 in item.enchantments)
			{
				applied_enchantments.Add(enchantment2.GetType());
			}
			foreach (BaseEnchantment enchantment in enchantments)
			{
				if (enchantment.CanApplyTo(item) && !applied_enchantments.Contains(enchantment.GetType()))
				{
					item_enchantments.Add(enchantment);
				}
			}
			foreach (string previousEnchantment in item.previousEnchantments)
			{
				if (item_enchantments.Count > 1)
				{
					item_enchantments.RemoveAll((BaseEnchantment cur) => cur.GetName() == previousEnchantment);
					continue;
				}
				return item_enchantments;
			}
			return item_enchantments;
		}

		public static List<BaseEnchantment> GetAvailableEnchantments()
		{
			if (_enchantments == null)
			{
				_enchantments = new List<BaseEnchantment>
				{
					new ArtfulEnchantment(),
					new BugKillerEnchantment(),
					new VampiricEnchantment(),
					new CrusaderEnchantment(),
					new HaymakerEnchantment(),
					new PowerfulEnchantment(),
					new ReachingToolEnchantment(),
					new ShavingEnchantment(),
					new BottomlessEnchantment(),
					new GenerousEnchantment(),
					new ArchaeologistEnchantment(),
					new MasterEnchantment(),
					new AutoHookEnchantment(),
					new PreservingEnchantment(),
					new EfficientToolEnchantment(),
					new SwiftToolEnchantment(),
					new FisherEnchantment()
				};
			}
			return _enchantments;
		}

		/// <summary>Reset cached enchantment data.</summary>
		public static void ResetEnchantments()
		{
			_enchantments = null;
		}

		public virtual bool IsForge()
		{
			return false;
		}

		public virtual bool IsSecondaryEnchantment()
		{
			return false;
		}

		public virtual void InitializeNetFields()
		{
			NetFields.SetOwner(this).AddField(level, "level");
		}

		public void OnEquip(Farmer farmer)
		{
			if (!_applied)
			{
				farmer.enchantments.Add(this);
				_applied = true;
				_OnEquip(farmer);
			}
		}

		public void OnUnequip(Farmer farmer)
		{
			if (_applied)
			{
				farmer.enchantments.Remove(this);
				_applied = false;
				_OnUnequip(farmer);
			}
		}

		protected virtual void _OnEquip(Farmer who)
		{
		}

		protected virtual void _OnUnequip(Farmer who)
		{
		}

		/// <summary>Apply effects to the base damage applied to a monster before modifiers like the monster's defense stat.</summary>
		/// <param name="monster">The monster being attacked.</param>
		/// <param name="location">The location containing the monster.</param>
		/// <param name="who">The player attacking the monster.</param>
		/// <param name="fromBomb">Whether the damage is from a bomb placed by the player.</param>
		/// <param name="amount">The amount of damage that will be applied.</param>
		public virtual void OnCalculateDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, ref int amount)
		{
		}

		/// <summary>Apply effects after damage is applied to a monster.</summary>
		/// <param name="monster">The monster being attacked.</param>
		/// <param name="location">The location containing the monster.</param>
		/// <param name="who">The player attacking the monster.</param>
		/// <param name="fromBomb">Whether the damage is from a bomb placed by the player.</param>
		/// <param name="amount">The amount of damage that was applied, after accounting for modifiers like the monster's defense stat.</param>
		public virtual void OnDealtDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, int amount)
		{
		}

		/// <summary>Apply effects when a monster is slain.</summary>
		/// <param name="monster">The monster slain.</param>
		/// <param name="location">The location where the monster was slain.</param>
		/// <param name="who">The player which has the enchantment effects.</param>
		/// <param name="slainByBomb">Whether the monster was killed by a bomb placed by the player.</param>
		public virtual void OnMonsterSlay(Monster monster, GameLocation location, Farmer who, bool slainByBomb)
		{
		}

		public virtual void AddEquipmentEffects(BuffEffects effects)
		{
		}

		public void OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
		{
			_OnCutWeed(tile_location, location, who);
		}

		protected virtual void _OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
		{
		}

		public virtual BaseEnchantment GetOne()
		{
			BaseEnchantment obj = Activator.CreateInstance(GetType()) as BaseEnchantment;
			obj.level.Value = level.Value;
			return obj;
		}

		public int GetLevel()
		{
			return level.Value;
		}

		public void SetLevel(Item item, int new_level)
		{
			if (new_level < 1)
			{
				new_level = 1;
			}
			else if (GetMaximumLevel() >= 0 && new_level > GetMaximumLevel())
			{
				new_level = GetMaximumLevel();
			}
			if (level.Value != new_level)
			{
				UnapplyTo(item);
				level.Value = new_level;
				ApplyTo(item);
			}
		}

		public virtual int GetMaximumLevel()
		{
			return -1;
		}

		public void ApplyTo(Item item, Farmer farmer = null)
		{
			_ApplyTo(item);
			if (IsItemCurrentlyEquipped(item, farmer))
			{
				OnEquip(farmer);
			}
		}

		protected virtual void _ApplyTo(Item item)
		{
		}

		public bool IsItemCurrentlyEquipped(Item item, Farmer farmer)
		{
			if (farmer == null)
			{
				return false;
			}
			return _IsCurrentlyEquipped(item, farmer);
		}

		protected virtual bool _IsCurrentlyEquipped(Item item, Farmer farmer)
		{
			return farmer.CurrentTool == item;
		}

		public void UnapplyTo(Item item, Farmer farmer = null)
		{
			_UnapplyTo(item);
			if (IsItemCurrentlyEquipped(item, farmer))
			{
				OnUnequip(farmer);
			}
		}

		protected virtual void _UnapplyTo(Item item)
		{
		}

		public virtual bool CanApplyTo(Item item)
		{
			return true;
		}

		public string GetDisplayName()
		{
			if (_displayName == null)
			{
				_displayName = Game1.content.LoadStringReturnNullIfNotFound("Strings\\EnchantmentNames:" + GetName());
				if (_displayName == null)
				{
					_displayName = GetName();
				}
			}
			return _displayName;
		}

		public virtual string GetName()
		{
			return "Unknown Enchantment";
		}

		public virtual bool ShouldBeDisplayed()
		{
			return true;
		}
	}
}
