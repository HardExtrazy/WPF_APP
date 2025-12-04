using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPF_APP
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application 
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ждем полной инициализации приложения
            Dispatcher.InvokeAsync(() =>
            {
                var paletteHelper = new PaletteHelper();
                Theme Theme = paletteHelper.GetTheme();                
                // ПЕРЕОПРЕДЕЛЯЕМ Primary (перезаписывает DeepPurple из XAML)
                Theme.SetPrimaryColor(Color.FromRgb(103, 86, 138)); // #67568a
                Theme.SetSecondaryColor(Color.FromRgb(98, 91, 110)); // #625b6e
                // Применяем
                paletteHelper.SetTheme(Theme);
                // Проверяем
                Debug.WriteLine($"Primary цвет: {Theme.PrimaryMid.Color}");
            }, DispatcherPriority.Loaded);
        }
    }
}
