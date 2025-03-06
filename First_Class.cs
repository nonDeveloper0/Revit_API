#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Security.Cryptography.X509Certificates;

// 새로 추가한 부분
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Configuration.Assemblies;
using System.IO;
using CivNonDev.Util;
#endregion

namespace CivNonDev.Commands
{
    [Transaction (TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]                   //Autodesk.Revit.Attributes.RegenerationAttribute

    public class First_Class : IExternalCommand                 //Autodesk.Revit.UI.IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)                                //Autodesk.Revit.DB.ElementSet //Autodesk.Revit.UI.ExternalCommandData
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;       //Autodesk.Revit.UI.UIApplication
            Document doc = uiapp.ActiveUIDocument.Document;      //Autodesk.Revit.DB.Document

            double a = UnitConverter.ConvertFeetToMeter(10);     // test

            try
            {
                //Define a reference Object to accept the pick result
                Reference pickedref = null;                         //Autodesk.Revit.DB.Reference

                //Pick a group
                Selection sel = uiapp.ActiveUIDocument.Selection;   //Autodesk.Revit.UI.Selection.Selection

                //기존 코드
                //pickedref = sel.PickObject(ObjectType.Element, "Please select a group");

                //변경코드 (Lesson5)
                GroupPickFilter selFilter = new GroupPickFilter();
                pickedref = sel.PickObject(ObjectType.Element, selFilter, "Please select a group");

                Element elem = doc.GetElement(pickedref);           //Autodesk.Revit.DB.Element
                Group group = elem as Group;                        //Autodesk.Revit.DB.Group
                if (group == null)
                {
                    message = "Unable to obtain a group";
                    return Result.Failed;
                }
                //Get the group's center point
                XYZ origin = GetElementCenter(group);

                /*
                //Pick point
                XYZ point = sel.PickPoint("Please pick a point to place group"); //Autodesk.Revit.DB.XYZ
                */

                //Get the room that the picked group is located in
                Room room = GetRoomOfGroup(doc, origin);

                if (room == null)
                {
                    message = "Could not find a room for the indiscated group";
                    return Result.Failed;
                }

                XYZ sourceCenter = GetRoomCenter(room);
                string coords = 
                    "X = " + sourceCenter.X.ToString() + "\r\n" + 
                    "Y = " + sourceCenter.Y.ToString() + "\r\n" + 
                    "Z = " + sourceCenter.Z.ToString();

                TaskDialog.Show("Source room Center", coords);

                //Place the group
                Transaction trans = new Transaction(doc);           //Autodesk.Revit.DB.Transaction
                trans.Start("Lab");                                 //Lab은 트랜잭션 이름
                XYZ groupLocation = sourceCenter + new XYZ(20, 0, 0); //Calculate the new group's position
                //doc.Create.PlaceGroup(point, group.GroupType);
                doc.Create.PlaceGroup(groupLocation, group.GroupType);
                trans.Commit();

                return Result.Succeeded;
            }

            //If the user right-clicks or presses ESC, handle the exception
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
       
        public XYZ GetElementCenter(Element elem)           //입력된 elem의 BoudingBox 중심점을 추출
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);   //Element.get_BoundingBox(View) -> View 자리에 null들어가면 모델 지오메트리의 경계 상자를 반환
            XYZ center = (bounding.Max + bounding.Min) / 2;         //BoundingBox의 Max는 앞-오른쪽-위, Min은 뒤-왼쪽-아래
            return center;
        }

        public XYZ GetRoomCenter(Room room)
        {
            XYZ boundCenter = GetElementCenter(room);              //입력된 room의 BoundingBox의 중심점을 boundCenter 변수에 대입
            LocationPoint locPt = (LocationPoint)room.Location;     //locPt는 입력된 room이 위치한 점
            XYZ roomCenter = new XYZ(boundCenter.X, boundCenter.Y, locPt.Point.Z);
            return roomCenter;
        }
        Room GetRoomOfGroup(Document doc, XYZ point)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);     //Autodesk.Revit.DB.FilteredElementCollector
            collector.OfCategory(BuiltInCategory.OST_Rooms).ToElements();               //해당 문서의 room을 필터링 후 element로 형 변환
                Room room = null;   //Room형의 room 변수를 null로 지정
                    foreach(Element elem in collector)          //해당 문서의 모든 room을 foreach문으로 순환
                    {
                        room = elem as Room;    //elem 요소를 Room형으로 형 변환
                            if (room != null)   //Room으로 변환되어 null값이 아니면 다음 단계 진행
                            {
                                if (room.IsPointInRoom(point))      //입력된 점 point가 room 안에 있는지 확인 bool값 return
                                {
                                    break;
                                }   
                            }
                    }
            return room;
        }
    }
    public class GroupPickFilter : ISelectionFilter     //Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e.Category.Id.Value.Equals((int)BuiltInCategory.OST_IOSModelGroups);
        }
        public bool AllowReference(Reference r, XYZ p)
        {
            return false;
        }
    }
}