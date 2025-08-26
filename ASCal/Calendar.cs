using System;
using System.Linq;
using System.Collections.Generic;
using CosineKitty;

namespace ASCal;

public class Calendar
{
    public List<Month> Months = new List<Month>();
    public List<Holiday> Holidays = new List<Holiday>();
    public int Year;
    public int MetonicNumber;
    private DateTime summerSolstice;
    private DateTime winterSolstice;
    public bool IsLeapYear = false;

    public Calendar(int year)
    {
        LoadYear(year);
    }
    public void LoadYear(int year)
    {
        Year = year;
        MetonicNumber = (Year - 3) % 19;
        int[] leapYears = { 0, 3, 6, 8, 11, 14, 17, 19 };
        IsLeapYear = (leapYears.Contains(MetonicNumber)) ? true : false;
        SeasonsInfo seasons = Astronomy.Seasons(year);
        summerSolstice = seasons.jun_solstice.ToUtcDateTime();
        winterSolstice = seasons.dec_solstice.ToUtcDateTime();

        DateTime newMoon = NextNewMoon(Astronomy.Seasons(year - 1).dec_solstice.ToUtcDateTime());
        string[] names = { "Æfterra Ġeola", "Solmonaþ", "Hreðmonaþ", "Ēosturmonaþ", "Þrimilcemonaþ", "Ærra Liða", "Þriliða", "Æfterra Liða", "Weodmonaþ", "Haliġmonaþ", "Wintermonaþ", "Blotmonaþ", "Ærra Ġeola" };
        List<string> monthNames = new List<string>(names);
        if (!IsLeapYear)
        {
            monthNames.RemoveAt(6);
        }

        foreach (string m in monthNames)
        {
            DateTime nextNewMoon = NextNewMoon(newMoon);
            Months.Add(new Month(m, newMoon, NextFullMoon(newMoon), nextNewMoon));
            newMoon = nextNewMoon;
        }
        DateOnly winterfylleth = (IsLeapYear) ? Months[10].FullMoon : Months[9].FullMoon;
        DateOnly modranihtMW = DateOnly.FromDateTime(winterSolstice.AddDays(2));
        DateOnly newYearsEve = Months[Months.Count - 1].LastDay;

        Holidays.Add(new Holiday("Ġeāres Dæġ (New Year's Day)", Months[0].NewMoon, ToASDate(Months[0].NewMoon)));
        Holidays.Add(new Holiday("Ēosturdæġ", Months[3].FullMoon, ToASDate(Months[3].FullMoon)));
        Holidays.Add(new Holiday("Blostmfreols", Months[3].LastDay, ToASDate(Months[3].LastDay)));
        Holidays.Add(new Holiday("Midsumor", DateOnly.FromDateTime(summerSolstice), ToASDate(DateOnly.FromDateTime(summerSolstice))));
        Holidays.Add(new Holiday("Winterfylleth", winterfylleth, ToASDate(winterfylleth)));
        Holidays.Add(new Holiday("Modraniht (Traditional)", DateOnly.FromDateTime(winterSolstice), ToASDate(DateOnly.FromDateTime(winterSolstice))));
        Holidays.Add(new Holiday("Modraniht (Mine Wyrtruman)", modranihtMW, ToASDate(modranihtMW)));
        Holidays.Add(new Holiday("Ġeāres Niht (New Year's Eve)", newYearsEve, ToASDate(newYearsEve)));

        //Console.WriteLine(ToASDate(DateOnly.FromDateTime(summerSolstice)));
    }
    public DateTime NextNewMoon(DateTime time)
    {
        AstroTime astroTime = new AstroTime(time);
        MoonQuarterInfo moonQuarter = Astronomy.SearchMoonQuarter(astroTime);
        while (moonQuarter.quarter != 0)
        {
            moonQuarter = Astronomy.NextMoonQuarter(moonQuarter);
        }
        time = moonQuarter.time.ToUtcDateTime();

        if (time.Hour < 12)
        {
            time = time.AddDays(1);
        }
        else
        {
            time = time.AddDays(2);
        }

        return time;
    }
    public DateTime NextFullMoon(DateTime time)
    {
        AstroTime astroTime = new AstroTime(time);
        MoonQuarterInfo moonQuarter = Astronomy.SearchMoonQuarter(astroTime);
        while (moonQuarter.quarter != 2)
        {
            moonQuarter = Astronomy.NextMoonQuarter(moonQuarter);
        }
        time = moonQuarter.time.ToUtcDateTime();

        return time;
    }
    public string ToASDate(DateOnly modernDate)
    {
        int i = 0;
        if (modernDate < Months[i].NewMoon)
        {
            LoadYear(Year - 1);
        }
        else if (modernDate > Months[Months.Count - 1].LastDay)
        {
            LoadYear(Year + 1);
        }
        Calendar.Month month = Months[i];

        while (modernDate.DayNumber >= month.NewMoon.DayNumber)
        {
            i++;
            if (i < Months.Count - 1)
            {
                month = Months[i];
            }
            else
            {
                break;
            }
        }
        if (i == Months.Count - 1)
        {
            month = Months[Months.Count - 1];
        }
        else
        {
            month = Months[i - 1];
        }

        int day = (modernDate.DayNumber - month.NewMoon.DayNumber) + 1;
        return $"{Calendar.GetOrdinal(day)} of {month.Name}";
    }
    public static string GetOrdinal(int num)
    {
        // from https://stackoverflow.com/questions/20156/is-there-an-easy-way-to-create-ordinals-in-c#20175
        if (num <= 0) return num.ToString();

        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }

        switch (num % 10)
        {
            case 1:
                return num + "st";
            case 2:
                return num + "nd";
            case 3:
                return num + "rd";
            default:
                return num + "th";
        }
    }
    public class Month
    {
        public string Name;
        public DateOnly NewMoon;
        public DateOnly FullMoon;
        public DateOnly NextNewMoon;
        public DateOnly LastDay;
        public int MonthLength;

        public Month(string name, DateTime newMoon, DateTime fullMoon, DateTime nextNewMoon)
        {
            Name = name;
            NewMoon = DateOnly.FromDateTime(newMoon);
            FullMoon = DateOnly.FromDateTime(fullMoon);
            NextNewMoon = DateOnly.FromDateTime(nextNewMoon);
            LastDay = DateOnly.FromDateTime(nextNewMoon.AddDays(-1));
            MonthLength = NextNewMoon.DayNumber - NewMoon.DayNumber;
        }
    }
    public class Holiday
    {
        public string Name;
        public DateOnly ModernDate;
        public string ASDate;

        public Holiday(string name, DateOnly modernDate, string asDate)
        {
            Name = name;
            ModernDate = modernDate;
            ASDate = asDate;
        }
    }
}
