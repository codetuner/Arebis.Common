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

            if (level == UnidecoderLevel.Ascii)
            {
                foreach (char c in input)
                {
                    if (c < 0x80)/*128*/
                    {
                        sb.Append(c);
                    }
                    else if (c < 161) sb.Append("");
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
            }
            else if (level == UnidecoderLevel.Ansi)
            {
                foreach (char c in input)
                {
                    if (c < 0x80)/*128*/
                    {
                        sb.Append(c);
                    }
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
            }
            else if (level == UnidecoderLevel.AnsiPlus)
            {
                foreach (char c in input)
                {
                    if (c < 0x80)/*128*/
                    {
                        sb.Append(c);
                    }
                    else if (c < 160) sb.Append(unkn);
                    else if (c == 160) sb.Append(" ");
                    else if (c < 256) sb.Append(c);
                    else if (c == 306) sb.Append("IJ");
                    else if (c == 307) sb.Append("ij");
                    else if (c == 312) sb.Append("k");
                    else if (c == 319) sb.Append("L");
                    else if (c == 320) sb.Append("l");
                    else if (c == 329) sb.Append("'n");
                    else if (c == 330) sb.Append("ng");
                    else if (c == 331) sb.Append("NG");
                    else if (c == 383) sb.Append("s");
                    else if (c < 384) sb.Append(c);
                    else if (c == 393) sb.Append(c);
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
            }
            else
            {
                throw new ArgumentException("Unsupported level argument value.", "level");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Transliterate Unicode character to ASCII string.
        /// </summary>
        /// <param name="c">Character you want to transliterate into ASCII</param>
        /// <param name="level">Level of transliteration.</param>
        /// <returns>
        /// Transliterated string.
        /// </returns>
        public static string Unidecode(this char c, UnidecoderLevel level = UnidecoderLevel.Ascii)
        {
            if (level == UnidecoderLevel.Off) return new String(c, 1);

            if (c < 0x80)/*128*/
            {
                return new string(c, 1);
            }
            else if (c < 161)
            {
                return String.Empty;
            }
            else if (c < 256)
            {
                switch (level)
                {
                    case UnidecoderLevel.Ansi:
                        return new string(c, 1);
                    case UnidecoderLevel.AnsiPlus:
                        return new string(c, 1);
                }
            }
            else if (level == UnidecoderLevel.AnsiPlus && c == 306) return "IJ";
            else if (level == UnidecoderLevel.AnsiPlus && c == 307) return "ij";
            else if (level == UnidecoderLevel.AnsiPlus && c == 312) return "k";
            else if (level == UnidecoderLevel.AnsiPlus && c == 319) return "L";
            else if (level == UnidecoderLevel.AnsiPlus && c == 320) return "l";
            else if (level == UnidecoderLevel.AnsiPlus && c == 329) return "'n";
            else if (level == UnidecoderLevel.AnsiPlus && c == 330) return "ng";
            else if (level == UnidecoderLevel.AnsiPlus && c == 331) return "NG";
            else if (level == UnidecoderLevel.AnsiPlus && c == 383) return "s";
            else if (level == UnidecoderLevel.AnsiPlus && c < 384) return new string(c, 1);
            else if (level == UnidecoderLevel.AnsiPlus && c == 393) return new string(c, 1);

            // By default:
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
                return result;
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ConsoleApplication
//{

//    public static class MyUnidecoder
//    {
//        public static string Unidecode(char c, MyUnidecoderLevel level = MyUnidecoderLevel.Full)
//        {
//            if (c < 127)
//                return new String(c, 1);
//            else if (c < 161)
//                return String.Empty;
//            else if (c < 256)
//            {
//                if (level == MyUnidecoderLevel.Full)
//                    return "";
//            }


//            throw new NotImplementedException();
//        }

//    }
//}
