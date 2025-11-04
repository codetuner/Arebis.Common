using System;
using System.Linq;
using System.Text;

namespace Arebis.Text
{
    ///<summary>
    /// ASCII transliterations of Unicode text
    /// </summary>
    public static partial class Unidecoder
    {
        /// <summary>
        /// Transliterate Unicode string to ASCII string.
        /// </summary>
        /// <param name="input">String you want to transliterate into ASCII</param>
        /// <param name="level">Level of transliteration.</param>
        /// <returns>
        /// Transliterated string.
        /// </returns>
        public static string Unidecode(this string input, UnidecoderLevel level = UnidecoderLevel.Ascii)
        {
            if (level == UnidecoderLevel.Off) return input;
            else if (string.IsNullOrEmpty(input)) return input;
            else if (input.All(x => x < 0x80)) return input;

            // Unidecode result often can be at least two times longer than input string.
            var sb = new StringBuilder(input.Length * 2);

            var mode = 0;
            foreach (char c in input)
            {
                //Console.WriteLine(c);
                //Console.WriteLine((long)c);

                if (mode == 0)
                {
                    if (c < 0x80)/*128*/
                    {
                        sb.Append(c);
                    }
                    else if (c < 161 && level == UnidecoderLevel.Ascii) sb.Append("");
                    else if (c < 160) sb.Append(unkn);
                    else if (c == 160) sb.Append(" ");
                    else if (c < 256) sb.Append(c);
                    else if (c == 306 && level == UnidecoderLevel.AnsiPlus) sb.Append("IJ");
                    else if (c == 307 && level == UnidecoderLevel.AnsiPlus) sb.Append("ij");
                    else if (c == 312 && level == UnidecoderLevel.AnsiPlus) sb.Append("k");
                    else if (c == 319 && level == UnidecoderLevel.AnsiPlus) sb.Append("L");
                    else if (c == 320 && level == UnidecoderLevel.AnsiPlus) sb.Append("l");
                    else if (c == 329 && level == UnidecoderLevel.AnsiPlus) sb.Append("'n");
                    else if (c == 330 && level == UnidecoderLevel.AnsiPlus) sb.Append("ng");
                    else if (c == 331 && level == UnidecoderLevel.AnsiPlus) sb.Append("NG");
                    else if (c == 383 && level == UnidecoderLevel.AnsiPlus) sb.Append("s");
                    else if (c < 384 && level == UnidecoderLevel.AnsiPlus) sb.Append(c);
                    else if (c == 393 && level == UnidecoderLevel.AnsiPlus) sb.Append(c);
                    else if (c == 55349) mode = 1; // high surrogate
                    else
                    {
                        int high = c >> 8;
                        int low = c & 0xff;
                        string[] transliterations;
                        string result;
                        if (CharacterMap.TryGetValue(high, out transliterations))
                        {
                            result = transliterations[low];
                        }
                        else
                        {
                            result = "";
                        }
                        sb.Append(result);
                    }
                }
                else if (mode == 1)
                {
                    if (c >= 56320 && c <= 57343) // low surrogate
                    {
                        int highSurrogate = 55349;
                        int lowSurrogate = c;
                        int codepoint = ((highSurrogate - 0xD800) << 10) + (lowSurrogate - 0xDC00) + 0x10000;
                        // Now transliterate codepoint:
                        int high = codepoint >> 8;
                        int low = codepoint & 0xff;
                        string[] transliterations;
                        string result;
                        if (CharacterMap.TryGetValue(high, out transliterations))
                        {
                            result = transliterations[low];
                        }
                        else
                        {
                            result = "";
                        }
                        sb.Append(result);
                    }
                    else
                    {
                        // Invalid surrogate pair, just transliterate the previous high surrogate as is:
                        sb.Append(unkn);
                        // And process this character again:
                        if (c < 0x80)/*128*/
                        {
                            sb.Append(c);
                        }
                        else if (c < 161 && level == UnidecoderLevel.Ascii) sb.Append("");
                        else if (c < 160) sb.Append(unkn);
                        else if (c == 160) sb.Append(" ");
                        else if (c < 256) sb.Append(c);
                        else
                        {
                            int high = c >> 8;
                            int low = c & 0xff;
                            string[] transliterations;
                            string result;
                            if (CharacterMap.TryGetValue(high, out transliterations))
                            {
                                result = transliterations[low];
                            }
                            else
                            {
                                result = "";
                            }
                            sb.Append(result);
                        }
                    }
                    mode = 0;
                }
            }

            return sb.ToString();
        }
    }
}
