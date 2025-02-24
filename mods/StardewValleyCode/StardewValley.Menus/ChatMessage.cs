using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus
{
	public class ChatMessage
	{
		public List<ChatSnippet> message = new List<ChatSnippet>();

		public int timeLeftToDisplay;

		public int verticalSize;

		public float alpha = 1f;

		public Color color;

		public LocalizedContentManager.LanguageCode language;

		public void parseMessageForEmoji(string messagePlaintext)
		{
			if (messagePlaintext == null)
			{
				return;
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < messagePlaintext.Length; i++)
			{
				int tag_close_index;
				if (messagePlaintext[i] == '[')
				{
					if (sb.Length > 0)
					{
						breakNewLines(sb);
					}
					sb.Clear();
					tag_close_index = messagePlaintext.IndexOf(']', i);
					int next_open_index = -1;
					if (i + 1 < messagePlaintext.Length)
					{
						next_open_index = messagePlaintext.IndexOf('[', i + 1);
					}
					if (tag_close_index != -1 && (next_open_index == -1 || next_open_index > tag_close_index))
					{
						string sub = messagePlaintext.Substring(i + 1, tag_close_index - i - 1);
						if (int.TryParse(sub, out var emojiIndex))
						{
							if (emojiIndex < EmojiMenu.totalEmojis)
							{
								message.Add(new ChatSnippet(emojiIndex));
							}
						}
						else
						{
							if (sub != null)
							{
								switch (sub.Length)
								{
								case 4:
								{
									char c = sub[0];
									if ((uint)c <= 98u)
									{
										if (c != 'a')
										{
											if (c != 'b' || !(sub == "blue"))
											{
												break;
											}
										}
										else if (!(sub == "aqua"))
										{
											break;
										}
									}
									else if (c != 'g')
									{
										if (c != 'j')
										{
											if (c != 'p' || (!(sub == "pink") && !(sub == "plum")))
											{
												break;
											}
										}
										else if (!(sub == "jade"))
										{
											break;
										}
									}
									else if (!(sub == "gray"))
									{
										break;
									}
									goto IL_02f3;
								}
								case 6:
									switch (sub[0])
									{
									case 'j':
										break;
									case 'y':
										goto IL_025c;
									case 'o':
										goto IL_0272;
									case 'p':
										goto IL_0285;
									case 's':
										goto IL_0295;
									default:
										goto end_IL_00bf;
									}
									if (!(sub == "jungle"))
									{
										break;
									}
									goto IL_02f3;
								case 5:
								{
									char c = sub[0];
									if ((uint)c <= 99u)
									{
										if (c != 'b')
										{
											if (c != 'c' || !(sub == "cream"))
											{
												break;
											}
										}
										else if (!(sub == "brown"))
										{
											break;
										}
									}
									else if (c != 'g')
									{
										if (c != 'p' || !(sub == "peach"))
										{
											break;
										}
									}
									else if (!(sub == "green"))
									{
										break;
									}
									goto IL_02f3;
								}
								case 3:
									if (!(sub == "red"))
									{
										break;
									}
									goto IL_02f3;
								case 11:
									{
										if (!(sub == "yellowgreen"))
										{
											break;
										}
										goto IL_02f3;
									}
									IL_02f3:
									if (color.Equals(Color.White))
									{
										color = getColorFromName(sub);
									}
									goto IL_0335;
									IL_0295:
									if (!(sub == "salmon"))
									{
										break;
									}
									goto IL_02f3;
									IL_0272:
									if (!(sub == "orange"))
									{
										break;
									}
									goto IL_02f3;
									IL_0285:
									if (!(sub == "purple"))
									{
										break;
									}
									goto IL_02f3;
									IL_025c:
									if (!(sub == "yellow"))
									{
										break;
									}
									goto IL_02f3;
									end_IL_00bf:
									break;
								}
							}
							sb.Append("[");
							sb.Append(sub);
							sb.Append("]");
						}
						goto IL_0335;
					}
					sb.Append("[");
					continue;
				}
				sb.Append(messagePlaintext[i]);
				continue;
				IL_0335:
				i = tag_close_index;
			}
			if (sb.Length > 0)
			{
				breakNewLines(sb);
			}
		}

		public static Color getColorFromName(string name)
		{
			if (name != null)
			{
				switch (name.Length)
				{
				case 4:
					switch (name[0])
					{
					case 'a':
						if (!(name == "aqua"))
						{
							break;
						}
						return Color.MediumTurquoise;
					case 'b':
						if (!(name == "blue"))
						{
							break;
						}
						return Color.DodgerBlue;
					case 'j':
						if (!(name == "jade"))
						{
							break;
						}
						return new Color(50, 230, 150);
					case 'p':
						if (!(name == "pink"))
						{
							if (!(name == "plum"))
							{
								break;
							}
							return new Color(190, 0, 190);
						}
						return Color.HotPink;
					case 'g':
						if (!(name == "gray"))
						{
							break;
						}
						return Color.Gray;
					}
					break;
				case 6:
					switch (name[0])
					{
					case 'j':
						if (!(name == "jungle"))
						{
							break;
						}
						return Color.SeaGreen;
					case 'y':
						if (!(name == "yellow"))
						{
							break;
						}
						return new Color(240, 200, 0);
					case 'o':
						if (!(name == "orange"))
						{
							break;
						}
						return new Color(255, 100, 0);
					case 'p':
						if (!(name == "purple"))
						{
							break;
						}
						return new Color(138, 43, 250);
					case 's':
						if (!(name == "salmon"))
						{
							break;
						}
						return Color.Salmon;
					}
					break;
				case 5:
					switch (name[0])
					{
					case 'g':
						if (!(name == "green"))
						{
							break;
						}
						return new Color(0, 180, 10);
					case 'c':
						if (!(name == "cream"))
						{
							break;
						}
						return new Color(255, 255, 180);
					case 'p':
						if (!(name == "peach"))
						{
							break;
						}
						return new Color(255, 180, 120);
					case 'b':
						if (!(name == "brown"))
						{
							break;
						}
						return new Color(160, 80, 30);
					}
					break;
				case 3:
					if (!(name == "red"))
					{
						break;
					}
					return new Color(220, 20, 20);
				case 11:
					if (!(name == "yellowgreen"))
					{
						break;
					}
					return new Color(182, 214, 0);
				}
			}
			return Color.White;
		}

		private void breakNewLines(StringBuilder sb)
		{
			string[] split = sb.ToString().Split(Environment.NewLine);
			for (int i = 0; i < split.Length; i++)
			{
				message.Add(new ChatSnippet(split[i], language));
				if (i != split.Length - 1)
				{
					message.Add(new ChatSnippet(Environment.NewLine, language));
				}
			}
		}

		public static string makeMessagePlaintext(List<ChatSnippet> message, bool include_color_information)
		{
			StringBuilder sb = new StringBuilder();
			foreach (ChatSnippet cs in message)
			{
				if (cs.message != null)
				{
					sb.Append(cs.message);
				}
				else if (cs.emojiIndex != -1)
				{
					sb.Append("[" + cs.emojiIndex + "]");
				}
			}
			if (include_color_information && Game1.player.defaultChatColor != null && !getColorFromName(Game1.player.defaultChatColor).Equals(Color.White))
			{
				sb.Append(" [");
				sb.Append(Game1.player.defaultChatColor);
				sb.Append("]");
			}
			return sb.ToString();
		}

		public void draw(SpriteBatch b, int x, int y)
		{
			float xPositionSoFar = 0f;
			float yPositionSoFar = 0f;
			for (int i = 0; i < message.Count; i++)
			{
				if (message[i].emojiIndex != -1)
				{
					b.Draw(ChatBox.emojiTexture, new Vector2((float)x + xPositionSoFar + 1f, (float)y + yPositionSoFar - 4f), new Rectangle(message[i].emojiIndex * 9 % ChatBox.emojiTexture.Width, message[i].emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
				}
				else if (message[i].message != null)
				{
					if (message[i].message.Equals(Environment.NewLine))
					{
						xPositionSoFar = 0f;
						yPositionSoFar += ChatBox.messageFont(language).MeasureString("(").Y;
					}
					else
					{
						b.DrawString(ChatBox.messageFont(language), message[i].message, new Vector2((float)x + xPositionSoFar, (float)y + yPositionSoFar), color * alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
					}
				}
				xPositionSoFar += message[i].myLength;
				if (xPositionSoFar >= 888f)
				{
					xPositionSoFar = 0f;
					yPositionSoFar += ChatBox.messageFont(language).MeasureString("(").Y;
					if (message.Count > i + 1 && message[i + 1].message != null && message[i + 1].message.Equals(Environment.NewLine))
					{
						i++;
					}
				}
			}
		}
	}
}
