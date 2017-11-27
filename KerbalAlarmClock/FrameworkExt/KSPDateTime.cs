using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;

namespace KSPPluginFramework
{
	/// <summary>Represents an instant in time, typically expressed as a date and time of day.</summary>
	public class KSPDateTime : IFormattable
	{
		//Shortcut to the Calendar Type
		private CalendarTypeEnum CalType { get { return KSPDateStructure.CalendarType; } }

		
		//Descriptors of DateTime - uses UT as the Root value

		/// <summary>Gets the year component of the date represented by this instance.</summary>
		public int Year {
			get { if (CalType == CalendarTypeEnum.Earth) 
				return _EarthDateTime.Year;
			else
				return KSPDateStructure.EpochYear + (Int32)(UT / KSPDateStructure.SecondsPerYear); 
			}
		}

		/// <summary>Gets the day of the year represented by this instance.</summary>
		/// <returns>The day of the year, expressed as a value between 1 and <see cref="KSPDateStructure.DaysPerYear"/>.</returns>
		public int DayOfYear {
			get { if (CalType == CalendarTypeEnum.Earth) 
				return _EarthDateTime.DayOfYear;
			else
				return KSPDateStructure.EpochDayOfYear + (Int32)(UT / KSPDateStructure.SecondsPerDay % KSPDateStructure.DaysPerYear); 
			}
		}

		/// <summary>Gets the day of the month represented by this instance.</summary>
		/// <returns>The day component, expressed as a value between 1 and the number of days in the month.</returns>
		public int Day
		{
			get
			{
				if (CalType == CalendarTypeEnum.Earth)
					return _EarthDateTime.Day;
				else
					return DayOfMonth;
			}
		}


		/// <summary>Gets the month component of the date represented by this instance.</summary>
		/// <returns>
		/// The day component, expressed as a value between 1 and the months in the year.
		/// If the Defined calendar has no months then this will be 0
		/// </returns>
		public int Month
		{
			get {
				if (CalType == CalendarTypeEnum.Earth)
					return _EarthDateTime.Month;
				else
				{
					if (KSPDateStructure.MonthCount < 1)
						return 0;
					else
						return KSPDateStructure.Months.IndexOf(MonthObj)+1;
				}
			}
		}

		private KSPMonth MonthObj {
			get {
				if (KSPDateStructure.MonthCount < 1)
					return null;
				Int32 monthMaxDay=0;
				for (int i = 0; i < KSPDateStructure.MonthCount; i++){
					if (DayOfYear <= monthMaxDay + KSPDateStructure.Months[i].Days)
						return KSPDateStructure.Months[i];
				}
				return KSPDateStructure.Months.Last();
			}
		}
		private Int32 DayOfMonth {
			get {
				if (KSPDateStructure.MonthCount < 1)
					return DayOfYear;

				Int32 monthMaxDay = 0;
				for (int i = 0; i < KSPDateStructure.MonthCount; i++)
				{
					if (DayOfYear <= monthMaxDay + KSPDateStructure.Months[i].Days)
						return DayOfYear - monthMaxDay;
				}
				return DayOfYear;
			}
		}


		/// <summary>Gets the hour component of the date represented by this instance.</summary>
		/// <returns>The hour component, expressed as a value between 1 and <see cref="KSPDateStructure.HoursPerDay"/>.</returns>
		public int Hour { get { if (CalType == CalendarTypeEnum.Earth) return _EarthDateTime.Hour; else return _TimeSpanFromEpoch.Hours; } }
		/// <summary>Gets the minute Component of the date represented by this instance.</summary>
		/// <returns>The minute component, expressed as a value between 1 and <see cref="KSPDateStructure.MinutesPerHour"/>.</returns>
		public int Minute { get { if (CalType == CalendarTypeEnum.Earth) return _EarthDateTime.Minute; else return _TimeSpanFromEpoch.Minutes; } }
		/// <summary>Gets the second component of the date represented by this instance.</summary>
		/// <returns>The second component, expressed as a value between 1 and <see cref="KSPDateStructure.SecondsperMinute"/>.</returns>
		public int Second { get { if (CalType == CalendarTypeEnum.Earth) return _EarthDateTime.Second; else return _TimeSpanFromEpoch.Seconds; } }
		/// <summary>Gets the millisecond component of the date represented by this instance.</summary>
		/// <returns>The hour component, expressed as a value between 1 and 999.</returns>
		public int Millisecond { get { if (CalType == CalendarTypeEnum.Earth) return _EarthDateTime.Millisecond; else return _TimeSpanFromEpoch.Milliseconds; } }

		/// <summary>Replaces the normal "Ticks" function. This is Seconds of UT since game time 0</summary>
		/// <returns>The number of seconds of game UT since Epoch</returns>
		public Double UT
		{
			get
			{
				//if (KSPDateStructure.CalendarType== CalendarTypeEnum.Earth)
				//    return _EarthDateTime.Subtract(KSPDateStructure.CustomEpochEarth).TotalSeconds;
				//else
					return _TimeSpanFromEpoch.UT; 
			}
			set { 
				_TimeSpanFromEpoch = new KSPTimeSpan(value);
				//if (KSPDateStructure.CalendarType == CalendarTypeEnum.Earth)
				//    _EarthDateTime = KSPDateStructure.CustomEpochEarth.AddSeconds(value);
			} 
		}

		private KSPTimeSpan _TimeSpanFromEpoch;
		private DateTime _EarthDateTime { get { return KSPDateStructure.CustomEpochEarth.AddSeconds(UT); } }
		private DateTime _EarthDateTimeEpoch { get { return new DateTime(KSPDateStructure.EpochYear, 1, KSPDateStructure.EpochDayOfYear); } }

		#region Constructors
		//public KSPDateTime()
		//{
		//    UT = 0;
		//}

		/// <summary>Initializes a new instance of the System.DateTime structure to the specified year and day.</summary>
		/// <param name="year">The year</param>
		/// <param name="dayofyear">The day of the year</param>
		public KSPDateTime(int year, int dayofyear)
		{
			UT = new KSPDateTime(year, dayofyear, 0, 0, 0).UT;
		}
        /// <summary>Initializes a new instance of the System.DateTime structure to the specified year and day.</summary>
        /// <param name="year">The year</param>
        /// <param name="dayofyear">The day of the year</param>
        public KSPDateTime(String year, String dayofyear)
        {
            UT = new KSPDateTime(year, dayofyear, "0", "0", "0").UT;
        }

		/// <summary>Initializes a new instance of the System.DateTime structure to the specified year, day, hour, minute, and second.</summary>
		/// <param name="year">The year</param>
		/// <param name="dayofyear">The day of the year</param>
		/// <param name="hour">The hour</param>
		/// <param name="minute">The minute</param>
		/// <param name="second">The second</param>
		public KSPDateTime(String year, String day, String hour, String minute, String second)
		{
			UT = new KSPDateTime(Convert.ToInt32(year), Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second), 0).UT;
		}
		/// <summary>Initializes a new instance of the System.DateTime structure to the specified year, day, hour, minute, and second.</summary>
		/// <param name="year">The year</param>
		/// <param name="day">The day of the year</param>
		/// <param name="hour">The hour</param>
		/// <param name="minute">The minute</param>
		/// <param name="second">The second</param>
		public KSPDateTime(int year, int day, int hour, int minute, int second)
		{
			UT = new KSPDateTime(year, day, hour, minute, second, 0).UT;
		}
		/// <summary>Initializes a new instance of the System.DateTime structure to the specified year, day, hour, minute, and second.</summary>
		/// <param name="year">The year</param>
		/// <param name="day">The day of the year</param>
		/// <param name="hour">The hour</param>
		/// <param name="minute">The minute</param>
		/// <param name="second">The second</param>
		/// <param name="millisecond">The milliseconds</param>
		public KSPDateTime(int year, int day, int hour, int minute, int second, int millisecond)
		{
			//Test for entering values outside the norm - eg 25 hours, day 600

			UT = new KSPTimeSpan((Int32)((year - KSPDateStructure.EpochYear) * KSPDateStructure.DaysPerYear)  +
								(day - KSPDateStructure.EpochDayOfYear),
								hour,
								minute,
								second,
								millisecond
								).UT;
		}

		/// <summary>Initializes a new instance of the System.DateTime structure to a specified number to the specified number of seconds of Game UT.</summary>
		/// <param name="ut">a time period expressed in seconds</param>
		public KSPDateTime(Double ut)
		{
			UT = ut;
		} 
		#endregion


		#region Calculated Properties
		/// <summary>Gets the date component of this instance.</summary>
		/// <returns>A new System.DateTime with the same date as this instance, and the time value set to 00:00:00.</returns>
		public KSPDateTime Date { get { return new KSPDateTime(Year, DayOfYear); } }


		/// <summary>Gets the time of day for this instance.</summary>
		/// <returns>A System.TimeSpan that represents the fraction of the day that has elapsed since midnight.</returns>
		public KSPTimeSpan TimeOfDay { get { return new KSPTimeSpan(UT % KSPDateStructure.SecondsPerDay); } }


		/// <summary>Gets a System.DateTime object that is set to the current date and time of the game.</summary>
		/// <returns>A System.DateTime whose value is the current game date and time.</returns>
		public static KSPDateTime Now
		{
			get { return new KSPDateTime(Planetarium.GetUniversalTime()); }
		}
		/// <summary>Gets the current date.</summary>
		/// <returns>A System.DateTime set to today's date, with the time component set to 00:00:00.</returns>
		public static KSPDateTime Today
		{
			get { return new KSPDateTime(Planetarium.GetUniversalTime()).Date; }
		}
		#endregion


		#region String Formatter

		private AMPMEnum AMPM {
			get {
				if (KSPDateStructure.HoursPerDay % 2 == 0)
				{
					if (Hour < (KSPDateStructure.HoursPerDay / 2))
						return AMPMEnum.AM;
					else
						return AMPMEnum.PM;
				}
				else
					return AMPMEnum.OddHoursPerDay;
			}
		}
		private enum AMPMEnum {
			AM,PM,OddHoursPerDay
		}

		/// <summary>Generates some standard Templated versions of output</summary>
		/// <param name="DateFormat">Enum of some common formats</param>
		/// <returns>A string that represents the value of this instance.</returns>
		public String ToStringStandard(DateStringFormatsEnum DateFormat){
			switch (DateFormat)
			{
				case DateStringFormatsEnum.TimeAsUT:
					String strReturn = "";
					if (UT < 0) strReturn += "+ ";
					strReturn += String.Format("{0:N0}s", Math.Abs(UT));
					return strReturn;
				case DateStringFormatsEnum.KSPFormat:
					return ToString();
				case DateStringFormatsEnum.KSPFormatWithSecs:
					return KSPUtil.dateTimeFormatter.PrintDate(UT, true, true); // ToString("Year y, Da\\y d - H\\h, m\\m, s\\s");
				case DateStringFormatsEnum.DateTimeFormat:
                    if (KSPDateStructure.CalendarType==CalendarTypeEnum.Earth)
                        return ToString("d MMM yyyy, HH:mm:ss");
                    else
					    return KSPUtil.dateTimeFormatter.PrintDateCompact(UT, true, true); // ToString("Year y, Da\\y d, HH:mm:ss");
				default:
					return ToString();
			}
		}

		/// <summary>Returns the string representation of the value of this instance.</summary> 
		/// <returns>A string that represents the value of this instance.</returns>
		public override String ToString()
		{
			if (CalType ==CalendarTypeEnum.Earth) {
				return ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
			} else {
                return KSPUtil.dateTimeFormatter.PrintDate(UT, true, false); // ToString("Year y, Da\\y d - H\\h, m\\m", null);
			}
		}
		/// <summary>Returns the string representation of the value of this instance.</summary> 
		/// <param name="format">Format string using the usual characters to interpret custom datetime - as per standard DateTime custom formats</param>
		/// <returns>A string that represents the value of this instance.</returns>
		public String ToString(String format)
		{
			return ToString(format, null);
		}
		/// <summary>Returns the string representation of the value of this instance.</summary> 
		/// <param name="format">Format string using the usual characters to interpret custom datetime - as per standard DateTime custom formats</param>
		/// <returns>A string that represents the value of this instance.</returns>
		public String ToString(String format, IFormatProvider provider)
		{
			//parse and replace the format stuff
			MatchCollection matches = Regex.Matches(format, "([a-zA-z])\\1{0,}");
			for (int i = matches.Count-1; i >=0; i--)
			{
				Match m = matches[i];
				Int32 mIndex = m.Index,mLength = m.Length;

				if (mIndex>0 && format[m.Index - 1] == '\\')
				{
					if (m.Length == 1)
						continue;
					else {
						mIndex++;
						mLength--;
					}
				}
				switch (m.Value[0])
				{
					case 'y': 
						format = format.Substring(0, mIndex) + Year.ToString("D" + mLength) + format.Substring(mIndex + mLength);
						break;
					case 'M':
                        if (mLength < 3)
                        {
                            String input2 = Month.ToString("D" + mLength);
                            format = format.Substring(0, mIndex) + input2 + format.Substring(mIndex + mLength);
                        }
                        else if (mLength == 3)
                        {
                            if (KSPDateStructure.CalendarType== CalendarTypeEnum.Earth)
                                format = format.Substring(0, mIndex) + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Month) + format.Substring(mIndex + mLength);
                            else
                                if (KSPDateStructure.MonthCount < 1)
                                {
                                    String input2 = Month.ToString("D" + mLength);
                                    format = format.Substring(0, mIndex) + input2 + format.Substring(mIndex + mLength);
                                }
                                else
                                {
                                    format = format.Substring(0, mIndex) + KSPDateStructure.Months[Month].ToString().Substring(0, 3) + format.Substring(mIndex + mLength);
                                }
                        }
                        else
                        {
                            if (KSPDateStructure.CalendarType== CalendarTypeEnum.Earth)
                                format = format.Substring(0, mIndex) + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month) + format.Substring(mIndex + mLength);
                            else
                                if (KSPDateStructure.MonthCount<1){
                                    String input2 = Month.ToString("D" + mLength);
                                    format = format.Substring(0, mIndex) + input2 + format.Substring(mIndex + mLength);
                                } else {
                                    format = format.Substring(0, mIndex) + KSPDateStructure.Months[Month] + format.Substring(mIndex + mLength);
                                }
                        }
						break;
					case 'd':
						format = format.Substring(0, mIndex) + Day.ToString("D" + mLength) + format.Substring(mIndex + mLength);
						break;
					case 'h':
						//how to do this one AM/PM Hours
						String HalfDayTime="";
						switch (AMPM)
						{
							case AMPMEnum.AM:
								HalfDayTime = Hour.ToString("D" + mLength.Clamp(1, (KSPDateStructure.HoursPerDay / 2).ToString().Length));
								break;
							case AMPMEnum.PM:
								HalfDayTime = (Hour - (KSPDateStructure.HoursPerDay / 2)).ToString("D" + mLength.Clamp(1, (KSPDateStructure.HoursPerDay / 2).ToString().Length));
								break;
							case AMPMEnum.OddHoursPerDay:
							default:
								HalfDayTime = Hour.ToString("D" + mLength.Clamp(1, KSPDateStructure.HoursPerDay.ToString().Length));
								break;
						}

						format = format.Substring(0, mIndex) + HalfDayTime + format.Substring(mIndex + mLength);
						break;
					case 't':
						if (AMPM != AMPMEnum.OddHoursPerDay)
							format = format.Substring(0, mIndex) + AMPM.ToString().ToLower() + format.Substring(mIndex + mLength);
						break;
					case 'T':
						if (AMPM != AMPMEnum.OddHoursPerDay)
							format = format.Substring(0, mIndex) + AMPM.ToString().ToUpper() + format.Substring(mIndex + mLength);
						break;
					case 'H':
						format = format.Substring(0, mIndex) + Hour.ToString("D" + mLength.Clamp(1,KSPDateStructure.HoursPerDay.ToString().Length)) + format.Substring(mIndex + mLength);
						break;
					case 'm':
						format = format.Substring(0, mIndex) + Minute.ToString("D" + mLength.Clamp(1,KSPDateStructure.MinutesPerHour.ToString().Length)) + format.Substring(mIndex + mLength);
						break;
					case 's':
						format = format.Substring(0, mIndex) + Second.ToString("D" + mLength.Clamp(1,KSPDateStructure.SecondsPerMinute.ToString().Length)) + format.Substring(mIndex + mLength);
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
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime object whose value is the sum of the specified KSPPluginFramework.KSPTimeSpan object and this instance.</summary> 
		/// <param name="value">A KSPPluginFramework.KSPTimeSpan.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the time interval represented by value.</returns>
		public KSPDateTime Add(KSPTimeSpan value)
		{
			return new KSPDateTime(UT + value.UT);
		}
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified years to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional years. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of years represented by value.</returns>
		public KSPDateTime AddYears(Int32 value)
		{
			if (CalType != CalendarTypeEnum.Earth)
				return new KSPDateTime(UT + value * KSPDateStructure.SecondsPerYear);
			else {
				DateTime newDate = _EarthDateTime.AddYears(value);
				return new KSPDateTime(newDate.Subtract(_EarthDateTimeEpoch).TotalSeconds);
			}
		}
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified days to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional days. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of days represented by value.</returns>
		public KSPDateTime AddDays(Double value)
		{
			return new KSPDateTime(UT + value * KSPDateStructure.SecondsPerDay);
		}
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified hours to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional hours. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of hours represented by value.</returns>
		public KSPDateTime AddHours(Double value)
		{
			return new KSPDateTime(UT + value * KSPDateStructure.SecondsPerHour);
		}
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified minutes to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional minutes. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of minutes represented by value.</returns>
		public KSPDateTime AddMinutes(Double value)
		{
			return new KSPDateTime(UT + value * KSPDateStructure.SecondsPerMinute);
		}
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified seconds to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional seconds. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of seconds represented by value.</returns>
		public KSPDateTime AddSeconds(Double value)
		{
			return new KSPDateTime(UT + value);
		}
		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified milliseconds to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional milliseconds. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of milliseconds represented by value.</returns>
		public KSPDateTime AddMilliSeconds(Double value)
		{
			return new KSPDateTime(UT + value / 1000);
		}

		/// <summary>Returns a new KSPPluginFramework.KSPDateTime that adds the specified seconds to the value of this instance.</summary> 
		/// <param name="value">a number of whole or fractional seconds. Can be positive or negative.</param>
		/// <returns>A KSPPluginFramework.KSPDateTime whose value is the sum of the date and time represented by this instance and the number of seconds represented by value.</returns>
		public KSPDateTime AddUT(Double value)
		{
			return new KSPDateTime(UT + value);
		}

		/// <summary>Subtracts the specified date and time from this instance.</summary>
		/// <param name="value">An instance of System.DateTime.</param>
		/// <returns>A System.DateTime equal to the date and time represented by this instance minus the date and time represented by value.</returns>
		public KSPDateTime Subtract(KSPDateTime value)
		{
			return new KSPDateTime(UT - value.UT);
		}
		/// <summary>Subtracts the specified duration from this instance.</summary>
		/// <param name="value">An instance of System.TimeSpan.</param>
		/// <returns>A System.DateTime equal to the date and time represented by this instance minus the time interval represented by value.</returns>
		public KSPTimeSpan Subtract(KSPTimeSpan value)
		{
			return new KSPTimeSpan(UT - value.UT);
		}

		#endregion


		#region Comparison Methods
		/// <summary>Compares this instance to a specified KSPPluginFramework.KSPDateTime object and returns an integer that indicates whether this instance is shorter than, equal to, or longer than the KSPPluginFramework.KSPDateTime object.</summary> 
		/// <param name="value">A KSPPluginFramework.KSPDateTime object to compare to this instance.</param>
		/// <returns>A signed number indicating the relative values of this instance and value.Value Description A negative integer This instance is shorter than value. Zero This instance is equal to value. A positive integer This instance is longer than value.</returns>
		public Int32 CompareTo(KSPDateTime value)
		{
			return KSPDateTime.Compare(this, value);
		}
		/// <summary>Value Condition -1 This instance is shorter than value. 0 This instance is equal to value. 1 This instance is longer than value.-or- value is null.</summary> 
		/// <param name="value">An object to compare, or null.</param>
		/// <returns>Value Condition -1 This instance is shorter than value. 0 This instance is equal to value. 1 This instance is longer than value.-or- value is null.</returns>
		public Int32 CompareTo(System.Object value)
		{
			if (value == null) return 1;
			return this.CompareTo((KSPDateTime)value);
		}
		/// <summary>Returns a value indicating whether this instance is equal to a specified KSPPluginFramework.KSPDateTime object.</summary> 
		/// <param name="value">An KSPPluginFramework.KSPDateTime object to compare with this instance.</param>
		/// <returns>true if obj represents the same time interval as this instance; otherwise, false.</returns>
		public Boolean Equals(KSPDateTime value)
		{
			return KSPDateTime.Equals(this, value);
		}
		/// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary> 
		/// <param name="value">An object to compare with this instance</param>
		/// <returns>true if value is a KSPPluginFramework.KSPDateTime object that represents the same time interval as the current KSPPluginFramework.KSPDateTime structure; otherwise, false.</returns>
		public override bool Equals(System.Object value)
		{
			return (value.GetType() == this.GetType()) && this.Equals((KSPDateTime)value);
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
		/// <summary>Compares two KSPPluginFramework.KSPDateTime values and returns an integer that indicates whether the first value is shorter than, equal to, or longer than the second value.</summary> 
		/// <param name="t1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="t2">A KSPPluginFramework.KSPDateTime.</param>
		/// <returns>Value Condition -1 t1 is shorter than t20 t1 is equal to t21 t1 is longer than t2</returns>
		public static Int32 Compare(KSPDateTime t1, KSPDateTime t2)
		{
			if (t1.UT < t2.UT)
				return -1;
			else if (t1.UT > t2.UT)
				return 1;
			else
				return 0;
		}
		/// <summary>Returns a value indicating whether two specified instances of KSPPluginFramework.KSPDateTime are equal.</summary> 
		/// <param name="t1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="t2">A DateTime.</param>
		/// <returns>true if the values of t1 and t2 are equal; otherwise, false.</returns>
		public static Boolean Equals(KSPDateTime t1, KSPDateTime t2)
		{
			return t1.UT == t2.UT;
		}

        public static KSPDateTime FromEarthValues(Int32 Year, Int32 Month, Int32 Day)
        {
            return new KSPDateTime(new DateTime(Year, Month, Day).Subtract(KSPDateStructure.CustomEpochEarth).TotalSeconds);
        }
        public static KSPDateTime FromEarthValues(String Year, String Month, String Day)
        {
            return FromEarthValues(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day));
        }

		#endregion


		#region Operators
		/// <summary>Subtracts a specified date and time from another specified date and time and returns a time interval.</summary> 
		/// <param name="d1"> A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2"> A KSPPluginFramework.KSPDateTime.</param>
		/// <returns>A DateTime whose value is the result of the value of d1 minus the value of d2.</returns>
		public static KSPTimeSpan operator -(KSPDateTime d1, KSPDateTime d2)
		{
			return new KSPTimeSpan(d1.UT - d2.UT);
		}
		/// <summary>Subtracts a specified duration from another specified date and time and returns a time interval.</summary> 
		/// <param name="d"> A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="t"> A KSPPluginFramework.KSPTimeSpan.</param>
		/// <returns>A DateTime whose value is the result of the value of d minus the value of t.</returns>
		public static KSPDateTime operator -(KSPDateTime d, KSPTimeSpan t)
		{
			return new KSPDateTime(d.UT - t.UT);
		}
		/// <summary>Adds a specified duration from another specified date and time and returns a time interval.</summary> 
		/// <param name="d"> A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="t"> A KSPPluginFramework.KSPTimeSpan.</param>
		/// <returns>A DateTime whose value is the result of the value of d plus the value of t.</returns>
		public static KSPDateTime operator +(KSPDateTime d, KSPTimeSpan t)
		{
			return new KSPDateTime(d.UT + t.UT);
		}

		/// <summary>Indicates whether two KSPPluginFramework.KSPDateTime instances are not equal.</summary> 
		/// <param name="d1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2">A DateTime.</param>
		/// <returns>true if the values of d1 and d2 are not equal; otherwise, false.</returns>
		public static Boolean operator !=(KSPDateTime d1, KSPDateTime d2)
		{
			return !(d1 == d2);
		}
		/// <summary>Indicates whether two KSPPluginFramework.KSPDateTime instances are equal.</summary> 
		/// <param name="d1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2">A KSPDateTime.</param>
		/// <returns>true if the values of d1 and d2 are equal; otherwise, false.</returns>
		public static Boolean operator ==(KSPDateTime d1, KSPDateTime d2)
		{
            if (object.ReferenceEquals(d1, d2))
            {
                // handles if both are null as well as object identity
                return true;
            }

            //handles if one is null and not the other
            if ((object)d1 == null || (object)d2 == null)
            {
                return false;
            }

            //now compares
            return d1.UT == d2.UT;
		}



		/// <summary>Indicates whether a specified KSPPluginFramework.KSPDateTime is less than or equal to another specified KSPPluginFramework.KSPDateTime.</summary> 
		/// <param name="d1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2">A DateTime.</param>
		/// <returns>true if the value of d1 is less than or equal to the value of d2; otherwise, false.</returns>
		public static Boolean operator <=(KSPDateTime d1, KSPDateTime d2)
		{
			return d1.CompareTo(d2) <= 0;
		}
		/// <summary>Indicates whether a specified KSPPluginFramework.KSPDateTime is less than another specified KSPPluginFramework.KSPDateTime.</summary> 
		/// <param name="d1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2">A DateTime.</param>
		/// <returns>true if the value of d1 is less than the value of d2; otherwise, false.</returns>
		public static Boolean operator <(KSPDateTime d1, KSPDateTime d2)
		{
			return d1.CompareTo(d2) < 0;
		}
		/// <summary>Indicates whether a specified KSPPluginFramework.KSPDateTime is greater than or equal to another specified KSPPluginFramework.KSPDateTime.</summary> 
		/// <param name="d1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2">A DateTime.</param>
		/// <returns>true if the value of d1 is greater than or equal to the value of d2; otherwise, false.</returns>
		public static Boolean operator >=(KSPDateTime d1, KSPDateTime d2)
		{
			return d1.CompareTo(d2) >= 0;
		}
		/// <summary>Indicates whether a specified KSPPluginFramework.KSPDateTime is greater than another specified KSPPluginFramework.KSPDateTime.</summary> 
		/// <param name="d1">A KSPPluginFramework.KSPDateTime.</param>
		/// <param name="d2">A DateTime.</param>
		/// <returns>true if the value of d1 is greater than the value of d2; otherwise, false.</returns>
		public static Boolean operator >(KSPDateTime d1, KSPDateTime d2)
		{
			return d1.CompareTo(d2) > 0;
		} 
		#endregion

	}


	/// <summary>
	/// Enum of standardised outputs for DateTimes as strings
	/// </summary>
	public enum DateStringFormatsEnum
	{
		TimeAsUT,
		KSPFormat,
		KSPFormatWithSecs,
		DateTimeFormat
	}
}
