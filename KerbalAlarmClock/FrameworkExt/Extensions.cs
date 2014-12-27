using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KSPPluginFramework
{
    public static class EnumExtensions
    {
        public static String Description(this Enum e)
        {
            DescriptionAttribute[] desc = (DescriptionAttribute[])e.GetType().GetMember(e.ToString())[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (desc.Length > 0)
                return desc[0].Description;
            else
                return e.ToString();
        }

        //public static List<KeyValuePair<TEnum, string>> ToEnumDescriptionsList<TEnum>(TEnum value) 
        //{
        //    return Enum
        //        .GetValues(typeof(TEnum))
        //        .Cast<TEnum>()
        //        .Select(x => new KeyValuePair<TEnum, string>(x, ((Enum)((object)x)).Description()))
        //        .ToList();
        //}
        //public static List<KeyValuePair<TEnum, string>> ToEnumDescriptionsList<TEnum>()
        //{
        //    return ToEnumDescriptionsList<TEnum>(default(TEnum));
        //}

        //limit it to accept enums only
        public static List<String> ToEnumDescriptions<TEnum>(TEnum value) where TEnum : struct,IConvertible
        {
            List<KeyValuePair<TEnum, string>> temp = Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(x => new KeyValuePair<TEnum, string>(x, ((Enum)((System.Object)x)).Description()))
                .ToList();
            return temp.Select(x => x.Value).ToList<String>();
        }
        public static List<String> ToEnumDescriptions<TEnum>() where TEnum : struct,IConvertible
        {
            return ToEnumDescriptions<TEnum>(default(TEnum)).ToList<String>();
        }



        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static Int32 ToInt32(this String s)
        {
            return Convert.ToInt32(s);
        }

        public static Int32 NormalizeAngle360(this Int32 val) {
            return (Int32)Convert.ToDouble(val).NormalizeAngle360();
        }

        public static Double NormalizeAngle360(this Double val) 
        {
            val %= 360;
            if (val < 0)
                val += 360;
            return val;
        }
    }
}
