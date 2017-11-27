using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;

namespace KSPPluginFramework
{
    /// <summary>Represents a time interval.</summary> 
    public class KSPTimeSpan : IFormattable
    {
        //Shortcut to the Calendar Type
        private CalendarTypeEnum CalType { get { return KSPDateStructure.CalendarType; } }

        //Descriptors of Timespan - uses UT as the Root value


        /// <summary>Gets the years component  of the time interval represented by the current KSPPluginFramework.KSPTimeSpan structure</summary> 
        /// <returns>
        /// <para>Returns 0 if the <see cref="KSPDateStructure.CalendarType"/> == <see cref="CalendarTypeEnum.Earth"/></para>
        /// <para>otherwise</para>
        /// Returns the year component of this instance. The return value can be positive or negative.
        /// </returns>
        public Int32 Years
        {
            get
            {
                if (CalType != CalendarTypeEnum.Earth)
                {
                    return (Int32)(UT / KSPDateStructure.SecondsPerYear);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Gets the days component of the time interval represented by the current KSPPluginFramework.KSPTimeSpan structure.</summary> 
        /// <returns>
        /// <para>Returns Total Number of Days for the current component if the <see cref="KSPDateStructure.CalendarType"/> == <see cref="CalendarTypeEnum.Earth"/></para>
        /// <para>otherwise</para>
        /// The day component of the current KSPPluginFramework.KSPTimeSpan structure. The return value ranges between +/- <see cref="KSPDateStructure.DaysPerYear"/>
        /// </returns>
        public Int32 Days {
            get {
                if (CalType != CalendarTypeEnum.Earth) {
                    return (Int32)(UT / KSPDateStructure.SecondsPerDay % KSPDateStructure.DaysPerYear);
                } else {
                    return (Int32)(UT / KSPDateStructure.SecondsPerDay);
                } 
            }
        }

        /// <summary>Gets the hours component of the time interval represented by the current KSPPluginFramework.KSPTimeSpan structure.</summary> 
        /// <returns>The hour component of the current KSPPluginFramework.KSPTimeSpan structure. The return value ranges between +/- <see cref="KSPDateStructure.HoursPerDay"/></returns>
        public int Hours {
            get { return (Int32)(UT / KSPDateStructure.SecondsPerHour % KSPDateStructure.HoursPerDay); }
        }

        /// <summary>Gets the minutes component of the time interval represented by the current KSPPluginFramework.KSPTimeSpan structure.</summary> 
        /// <returns>
        /// The minute component of the current KSPPluginFramework.KSPTimeSpan structure. The return value ranges between +/- <see cref="KSPDateStructure.MinutesPerHour"/>
        /// </returns>
        public int Minutes {
            get { return (Int32)UT / KSPDateStructure.SecondsPerMinute % KSPDateStructure.MinutesPerHour; }
        }

        /// <summary>Gets the seconds component of the time interval represented by the current KSPPluginFramework.KSPTimeSpan structure.</summary> 
        /// <returns>
        /// The second component of the current KSPPluginFramework.KSPTimeSpan structure. The return value ranges between +/- <see cref="KSPDateStructure.SecondsPerMinute"/>
        /// </returns>
        public int Seconds {
            get { return (Int32)UT % KSPDateStructure.SecondsPerMinute; }
        }

        /// <summary>Gets the milliseconds component of the time interval represented by the current KSPPluginFramework.KSPTimeSpan structure.</summary> 
        /// <returns>The millisecond component of the current KSPPluginFramework.KSPTimeSpan structure. The return value ranges from -999 through 999.</returns>
        public int Milliseconds {
            get { return (Int32)(Math.Round(UT - Math.Floor(UT), 3) * 1000); }
        }




        /// <summary>Replaces the normal "Ticks" function. This is Seconds of UT</summary> 
        /// <returns>The number of seconds of game UT in this instance</returns>
        public Double UT { get; set; }

        #region Constructors
        //public KSPTimeSpan()
        //{
        //    UT = 0;
        //}

        /// <summary>Initializes a new KSPPluginFramework.KSPTimeSpan to a specified number of hours, minutes, and seconds.</summary> 
        /// <param name="hours">Number of hours.</param>
        /// <param name="minutes">Number of minutes.</param>
        /// <param name="seconds">Number of seconds.</param>
        public KSPTimeSpan(int hours, int minutes, int seconds)
        {
            UT = new KSPTimeSpan(0, hours, minutes, seconds, 0).UT;
        }

        /// <summary>Initializes a new KSPPluginFramework.KSPTimeSpan to a specified number of days, hours, minutes, and seconds.</summary> 
        /// <param name="days">Number of days.</param>
        /// <param name="hours">Number of hours.</param>
        /// <param name="minutes">Number of minutes.</param>
        /// <param name="seconds">Number of seconds.</param>
        public KSPTimeSpan(String days, String hours, String minutes, String seconds)
        {
            UT = new KSPTimeSpan(Convert.ToInt32(days), Convert.ToInt32(hours), Convert.ToInt32(minutes), Convert.ToInt32(seconds), 0).UT;
        }
        /// <summary>Initializes a new KSPPluginFramework.KSPTimeSpan to a specified number of days, hours, minutes, and seconds.</summary> 
        /// <param name="days">Number of days.</param>
        /// <param name="hours">Number of hours.</param>
        /// <param name="minutes">Number of minutes.</param>
        /// <param name="seconds">Number of seconds.</param>
        public KSPTimeSpan(int days, int hours, int minutes, int seconds)
        {
            UT = new KSPTimeSpan(days, hours, minutes, seconds, 0).UT;
        }

        /// <summary>Initializes a new KSPPluginFramework.KSPTimeSpan to a specified number of days, hours, minutes, seconds, and milliseconds.</summary> 
        /// <param name="days">Number of days.</param>
        /// <param name="hours">Number of hours.</param>
        /// <param name="minutes">Number of minutes.</param>
        /// <param name="seconds">Number of seconds.</param>
        /// <param name="milliseconds">Number of milliseconds.</param>
        public KSPTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            UT = days * KSPDateStructure.SecondsPerDay +
                 hours * KSPDateStructure.SecondsPerHour +
                 minutes * KSPDateStructure.SecondsPerMinute +
                 seconds +
                (Double)milliseconds / 1000;
        }

        /// <summary>Initialises a new KSPPluginFramework.KSPTimeSpan to the specified number of seconds of Game UT</summary>
        /// <param name="ut">a time period expressed in seconds</param>
        public KSPTimeSpan(Double ut)
        {
            UT = ut;
        } 
        #endregion


        #region Calculated Properties
        /// <summary>Gets the value of the current KSPPluginFramework.KSPTimeSpan structure expressed in whole and fractional milliseconds.</summary>
        /// <returns>The total number of milliseconds represented by this instance.</returns>
        public Double TotalMilliseconds { get { return UT * 1000; } }
        /// <summary>Gets the value of the current KSPPluginFramework.KSPTimeSpan structure expressed in whole and fractional seconds.</summary>
        /// <returns>The total number of seconds represented by this instance.</returns>
        public Double TotalSeconds { get { return UT; } }
        /// <summary>Gets the value of the current KSPPluginFramework.KSPTimeSpan structure expressed in whole and fractional minutes.</summary>
        /// <returns>The total number of minutes represented by this instance.</returns>
        public Double TotalMinutes { get { return UT / KSPDateStructure.SecondsPerMinute; } }
        /// <summary>Gets the value of the current KSPPluginFramework.KSPTimeSpan structure expressed in whole and fractional hours.</summary>
        /// <returns>The total number of hours represented by this instance.</returns>
        public Double TotalHours { get { return UT / KSPDateStructure.SecondsPerHour; } }
        /// <summary>Gets the value of the current KSPPluginFramework.KSPTimeSpan structure expressed in whole and fractional days.</summary>
        /// <returns>The total number of days represented by this instance.</returns>
        public Double TotalDays { get { return UT / KSPDateStructure.SecondsPerDay; } }
        #endregion

        #region String Formatter

        /// <summary>Generates some standard Templated versions of output</summary>
        /// <param name="TimeSpanFormat">Enum of some common formats</param>
        /// <returns>A string that represents the value of this instance.</returns>
        public String ToStringStandard(TimeSpanStringFormatsEnum TimeSpanFormat, Int32 Precision = 6)
        {
            switch (TimeSpanFormat)
            {
                case TimeSpanStringFormatsEnum.TimeAsUT:
                    String strReturn = "";
                    if (UT < 0) strReturn += "+ ";
                    strReturn += String.Format("{0:N0}s", Math.Abs(UT));
                    return strReturn;
                case TimeSpanStringFormatsEnum.KSPFormat:
                    return ToString(Precision);
                case TimeSpanStringFormatsEnum.DateTimeFormat:
                    return ToDateTimeString(Precision);
                case TimeSpanStringFormatsEnum.IntervalLong:
                    return (UT < 0 ? "+ " : "") + KSPUtil.dateTimeFormatter.PrintTimeLong(UT); // ToString((UT < 0?"+ ":"") + "y Year\\s, d Da\\y\\s, hh:mm:ss");
                case TimeSpanStringFormatsEnum.IntervalLongTrimYears:
                    return (UT < 0 ? "+ " : "") + KSPUtil.dateTimeFormatter.PrintTimeLong(UT); // ToString((UT < 0 ? "+ " : "") + "y Year\\s, d Da\\y\\s, hh:mm:ss").Replace("0 Years, ", "");
                case TimeSpanStringFormatsEnum.DateTimeFormatLong:
                    return (UT < 0 ? "+ " : "") + KSPUtil.dateTimeFormatter.PrintDateDeltaCompact(UT, true, true, true);
                //String strFormat = "";
                //if (Years > 0) strFormat += "y\\y";
                //if (Days > 0) strFormat += (strFormat.EndsWith("y") ? ", ":"") + "d\\d";
                //if (strFormat!="") strFormat += " ";
                //strFormat += "hh:mm:ss";

                //if (UT < 0) strFormat = "+ " + strFormat;

                //return ToString(strFormat);
                default:
                    return ToString();
            }
        }

        /// <summary>Returns the string representation of the value of this instance.</summary> 
        /// <returns>A string that represents the value of this instance.</returns>
        public override String ToString()
        {
            return ToString(3);
        }

        /// <summary>Returns the string representation of the value of this instance.</summary> 
        /// <param name="Precision">How many parts of the timespan to return (of year, Day, hour, minute, second)</param>
        /// <returns>A string that represents the value of this instance.</returns>
        public String ToString(Int32 Precision)
        {
            return (UT < 0 ? "+ " : "") + KSPUtil.dateTimeFormatter.PrintDateDeltaCompact(UT, Precision > 2, Precision >= 5, true);

            //Int32 Displayed = 0;
            //String format = "";

            //if (UT < 0) format += "+";


            //if (CalType != CalendarTypeEnum.Earth) {
            //    if ((Years != 0) && Displayed < Precision) {
            //        format = "y\\y,";
            //        Displayed++;
            //    }
            //}

            //if ((Days != 0 || format.EndsWith(",")) && Displayed < Precision)
            //{
            //    format += (format == "" ? "" : " ") + "d\\d,";
            //    Displayed++;
            //}
            //if ((Hours != 0 || format.EndsWith(",")) && Displayed < Precision)
            //{
            //    format += (format==""?"":" ") + "h\\h,";
            //    Displayed++;

            //}
            //if ((Minutes != 0 || format.EndsWith(",")) && Displayed < Precision)
            //{
            //    format += (format==""?"":" ") + "m\\m,";
            //    Displayed++;

            //}
            //if (Displayed<Precision) {
            //    format += (format==""?"":" ") + "s\\s,";
            //    Displayed++;

            //}

            //format = format.TrimEnd(',');

            //return ToString(format, null);
        }

        /// <summary>Returns the string representation of the value of this instance.</summary> 
        /// <param name="Precision">How many parts of the timespan to return (of year, Day, hour, minute, second)</param>
        /// <returns>A string that represents the value of this instance.</returns>
        public String ToDateTimeString(Int32 Precision)
        {
            return (UT < 0 ? "+ " : "") + KSPUtil.dateTimeFormatter.PrintDateDeltaCompact(UT, Precision > 2, Precision >= 5, true);
            //Int32 Displayed = 0;
            //String format = "";

            //if (UT < 0) format += "+ ";


            //if (CalType != CalendarTypeEnum.Earth)
            //{
            //    if ((Years != 0) && Displayed < Precision)
            //    {
            //        format = "y\\y,";
            //        Displayed++;
            //    }
            //}

            //if ((Days != 0 || format.EndsWith(",")) && Displayed < Precision)
            //{
            //    format += (format == "" ? "" : " ") + "d\\d,";
            //    Displayed++;
            //}
            //if ((Hours != 0 || format.EndsWith(",")) && Displayed < Precision)
            //{
            //    format += (format == "" ? "" : " ") + "hh:";
            //    Displayed++;

            //}
            //if (Displayed < Precision)
            //{
            //    format += "mm:";
            //    Displayed++;

            //}
            //if (Displayed < Precision)
            //{
            //    format += "ss";
            //    Displayed++;

            //}

            //format = format.TrimEnd(',').TrimEnd(':');

            //return ToString(format, null);
        }

        /// <summary>Returns the string representation of the value of this instance.</summary> 
        /// <param name="format">Format string using the usual characters to interpret custom datetime - as per standard Timespan custom formats</param>
        /// <returns>A string that represents the value of this instance.</returns>
        public String ToString(String format)
        {
            return ToString(format, null);
        }
        /// <summary>Returns the string representation of the value of this instance.</summary> 
        /// <param name="format">Format string using the usual characters to interpret custom datetime - as per standard Timespan custom formats</param>
        /// <returns>A string that represents the value of this instance.</returns>
        public String ToString(String format, IFormatProvider provider)
        {
            //parse and replace the format stuff
            MatchCollection matches = Regex.Matches(format, "([a-zA-z])\\1{0,}");
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                Match m = matches[i];
                Int32 mIndex = m.Index, mLength = m.Length;

                if (mIndex > 0 && format[m.Index - 1] == '\\')
                {
                    if (m.Length == 1)
                        continue;
                    else
                    {
                        mIndex++;
                        mLength--;
                    }
                }
                switch (m.Value[0])
                {
                    case 'y':
                        format = format.Substring(0, mIndex) + Math.Abs(Years).ToString("D" + mLength) + format.Substring(mIndex + mLength);
                        break;
                    case 'd':
                        format = format.Substring(0, mIndex) + Math.Abs(Days).ToString("D" + mLength) + format.Substring(mIndex + mLength);
                        break;
                    case 'h':
                        format = format.Substring(0, mIndex) + Math.Abs(Hours).ToString("D" + mLength.Clamp(1, KSPDateStructure.HoursPerDay.ToString().Length)) + format.Substring(mIndex + mLength);
                        break;
                    case 'm':
                        format = format.Substring(0, mIndex) + Math.Abs(Minutes).ToString("D" + mLength.Clamp(1, KSPDateStructure.MinutesPerHour.ToString().Length)) + format.Substring(mIndex + mLength);
                        break;
                    case 's':
                        format = format.Substring(0, mIndex) + Math.Abs(Seconds).ToString("D" + mLength.Clamp(1, KSPDateStructure.SecondsPerMinute.ToString().Length)) + format.Substring(mIndex + mLength);
                        break;

                    default:
                        break;
                }
            }

            //Now strip out the \ , but not multiple \\
            format = Regex.Replace(format, "\\\\(?=[a-z])", "");

            return format;
            //if (KSPDateStructure.CalendarType == CalendarTypeEnum.Earth)
            //    return String.Format(format, _EarthDateTime);
            //else
            //    return String.Format(format, this); //"TEST";
        }

        #endregion

        #region Instance Methods
        #region Mathematic Methods
        /// <summary>Returns a new KSPPluginFramework.KSPTimeSpan object whose value is the sum of the specified KSPPluginFramework.KSPTimeSpan object and this instance.</summary> 
        /// <param name="value">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <returns>A new object that represents the value of this instance plus the value of the timespan supplied.</returns>
        public KSPTimeSpan Add(KSPTimeSpan value) {
            return new KSPTimeSpan(UT + value.UT);
        }
        /// <summary>Returns a new KSPPluginFramework.KSPTimeSpan object whose value is the absolute value of the current KSPPluginFramework.KSPTimeSpan object.</summary> 
        /// <returns>A new object whose value is the absolute value of the current KSPPluginFramework.KSPTimeSpan object.</returns>
        public KSPTimeSpan Duration() {
            return new KSPTimeSpan(Math.Abs(UT));
        }
        /// <summary>Returns a new KSPPluginFramework.KSPTimeSpan object whose value is the negated value of this instance.</summary> 
        /// <returns>A new object with the same numeric value as this instance, but with the opposite sign.</returns>
        public KSPTimeSpan Negate() {
            return new KSPTimeSpan(UT*-1);
        }
        #endregion

        #region Comparison Methods
        /// <summary>Compares this instance to a specified KSPPluginFramework.KSPTimeSpan object and returns an integer that indicates whether this instance is shorter than, equal to, or longer than the KSPPluginFramework.KSPTimeSpan object.</summary> 
        /// <param name="value">A KSPPluginFramework.KSPTimeSpan object to compare to this instance.</param>
        /// <returns>A signed number indicating the relative values of this instance and value.Value Description A negative integer This instance is shorter than value. Zero This instance is equal to value. A positive integer This instance is longer than value.</returns>
        public Int32 CompareTo(KSPTimeSpan value) {
            return KSPTimeSpan.Compare(this, value);
        }
        /// <summary>Value Condition -1 This instance is shorter than value. 0 This instance is equal to value. 1 This instance is longer than value.-or- value is null.</summary> 
        /// <param name="value">An object to compare, or null.</param>
        /// <returns>Value Condition -1 This instance is shorter than value. 0 This instance is equal to value. 1 This instance is longer than value.-or- value is null.</returns>
        public Int32 CompareTo(System.Object value) {
            if (value == null) return 1;

            return this.CompareTo((KSPTimeSpan)value);
        }
        /// <summary>Returns a value indicating whether this instance is equal to a specified KSPPluginFramework.KSPTimeSpan object.</summary> 
        /// <param name="value">An KSPPluginFramework.KSPTimeSpan object to compare with this instance.</param>
        /// <returns>true if obj represents the same time interval as this instance; otherwise, false.</returns>
        public Boolean Equals(KSPTimeSpan value) {
            return KSPTimeSpan.Equals(this, value);
        }
        /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary> 
        /// <param name="value">An object to compare with this instance</param>
        /// <returns>true if value is a KSPPluginFramework.KSPTimeSpan object that represents the same time interval as the current KSPPluginFramework.KSPTimeSpan structure; otherwise, false.</returns>
        public override bool Equals(System.Object value) {
            return (value.GetType() == this.GetType()) && this.Equals((KSPTimeSpan)value);
        }
        #endregion        
        

        /// <summary>Returns a hash code for this instance.</summary> 
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return UT.GetHashCode();
        }

        #endregion


        #region Static Methods
        /// <summary>Compares two KSPPluginFramework.KSPTimeSpan values and returns an integer that indicates whether the first value is shorter than, equal to, or longer than the second value.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <returns>Value Condition -1 t1 is shorter than t20 t1 is equal to t21 t1 is longer than t2</returns>
        public static Int32 Compare(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            if (t1.UT < t2.UT)
                return -1;
            else if (t1.UT > t2.UT)
                return 1;
            else
                return 0;
        }
        /// <summary>Returns a value indicating whether two specified instances of KSPPluginFramework.KSPTimeSpan are equal.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the values of t1 and t2 are equal; otherwise, false.</returns>
        public static Boolean Equals(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return t1.UT == t2.UT;
        }


        /// <summary>Returns a KSPPluginFramework.KSPTimeSpan that represents a specified number of days, where the specification is accurate to the nearest millisecond.</summary> 
        /// <param name="value">A number of days, accurate to the nearest millisecond.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan that represents value.</returns>
        public static KSPTimeSpan FromDays(Double value) {
            return new KSPTimeSpan(value * KSPDateStructure.SecondsPerDay);
        }
        /// <summary>Returns a KSPPluginFramework.KSPTimeSpan that represents a specified number of hours, where the specification is accurate to the nearest millisecond.</summary> 
        /// <param name="value">A number of hours, accurate to the nearest millisecond.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan that represents value.</returns>
        public static KSPTimeSpan FromHours(Double value)
        {
            return new KSPTimeSpan(value * KSPDateStructure.SecondsPerHour);
        }
        /// <summary>Returns a KSPPluginFramework.KSPTimeSpan that represents a specified number of minutes, where the specification is accurate to the nearest millisecond.</summary> 
        /// <param name="value">A number of minutes, accurate to the nearest millisecond.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan that represents value.</returns>
        public static KSPTimeSpan FromMinutes(Double value)
        {
            return new KSPTimeSpan(value * KSPDateStructure.SecondsPerMinute);
        }
        /// <summary>Returns a KSPPluginFramework.KSPTimeSpan that represents a specified number of seconds, where the specification is accurate to the nearest millisecond.</summary> 
        /// <param name="value">A number of seconds, accurate to the nearest millisecond.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan that represents value.</returns>
        public static KSPTimeSpan FromSeconds(Double value)
        {
            return new KSPTimeSpan(value);
        }
        /// <summary>Returns a KSPPluginFramework.KSPTimeSpan that represents a specified number of milliseconds.</summary> 
        /// <param name="value">A number of milliseconds.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan that represents value.</returns>
        public static KSPTimeSpan FromMilliseconds(Double value)
        {
            return new KSPTimeSpan(value / 1000);
        }

        #endregion


        #region Operators
        /// <summary>Subtracts a specified KSPPluginFramework.KSPTimeSpan from another specified KSPPluginFramework.KSPTimeSpan.</summary> 
        /// <param name="t1"> A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2"> A TimeSpan.</param>
        /// <returns>A TimeSpan whose value is the result of the value of t1 minus the value of t2.</returns>
        public static KSPTimeSpan operator -(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return new KSPTimeSpan(t1.UT - t2.UT);
        }
        /// <summary>Returns a KSPPluginFramework.KSPTimeSpan whose value is the negated value of the specified instance.</summary> 
        /// <param name="t">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan with the same numeric value as this instance, but the opposite sign.</returns>
        public static KSPTimeSpan operator -(KSPTimeSpan t)
        {
            return new KSPTimeSpan(t.UT).Negate();
        }
        /// <summary>Adds two specified KSPPluginFramework.KSPTimeSpan instances.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <returns>A KSPPluginFramework.KSPTimeSpan whose value is the sum of the values of t1 and t2.</returns>
        public static KSPTimeSpan operator +(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return new KSPTimeSpan(t1.UT + t2.UT);
        }
        /// <summary>Returns the specified instance of KSPPluginFramework.KSPTimeSpan.</summary> 
        /// <param name="t">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <returns>Returns t.</returns>
        public static KSPTimeSpan operator +(KSPTimeSpan t)
        {
            return new KSPTimeSpan(t.UT);
        }

        /// <summary>Indicates whether two KSPPluginFramework.KSPTimeSpan instances are not equal.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the values of t1 and t2 are not equal; otherwise, false.</returns>
        public static Boolean operator !=(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return !(t1 == t2);
        }
        /// <summary>Indicates whether two KSPPluginFramework.KSPTimeSpan instances are equal.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the values of t1 and t2 are equal; otherwise, false.</returns>
        public static Boolean operator ==(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            if (object.ReferenceEquals(t1, t2))
            {
                // handles if both are null as well as object identity
                return true;
            }

            //handles if one is null and not the other
            if ((object)t1 == null || (object)t2 == null)
            {
                return false;
            }

            //now compares
            return t1.UT == t2.UT;
        }



        /// <summary>Indicates whether a specified KSPPluginFramework.KSPTimeSpan is less than another specified KSPPluginFramework.KSPTimeSpan.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the value of t1 is less than the value of t2; otherwise, false.</returns>
        public static Boolean operator <=(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return t1.CompareTo(t2) <= 0;
        }
        /// <summary>Indicates whether a specified KSPPluginFramework.KSPTimeSpan is less than or equal to another specified KSPPluginFramework.KSPTimeSpan.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the value of t1 is less than or equal to the value of t2; otherwise, false.</returns>
        public static Boolean operator <(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return t1.CompareTo(t2) < 0;
        }
        /// <summary>Indicates whether a specified KSPPluginFramework.KSPTimeSpan is greater than or equal to another specified KSPPluginFramework.KSPTimeSpan.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the value of t1 is greater than or equal to the value of t2; otherwise, false.</returns>
        public static Boolean operator >=(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return t1.CompareTo(t2) >= 0;
        }
        /// <summary>Indicates whether a specified KSPPluginFramework.KSPTimeSpan is greater than another specified KSPPluginFramework.KSPTimeSpan.</summary> 
        /// <param name="t1">A KSPPluginFramework.KSPTimeSpan.</param>
        /// <param name="t2">A TimeSpan.</param>
        /// <returns>true if the value of t1 is greater than the value of t2; otherwise, false.</returns>
        public static Boolean operator >(KSPTimeSpan t1, KSPTimeSpan t2)
        {
            return t1.CompareTo(t2) > 0;
        }
        #endregion
    }

    /// <summary>
    /// Enum of standardised outputs for Timespans as strings
    /// </summary>
    public enum TimeSpanStringFormatsEnum
    {
        TimeAsUT,
        KSPFormat,
        DateTimeFormatLong,
        IntervalLong,
        IntervalLongTrimYears,
        DateTimeFormat,
    }
}
