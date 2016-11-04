using System;
using System.Collections.Generic;
using System.Text;

namespace SeeCool.GISFramework.Object
{
    public class EncodeStr
    {
        private static string _escape = "$";
        private static string _escapeCode = "$1";
        private static string _separator = ",";
        private static string _separatorCode = "$2";
        public static string Encode(string str)
        {
            string str1 = str.Replace(_escape, _escapeCode);
            return str1.Replace(_separator, _separatorCode);
        }

        public static string Decode(string str)
        {
            string str1 = str.Replace(_separatorCode, _separator);
            return str.Replace(_escapeCode, _escape);
        }
    }
}
