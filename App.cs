#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

//마법사 자동생성 아닌 직접 추가부분
using System.Diagnostics; //Debug 목적. Debug.WriteLine(ex.Message)
using System.Linq; //LINQ 사용. WHERE 문 사용할 때 필요.
using System.Reflection; //리플렉션 사용?
using System.Windows.Media.Imaging; //이미지 사용
using System.IO; //파일 입출력 사용 Startup에서 Uri 구현할때 사용
#endregion

namespace CivNonDev
{
    public class App : IExternalApplication
    {
        //필드
       public const string tabName = "DH_ONE"; // 고정된 탭 이름

        public Result OnShutdown(UIControlledApplication application)   //메서드1
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)    //메서드2
        {
            try
            {
                List<RibbonPanel> ribbonPanels = CreateTabAndRibbonPanel(application);   //CreateTabAndRibbonPanel 메서드를 이용하여 리본패널 생성
                string thisAssemblyPath = Assembly.GetExecutingAssembly().Location; //현재 실행중인 어셈블리(프로젝트 파일 .dll 또는 .exe)파일 경로를 가져오는 코드  //Systsem.Reflection.Assembly
                                                                                    // => C:\Users\admin\AppData\Roaming\Autodesk\Revit\Addins\2024 
                
                // 버튼 추가
                // 첫 번째 패널에 첫번째 버튼 추가
                AddPushButton(application, "CivNonDev_Panel", "Button1-1", "Tunnel\nAutomation", thisAssemblyPath, "CivNonDev.Commands.First_Class", "툴팁1", "Tunnel.ico");
                // 첫 번째 패널에 구분선 추가
                AddSeparator(application, "CivNonDev_Panel");
                // 첫 번째 패널에 두번째 버튼 추가
                AddPushButton(application, "CivNonDev_Panel", "Button1-2", "ImportExcel", thisAssemblyPath, "CivNonDev.Commands.Second_Class", "툴팁2", "Excel.ico");
                // 두 번째 패널에 첫번째 버튼 추가
                AddPushButton(application, "SecondPanel", "Button2-1", "ThirdCommand", thisAssemblyPath, "CivNonDev.Commands.Third_Class", "툴팁3", "three.ico");
                // 세번째 패널(TestPanel)에 첫번째 버튼 추가
                AddPushButton(application, "TestPanel", "Button3-1", "TestButton", thisAssemblyPath, "CivNonDev.Commands.Test_Class", "툴팁4", "test.ico");
                AddPushButton(application, "TestPanel", "Button3-2", "ConnectSurface", thisAssemblyPath, "CivNonDev.CreateSurface_class", "툴팁5", "surface.ico");

                /* 간소화
                //버튼Data 생성 (ver0.0)
                PushButtonData pushButtonData = new PushButtonData("Button1", "TunnelAutomation", thisAssemblyPath, "CivNonDev.First_Class");
                //패널에 버튼 추가
                PushButton button1 = ribbonPanels[0].AddItem(pushButtonData) as PushButton;

                Uri imagePath = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Tunnel.ico"));
                BitmapImage bitmap = new BitmapImage(imagePath);
                button1.LargeImage = bitmap;
                button1.ToolTip = "툴팁1";

                //버튼Data 생성
                PushButtonData pushButtonData2 = new PushButtonData("Button2", "ImportExcel", thisAssemblyPath, "CivNonDev.Second_Class");
                //패널에 버튼 추가
                PushButton button2 = ribbonPanels[0].AddItem(pushButtonData2) as PushButton;

                Uri imagePath2 = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Excel.ico"));
                BitmapImage bitmap2 = new BitmapImage(imagePath2);
                button2.LargeImage = bitmap2;
                button2.ToolTip = "툴팁2";

                //버튼Data 생성
                PushButtonData pushButtonData3 = new PushButtonData("Button3", "ThirdCommnad", thisAssemblyPath, "CivNonDev.Third_Class");
                //패널에 버튼 추가
                PushButton button3 = ribbonPanels[1].AddItem(pushButtonData3) as PushButton;

                Uri imagePath3 = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "three.ico"));
                BitmapImage bitmap3 = new BitmapImage(imagePath3);
                button3.LargeImage = bitmap3;
                button3.ToolTip = "툴팁3";
                */

                /*
                if (panel.AddItem(new PushButtonData("FirstButton", "TunnelAutomation", thisAssemblyPath, "CivNonDev.First_Class")) is PushButton button)  //CivNonDev.dll 파일의 Fisrt_Class 클래스를 실행하는 버튼 생성
                {
                    button.ToolTip = "CivNonDev_tooltip";    //버튼에 마우스를 올렸을 때 나오는 툴팁 설정
                    Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Tunnel.ico"));    //경로지정
                    BitmapImage bitmap = new BitmapImage(uri);          //uri 경로의 이미지를 불러오는 코드
                    button.LargeImage = bitmap;                         //리본 버튼의 이미지 설정
                }
                */
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Result.Succeeded;
        }

        //리본 탭 및 패널 생성 메서드
        public List<RibbonPanel> CreateTabAndRibbonPanel(UIControlledApplication a)    //3. CreateRibbonPanel 메서드 // Autodesk.Revit.UI.RibbonPanel
        {
            List<RibbonPanel> ribbonPanels = new List<RibbonPanel>();    //리본패널 리스트 타입의 ribbonPanels 변수명 생성 (인스턴스 생성)
            
            // 1. 리본 탭 생성
            try
            {
                a.CreateRibbonTab(tabName);                             //Autodesk.Revit.UI.UIControlledApplication.CreateRibbonTab
            }
            catch (Exception ex)
            {
            // 이미 같은 이름을 가진 탭이 존재할 경우
                Debug.WriteLine(ex.Message);
            }

            // 2. 리본 패널 생성
            try
            {
                ribbonPanels.Add(a.CreateRibbonPanel(tabName, "CivNonDev_Panel"));    //생성한 탭 안에 패널 생성
                ribbonPanels.Add(a.CreateRibbonPanel(tabName, "SecondPanel"));        //생성한 탭 안에 패널 생성
                ribbonPanels.Add(a.CreateRibbonPanel(tabName, "TestPanel"));        //생성한 탭 안에 패널 생성
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //지정한 탭에 있는 패널들을 리스트화 후 panel 리스트에 대입
            /*
            List<RibbonPanel> panels = a.GetRibbonPanels(tabName).ToList();    //Autodesk.Revit.UI.UIControlledApplication.GetRibbonPanels
            foreach (RibbonPanel p in panels.Where(p => p.Name == "CivNonDev_Panel"))   //패널의 이름이 CivNonDev_Paenl인 패널 p를 ribbonPanel에 대입하고 이를 반환
            {
                ribbonPanel1 = p;
            }
            */
            //굳이 필요한가? ↑ Jacobian Dev 영상에선 ribbon Panel을 반환하기 위해서 썼지만 이미 RibbonPanel 리스트를 반환하므로 필요 X

            return ribbonPanels;
        }

        // 버튼 추가 메서드
        private void AddPushButton(UIControlledApplication application, string panelName, string buttonName, string buttonText, string assemblyPath, string className, string toolTip, string iconName)
        {
            try
            {
                RibbonPanel panel = application.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == panelName);
                if (panel != null)
                {
                    PushButtonData buttonData = new PushButtonData(buttonName, buttonText, assemblyPath, className);
                    PushButton button = panel.AddItem(buttonData) as PushButton;
                    if (button != null)
                    {
                        Uri imagePath = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Resources", iconName));
                        BitmapImage bitmap = new BitmapImage(imagePath);
                        button.LargeImage = bitmap;
                        button.ToolTip = toolTip;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // 구분선 추가 메서드
        private void AddSeparator(UIControlledApplication application, string panelName)
        {
            try
            {
                RibbonPanel panel = application.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == panelName);
                if (panel != null)
                {
                    panel.AddSeparator();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding separator: {ex.Message}");
            }
        }
    }
}
