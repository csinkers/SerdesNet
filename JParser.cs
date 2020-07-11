using System;
using System.Collections.Generic;
using System.Text;

namespace SerdesNet
{
    public class JToken { }
    public class JObject : JToken { public Dictionary<string, JToken> Values { get; } = new Dictionary<string, JToken>(); }
    public class JArray : JToken { public List<JToken> Values { get; } = new List<JToken>(); }
    public class JString : JToken { public JString(string value) => Value = value; public string Value { get; } }
    public class JNumber : JToken { public JNumber(string value) => Value = double.Parse(value); public double Value { get; } }
    public class JTrue : JToken { public bool Value => true; }
    public class JFalse : JToken { public bool Value => false; }
    public class JNull : JToken { public object Value => null; }
    public class JTerminus : JToken { }

    public static class JParser
    {
        public static IEnumerable<(char, string, int)> Tokenise(string s)
        {
            char state = ' ';
            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                switch (state)
                {
                    case ' ':
                        switch (c)
                        {
                            case ' ': break;
                            case '\n': break;
                            case '\r': break;
                            case '\t': break;
                            case ':': yield return (':', null, i); break;
                            case '[': yield return ('[', null, i); break;
                            case ']': yield return (']', null, i); break;
                            case '{': yield return ('{', null, i); break;
                            case '}': yield return ('}', null, i); break;
                            case ',': yield return (',', null, i); break;
                            case '"': state = '"'; break;
                            case 'n': state = 'n'; break;
                            case 't': state = 't'; break;
                            case 'f': state = 'f'; break;
                            case '/': state = '/'; break;
                            case '0': case '1': case '2':
                            case '3': case '4': case '5':
                            case '6': case '7': case '8':
                            case '9': case '-': 
                                state = '#';
                                sb.Append(c);
                                break;
                        }
                        break;
                    case '"':
                        switch (c)
                        {
                            case '\\':
                                state = '\\';
                                break;
                            case '"':
                                yield return ('"', sb.ToString(), i);
                                sb.Clear();
                                state = ' ';
                                break;
                            default:
                                sb.Append(c);
                                break;
                        }

                        break;
                    case '/':
                        if (c == '/')
                            state = '*';
                        else
                        {
                            yield return ('!', "Unexpected /", i);
                            state = ' ';
                        }
                        break;
                    case '*':
                        if(c == '\n' || c == '\r')
                            state = ' ';
                        break;
                    case '\\':
                        switch (c)
                        {
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'u': yield return ('!', "Unicode unsupported", i); break;
                        }

                        state = '"';
                        break;
                    case 'n':
                    {
                        sb.Append(c);
                        var t = sb.ToString();
                        if (!"ull".StartsWith(t))
                        {
                            yield return ('!', t, i);
                            state = ' ';
                            sb.Clear();
                        }
                        else if ("ull" == t)
                        {
                            yield return ('0', null, i);
                            state = ' ';
                            sb.Clear();
                        }

                        break;
                    }

                    case 't':
                    {
                        sb.Append(c);
                        var t = sb.ToString();
                        if (!"rue".StartsWith(t))
                        {
                            yield return ('!', t, i);
                            state = ' ';
                            sb.Clear();
                        }
                        else if ("rue" == t)
                        {
                            yield return ('t', null, i);
                            state = ' ';
                            sb.Clear();
                        }

                        break;
                    }

                    case 'f':
                    {
                        sb.Append(c);
                        var t = sb.ToString();
                        if (!"alse".StartsWith(t))
                        {
                            yield return ('!', t, i);
                            state = ' ';
                            sb.Clear();
                        }
                        else if ("alse" == t)
                        {
                            yield return ('f', null, i);
                            state = ' ';
                            sb.Clear();
                        }

                        break;
                    }

                    case '#':
                        switch (c)
                        {
                            case '0': case '1': case '2':
                            case '3': case '4': case '5':
                            case '6': case '7': case '8':
                            case '9': case '-': case '+':
                            case 'e': case 'E':
                                sb.Append(c);
                                break;
                            default:
                                yield return ('#', sb.ToString(), i);
                                state = ' ';
                                sb.Clear();
                                i--;
                                break;
                        }

                        break;
                    default: throw new InvalidOperationException($"Invalid tokeniser state '{state}'");
                }
            }

            if (state == '#')
                yield return ('#', sb.ToString(), s.Length);
        }

        public static IEnumerable<JToken> Parse(string json)
        {
            using (var tokens = Tokenise(json).GetEnumerator())
            {
                while (tokens.MoveNext())
                {
                    var t = Inner(tokens);
                    if (t is JTerminus)
                        yield break;
                    yield return t;
                }
            }
        }

        static JToken Inner(IEnumerator<(char, string, int)> tokens)
        {
            var (t, targ, pos) = tokens.Current;
            switch (t)
            {
                case '[': return InnerArray(tokens, new JArray());
                case '{': return InnerObject(tokens, new JObject());
                case '0': return new JNull();
                case 't': return new JTrue();
                case 'f': return new JFalse();
                case '#': return new JNumber(targ);
                case '"': return new JString(targ);
                default: throw new InvalidOperationException($"Invalid token ({t}, {targ}) at {pos}");
            }
        }

        static string ConsumeLabel(IEnumerator<(char, string, int)> tokens)
        {
            var (t, targ, pos) = tokens.Current;
            string label;
            if (t == '#' || t == '"')
                label = targ;
            else throw new InvalidOperationException($"Unexpected token ({t}, {targ}) when consuming label at {pos}");

            if (!tokens.MoveNext())
                throw new InvalidOperationException("Unexpected EOF when reading label at {pos}");

            (t, _, pos) = tokens.Current;
            if (t != ':')
                throw new InvalidOperationException($"Expected : but found ({t}, {targ}) at {pos}");

            if (!tokens.MoveNext())
                throw new InvalidOperationException("Unexpected EOF when reading label at {pos}");

            return label;
        }

        static void ConsumeComma(IEnumerator<(char, string, int)> tokens)
        {
            var (t, targ, pos) = tokens.Current;
            if (t != ',')
                throw new InvalidOperationException($"Expected , but found ({t}, {targ}) at {pos}");

            if (!tokens.MoveNext())
                throw new InvalidOperationException("Unexpected EOF when reading comma at {pos}");
        }

        static JToken InnerObject(IEnumerator<(char, string, int)> tokens, JObject a)
        {
            bool first = true;
            while (tokens.MoveNext())
            {
                var (t, _, _) = tokens.Current;
                if (t == '}')
                    return a;

                if (!first)
                    ConsumeComma(tokens);
                first = false;

                var label = ConsumeLabel(tokens);
                a.Values[label] = Inner(tokens);
            }
            return a;
        }

        static JToken InnerArray(IEnumerator<(char, string, int)> tokens, JArray a)
        {
            bool first = true;
            while (tokens.MoveNext())
            {
                var (t, _, _) = tokens.Current;
                if (t == ']')
                    return a;

                if (!first)
                    ConsumeComma(tokens);
                first = false;

                a.Values.Add(Inner(tokens));
            }
            return a;
        }
    }
}