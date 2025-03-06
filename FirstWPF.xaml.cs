using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using Autodesk.Revit.DB;
using TextBox = System.Windows.Controls.TextBox;

namespace CivNonDev
{
    /// <summary>
    /// Interaction logic for FirstWPF.xaml
    /// </summary>
    public partial class FirstWPF : UserControl
    {
        private ExternalCommandData _commandData;
        public FirstWPF(ExternalCommandData commandData)
        {
            InitializeComponent();
            _commandData = commandData;
            LoadAdaptiveFamilies();
        }

        private void LoadAdaptiveFamilies()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;
            List<string> adaptiveFamilies = GetAdaptiveFamilies(doc);
            AdaptiveFamilyComboBox.ItemsSource = adaptiveFamilies;
        }
        private List<string> GetAdaptiveFamilies(Document doc)
        {
            List<string> adaptiveFamilies = new List<string>();
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Family));
            foreach (Family family in collector)
            {
                if (IsAdaptiveFamily(family))
                {
                    adaptiveFamilies.Add(family.Name);
                }
            }
            return adaptiveFamilies;
        }
        private bool IsAdaptiveFamily(Family family)
        {
            foreach (ElementId id in family.GetFamilySymbolIds())
            {
                FamilySymbol symbol = family.Document.GetElement(id) as FamilySymbol;
                if (symbol != null && symbol.Family.FamilyCategory.Id.Value == (int)BuiltInCategory.OST_GenericModel)
                {
                    return true;
                }
            }
            return false;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedFamily = AdaptiveFamilyComboBox.SelectedItem as string;
            MessageBox.Show($"Selected Adaptive Family: {selectedFamily}");
            // 선택된 패밀리를 활용하는 로직을 여기에 추가합니다.
        }
    
        private void OpenExcelFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog               // Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Title = "Select an Excel File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // Excel 파일을 처리하는 코드를 여기에 추가합니다.

                //0. 파일 경로 추출
                MessageBox.Show($"Selected Excel File: {filePath}");        //System.Windows.MessageBox
            }
        }
                private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string newText = textBox.Text;
            MessageBox.Show($"Text changed: {newText}");
            // 텍스트 변경 시 수행할 로직을 여기에 추가합니다.
        }
    }
}