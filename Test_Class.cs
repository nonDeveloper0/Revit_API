using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CivNonDev;
using System.Net.Configuration;

namespace CivNonDev.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Test_Class : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // 필터링할 카테고리 목록을 정의
                var categories = new List<BuiltInCategory>
                {
                    BuiltInCategory.OST_Walls,
                    BuiltInCategory.OST_Doors,
                    BuiltInCategory.OST_Windows,
                };

                // MultiCategorySelectionFilter 인스턴스 생성
                MultiCategorySelectionFilter multiCategoryFilter = new MultiCategorySelectionFilter(categories);
                ICollection<Element> selectedElems = uidoc.Selection.PickElementsByRectangle(multiCategoryFilter, "요소를 선택하세요");

                // 선택된 요소의 ID를 수집
                List<ElementId> selectedElementIds = selectedElems.Select(e => e.Id).ToList();

                // 선택된 상태로 유지
                uidoc.Selection.SetElementIds(selectedElementIds);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }


    public class MultiCategorySelectionFilter : ISelectionFilter
    {
        private readonly HashSet<BuiltInCategory> _categories;

        public MultiCategorySelectionFilter(IEnumerable<BuiltInCategory> categories)
        {
            _categories = new HashSet<BuiltInCategory>(categories);
        }

        public bool AllowElement(Element elem)
        {
            return elem.Category != null && _categories.Contains((BuiltInCategory)elem.Category.Id.Value);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
