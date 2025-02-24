using System;
using System.Linq;
using StardewValley.Extensions;
using StardewValley.TokenizableStrings;

namespace StardewValley.BellsAndWhistles
{
	public class Lexicon
	{
		/// <summary>
		///
		/// A noun to represent some kind of "bad" object. Kind of has connotations of it being disgusting or cheap. preface with "that" or "such"
		///
		/// </summary>
		/// <returns></returns>
		public static string getRandomNegativeItemSlanderNoun()
		{
			Random random = Utility.CreateDaySaveRandom();
			string[] choices = Game1.content.LoadString("Strings\\Lexicon:RandomNegativeItemNoun").Split('#');
			return random.Choose(choices);
		}

		public static string getProperArticleForWord(string word)
		{
			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				return "";
			}
			if (word != null && word.Length > 0)
			{
				switch (word.ToLower()[0])
				{
				case 'a':
				case 'e':
				case 'i':
				case 'o':
				case 'u':
					return "an";
				}
			}
			return "a";
		}

		public static string capitalize(string text)
		{
			if (string.IsNullOrEmpty(text) || LocalizedContentManager.CurrentLanguageCode != 0)
			{
				return text;
			}
			int positionOfFirstCapitalizableCharacter = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsLetter(text[i]))
				{
					positionOfFirstCapitalizableCharacter = i;
					break;
				}
			}
			if (positionOfFirstCapitalizableCharacter == 0)
			{
				return text[0].ToString().ToUpper() + text.Substring(1);
			}
			return text.Substring(0, positionOfFirstCapitalizableCharacter) + text[positionOfFirstCapitalizableCharacter].ToString().ToUpper() + text.Substring(positionOfFirstCapitalizableCharacter + 1);
		}

		public static string makePlural(string word, bool ignore = false)
		{
			if (ignore || LocalizedContentManager.CurrentLanguageCode != 0 || word == null)
			{
				return word;
			}
			if (word != null)
			{
				switch (word.Length)
				{
				case 12:
				{
					char c = word[0];
					if (c != 'D')
					{
						if (c != 'G')
						{
							if (c != 'R' || !(word == "Rice Pudding"))
							{
								break;
							}
							return "bowls of Rice Pudding";
						}
						if (!(word == "Glass Shards"))
						{
							break;
						}
						goto IL_05c5;
					}
					if (!(word == "Dragon Tooth"))
					{
						break;
					}
					return "Dragon Teeth";
				}
				case 10:
				{
					char c = word[0];
					if ((uint)c <= 67u)
					{
						if (c == 'A')
						{
							if (!(word == "Algae Soup"))
							{
								break;
							}
							return "bowls of Algae Soup";
						}
						if (c != 'C' || !(word == "Crab Cakes"))
						{
							break;
						}
					}
					else if (c != 'H')
					{
						if (c != 'T' || !(word == "Tea Leaves"))
						{
							break;
						}
					}
					else if (!(word == "Hashbrowns"))
					{
						break;
					}
					goto IL_05c5;
				}
				case 4:
					switch (word[3])
					{
					case 'l':
						if (!(word == "Coal"))
						{
							goto end_IL_001f;
						}
						return "lumps of Coal";
					case 't':
						if (!(word == "Salt"))
						{
							goto end_IL_001f;
						}
						return "pieces of Salt";
					case 'p':
						break;
					case 'b':
						goto IL_0322;
					case 'y':
						goto IL_0337;
					case 's':
						goto IL_034c;
					default:
						goto end_IL_001f;
					}
					if (!(word == "Carp"))
					{
						break;
					}
					goto IL_05c5;
				case 5:
				{
					char c = word[4];
					if ((uint)c <= 115u)
					{
						if (c != 'm')
						{
							if (c != 's' || !(word == "Weeds"))
							{
								break;
							}
						}
						else if (!(word == "Bream"))
						{
							break;
						}
						goto IL_05c5;
					}
					switch (c)
					{
					case 'y':
						if (!(word == "Jelly"))
						{
							break;
						}
						return "Jellies";
					case 't':
						if (!(word == "Wheat"))
						{
							break;
						}
						return "bushels of Wheat";
					}
					break;
				}
				case 6:
					switch (word[1])
					{
					case 'i':
						if (!(word == "Ginger"))
						{
							break;
						}
						return "pieces of Ginger";
					case 'a':
						if (!(word == "Garlic"))
						{
							break;
						}
						return "bulbs of Garlic";
					}
					break;
				case 8:
				{
					char c = word[0];
					if (c != 'B')
					{
						if (c != 'P')
						{
							if (c != 'S' || !(word == "Sandfish"))
							{
								break;
							}
						}
						else if (!(word == "Pancakes"))
						{
							break;
						}
					}
					else if (!(word == "Bok Choy"))
					{
						break;
					}
					goto IL_05c5;
				}
				case 14:
				{
					char c = word[0];
					if (c != 'B')
					{
						if (c != 'P' || !(word == "Pepper Poppers"))
						{
							break;
						}
					}
					else if (!(word == "Broken Glasses"))
					{
						break;
					}
					goto IL_05c5;
				}
				case 11:
					switch (word[4])
					{
					case 'b':
						break;
					case 'e':
						goto IL_045d;
					case 'n':
						goto IL_0472;
					case 'd':
						goto IL_0487;
					case ' ':
						goto IL_049c;
					default:
						goto end_IL_001f;
					}
					if (!(word == "Cranberries"))
					{
						break;
					}
					goto IL_05c5;
				case 17:
				{
					char c = word[0];
					if (c != 'D')
					{
						if (c != 'R' || !(word == "Roasted Hazelnuts"))
						{
							break;
						}
					}
					else if (!(word == "Dried Cranberries"))
					{
						break;
					}
					goto IL_05c5;
				}
				case 9:
				{
					char c = word[0];
					if (c != 'D')
					{
						if (c != 'G')
						{
							if (c != 'R' || !(word == "Red Canes"))
							{
								break;
							}
						}
						else if (!(word == "Ghostfish"))
						{
							break;
						}
					}
					else if (!(word == "Driftwood"))
					{
						break;
					}
					goto IL_05c5;
				}
				case 15:
				{
					char c = word[0];
					if (c != 'F')
					{
						if (c != 'L')
						{
							if (c != 'S' || !(word == "Smallmouth Bass"))
							{
								break;
							}
						}
						else if (!(word == "Largemouth Bass"))
						{
							break;
						}
					}
					else if (!(word == "Fossilized Ribs"))
					{
						break;
					}
					goto IL_05c5;
				}
				case 16:
					if (!(word == "Dried Sunflowers"))
					{
						break;
					}
					goto IL_05c5;
				case 3:
					if (!(word == "Hay"))
					{
						break;
					}
					goto IL_05c5;
				case 7:
					if (!(word == "Pickles"))
					{
						break;
					}
					goto IL_05c5;
				case 21:
					{
						if (!(word == "Warp Totem: Mountains"))
						{
							break;
						}
						goto IL_05c5;
					}
					IL_0472:
					if (!(word == "Green Canes"))
					{
						break;
					}
					goto IL_05c5;
					IL_045d:
					if (!(word == "Glazed Yams"))
					{
						break;
					}
					goto IL_05c5;
					IL_05c5:
					return word;
					IL_0322:
					if (!(word == "Chub"))
					{
						break;
					}
					goto IL_05c5;
					IL_0337:
					if (!(word == "Clay"))
					{
						break;
					}
					goto IL_05c5;
					IL_034c:
					if (!(word == "Hops"))
					{
						break;
					}
					goto IL_05c5;
					IL_049c:
					if (!(word == "Star Shards"))
					{
						break;
					}
					goto IL_05c5;
					IL_0487:
					if (!(word == "Mixed Seeds"))
					{
						break;
					}
					goto IL_05c5;
					end_IL_001f:
					break;
				}
			}
			switch (word.Last())
			{
			case 'y':
				return word.Substring(0, word.Length - 1) + "ies";
			case 's':
				if (!word.EndsWith(" Seeds") && !word.EndsWith(" Shorts") && !word.EndsWith(" Bass") && !word.EndsWith(" Flowers") && !word.EndsWith(" Peach"))
				{
					return word + "es";
				}
				return word;
			case 'x':
			case 'z':
				return word + "es";
			default:
				if (word.Length > 2)
				{
					string ending = word.Substring(word.Length - 2);
					if (ending == "sh" || ending == "ch")
					{
						return word + "es";
					}
				}
				return word + "s";
			}
		}

		/// <summary>In English only, prepend an article like 'a' or 'an' to a word.</summary>
		/// <param name="word">The word for which to prepend an article.</param>
		public static string prependArticle(string word)
		{
			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				return word;
			}
			return getProperArticleForWord(word) + " " + word;
		}

		/// <summary>In English only, prepend an article like 'a' or 'an' to a word as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
		/// <param name="word">The tokenizable string which returns the word for which to prepend an article.</param>
		public static string prependTokenizedArticle(string word)
		{
			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				return word;
			}
			return TokenStringBuilder.ArticleFor(word) + " " + word;
		}

		/// <summary>
		///
		/// Adjectives like "wonderful" "amazing" "excellent", prefaced with "are"  "is"  "was" "will be" "usually is", etc.
		/// these wouldn't really make sense for an object, more for a person,place, or event
		/// </summary>
		/// <returns></returns>
		public static string getRandomPositiveAdjectiveForEventOrPerson(NPC n = null)
		{
			Random r = Utility.CreateDaySaveRandom();
			string[] choices = ((n != null && n.Age != 0) ? Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_Child").Split('#') : (n?.Gender switch
			{
				Gender.Male => Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_AdultMale").Split('#'), 
				Gender.Female => Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_AdultFemale").Split('#'), 
				_ => Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_PlaceOrEvent").Split('#'), 
			}));
			return r.Choose(choices);
		}

		/// <summary>
		///
		/// An adjective to represent something tasty, like "delicious", "tasty", "wonderful", "satisfying"
		///
		/// </summary>
		/// <returns></returns>
		public static string getRandomDeliciousAdjective(NPC n = null)
		{
			Random random = Utility.CreateDaySaveRandom();
			string[] choices = ((n == null || n.Age != 2) ? Game1.content.LoadString("Strings\\Lexicon:RandomDeliciousAdjective").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomDeliciousAdjective_Child").Split('#'));
			return random.Choose(choices);
		}

		/// <summary>
		///
		/// Adjective to describe something that is not tasty. "gross", "disgusting", "nasty"
		/// </summary>
		/// <returns></returns>
		public static string getRandomNegativeFoodAdjective(NPC n = null)
		{
			Random random = Utility.CreateDaySaveRandom();
			string[] choices = ((n != null && n.Age == 2) ? Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective_Child").Split('#') : ((n == null || n.Manners != 1) ? Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective_Polite").Split('#')));
			return random.Choose(choices);
		}

		/// <summary>
		///
		/// Adjectives like "decent" "good"
		/// </summary>
		/// <returns></returns>
		public static string getRandomSlightlyPositiveAdjectiveForEdibleNoun(NPC n = null)
		{
			Random random = Utility.CreateDaySaveRandom();
			string[] choices = Game1.content.LoadString("Strings\\Lexicon:RandomSlightlyPositiveFoodAdjective").Split('#');
			return random.Choose(choices);
		}

		/// <summary>Get a generic term for a child of a given gender (i.e. "boy" or "girl").</summary>
		/// <param name="isMale">Whether the child is male.</param>
		public static string getGenderedChildTerm(bool isMale)
		{
			return Game1.content.LoadString(isMale ? "Strings\\Lexicon:ChildTerm_Male" : "Strings\\Lexicon:ChildTerm_Female");
		}

		/// <summary>Get a generic term for a child of a given gender (i.e. "boy" or "girl"), as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
		/// <param name="isMale">Whether the child is male.</param>
		public static string getTokenizedGenderedChildTerm(bool isMale)
		{
			return TokenStringBuilder.LocalizedText(isMale ? "Strings\\Lexicon:ChildTerm_Male" : "Strings\\Lexicon:ChildTerm_Female");
		}

		/// <summary>Get a gendered pronoun (i.e. "him" or "her").</summary>
		/// <param name="isMale">Whether to get a male pronoun.</param>
		public static string getPronoun(bool isMale)
		{
			return Game1.content.LoadString(isMale ? "Strings\\Lexicon:Pronoun_Male" : "Strings\\Lexicon:Pronoun_Female");
		}

		/// <summary>Get a gendered pronoun (i.e. "him" or "her"), as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
		/// <param name="isMale">Whether to get a male pronoun.</param>
		public static string getTokenizedPronoun(bool isMale)
		{
			return TokenStringBuilder.LocalizedText(isMale ? "Strings\\Lexicon:Pronoun_Male" : "Strings\\Lexicon:Pronoun_Female");
		}

		/// <summary>Get a possessive gendered pronoun (i.e. "his" or "her").</summary>
		/// <param name="isMale">Whether to get a male pronoun.</param>
		public static string getPossessivePronoun(bool isMale)
		{
			return Game1.content.LoadString(isMale ? "Strings\\Lexicon:Possessive_Pronoun_Male" : "Strings\\Lexicon:Possessive_Pronoun_Female");
		}

		/// <summary>Get a possessive gendered pronoun (i.e. "his" or "her"), as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
		/// <param name="isMale">Whether to get a male pronoun.</param>
		public static string getTokenizedPossessivePronoun(bool isMale)
		{
			return TokenStringBuilder.LocalizedText(isMale ? "Strings\\Lexicon:Possessive_Pronoun_Male" : "Strings\\Lexicon:Possessive_Pronoun_Female");
		}
	}
}
