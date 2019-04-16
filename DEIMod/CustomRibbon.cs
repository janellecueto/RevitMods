using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Xaml;
using System.Diagnostics;
using System.IO; //for reading folders
using System.Windows.Media.Imaging; //for bitmap images

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace DEIMod
{
    
    class CustomRibbon : IExternalApplication
    {
        string _path;       //full path where this project is located 

        public Result OnStartup(UIControlledApplication app)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _path = Path.Combine(dir, "DEIMod.dll");

            if (!File.Exists(_path))
            {
                TaskDialog.Show("UIRibbon", "External command assembly not found: " + _path);
                return Result.Failed;
            }

            AddCustomRibbon(app);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        public void AddCustomRibbon(UIControlledApplication app)
        {
            app.CreateRibbonTab("Custom");

            RibbonPanel panel = app.CreateRibbonPanel("Custom", "Custom Commands");

            //dynamically add buttons:

            //HelloWorld button
            PushButtonData buttonDataHello = new PushButtonData("PushButtonHello", "Hello World", _path, "DEIMod.HelloWorld");
            PushButton buttonHello = panel.AddItem(buttonDataHello) as PushButton;
            buttonHello.ToolTip = "Displays 'Hello World!' in a dialog box";

            //DBElement button
            PushButtonData buttonDataDB = new PushButtonData("PushButtonDB", "DB Element", _path, "DEIMod.DBElement");
            PushButton buttonDB = panel.AddItem(buttonDataDB) as PushButton;
            buttonDB.ToolTip = "Displays basic info of a selected element";

            //ElementFiltering button
            PushButtonData buttonDataFilter = new PushButtonData("PushButtonFilter", "Element Filtering", _path, "DEIMod.ElementFiltering");
            PushButton buttonFilter = panel.AddItem(buttonDataFilter) as PushButton;
            buttonFilter.ToolTip = "Lists elements of the Electrical Fixtures Category";

            //PlaceGroup button
            PushButtonData buttonDataPlace = new PushButtonData("PushButtonPlace", "Place Group", _path, "DEIMod.PlaceGroup");
            PushButton buttonPlace = panel.AddItem(buttonDataPlace) as PushButton;
            buttonPlace.ToolTip = "Allows user to copy and place a group";

            //LoadFamily button
            PushButtonData buttonDataLoad = new PushButtonData("PushButtonLoad", "Load Family", _path, "DEIMod.LoadFamily");
            PushButton buttonLoad = panel.AddItem(buttonDataLoad) as PushButton;
            buttonLoad.ToolTip = "Loads the 'Balanced Power Connector' family from the US Imperial MEP Electrical library";
        }
    }

    //BuiltInCategory.
    //OST_ElectricalFixtures
    //OST_CommunicationDevices
    //OST_DataDevices
    //OST_FireAlarmDevices
    //OST_LightingDevices
    //OST_NurseCallDevices
    //OST_SecurityDevices
    //OST_TelephoneDevices ????

    //public Element getElement(BuiltInCategory cat)
    //{
    //   // FilteredElementCollector collector = new Filtered
    //}
}
