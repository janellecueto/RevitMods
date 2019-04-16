using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xaml;
using System.Diagnostics;
using System.IO; //for reading folders
using System.Windows.Media.Imaging; //for bitmap images
using System.Drawing;
using System.Windows.Media;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace DEIMod
{
    class DEIRibbon : IExternalApplication
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
            app.CreateRibbonTab("DEI");

            RibbonPanel panel = app.CreateRibbonPanel("DEI", "Custom Commands");
            RibbonPanel devices = app.CreateRibbonPanel("DEI", "Systems Devices");

            ////DBElement button
            //PushButtonData buttonDataDB = new PushButtonData("PushButtonDB", "DB Element", _path, "DEIMod.DBElement");
            //PushButton buttonDB = panel.AddItem(buttonDataDB) as PushButton;
            //buttonDB.ToolTip = "Displays basic info of a selected element";

            //ElementFiltering button
            PushButtonData buttonDataFilter = new PushButtonData("PushButtonFilter", "Element Filtering", _path, "DEIMod.ElementFiltering");
            PushButton buttonFilter = panel.AddItem(buttonDataFilter) as PushButton;
            buttonFilter.ToolTip = "Lists elements of the Electrical Fixtures Category";

            //BuiltInCategory.
            //OST_ElectricalFixtures
            //OST_CommunicationDevices
            //OST_DataDevices
            //OST_FireAlarmDevices
            //OST_LightingDevices
            //OST_NurseCallDevices
            //OST_SecurityDevices
            //OST_TelephoneDevices

            //ElectricalFixtures Buttons 
            PushButtonData efData = new PushButtonData("efData", "Electrical Fixtures", _path, "DEIMod.ElectricalDevices");
            PushButton efButton = devices.AddItem(efData) as PushButton;
            efButton.ToolTip = "Places electrical devices such as receptacles, junction boxes, and other power devices. ";
            efButton.LargeImage = toBitmapImage(Properties.Resources.Utility_9920);

            //CommunicationDevices
            PushButtonData comData = new PushButtonData("comData", "Communication Devices", _path, "DEIMod.Communication");
            PushButton comButton = devices.AddItem(comData) as PushButton;
            comButton.ToolTip = "Places communication devices such as intercom system components.";
            comButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13312);

            //DataDevices
            PushButtonData dataData = new PushButtonData("dataData", "Data Devices", _path, "DEIMod.DataDevices");
            PushButton dataButton = devices.AddItem(dataData) as PushButton;
            dataButton.ToolTip = "Places data devices such as ethernet and other network connections.";
            dataButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13314);

            //FireAlarmDevices
            PushButtonData fireData = new PushButtonData("fireData", "Fire Alarm Devices", _path, "DEIMod.FireAlarm");
            PushButton fireButton = devices.AddItem(fireData) as PushButton;
            fireButton.ToolTip = "Places fire alarm devices such as smoke detectors, manual pull stations, and annunciators.";
            fireButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13315);

            //LightingDevices
            PushButtonData lightData = new PushButtonData("lightData", "Lighting Devices", _path, "DEIMod.Lighting");
            PushButton lightButton = devices.AddItem(lightData) as PushButton;
            lightButton.ToolTip = "Places lighting switches such as daylight sensors, occupancy sensors, and manual switches";
            lightButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13001);

            //NurseCallDevices
            PushButtonData nurseData = new PushButtonData("nurseData", "Nurse Call Devices", _path, "DEIMod.NurseCall");
            PushButton nurseButton = devices.AddItem(nurseData) as PushButton;
            nurseButton.ToolTip = "Places nurse call devices such as call stations, code blue stations, and door lights.";
            nurseButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13316);

            //SecurityDevices
            PushButtonData secureData = new PushButtonData("secureData", "Security Devices", _path, "DEIMod.Security");
            PushButton secureButton = devices.AddItem(secureData) as PushButton;
            secureButton.ToolTip = "Places security devices such as door locks, motion sensors, and surveillance cameras.";
            secureButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13317);

            //TelephoneDevices
            PushButtonData phoneData = new PushButtonData("phoneData", "Telephone Devices", _path, "DEIMod.Telephone");
            PushButton phoneButton = devices.AddItem(phoneData) as PushButton;
            phoneButton.ToolTip = "Places a telephone jack.";
            phoneButton.LargeImage = toBitmapImage(Properties.Resources.Utility_13318);

            ComboBoxData typeSelector = new ComboBoxData("typeSelector");

            ComboBox cBox = panel.AddItem(typeSelector) as ComboBox;

            ComboBoxMemberData cboxMemDataA = new ComboBoxMemberData("A", "Option A");
            cboxMemDataA.GroupName = "Letters";
            cBox.AddItem(cboxMemDataA);

            ComboBoxMemberData cboxMemDataB = new ComboBoxMemberData("B", "Option B");
            cboxMemDataA.GroupName = "Letters";
            cBox.AddItem(cboxMemDataB);

        }

        public static BitmapImage toBitmapImage(Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            using(MemoryStream memStream = new MemoryStream())
            {
                bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                memStream.Position = 0;
                BitmapImage bImg = new BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = memStream;
                bImg.CacheOption = BitmapCacheOption.OnLoad;
                bImg.EndInit();

                return bImg;
            }
            
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ElectricalDevices : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector ElectricalFixturesCollector = new FilteredElementCollector(doc);
            ElectricalFixturesCollector.OfClass(typeof(FamilySymbol));
            ElectricalFixturesCollector.OfCategory(BuiltInCategory.OST_ElectricalFixtures);

            //NOTES: 
            //  doc.Create.FamilyInstanceCreationData(XYZ, FamilySymbol, Element, StructuralType) ?
            //  Element.ElementId ?
            //  Selection.SetElementIds(ICollection<ElementId>) ? 
            //  NewFamilyInstance(XYZ, FamilySymbol, Element, StructureType)
            
            if(ElectricalFixturesCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Electrical Fixrues family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = ElectricalFixturesCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;

        }

    }

    [Transaction(TransactionMode.Manual)]
    class Communication : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector CommunicationsCollector = new FilteredElementCollector(doc);
            CommunicationsCollector.OfClass(typeof(FamilySymbol));
            CommunicationsCollector.OfCategory(BuiltInCategory.OST_CommunicationDevices);

            if (CommunicationsCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Communication Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = CommunicationsCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class DataDevices : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector DataCollector = new FilteredElementCollector(doc);
            DataCollector.OfClass(typeof(FamilySymbol));
            DataCollector.OfCategory(BuiltInCategory.OST_DataDevices);

            if (DataCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Data Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = DataCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class FireAlarm : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector FireAlarmCollector = new FilteredElementCollector(doc);
            FireAlarmCollector.OfClass(typeof(FamilySymbol));
            FireAlarmCollector.OfCategory(BuiltInCategory.OST_FireAlarmDevices);

            if (FireAlarmCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Fire Alarm Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = FireAlarmCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class Lighting : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector LightingCollector = new FilteredElementCollector(doc);
            LightingCollector.OfClass(typeof(FamilySymbol));
            LightingCollector.OfCategory(BuiltInCategory.OST_LightingDevices);

            if (LightingCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Lighting Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = LightingCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    } 

    [Transaction(TransactionMode.Manual)]
    class NurseCall : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector NurseCallCollector = new FilteredElementCollector(doc);
            NurseCallCollector.OfClass(typeof(FamilySymbol));
            NurseCallCollector.OfCategory(BuiltInCategory.OST_NurseCallDevices);

            if (NurseCallCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Nurse Call Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = NurseCallCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class Security : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector SecurityCollector = new FilteredElementCollector(doc);
            SecurityCollector.OfClass(typeof(FamilySymbol));
            SecurityCollector.OfCategory(BuiltInCategory.OST_SecurityDevices);

            if (SecurityCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Security Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = SecurityCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class Telephone : IExternalCommand
    {
        Application app;
        Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            app = uiApp.Application;
            doc = uiDoc.Document;

            //the idea is to filter for all of the electrical fixtures, grab the first element in that list, and then place that item in the thing
            FilteredElementCollector TelephoneCollector = new FilteredElementCollector(doc);
            TelephoneCollector.OfClass(typeof(FamilySymbol));
            TelephoneCollector.OfCategory(BuiltInCategory.OST_TelephoneDevices);

            if (TelephoneCollector.FirstElement() == null)
            {
                TaskDialog.Show("Electrical Devices", "No Telephone Devices family is loaded in the project.");
                return Result.Cancelled;
            }

            ElementType elemType = TelephoneCollector.FirstElement() as ElementType;
            uiDoc.PostRequestForElementTypePlacement(elemType);

            return Result.Succeeded;
        }
    }
}
