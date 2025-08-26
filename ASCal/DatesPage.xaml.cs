using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using ASCal;

namespace ASCal
{
    public partial class DatesPage : ContentPage
    {
        public DatesPage()
        {
            InitializeComponent();

            int year = DateTime.Now.Year;
            title.Text += Convert.ToString(year);
            Calendar cal = new Calendar(year);

            int row = 1;
            foreach (Calendar.Month month in cal.Months)
            {
                Label label0 = new Label
                {
                    Text = $"{month.Name}",
                };
                table1.Add(label0, 0, row);
                Label label1 = new Label
                {
                    Text = $"{month.NewMoon}",

                };
                table1.Add(label1, 1, row);
                Label label2 = new Label
                {
                    Text = $"{month.FullMoon}",

                };
                table1.Add(label2, 2, row);
                Label label3 = new Label
                {
                    Text = $"{month.MonthLength} days",

                };
                table1.Add(label3, 3, row);
                row++;
            }
            row = 1;
            foreach (Calendar.Holiday holiday in cal.Holidays)
            {
                Label label0 = new Label
                {
                    Text = $"{holiday.Name}",
                };
                table2.Add(label0, 0, row);
                Label label1 = new Label
                {
                    Text = $"{holiday.ModernDate}",
                };
                table2.Add(label1, 1, row);
                Label label2 = new Label
                {
                    Text = $"{holiday.ASDate}",
                };
                table2.Add(label2, 2, row);
                row++;
            }
        }
        /*
        public static DateTime ToDateTime(AstroTime time)
        {
            string[] parts = Convert.ToString(time).Split('-', 'T', ':', 'Z');

            return new DateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]), 0);
        }
        public static DateOnly ToDateOnly(DateTime time)
        {
            return new DateOnly(time.Year, time.Month, time.Day);
        }
        */

    }
}