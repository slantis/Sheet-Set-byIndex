using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetByIndex
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ThisCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                // Get user selection to see if it's a single sheet schedule
                ICollection<ElementId> elementIds = uidoc.Selection.GetElementIds();
                ElementId elementId = elementIds.FirstOrDefault();

                if (elementIds.Count() == 1)
                {
                    Element element = doc.GetElement(elementId);

                    ViewSchedule viewSched = null;

                    // If element is a placed schedule
                    if (element is ScheduleSheetInstance)
                    {
                        ScheduleSheetInstance SchedInst = element as ScheduleSheetInstance;

                        viewSched = doc.GetElement(
                            SchedInst.ScheduleId) as ViewSchedule;
                    }

                    // If element is an unplaced schedule (from the view tree)
                    else if (element is ViewSchedule)
                    {
                        viewSched = element as ViewSchedule;
                    }


                    if (viewSched != null)
                    {
                        MainWindow MainWn = new MainWindow(uidoc, viewSched);
                        MainWn.ShowDialog();
                        return Result.Succeeded;

                    }

                    // Selection is not valid
                    else
                    {
                        Utils.SimpleDialog("Select a schedule with sheets in it", "");
                        return Result.Cancelled;
                    }
                }

                // Nothing selected
                else
                {
                    Utils.SimpleDialog("Select a schedule with sheets in it", "");
                    return Result.Cancelled;
                }

            }
            catch (Exception ex)
            {
                Utils.CatchDialog(ex);
                return Result.Failed;
            }
        }
    }
}
