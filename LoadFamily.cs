using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace DEIMod
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class LoadFamilySymbol : IExternalCommand
    {
        //I've tried using this as a command in the custom ribbon panel but it ain't working :/ 
        //My guess is that i'm trying to load families with all these different types and whatever, not just a single model/thing
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string filename = "C:\\ProgramData\\Autodesk\\RVT 2018\\Libraries\\US Imperial\\Electrical\\MEP\\Electric Power\\Appliances\\Fan.rfa";
            //filename = Path.GetFullPath(filename);
            string symName = "Fan";

            if (!File.Exists(filename))
            {
                message = "Cannot find file: " + filename;
                return Result.Failed;
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(doc);

            try
            {
                TaskDialog.Show("Load Family", "Loading in Electrical Fixtures from "+filename);
                trans.Start("LoadFamily");
                bool loadSuccess = doc.LoadFamilySymbol(filename, symName);
                trans.Commit();

                if (loadSuccess)
                {
                    message = "Family " + symName + " successfully loaded (?)";
                    return Result.Succeeded;
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
                trans.RollBack();
            }

            message = "Error loading in family";
            return Result.Failed;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class LoadFamily : IExternalCommand   
    {
        private string _path;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            Document doc = app.ActiveUIDocument.Document;
            _path = null;

            //hardcoded family path:
            string fileName = "fan.rfa";
            string filePath = "C:\\ProgramData\\Autodesk\\RVT 2018\\Libraries\\US Imperial\\Electrical\\MEP\\Electric Power\\Appliances";

            string fullPath = filePath + "\\" + fileName;

            Transaction trans = new Transaction(doc);

            try
            {
                trans.Start("Load");
                bool loadSuccess = doc.LoadFamily(fullPath);
                trans.Commit();
                if (loadSuccess)
                {
                    TaskDialog.Show("Load Family", "Fan.rfa loaded successfully");
                    return Result.Succeeded;
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            message = "Failed to load family at " + fullPath;
            return Result.Failed;
        }
    }
}
