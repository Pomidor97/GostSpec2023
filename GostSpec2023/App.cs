using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using GostSpec2023.Commands;
using GostSpec2023.Services;

namespace GostSpec2023
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            ConfigureServices();

            string tabName = "KAZGOR";
            try { a.CreateRibbonTab(tabName); } catch { }

            var panel = a.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == "Оформление")
                        ?? a.CreateRibbonPanel(tabName, "Оформление");

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            // Создание кнопки "Копировать параметры"
            PushButtonData copyParamsBtnData = new PushButtonData("copyParamsBtn",
                "Копировать\nпараметры",
                assemblyPath,
                typeof(CopyParametersCommand).FullName);
            // Устанавливаем иконку
            copyParamsBtnData.LargeImage = GetPngImage("GostSpec2023.Resources.icons8_copy_32.png");

            // Если нужно, создайте другие кнопки аналогичным способом:
            PushButtonData numberingBtnData = new PushButtonData("numberingBtn",
                "Автонумерация\nпозиций",
                assemblyPath,
                typeof(Commands.NumberingCommand).FullName);
            numberingBtnData.LargeImage = GetPngImage("GostSpec2023.Resources.icons8_counter_32.png");

            PushButtonData autoScheduleBtnData = new PushButtonData("autoScheduleBtn",
                "Авто\nспецификация",
                assemblyPath,
                typeof(Commands.AutoScheduleCommand).FullName);
            autoScheduleBtnData.LargeImage = GetPngImage("GostSpec2023.Resources.icons8_schedule_32.png");

            panel.AddItem(copyParamsBtnData);
            panel.AddItem(numberingBtnData);
            panel.AddItem(autoScheduleBtnData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        private void ConfigureServices()
        {
            ServiceLocator.Register<IParameterCopyService, ParameterCopyService>();
            // Зарегистрируйте остальные сервисы, если они есть
        }

        /// <summary>
        /// Метод для загрузки PNG-изображения из встроенных ресурсов.
        /// </summary>
        /// <param name="resourceName">Полное имя ресурса (например, "GOSTSpec.Resources.icons8_copy_32.png")</param>
        /// <returns>BitmapSource с изображением</returns>
        private BitmapSource GetPngImage(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return null;
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
    }
}
