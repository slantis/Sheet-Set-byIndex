using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SetByIndex
{
    internal static class Utils
    {
        /// <summary>
        /// Simple dialog with header and content
        /// </summary>
        /// <param name="header">The header</param>
        /// <param name="content">The content</param>
        internal static void SimpleDialog(string header, string content)
        {
            TaskDialog mainDialog = new TaskDialog("ASG");
            mainDialog.TitleAutoPrefix = false;
            mainDialog.MainInstruction = header;
            mainDialog.MainContent = content;
            mainDialog.Show();
        }

        /// <summary>
        /// Simple dialog with a header
        /// </summary>
        /// <param name="header">The header</param>
        internal static void SimpleDialog(string header)
        {
            TaskDialog mainDialog = new TaskDialog("ASG");
            mainDialog.TitleAutoPrefix = false;
            mainDialog.MainInstruction = header;
            mainDialog.Show();
        }

        /// <summary>
        /// Standard dialog to output an exception
        /// </summary>
        /// <param name="ex">The exception</param>
        internal static void CatchDialog(Exception ex)
        {
            string head = ex.Source + " - " + ex.GetType().ToString();
            string moreText = ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Data;

            Utils.SimpleDialog(head, moreText);
        }


        /// <summary>
        /// Manages the button icon to be used in Revit
        /// </summary>
        /// <param name="imagePath">Path to embedded resource</param>
        /// <returns></returns>
        internal static ImageSource RetriveImage(string imagePath)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);
            string str = imagePath.Substring(imagePath.Length - 3);

            if (str == "ico")
                return (ImageSource)new IconBitmapDecoder(manifestResourceStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            else
                return (ImageSource)null;
        }
    }
}
