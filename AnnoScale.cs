using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;

namespace DEIMod
{
    [Transaction(TransactionMode.Manual)]
    class AnnoScale: IExternalCommand
    {
        Application m_App;
        Document m_Doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_App = uiApp.Application;
            m_Doc = uiDoc.Document;

            return Result.Succeeded;
        }

    }
}
