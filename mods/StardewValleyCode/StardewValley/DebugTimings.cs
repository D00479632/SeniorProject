using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
	public class DebugTimings
	{
		private static readonly Vector2 DrawPos = Vector2.One * 12f;

		private readonly Stopwatch StopwatchDraw = new Stopwatch();

		private readonly Stopwatch StopwatchUpdate = new Stopwatch();

		private double LastTimingDraw;

		private double LastTimingUpdate;

		private float DrawTextWidth = -1f;

		private bool Active;

		public bool Toggle()
		{
			if ((!(Game1.game1?.IsMainInstance)) ?? true)
			{
				return false;
			}
			Active = !Active;
			return Active;
		}

		public void StartDrawTimer()
		{
			if (Active && (Game1.game1?.IsMainInstance ?? false))
			{
				StopwatchDraw.Restart();
			}
		}

		public void StopDrawTimer()
		{
			if (Active && (Game1.game1?.IsMainInstance ?? false))
			{
				StopwatchDraw.Stop();
				LastTimingDraw = StopwatchDraw.Elapsed.TotalMilliseconds;
			}
		}

		public void StartUpdateTimer()
		{
			if (Active && (Game1.game1?.IsMainInstance ?? false))
			{
				StopwatchUpdate.Restart();
			}
		}

		public void StopUpdateTimer()
		{
			if (Active && (Game1.game1?.IsMainInstance ?? false))
			{
				StopwatchUpdate.Stop();
				LastTimingUpdate = StopwatchUpdate.Elapsed.TotalMilliseconds;
			}
		}

		public void Draw()
		{
			if (!Active)
			{
				return;
			}
			bool? flag = Game1.game1?.IsMainInstance;
			if (flag.HasValue && flag.GetValueOrDefault() && Game1.spriteBatch != null && Game1.dialogueFont != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				if (DrawTextWidth <= 0f)
				{
					SpriteFont dialogueFont = Game1.dialogueFont;
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Draw time: ");
					defaultInterpolatedStringHandler.AppendFormatted(0, "00.00");
					defaultInterpolatedStringHandler.AppendLiteral(" ms  ");
					DrawTextWidth = dialogueFont.MeasureString(defaultInterpolatedStringHandler.ToStringAndClear()).X;
				}
				Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, 64), Color.Black * 0.5f);
				SpriteBatch spriteBatch = Game1.spriteBatch;
				SpriteFont dialogueFont2 = Game1.dialogueFont;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Draw time: ");
				defaultInterpolatedStringHandler.AppendFormatted(LastTimingDraw, "00.00");
				defaultInterpolatedStringHandler.AppendLiteral(" ms  ");
				spriteBatch.DrawString(dialogueFont2, defaultInterpolatedStringHandler.ToStringAndClear(), DrawPos, Color.White);
				SpriteBatch spriteBatch2 = Game1.spriteBatch;
				SpriteFont dialogueFont3 = Game1.dialogueFont;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Update time: ");
				defaultInterpolatedStringHandler.AppendFormatted(LastTimingUpdate, "00.00");
				defaultInterpolatedStringHandler.AppendLiteral(" ms");
				spriteBatch2.DrawString(dialogueFont3, defaultInterpolatedStringHandler.ToStringAndClear(), new Vector2(DrawPos.X + DrawTextWidth, DrawPos.Y), Color.White);
			}
		}
	}
}
