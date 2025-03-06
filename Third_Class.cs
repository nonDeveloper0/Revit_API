#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CivNonDev.Util;
#endregion

namespace CivNonDev.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Third_Class : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;     //현재 열린 프로젝트 파일

            UnitConverter.ConvertFeetToMeter(10);

            try
            {
                // 예제 좌표와 패밀리 유형
                XYZ point1 = new XYZ(-2000, 0, 0);
                XYZ point2 = new XYZ(-1000, 0, 0);
                XYZ point3 = new XYZ(200, 0, 0);
                XYZ point4 = new XYZ(1000, 0, 0);
                XYZ point5 = new XYZ(2000, 0, 0);
                string famPath = @"D:\기술개발연구원\교육\교육자료\24년도 철도부 BIM 실무교육\241127_터널접속부 모델링\#241205_원형to타원_3심원터널\가변_3심원터널_v2412091342.rfa";

                // 패밀리 로드 (패밀리 유형 = FamilySymbol
                FamilySymbol familySymbol = LoadFamilySymbol(doc, famPath);

                // 가변 패밀리 생성
                CreateAdaptiveComponent(doc, familySymbol, point1, point2, point3);
                CreateAdaptiveComponent(doc, familySymbol, point3, point4, point5);

                TaskDialog.Show("Success", "Adaptive Components added successfully.");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Result.Failed;
            }
        }
        private FamilySymbol LoadFamilySymbol(Document doc, string familyPath)
        {
            Family family = null;
            FamilySymbol familySymbol = null;

            // 패밀리가 이미 로드되어 있는지 확인
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Family));
            foreach (Family fam in collector)
            {
                if (fam.Name == System.IO.Path.GetFileNameWithoutExtension(familyPath))
                {
                    family = fam;
                    break;
                }
            }

            // 패밀리가 로드되어 있지 않으면 로드
            if (family == null)
            {
                using (Transaction tx = new Transaction(doc, "Load Family"))
                {
                    tx.Start();
                    if (!doc.LoadFamily(familyPath, out family))
                    {
                        throw new InvalidOperationException("Failed to load family.");
                    }
                    tx.Commit();
                }
            }

            // FamilySymbol 가져오기
            foreach (ElementId id in family.GetFamilySymbolIds())
            {
                familySymbol = doc.GetElement(id) as FamilySymbol;
                break;
            }

            if (familySymbol == null)
            {
                throw new InvalidOperationException("Failed to get family symbol.");
            }

            // FamilySymbol 활성화
            using (Transaction tx = new Transaction(doc, "Activate Family Symbol"))
            {
                tx.Start();
                if (!familySymbol.IsActive)
                {
                    familySymbol.Activate();
                }
                tx.Commit();
            }

            return familySymbol;
        }

        public void CreateAdaptiveComponent(Document doc, FamilySymbol familySymbol, XYZ point1, XYZ point2, XYZ point3)
        {
            try
            {
                using (Transaction tx = new Transaction(doc, "Place Adaptive Component"))
                {
                    tx.Start();

                    // Adaptive Component 인스턴스 생성
                    FamilyInstance famInst = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(doc, familySymbol);
                    IList<ElementId> placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(famInst);

                    // 가변점 좌표 설정
                    if (placePointIds.Count >= 3)
                    {
                        ReferencePoint refPoint1 = doc.GetElement(placePointIds[0]) as ReferencePoint;
                        ReferencePoint refPoint2 = doc.GetElement(placePointIds[1]) as ReferencePoint;
                        ReferencePoint refPoint3 = doc.GetElement(placePointIds[2]) as ReferencePoint;

                        refPoint1.Position = point1;
                        refPoint2.Position = point2;
                        refPoint3.Position = point3;
                    }

                    // 인스턴스 매개변수 설정 필요 (선형 벡터 추출해서 각도 rot_XY_A~C 지정)
                    SetInstanceParameter(famInst, "rot_XY_A", 10); // 예제: 10도 설정

                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void SetInstanceParameter(FamilyInstance famInst, string paramName, double angleInDegrees)
        {
            // 각도를 라디안으로 변환
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);

            // 파라미터 설정
            Parameter param = famInst.LookupParameter(paramName);
            if (param != null && !param.IsReadOnly)
            {
                param.Set(angleInRadians);
            }
        }
    }
}