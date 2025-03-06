#region
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Media.Converters;
using Autodesk.Revit.DB;
#endregion

namespace CivNonDev
{
    public static class Utility
    {
        private const double FeetToMeterFactor = 0.3048;
        private const double MeterToFeetFactor = 1/0.3048;
        private const double DegreeToRadianFactor = Math.PI / 180.0;
        private const double RadianToDegreeFactor = 180.0 / Math.PI;

        // 미터를 피트로 변환
        public static XYZ ConvertToFeet(XYZ point)
        {
            return new XYZ(point.X * MeterToFeetFactor, point.Y * MeterToFeetFactor, point.Z * MeterToFeetFactor);
        }

        // 피트를 미터로 변환
        public static XYZ ConvertToMeters(XYZ point)
        {
            return new XYZ(point.X * FeetToMeterFactor, point.Y * FeetToMeterFactor, point.Z * FeetToMeterFactor);
        }

        // 도를 라디안으로 변환
        public static double ConvertToRadians(double angleInDegrees)
        {
            return angleInDegrees * DegreeToRadianFactor;
        }

        // 라디안을 도로 변환
        public static double ConvertToDegrees(double angleInRadians)
        {
            return angleInRadians * RadianToDegreeFactor;
        }

        // 내부원점 기준의 좌표를 REVIT 좌표로 변환
        public static List<XYZ> ConvertOriginToRevit(List<XYZ> PTs, Document doc)
        {
            // 프로젝트 기준점 요소 가져오기
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ProjectBasePoint)
                .OfClass(typeof(BasePoint));

            BasePoint basePoint = collector.FirstElement() as BasePoint;
            
            // 프로젝트 기준점이 null일 경우는 없겠지만 파일 손상이나 오류로 null이 나올수도 있으므로 예외처리
            if (basePoint == null)
            {
                throw new Exception("Project Base Point not found.");
            }

            // 프로젝트 기준점의 위치와 회전각도 가져오기
            double basePoint_EW = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_EASTWEST_PARAM).AsDouble();   // SharedPosition은 (E/W, N/S, Elevation) 순서. 단위 피트
            double basePoint_NS = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_NORTHSOUTH_PARAM).AsDouble();
            double basePointAngle = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_ANGLETON_PARAM).AsDouble();  // Angle To True North (radian)

            XYZ originToBasePoint = basePoint.Position;     // 원점으로부터 프로젝트 기준점의 상대적 위치

            List<XYZ> transformedPoints = new List<XYZ>();  //using System.Collections.Generic; 리스트 사용하려면 필요

            // 회전 변환 생성
            Transform rotTrans = Transform.CreateRotationAtPoint(XYZ.BasisZ, basePointAngle, originToBasePoint);
            XYZ transformedPoint = rotTrans.OfPoint(originToBasePoint);

            return transformedPoints;
        }
    }

    public class UtilTest : object 
    {
    }
}