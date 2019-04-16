using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace DEIMod
{
    [Transaction(TransactionMode.Manual)]
    class PlaceGroup : IExternalCommand
    {
        Application m_App;
        Document m_Doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            m_App = uiApp.Application;
            m_Doc = uiDoc.Document;

            try
            {
                //Define a Reference object to accept the picked result
                Reference pickedRef = null;

                //pick a group using the group filter 
                Selection sel = uiDoc.Selection;
                GroupPickFilter selFilter = new GroupPickFilter();

                pickedRef = sel.PickObject(ObjectType.Element, selFilter, "Please select a group");

                Element e = m_Doc.GetElement(pickedRef);

                Group group = e as Group;

                //Pick a point
                XYZ point = sel.PickPoint("Please pick a point");

                //Place the group
                Transaction trans = new Transaction(m_Doc);
                trans.Start("Lab");
                m_Doc.Create.PlaceGroup(point, group.GroupType);
                trans.Commit();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }

    public class GroupPickFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_IOSModelGroups));
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return false;
        }
    }

}

