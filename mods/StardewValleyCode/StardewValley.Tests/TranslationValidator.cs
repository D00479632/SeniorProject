using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StardewValley.Tests
{
	/// <summary>Provides methods to compare and validate translations, used in the game's internal unit tests.</summary>
	public class TranslationValidator
	{
		/// <summary>Converts raw text into language-independent syntax representations, which can be compared between languages.</summary>
		private readonly SyntaxAbstractor Abstractor = new SyntaxAbstractor();

		/// <summary>Compare the base and translated variants of an asset and return a list of keys which are missing, unknown, or have a different syntax.</summary>
		/// <typeparam name="TValue">The value type in the asset data.</typeparam>
		/// <param name="baseData">The original untranslated data.</param>
		/// <param name="translatedData">The translated data.</param>
		/// <param name="getText">Get the text to compare for an entry.</param>
		/// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
		public IEnumerable<TranslationValidatorResult> Compare<TValue>(Dictionary<string, TValue> baseData, Dictionary<string, TValue> translatedData, Func<TValue, string> getText, string baseAssetName)
		{
			return Compare(baseData, translatedData, getText, (string key, string text) => Abstractor.ExtractSyntaxFor(baseAssetName, key, text));
		}

		/// <summary>Compare the base and translated variants of an asset and return a list of keys which are missing, unknown, or have a different syntax.</summary>
		/// <typeparam name="TValue">The value type in the asset data.</typeparam>
		/// <param name="baseData">The original untranslated data.</param>
		/// <param name="translatedData">The translated data.</param>
		/// <param name="getText">Get the text to compare for an entry.</param>
		/// <param name="getSyntax">Get the syntax for a data entry, given its key and value.</param>
		public IEnumerable<TranslationValidatorResult> Compare<TValue>(Dictionary<string, TValue> baseData, Dictionary<string, TValue> translatedData, Func<TValue, string> getText, Func<string, string, string> getSyntax)
		{
			foreach (KeyValuePair<string, TValue> basePair in baseData)
			{
				string key2 = basePair.Key;
				string baseText = getText(basePair.Value);
				if (!translatedData.TryGetValue(key2, out var translationEntry))
				{
					yield return new TranslationValidatorResult(TranslationValidatorIssue.MissingKey, key2, getSyntax(key2, baseText), baseText, null, null, "Key not found in the translated asset.");
					continue;
				}
				string translationText2 = getText(translationEntry);
				TranslationValidatorResult syntaxResult = CompareEntry(key2, baseText, translationText2, (string value) => getSyntax(key2, value));
				if (syntaxResult != null)
				{
					yield return syntaxResult;
				}
			}
			foreach (KeyValuePair<string, TValue> translatedPair in translatedData)
			{
				string key = translatedPair.Key;
				if (!baseData.ContainsKey(key))
				{
					string translationText = getText(translatedPair.Value);
					string translationSyntax = getSyntax(key, translationText);
					yield return new TranslationValidatorResult(TranslationValidatorIssue.UnknownKey, key, null, null, translationSyntax, translationText, "Unknown key in translation which isn't in the base asset.");
				}
			}
		}

		/// <summary>Compare the base and translated variants of a single entry in an asset and return a result if the entries have a different syntax.</summary>
		/// <param name="key">The key for this entry in the asset.</param>
		/// <param name="baseText">The original untranslated text.</param>
		/// <param name="translationText">The translated text.</param>
		/// <param name="getSyntax">Get the syntax for an entry, given its value.</param>
		/// <returns>Returns the validator result if an issue was found, else <c>null</c>.</returns>
		public TranslationValidatorResult CompareEntry(string key, string baseText, string translationText, Func<string, string> getSyntax)
		{
			string baseSyntax = getSyntax(baseText);
			string translationSyntax = getSyntax(translationText);
			if (baseSyntax != translationSyntax)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(148, 6);
				defaultInterpolatedStringHandler.AppendLiteral("The translation has a different syntax than the base text.\n");
				defaultInterpolatedStringHandler.AppendLiteral("Syntax:\n");
				defaultInterpolatedStringHandler.AppendLiteral("    base:  ");
				defaultInterpolatedStringHandler.AppendFormatted(baseSyntax);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("    local: ");
				defaultInterpolatedStringHandler.AppendFormatted(translationSyntax);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("           ");
				defaultInterpolatedStringHandler.AppendFormatted("".PadRight(GetDiffIndex(baseSyntax, translationSyntax), ' '));
				defaultInterpolatedStringHandler.AppendLiteral("^\n");
				defaultInterpolatedStringHandler.AppendLiteral("Text:\n");
				defaultInterpolatedStringHandler.AppendLiteral("    base:  ");
				defaultInterpolatedStringHandler.AppendFormatted(baseText);
				defaultInterpolatedStringHandler.AppendLiteral("\n");
				defaultInterpolatedStringHandler.AppendLiteral("    local: ");
				defaultInterpolatedStringHandler.AppendFormatted(translationText);
				defaultInterpolatedStringHandler.AppendLiteral("\n\n");
				defaultInterpolatedStringHandler.AppendLiteral("           ");
				defaultInterpolatedStringHandler.AppendFormatted("".PadRight(GetDiffIndex(baseText, translationText), ' '));
				defaultInterpolatedStringHandler.AppendLiteral("^\n");
				return new TranslationValidatorResult(TranslationValidatorIssue.SyntaxMismatch, key, baseSyntax, baseText, translationSyntax, translationText, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!ValidateGenderSwitchBlocks(baseText, out var error, out var errorBlock))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Base text has invalid gender switch block: ");
				defaultInterpolatedStringHandler.AppendFormatted(error);
				defaultInterpolatedStringHandler.AppendLiteral(".\nAffected block: ");
				defaultInterpolatedStringHandler.AppendFormatted(errorBlock);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				return new TranslationValidatorResult(TranslationValidatorIssue.MalformedSyntax, key, baseSyntax, baseText, translationSyntax, translationText, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!ValidateGenderSwitchBlocks(baseText, out error, out errorBlock))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Translated text has invalid gender switch block: ");
				defaultInterpolatedStringHandler.AppendFormatted(error);
				defaultInterpolatedStringHandler.AppendLiteral(".\nAffected block: ");
				defaultInterpolatedStringHandler.AppendFormatted(errorBlock);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				return new TranslationValidatorResult(TranslationValidatorIssue.MalformedSyntax, key, baseSyntax, baseText, translationSyntax, translationText, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return null;
		}

		/// <summary>Validate that all gender-switch blocks in a given text are correctly formatted.</summary>
		/// <param name="text">The text which may contain gender-switch blocks to validate.</param>
		/// <param name="error">If applicable, a human-readable phrase indicating why the gender-switch blocks are invalid.</param>
		/// <param name="errorBlock">The gender-switch block which is invalid.</param>
		public bool ValidateGenderSwitchBlocks(string text, out string error, out string errorBlock)
		{
			int minIndex = 0;
			while (true)
			{
				int start = text.IndexOf("${", minIndex, StringComparison.OrdinalIgnoreCase);
				if (start == -1)
				{
					break;
				}
				int end = text.IndexOf("}$", start, StringComparison.OrdinalIgnoreCase);
				if (end == -1)
				{
					error = "closing '}$' not found";
					errorBlock = text.Substring(start);
					return false;
				}
				errorBlock = text.Substring(start, end - start);
				string text2 = text.Substring(start + 2, end - start - 2);
				char splitCharacter = (text2.Contains('^') ? '^' : '¦');
				string[] branches = text2.Split(splitCharacter);
				if (text2.Contains("${"))
				{
					error = "can't start a new gender-switch block inside another";
					return false;
				}
				if (branches.Length < 2)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
					defaultInterpolatedStringHandler.AppendLiteral("must have at least two branches delimited by ");
					defaultInterpolatedStringHandler.AppendFormatted('^');
					defaultInterpolatedStringHandler.AppendLiteral(" or ");
					defaultInterpolatedStringHandler.AppendFormatted('¦');
					error = defaultInterpolatedStringHandler.ToStringAndClear();
					return false;
				}
				if (branches.Length > 3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(82, 5);
					defaultInterpolatedStringHandler.AppendLiteral("found ");
					defaultInterpolatedStringHandler.AppendFormatted(branches.Length);
					defaultInterpolatedStringHandler.AppendLiteral(" branches delimited by ");
					defaultInterpolatedStringHandler.AppendFormatted(splitCharacter);
					defaultInterpolatedStringHandler.AppendLiteral(", must be two (male");
					defaultInterpolatedStringHandler.AppendFormatted(splitCharacter);
					defaultInterpolatedStringHandler.AppendLiteral("female) or three (male");
					defaultInterpolatedStringHandler.AppendFormatted(splitCharacter);
					defaultInterpolatedStringHandler.AppendLiteral("female");
					defaultInterpolatedStringHandler.AppendFormatted(splitCharacter);
					defaultInterpolatedStringHandler.AppendLiteral("other)");
					error = defaultInterpolatedStringHandler.ToStringAndClear();
					return false;
				}
				string firstSyntax = Abstractor.ExtractDialogueSyntax(branches[0]);
				for (int i = 1; i < branches.Length; i++)
				{
					string curSyntax = Abstractor.ExtractDialogueSyntax(branches[1]);
					if (firstSyntax != curSyntax)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
						defaultInterpolatedStringHandler.AppendLiteral("branches have different syntax (0: `");
						defaultInterpolatedStringHandler.AppendFormatted(firstSyntax);
						defaultInterpolatedStringHandler.AppendLiteral("`, ");
						defaultInterpolatedStringHandler.AppendFormatted(i);
						defaultInterpolatedStringHandler.AppendLiteral(": `");
						defaultInterpolatedStringHandler.AppendFormatted(curSyntax);
						defaultInterpolatedStringHandler.AppendLiteral("`)");
						error = defaultInterpolatedStringHandler.ToStringAndClear();
						return false;
					}
				}
				minIndex = end + 2;
			}
			error = null;
			errorBlock = null;
			return true;
		}

		/// <summary>Get the index at which two strings first differ.</summary>
		/// <param name="baseText">The base text being compare to.</param>
		/// <param name="translatedText">The translated text to compare with the base text.</param>
		public int GetDiffIndex(string baseText, string translatedText)
		{
			int minLength = Math.Min(baseText.Length, translatedText.Length);
			for (int i = 0; i < minLength; i++)
			{
				if (baseText[i] != translatedText[i])
				{
					return i;
				}
			}
			return minLength;
		}
	}
}
