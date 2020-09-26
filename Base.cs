using System;
using System.Collections.Generic;
using System.Text;

namespace MSTUApi
{
    public class Base
    {
        public static List<string> Times = new List<string>()
        {
            "08:30 - 10:05",
            "10:15 - 11:50",
            "12:00 - 13:35",
            "13:50 - 15:25",
            "15:40 - 17:15",
            "17:25 - 19:00",
            "19:10 - 20:45"
        };
        public static List<string> Days = new List<string>()
        {
            "ПН",
            "ВТ",
            "СР",
            "ЧТ",
            "ПТ",
            "СБ"
        };
        public static List<string> DaysFull = new List<string>()
        {
            "Понедельник",
            "Вторник",
            "Среда",
            "Четверг",
            "Пятница",
            "Суббота"
        };
    }
}
