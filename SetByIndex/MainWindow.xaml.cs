using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Windows;

namespace SetByIndex
{
    public partial class MainWindow : Window
    {

        UIDocument uidoc;
        Document doc;

        private readonly FilteredElementCollector sheetList;
        private readonly ViewSchedule viewSched;
        private readonly ViewSet viewSet;
        private readonly int nRows;
        private readonly int nCols;

        public MainWindow(UIDocument _uidoc, ViewSchedule _viewSched)
        {
            InitializeComponent();

            uidoc = _uidoc;
            doc = uidoc.Document;
            viewSched = _viewSched;

            // Placeholder sheets print as blank pages, we don't want them
            sheetList = new FilteredElementCollector(doc);
            sheetList.OfClass(typeof(ViewSheet))
                        .Cast<ViewSheet>()
                        .Where<ViewSheet>(i => !i.IsPlaceholder);


            viewSet = new ViewSet();

            // Read the data from the schedule
            TableData table = viewSched.GetTableData();
            TableSectionData section = table.GetSectionData(SectionType.Body);

            nRows = section.NumberOfRows;
            nCols = section.NumberOfColumns;
            SetNameBox.Text = viewSched.Name;

            // This is to select which column the sheet numbers are
            foreach (int num in Enumerable.Range(1, nCols))
            {
                Cbx.Items.Add(num);
            }
            Cbx.SelectedIndex = 0;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string SetName = SetNameBox.Text;
            int selectedIndex = Cbx.SelectedIndex;

            // Read each cell and find the sheet with that number
            foreach (int num in Enumerable.Range(0, nRows))
            {

                string SchedSheetNum = viewSched.GetCellText((SectionType.Body), num, selectedIndex).ToString();

                foreach (ViewSheet sheet in sheetList)
                {
                    if (SchedSheetNum == sheet.SheetNumber)
                    {
                        viewSet.Insert(sheet);
                        break;
                    }
                }
            }



            if (viewSet.Size == 0)
            {
                SetButton.IsEnabled = false; // Disable SetButton or user may crash revit
                Utils.SimpleDialog("No sheet numbers in this column", "");
                SetButton.IsEnabled = true;
            }

            else
            {
                using (Transaction transac = new Transaction(doc))
                {
                    transac.Start("Set by index");

                    // PrintManager saves the set and selects it
                    PrintManager printManager = doc.PrintManager;
                    printManager.PrintRange = PrintRange.Select;

                    ViewSheetSetting viewSheetSetting = printManager.ViewSheetSetting;
                    viewSheetSetting.CurrentViewSheetSet.Views = viewSet;

                    bool flag = false;
                    try
                    {
                        viewSheetSetting.SaveAs(SetName);
                        flag = true;
                    }

                    // Set already exists
                    catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
                    {
                        TaskDialog mainDialog = new TaskDialog("ASG");
                        mainDialog.TitleAutoPrefix = false;
                        mainDialog.MainInstruction = "Replace existing set?";
                        mainDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
                        TaskDialogResult res = mainDialog.Show();

                        if (res == TaskDialogResult.Yes)
                        {

                            // Delete existing set
                            FilteredElementCollector Sheet_Set_Collector = new FilteredElementCollector(doc);

                            ViewSheetSet SetToDel = Sheet_Set_Collector.OfClass(typeof(ViewSheetSet))
                                                          .Cast<ViewSheetSet>()
                                                          .Where(i => i.Name == SetName)
                                                          .FirstOrDefault();
                            doc.Delete(SetToDel.Id);

                            viewSheetSetting.SaveAs(SetName);
                            flag = true;
                        }

                        // Else cancel
                        else
                        {
                            SetButton.IsEnabled = false;
                            SetButton.IsEnabled = true;

                            flag = false;

                            transac.RollBack();
                        }


                    }
                    finally
                    {
                        if (flag)
                        {
                            Close();
                            Utils.SimpleDialog(
                                $"Created set '{SetName}' with {viewSet.Size} sheet", "");

                            transac.Commit();
                        }
                    }
                }
            }
        }
    }
}