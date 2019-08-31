using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KSPPluginFramework
{

    /// <summary>
    /// Static class to control the Calendar used by KSPDateTime and KSPTimeSpan
    /// </summary>
    public static class KSPDateStructure
    {
        //Define the Epoch
        /// <summary>What Day does UT 0 represent</summary>
        static public Int32 EpochDayOfYear { get; private set; }
        /// <summary>What Year does UT 0 represent</summary>
        static public Int32 EpochYear { get; private set; }
        
        static public KSPDateTime EpochAsKSPDateTime { 
            get {
                return new KSPDateTime(EpochYear * SecondsPerYear + EpochDayOfYear* SecondsPerDay);
            }
        }
        
        //Define the Calendar
        /// <summary>How many seconds (game UT) make up a minute</summary>
        static public Int32 SecondsPerMinute { get; private set; }
        /// <summary>How many minutes make up an hour</summary>
        static public Int32 MinutesPerHour { get; private set; }
        /// <summary>How many hours make up a day</summary>
        static public double HoursPerDay { get; private set; }
        /// <summary>How many days make up a year</summary>
        static public double DaysPerYear { get; private set; }

        /// <summary>How many seconds (game UT) make up an hour</summary>
        static public Int32 SecondsPerHour { get { return SecondsPerMinute * MinutesPerHour; } }
        /// <summary>How many seconds (game UT) make up a day</summary>
        static public double SecondsPerDay { get { return SecondsPerHour * HoursPerDay; } }
        /// <summary>How many seconds (game UT) make up a year - not relevant for Earth time</summary>
        static public double SecondsPerYear { get { return SecondsPerDay * DaysPerYear; } }

        /// <summary>How many seconds (game UT) make up a year - not relevant for Earth time</summary>
        static public double HoursPerYear { get { return HoursPerDay * DaysPerYear; } }

        
        /// <summary>What Earth date does UT 0 represent</summary>
        static public DateTime CustomEpochEarth { get; private set; }

        /// <summary>What type of Calendar is being used - KSPStock, Earth, or custom</summary>
        static public CalendarTypeEnum CalendarType {get; private set;}

        /// <summary>Set true to use the KSP IDateTimeFormatter outputs where possible</summary>
        static public bool UseStockDateFormatters = true;

        /// <summary>Sets the Date Structure to be stock KSP</summary>
        static public void SetKSPStockCalendar()
        {
            CalendarType = CalendarTypeEnum.KSPStock;

            EpochYear = 1;
            EpochDayOfYear = 1;
            SecondsPerMinute = 60;
            MinutesPerHour = 60;

            HoursPerDay = KSPUtil.dateTimeFormatter.Day / 3600; // GameSettings.KERBIN_TIME ? 6 : 24;
            DaysPerYear = KSPUtil.dateTimeFormatter.Year / KSPUtil.dateTimeFormatter.Day; // GameSettings.KERBIN_TIME ? 426 : 365;
        }

        /// <summary>Sets the Date Structure to be Earth based - Accepts Epoch date as string</summary>
        /// <param name="EpochString">Date in form of yyyy-MM-dd - eg 1951-02-20</param>
        static public void SetEarthCalendar(String EpochString)
        {
            KSPDateStructure.SetEarthCalendar(EpochString.Split('-')[0].ToInt32(),
                                                    EpochString.Split('-')[1].ToInt32(),
                                                    EpochString.Split('-')[2].ToInt32());
        }
        /// <summary>Sets the Date Structure to be Earth based - Epoch of 1/1/1951 (RSS default)</summary>
        static public void SetEarthCalendar()
        {
            SetEarthCalendar(1951, 1, 1);
        }
        /// <summary>Sets the Date Structure to be Earth based - With an epoch date supplied</summary>
        /// <param name="epochyear">year represented by UT0</param>
        /// <param name="epochmonth">month represented by UT0</param>
        /// <param name="epochday">day represented by UT0</param>
        static public void SetEarthCalendar(Int32 epochyear, Int32 epochmonth, Int32 epochday)
        {
            CalendarType = CalendarTypeEnum.Earth;

            CustomEpochEarth = new DateTime(epochyear, epochmonth, epochday);

            EpochYear = epochyear;
            EpochDayOfYear = CustomEpochEarth.DayOfYear;
            SecondsPerMinute = 60;
            MinutesPerHour = 60;

            HoursPerDay = 24;
            DaysPerYear = 365;

        }

        /// <summary>Set Calendar type to be a custom type</summary>
        static public void SetCustomCalendar()
        {
            SetKSPStockCalendar();
            CalendarType = CalendarTypeEnum.Custom;
        }

        /// <summary>Set Calendar type be a custom type with the supplied values</summary>
        /// <param name="CustomEpochYear">Year represented by UT 0</param>
        /// <param name="CustomEpochDayOfYear">DayOfYear represented by UT 0</param>
        /// <param name="CustomDaysPerYear">How many days per year in this calendar</param>
        /// <param name="CustomHoursPerDay">How many hours per day  in this calendar</param>
        /// <param name="CustomMinutesPerHour">How many minutes per hour in this calendar</param>
        /// <param name="CustomSecondsPerMinute">How many seconds per minute in this calendar</param>
        static public void SetCustomCalendar(Int32 CustomEpochYear, Int32 CustomEpochDayOfYear, Int32 CustomDaysPerYear, Int32 CustomHoursPerDay, Int32 CustomMinutesPerHour, Int32 CustomSecondsPerMinute)
        {
            CalendarType = CalendarTypeEnum.Custom;

            EpochYear = CustomEpochYear;
            EpochDayOfYear = CustomEpochDayOfYear;
            SecondsPerMinute = CustomSecondsPerMinute;
            MinutesPerHour = CustomMinutesPerHour;
            HoursPerDay = CustomHoursPerDay;
            DaysPerYear = CustomDaysPerYear;

        }

        /// <summary>Default Constructor</summary>
        static KSPDateStructure()
        {
            SetKSPStockCalendar();

            Months = new List<KSPMonth>();
            //LeapDays = new List<KSPLeapDay>();

            UseStockDateFormatters = true;
        }

        /// <summary>List of KSPMonth objects representing the months in the year</summary>
        static public List<KSPMonth> Months { get; set; }
        /// <summary>How many months have been defined</summary>
        static public Int32 MonthCount { get { return Months.Count; } }

        //static public List<KSPLeapDay> LeapDays { get; set; }
        //static public Int32 LeapDaysCount { get { return LeapDays.Count; } }
    }

    /// <summary>
    /// options for KSPDateStructure Calendar Type
    /// </summary>
    public enum CalendarTypeEnum
    {
        [Description("KSP Stock Calendar")]     KSPStock,
        [Description("Earth Calendar")]         Earth,
        [Description("Custom Calendar")]        Custom
    }

    /// <summary>
    /// Definition of a calendar month
    /// </summary>
    public class KSPMonth
    {
        public KSPMonth(String name, Int32 days) { Name = name; Days = days; }

        /// <summary>
        /// Name of the month
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// How many days in this month
        /// </summary>
        public Int32 Days { get; set; }
    }

    //public class KSPLeapDay
    //{
    //    public Int32 Frequency { get; set; }
    //    public String MonthApplied { get; set; }
    //    public Int32 DaysToAdd { get; set; }
    //}


}
