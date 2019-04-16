using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace DEIMod
{
    [Transaction(TransactionMode.Manual)]
    class ElementFiltering : IExternalCommand
    {
        //Below is copied from DBElement.cs for keeping DB level application and document
        Application m_App;
        Document m_Doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_App = uiApp.Application;
            m_Doc = uiDoc.Document;

            //var ElectricalFixtureCollector = new FilteredElementCollector(m_Doc);  //the idea is to grab all elements of type 'family instance' under category 'electrical fixture'
            FilteredElementCollector ElectricalFixturesCollector = new FilteredElementCollector(m_Doc);
            ElectricalFixturesCollector.OfClass(typeof(FamilySymbol));
            ElectricalFixturesCollector.OfCategory(BuiltInCategory.OST_ElectricalFixtures);
            IList<Element> electricalFixtures = ElectricalFixturesCollector.ToElements();

            ShowElementList(electricalFixtures, "Electrical Fixtures:");
            return Result.Succeeded;
        }

        //Display FilteredElementCollector elements in a dialog
        public void ShowElementList(IList<Element> elements, string header)
        {
            string s = " - Class - Category - Name (or Family: Type Name) - Id - \r\n";
            foreach(Element e in elements)
            {
                s += ElementToString(e);
            }
            TaskDialog.Show(header + "(" + elements.Count.ToString() + "):", s);
        }

        //helper function for displaying an element, called in ShowElementList()
        public string ElementToString(Element e)
        {
            if(e == null)
            {
                return "none";
            }

            string name = "";
            if(e is ElementType)        //display Family: Type Name
            {
                Parameter param = e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);
                if(param != null)
                {
                    name = param.AsString();
                }
            }
            else
            {
                name = e.Name;
            }

            return e.GetType() + "; " + e.Category.Name + "; " + name + "; " + e.Id.IntegerValue.ToString() + "\r\n";

        }
    }
}
