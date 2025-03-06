#region Namespaces
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace CivNonDev.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Second_Class : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // WPF 창 생성 및 표시
                Window window = new Window
                {
                    Title = "DH_ONE - ImportExcel",
                    Icon = new BitmapImage(new Uri("pack://application:,,,/CivNonDev;component/Resources/Excel.ico")), // 아이콘 설정
                    Content = new FirstWPF(commandData),
                    Width = 400,
                    Height = 300
                };
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}