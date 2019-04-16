using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xaml;
using System.Diagnostics;
using System.IO; //for reading folders
using System.Windows.Media.Imaging; //for bitmap images
using System.Windows;
using System.Web.UI;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace DEIMod
{
    class CustomPane : IExternalApplication
    {
        string _path;
        public Result OnStartup(UIControlledApplication app)
        {

            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _path = Path.Combine(dir, "DEIMod.dll");

            if (!File.Exists(_path))
            {
                TaskDialog.Show("UIRibbon", "External command assembly not found: " + _path);
                return Result.Failed;
            }

            AddCustomPane(app); //no idea if this will even work :/
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        //This method will be called OnStartup() to (hopefully) create a custom dockable pane maybe
        public void AddCustomPane(UIControlledApplication app)
        {
            PaneProvider p = new PaneProvider(new DockablePaneProviderData());

            Guid id = new Guid("827AC140-6F44-4c03-82FE-292705581800");
            DockablePaneId g = new DockablePaneId(id);

            app.RegisterDockablePane(g, "Custom Pane", p);
            DockablePane myPain = new DockablePane(g);

            myPain.Show();
        }
    }

    public partial class PaneProvider : Page, IDockablePaneProvider
    {
        public PaneProvider(DockablePaneProviderData data)
        {
            SetupDockablePane(data);
        }
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            
            data.FrameworkElement = new FrameworkElement();
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Tabbed;

        }
    }
}
