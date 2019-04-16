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
    class DBElement :IExternalCommand
    {
        Application m_App;
        Document m_Doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_App = uiApp.Application;
            m_Doc = uiDoc.Document;

            Reference refPick = uiDoc.Selection.PickObject(ObjectType.Element, "Pick an element");
            Element elem = m_Doc.GetElement(refPick);

            showBasicInfo(elem);

            return Result.Succeeded;
        }

        //Helper function for displaying info about the selected element
        public void showBasicInfo(Element elem)
        {
            string s = "You picked: \n";
            s += "  Class name = " + elem.GetType().Name + "\n";
            s += "  Category = " + elem.Category.Name + "\n";
            s += "  Element id = " + elem.Id.ToString() + "\n\n";

            ElementId elemTypeId = elem.GetTypeId();
            ElementType elemType = (ElementType)m_Doc.GetElement(elemTypeId);
            Parameter param = elemType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM);
            Parameter type = elemType.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM);

            s += "Its Element Type: \n";
            s += "  Class name = " + elemType.GetType().Name + "\n";
            s += "  Category = " + elemType.Category.Name + "\n";
            s += "  Element type id = " + elemType.Id.ToString() + "\n";
            if (param != null) s += "  Family Symbol = " + param.AsString() + "\n";
            if (type != null) s += "  Name = " + type.AsString() + "\n";

            TaskDialog.Show("Basic Element Info", s);
        }
    }
}


