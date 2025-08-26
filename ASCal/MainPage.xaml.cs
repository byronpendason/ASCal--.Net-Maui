using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ASCal
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            Calendar cal = new Calendar(today.Year);
            todayLbl.Text += cal.ToASDate(today);

            int i = 0;
            Calendar.Month month = cal.Months[i];

            while (today.DayNumber >= month.NewMoon.DayNumber)
            {
                i++;
                if (i < cal.Months.Count - 1)
                {
                    month = cal.Months[i];
                }
                else
                {
                    break;
                }
            }
            if (i == cal.Months.Count - 1)
            {
                month = cal.Months[cal.Months.Count - 1];
            }
            else
            {
                month = cal.Months[i - 1];
            }
            nextMonthLbl.Text += $"{month.Name}, which begins on {month.NewMoon}.";

            i = 0;
            Calendar.Holiday holiday = cal.Holidays[i];
            while (today.DayNumber >= holiday.ModernDate.DayNumber)
            {
                i++;
                holiday = cal.Holidays[i];
            }
            //holiday = cal.Holidays[i - 1];
            nextHolidayLbl.Text += $"{holiday.Name} on {holiday.ModernDate}.";
        }
    }
}