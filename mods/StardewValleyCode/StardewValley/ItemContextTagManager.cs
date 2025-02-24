using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;

namespace StardewValley
{
	/// <summary>Handles parsing and caching item context tags.</summary>
	public static class ItemContextTagManager
	{
		/// <summary>A cache of the base context tags by qualified item ID, excluding context tags added dynamically by the item instance.</summary>
		private static readonly Dictionary<string, HashSet<string>> BaseTagsCache = new Dictionary<string, HashSet<string>>();

		/// <summary>Get the base context tags for an item ID, excluding context tags added dynamically by the item instance.</summary>
		/// <param name="itemId">The qualified or unqualified item ID.</param>
		public static HashSet<string> GetBaseContextTags(string itemId)
		{
			ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(itemId);
			if (!BaseTagsCache.TryGetValue(itemData.QualifiedItemId, out var tags))
			{
				IItemDataDefinition itemType = itemData.ItemType;
				tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				string idTag = SanitizeContextTag("id_" + itemData.QualifiedItemId);
				tags.Add(idTag);
				if (itemType.StandardDescriptor != null)
				{
					string legacyIdTag = SanitizeContextTag("id_" + itemData.ItemType.StandardDescriptor + "_" + itemData.ItemId);
					tags.Add(legacyIdTag);
				}
				switch (itemType.Identifier)
				{
				case "(BC)":
				{
					if (!(itemData.RawData is BigCraftableData bigCraftableData))
					{
						break;
					}
					List<string> contextTags2 = bigCraftableData.ContextTags;
					if (contextTags2 == null || contextTags2.Count <= 0)
					{
						break;
					}
					foreach (string tag in bigCraftableData.ContextTags)
					{
						tags.Add(tag);
					}
					break;
				}
				case "(F)":
					if (itemData.RawData is string[] furnitureData)
					{
						string rawTags = ArgUtility.Get(furnitureData, 11);
						tags.AddRange(ArgUtility.SplitBySpace(rawTags));
					}
					break;
				case "(O)":
				{
					if (!(itemData.RawData is ObjectData objectData))
					{
						break;
					}
					List<string> contextTags = objectData.ContextTags;
					if (contextTags != null && contextTags.Count > 0)
					{
						foreach (string tag2 in objectData.ContextTags)
						{
							tags.Add(tag2);
						}
					}
					if (!objectData.GeodeDropsDefaultItems)
					{
						List<ObjectGeodeDropData> geodeDrops = objectData.GeodeDrops;
						if (geodeDrops == null || geodeDrops.Count <= 0)
						{
							goto IL_0207;
						}
					}
					tags.Add("geode");
					goto IL_0207;
				}
				case "(H)":
					{
						if (itemData.RawData is string[] hatData)
						{
							string rawTags2 = ArgUtility.Get(hatData, 4);
							tags.AddRange(ArgUtility.SplitBySpace(rawTags2));
						}
						break;
					}
					IL_0207:
					if (!objectData.CanBeGivenAsGift)
					{
						tags.Add("not_giftable");
					}
					break;
				}
				if (itemData.InternalName != null)
				{
					tags.Add("item_" + SanitizeContextTag(itemData.InternalName));
				}
				if (itemData.ObjectType != null)
				{
					tags.Add("item_type_" + SanitizeContextTag(itemData.ObjectType));
				}
				if (DataLoader.Machines(Game1.content).TryGetValue(itemData.QualifiedItemId, out var machineData))
				{
					tags.Add("is_machine");
					int num;
					if (!machineData.HasOutput)
					{
						List<MachineOutputRule> outputRules = machineData.OutputRules;
						num = ((outputRules != null && outputRules.Count > 0) ? 1 : 0);
					}
					else
					{
						num = 1;
					}
					bool machineOutputs = (byte)num != 0;
					bool machineInputs = machineData.HasInput;
					if (!machineInputs)
					{
						List<MachineOutputRule> outputRules2 = machineData.OutputRules;
						if (outputRules2 != null && outputRules2.Count > 0)
						{
							foreach (MachineOutputRule rule in machineData.OutputRules)
							{
								if (rule.Triggers == null)
								{
									continue;
								}
								foreach (MachineOutputTriggerRule trigger in rule.Triggers)
								{
									if (trigger.Trigger.HasFlag(MachineOutputTrigger.ItemPlacedInMachine))
									{
										machineInputs = true;
										break;
									}
								}
								if (machineInputs)
								{
									break;
								}
							}
						}
					}
					if (machineOutputs)
					{
						tags.Add("machine_output");
					}
					if (machineInputs)
					{
						tags.Add("machine_input");
					}
				}
				if (itemData.Category == -4 && DataLoader.Fish(Game1.content).TryGetValue(itemData.ItemId, out var rawFishData))
				{
					string[] fields = rawFishData.Split('/');
					if (fields[1] == "trap")
					{
						tags.Add("fish_trap_location_" + fields[4]);
					}
					else
					{
						tags.Add("fish_motion_" + fields[2]);
						int difficulty = Convert.ToInt32(fields[1]);
						if (difficulty <= 33)
						{
							tags.Add("fish_difficulty_easy");
						}
						else if (difficulty <= 66)
						{
							tags.Add("fish_difficulty_medium");
						}
						else if (difficulty <= 100)
						{
							tags.Add("fish_difficulty_hard");
						}
						else
						{
							tags.Add("fish_difficulty_extremely_hard");
						}
						tags.Add("fish_favor_weather_" + fields[7]);
					}
				}
				switch (itemData.Category)
				{
				case -26:
					tags.Add("category_artisan_goods");
					break;
				case -21:
					tags.Add("category_bait");
					break;
				case -9:
					tags.Add("category_big_craftable");
					break;
				case -97:
					tags.Add("category_boots");
					break;
				case -100:
					tags.Add("category_clothing");
					break;
				case -7:
					tags.Add("category_cooking");
					break;
				case -8:
					tags.Add("category_crafting");
					break;
				case -5:
					tags.Add("category_egg");
					break;
				case -29:
					tags.Add("category_equipment");
					break;
				case -19:
					tags.Add("category_fertilizer");
					break;
				case -4:
					tags.Add("category_fish");
					break;
				case -80:
					tags.Add("category_flowers");
					break;
				case -79:
					tags.Add("category_fruits");
					break;
				case -24:
					tags.Add("category_furniture");
					break;
				case -2:
					tags.Add("category_gem");
					break;
				case -81:
					tags.Add("category_greens");
					break;
				case -95:
					tags.Add("category_hat");
					break;
				case -25:
					tags.Add("category_ingredients");
					break;
				case -20:
					tags.Add("category_junk");
					break;
				case -999:
					tags.Add("category_litter");
					break;
				case -14:
					tags.Add("category_meat");
					break;
				case -6:
					tags.Add("category_milk");
					break;
				case -12:
					tags.Add("category_minerals");
					break;
				case -28:
					tags.Add("category_monster_loot");
					break;
				case -96:
					tags.Add("category_ring");
					break;
				case -74:
					tags.Add("category_seeds");
					break;
				case -23:
					tags.Add("category_sell_at_fish_shop");
					break;
				case -27:
					tags.Add("category_syrup");
					break;
				case -22:
					tags.Add("category_tackle");
					break;
				case -99:
					tags.Add("category_tool");
					break;
				case -75:
					tags.Add("category_vegetable");
					break;
				case -98:
					tags.Add("category_weapon");
					break;
				case -17:
					tags.Add("category_sell_at_pierres");
					break;
				case -18:
					tags.Add("category_sell_at_pierres_and_marnies");
					break;
				case -15:
					tags.Add("category_metal_resources");
					break;
				case -16:
					tags.Add("category_building_resources");
					break;
				case -101:
					tags.Add("category_trinket");
					break;
				}
				BaseTagsCache[itemData.QualifiedItemId] = tags;
			}
			return tags;
		}

		/// <summary>Get whether an item has a given base context tag, excluding context tags added dynamically by the item instance.</summary>
		/// <param name="itemId">The qualified or unqualified item ID.</param>
		/// <param name="tag">The tag to match.</param>
		public static bool HasBaseTag(string itemId, string tag)
		{
			return GetBaseContextTags(itemId).Contains(tag);
		}

		/// <summary>Get whether a tag query string (containing one or more context tags) matches the given item tags.</summary>
		/// <param name="tagQueryString">The comma-delimited list of context tags. Each tag can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
		/// <param name="tags">The context tags for the item to check.</param>
		public static bool DoesTagQueryMatch(string tagQueryString, HashSet<string> tags)
		{
			return DoAllTagsMatch(tagQueryString?.Split(','), tags);
		}

		/// <summary>Get whether each tag matches the actual item tags.</summary>
		/// <param name="requiredTags">The tag values to match against the actual tag. Each tag can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
		/// <param name="actualTags">The actual tags for the item being checked.</param>
		public static bool DoAllTagsMatch(IList<string> requiredTags, HashSet<string> actualTags)
		{
			if (requiredTags == null || requiredTags.Count == 0)
			{
				return false;
			}
			foreach (string requiredTag in requiredTags)
			{
				if (!DoesTagMatch(requiredTag, actualTags))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Get whether any tag matches the actual item tags.</summary>
		/// <param name="requiredTags">The tag values to match against the actual tag. Each tag can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
		/// <param name="actualTags">The actual tags for the item being checked.</param>
		public static bool DoAnyTagsMatch(IList<string> requiredTags, HashSet<string> actualTags)
		{
			if (requiredTags != null && requiredTags.Count > 0)
			{
				foreach (string requiredTag in requiredTags)
				{
					if (requiredTag != null && requiredTag.Length > 0 && DoesTagMatch(requiredTag, actualTags))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>Get whether a single-tag query matches the given item tags.</summary>
		/// <param name="tag">The tag to match. This can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
		/// <param name="tags">The list of tags to search for a match to <paramref name="tag" />.</param>
		public static bool DoesTagMatch(string tag, HashSet<string> tags)
		{
			if (tag == null)
			{
				return false;
			}
			tag = tag.Trim();
			bool match = true;
			if (tag.StartsWith('!'))
			{
				tag = tag.Substring(1).TrimStart();
				match = false;
			}
			if (tag.Length > 0)
			{
				return tags.Contains(tag) == match;
			}
			return false;
		}

		/// <summary>Get a tag value with invalid characters (like spaces) escaped.</summary>
		/// <param name="tag">The raw tag value to sanitize.</param>
		public static string SanitizeContextTag(string tag)
		{
			return tag.Trim().ToLower().Replace(' ', '_')
				.Replace("'", "");
		}

		/// <summary>Get the color of an item based on its <c>color_*</c> context tag, if any.</summary>
		/// <param name="item">The item whose context tags to check.</param>
		public static Color? GetColorFromTags(Item item)
		{
			foreach (string tag in item.GetContextTags())
			{
				if (!tag.StartsWithIgnoreCase("color_"))
				{
					continue;
				}
				string text = tag.ToLowerInvariant();
				if (text == null)
				{
					continue;
				}
				switch (text.Length)
				{
				case 11:
					switch (text[8])
					{
					case 'a':
						if (!(text == "color_black"))
						{
							break;
						}
						return new Color(45, 45, 45);
					case 'i':
						if (!(text == "color_white"))
						{
							break;
						}
						return Color.White;
					case 'e':
						if (!(text == "color_green"))
						{
							break;
						}
						return new Color(10, 143, 0);
					case 'o':
						if (!(text == "color_brown"))
						{
							break;
						}
						return new Color(130, 73, 37);
					}
					break;
				case 10:
					switch (text[6])
					{
					case 'g':
						if (!(text == "color_gray"))
						{
							if (!(text == "color_gold"))
							{
								break;
							}
							return Color.Gold;
						}
						return Color.Gray;
					case 'p':
						if (!(text == "color_pink"))
						{
							break;
						}
						return new Color(255, 163, 186);
					case 'b':
						if (!(text == "color_blue"))
						{
							break;
						}
						return new Color(46, 85, 183);
					case 'c':
						if (!(text == "color_cyan"))
						{
							break;
						}
						return Color.Cyan;
					case 'l':
						if (!(text == "color_lime"))
						{
							break;
						}
						return Color.Lime;
					case 'j':
						if (!(text == "color_jade"))
						{
							break;
						}
						return new Color(130, 158, 93);
					case 's':
						if (!(text == "color_sand"))
						{
							break;
						}
						return Color.NavajoWhite;
					case 'i':
						if (!(text == "color_iron"))
						{
							break;
						}
						return new Color(197, 213, 224);
					}
					break;
				case 12:
					switch (text[6])
					{
					case 'o':
						if (!(text == "color_orange"))
						{
							break;
						}
						return new Color(255, 128, 0);
					case 'y':
						if (!(text == "color_yellow"))
						{
							break;
						}
						return new Color(255, 230, 0);
					case 'p':
						if (!(text == "color_purple"))
						{
							break;
						}
						return new Color(115, 41, 181);
					case 's':
						if (!(text == "color_salmon"))
						{
							break;
						}
						return new Color(255, 85, 95);
					case 'c':
						if (!(text == "color_copper"))
						{
							break;
						}
						return new Color(179, 85, 0);
					}
					break;
				case 16:
					switch (text[11])
					{
					case '_':
						if (!(text == "color_light_cyan"))
						{
							break;
						}
						return new Color(180, 255, 255);
					case 'a':
						if (!(text == "color_aquamarine"))
						{
							break;
						}
						return Color.Aquamarine;
					case 'g':
						if (!(text == "color_dark_green"))
						{
							break;
						}
						return Color.DarkGreen;
					case 'b':
						if (!(text == "color_dark_brown"))
						{
							break;
						}
						return Color.SaddleBrown;
					}
					break;
				case 15:
					switch (text[11])
					{
					case 'r':
						if (!(text == "color_sea_green"))
						{
							break;
						}
						return Color.SeaGreen;
					case 's':
						if (!(text == "color_poppyseed"))
						{
							break;
						}
						return new Color(82, 47, 153);
					case 'b':
						if (!(text == "color_dark_blue"))
						{
							break;
						}
						return Color.DarkBlue;
					case 'p':
						if (!(text == "color_dark_pink"))
						{
							break;
						}
						return Color.DeepPink;
					case 'c':
						if (!(text == "color_dark_cyan"))
						{
							break;
						}
						return Color.DarkCyan;
					case 'g':
						if (!(text == "color_dark_gray"))
						{
							break;
						}
						return Color.DarkGray;
					}
					break;
				case 17:
					switch (text[11])
					{
					case 'o':
						if (!(text == "color_dark_orange"))
						{
							break;
						}
						return Color.DarkOrange;
					case 'y':
						if (!(text == "color_dark_yellow"))
						{
							break;
						}
						return Color.DarkGoldenrod;
					case 'p':
						if (!(text == "color_dark_purple"))
						{
							break;
						}
						return Color.DarkViolet;
					}
					break;
				case 9:
					if (!(text == "color_red"))
					{
						break;
					}
					return new Color(220, 0, 0);
				case 18:
					if (!(text == "color_yellow_green"))
					{
						break;
					}
					return Color.GreenYellow;
				case 21:
					if (!(text == "color_pale_violet_red"))
					{
						break;
					}
					return Color.PaleVioletRed;
				case 14:
					if (!(text == "color_dark_red"))
					{
						break;
					}
					return Color.DarkRed;
				case 13:
					if (!(text == "color_iridium"))
					{
						break;
					}
					return new Color(105, 15, 255);
				}
			}
			return null;
		}

		/// <summary>Reset all cached item context tags.</summary>
		/// <remarks>This is called from <see cref="M:StardewValley.ItemRegistry.RebuildCache" /> and generally shouldn't be called directly by other code.</remarks>
		internal static void ResetCache()
		{
			BaseTagsCache.Clear();
		}
	}
}
