using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF_APP.Utilities
{
    public class HoursToColorConverter : IValueConverter
    {
        // Словарь с ограничениями для каждого класса (должен совпадать с LessonHoursVM)
        private readonly Dictionary<int, (int Min, int Max)> _classLimits = new Dictionary<int, (int Min, int Max)>
        {
            { 1, (18, 18) },   // 1 класс: ровно 18 часов
            { 2, (19, 19) },   // 2 класс: ровно 19 часов
            { 3, (22, 22) },   // 3 класс: ровно 22 часа
            { 4, (22, 22) },   // 4 класс: ровно 22 часа
            { 5, (25, 27) },   // 5 класс: 25-27 часов
            { 6, (27, 29) },   // 6 класс: 27-29 часов
            { 7, (28, 30) },   // 7 класс: 28-30 часов
            { 8, (29, 31) },   // 8 класс: 29-31 часов
            { 9, (29, 31) },   // 9 класс: 29-31 часов
            { 10, (28, 31) },  // 10 класс: 28-31 часов
            { 11, (28, 31) }   // 11 класс: 28-31 часов
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Параметр должен содержать номер класса
            if (value is int hours && parameter != null)
            {
                if (int.TryParse(parameter.ToString(), out int classNumber))
                {
                    var limits = _classLimits[classNumber];

                    // Если часы в пределах нормы - зеленый
                    if (hours >= limits.Min && hours <= limits.Max)
                    {
                        return new SolidColorBrush(Colors.Green);
                    }
                    // Если меньше минимума или больше максимума - красный
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }
            }

            // Если значение не число или null - серый
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}