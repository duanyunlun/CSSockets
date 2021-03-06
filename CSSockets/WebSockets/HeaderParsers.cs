﻿using System.Collections.Generic;
using System.Collections.Specialized;

namespace CSSockets.WebSockets
{
    public class ExtensionNegotiation
    {
        public StringDictionary Parameters { get; }
        public string ExtensionName { get; set; }

        public static bool TryParse(string s, out ExtensionNegotiation[] result)
        {
            result = null;
            if (s == "") return true;
            List<ExtensionNegotiation> parsed = new List<ExtensionNegotiation>();
            string[] spl = s.Split(",");
            for (int i = 0; i < spl.Length; i++)
            {
                string ext = spl[i].Trim();
                if (ext.Length == 0) return false;
                string[] spl2 = ext.Split(";");
                ExtensionNegotiation current = new ExtensionNegotiation();
                if ((current.ExtensionName = spl2[0].Trim()).Length == 0)
                    return false;
                for (int j = 1; j < spl2.Length; j++)
                {
                    string[] spl3 = spl2[j].Split("=");
                    if (spl3.Length > 2) return false;
                    current.Parameters.Add(spl3[0].Trim(), spl3.Length == 2 ? spl3[1].Trim() : "");
                }
                parsed.Add(current);
            }
            result = parsed.ToArray();
            return true;
        }

        public static string Stringify(IEnumerable<ExtensionNegotiation> extensions)
        {
            string s = "";
            foreach (ExtensionNegotiation ext in extensions) s += ext + ", ";
            return s.Length == 0 ? s : s.Substring(0, s.Length - 2);
        }

        public ExtensionNegotiation()
        {
            Parameters = new StringDictionary();
            ExtensionName = null;
        }

        public override string ToString()
        {
            string s = ExtensionName;
            if (Parameters.Count == 0) return s;
            s += "; ";
            string[] keys = new string[Parameters.Count];
            Parameters.Keys.CopyTo(keys, 0);
            for (int i = 0; i < Parameters.Count; i++)
            {
                string param = Parameters[keys[i]];
                s += keys[i] + (param == "" ? "" : "=" + param) + (i == Parameters.Count - 1 ? "" : "; ");
            }
            return s;
        }
    }
    public static class SubprotocolNegotiation
    {
        public static bool TryParseHeader(string s, out string[] result)
        {
            result = null;
            string[] spl = s.Split(',');
            for (int i = 0; i < spl.Length; i++)
                if (spl[i].Length == 0) return false; else spl[i] = spl[i].Trim();
            result = spl;
            return true;
        }

        public static string Stringify(IEnumerable<string> subprotocols)
        {
            string s = "";
            foreach (string subp in subprotocols) s += subp + ", ";
            return s.Length == 0 ? s : s.Substring(0, s.Length - 2);
        }
    }
}
