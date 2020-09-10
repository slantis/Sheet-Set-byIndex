using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Reflection;
using System.IO;

namespace SetByIndex
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("00A4C164-52CE-437F-8E11-2C6FA63A0334")]
    public class ThisApplication : IExternalApplication
    {
        RibbonPanel DefaultPanel;

        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication uiApp)
        {
            try
            {
                string ThisDllPath = Assembly.GetExecutingAssembly().Location;

                String exeConfigPath = Path.GetDirectoryName(ThisDllPath) + "\\SetByIndex.dll";

                string RibbonTabName = "ASG";
                string PanelName = "Set by index";

                // Create ribbon and the panel
                try
                {
                    uiApp.CreateRibbonTab(RibbonTabName);
                }

                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    // Do nothing, tab already exists
                }

                try
                {
                    DefaultPanel = uiApp.CreateRibbonPanel(RibbonTabName, PanelName);
                }

                // Panel already exists
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    List<RibbonPanel> Panels = uiApp.GetRibbonPanels(RibbonTabName);
                    DefaultPanel = Panels.Find(p => p.Name.Equals(PanelName));
                }


                // Button configuration
                string SetByIndexName = "Set by index";
                PushButtonData SetByIndexData = new PushButtonData(SetByIndexName, SetByIndexName, exeConfigPath, "SetByIndex.ThisCommand");
                SetByIndexData.LargeImage = Utils.RetriveImage("SetByIndex.Resources.Sched.ico"); // Pushbutton image
                SetByIndexData.ToolTip = "Create printing sheet sets from sheet schedules";
                RibbonItem CadDetectiveButton = DefaultPanel.AddItem(SetByIndexData); // Add pushbutton

                return Result.Succeeded;
            }


            catch (Exception ex)
            {
                Utils.CatchDialog(ex);
                return Result.Failed;
            }
        }
    }
}
