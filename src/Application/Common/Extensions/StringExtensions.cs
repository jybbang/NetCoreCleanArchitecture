// 
// Copyright (c) Rafael Garcia <https://github.com/rafaelfgx>
// 
// All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Serialize<T>(this T value) where T : class
        {
            if (value is null) return "{}";

            return JsonSerializer.Serialize<T>(value, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            });
        }

        public static T Deserialize<T>(this string value) where T : class
        {
            if (string.IsNullOrWhiteSpace(value)) return default;

            return JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNameCaseInsensitive = true
            });
        }

        public static string LowerCamelCase(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value[0].ToString().ToLowerInvariant() + value[1..];
        }

        public static string NonSpecialCharacters(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var bytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(value);

            value = Encoding.Default.GetString(bytes);

            return new Regex("[^0-9a-zA-Z._ ]+?").Replace(value, string.Empty);
        }

        public static string NumericCharacters(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            return Regex.Replace(value, "[^0-9]", string.Empty);
        }

        public static string RemoveExtraSpaces(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : Regex.Replace(value, @"\s+", " ");
        }

        public static string IntergerCharacters(this string value)
        {
            if (value is null) return "0";

            var cnt = value.Count();

            var isStarted = false;

            var sb = new StringBuilder(cnt);

            foreach (var c in value)
            {
                if (char.IsControl(c)) continue;
                switch (c)
                {
                    case ' ':
                        break;
                    case '.':
                        goto stop;
                    case '-':
                        if (!isStarted) sb.Append(c);
                        else goto stop;
                        break;
                    case '0':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '1':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '2':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '3':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '4':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '5':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '6':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '7':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '8':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '9':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    default:
                        {
                            if (isStarted) goto stop;
                        }
                        break;
                }
            }
        stop:
            return sb.ToString();
        }

        public static string FloatCharacters(this string value)
        {
            if (value is null) return "0";

            var cnt = value.Count();

            var hasDot = false;

            var isStarted = false;

            var sb = new StringBuilder(cnt);

            foreach (var c in value)
            {
                if (char.IsControl(c)) continue;
                switch (c)
                {
                    case ' ':
                        break;
                    case '.':
                        if (!hasDot)
                        {
                            sb.Append(c);
                            hasDot = true;
                        }
                        else goto stop;
                        break;
                    case '-':
                        if (!isStarted) sb.Append(c);
                        else goto stop;
                        break;
                    case '0':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '1':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '2':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '3':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '4':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '5':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '6':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '7':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '8':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    case '9':
                        {
                            sb.Append(c);
                            isStarted = true;
                        }
                        break;
                    default:
                        {
                            if (isStarted) goto stop;
                        }
                        break;
                }
            }
        stop:
            return sb.ToString();
        }

        public static int ToInt32(this string value)
        {
            var val = IntergerCharacters(value);

            return string.IsNullOrWhiteSpace(val) ? 0 : Convert.ToInt32(val);
        }

        public static double ToDouble(this string value)
        {
            var val = FloatCharacters(value);

            return string.IsNullOrWhiteSpace(val) ? 0 : Convert.ToDouble(val);
        }
    }
}
