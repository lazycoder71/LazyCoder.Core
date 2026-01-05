using System;
using System.IO;
using UnityEngine;

namespace LazyCoder.Core.Editor
{
    public static class EditorFileBrowser
    {
        private static bool IsMacOS => SystemInfo.operatingSystem.IndexOf("Mac OS", StringComparison.Ordinal) != -1;

        private static bool IsWinOS => SystemInfo.operatingSystem.IndexOf("Windows", StringComparison.Ordinal) != -1;

        private static void OpenInMac(string path)
        {
            bool openInsidesOfFolder = false;

            // try mac
            // mac finder doesn't like backward slashes
            // if path requested is a folder, automatically open insides of that folder
            string macPath = path.Replace("\\", "/");

            if (Directory.Exists(macPath))
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }

            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }

            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open mac finder in windows
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        private static void OpenInWin(string path)
        {
            bool openInsidesOfFolder = false;

            // try windows
            // windows explorer doesn't like forward slashes
            string winPath = path.Replace("/", "\\");
            // if path requested is a folder, automatically open insides of that folder
            if (Directory.Exists(winPath))
            {
                openInsidesOfFolder = true;
            }

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open win explorer in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        public static void OpenDirectory(string path)
        {
            if (IsWinOS)
            {
                OpenInWin(path);
            }
            else if (IsMacOS)
            {
                OpenInMac(path);
            }
            else // couldn't determine OS
            {
                OpenInWin(path);
                OpenInMac(path);
            }
        }
    }
}
