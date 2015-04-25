using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EACopy.TransferObject;

namespace EACopy
{
    public class EACopyFactory
    {
        private List<string> _destFolders;

        public EACopyFactory(List<string> destFolders)
        {
            this._destFolders = destFolders;
        }
        public CopyResponse Start()
        {
            string dropboxPath = GetDropBoxPath();
            string mql4Path = Constants.MQL4;
            string sourceFolder = Path.Combine(dropboxPath, mql4Path);
            CopyResponse response = new CopyResponse();

            if (_destFolders != null && _destFolders.Count() > 0)
            {
                foreach (string destFolder in _destFolders)
                {
                    var destFolderMql = Path.Combine(destFolder, Constants.MQL4);

                    response = copyFolder(sourceFolder, destFolderMql);

                    if (response.Success)
                        WriteLog(Path.Combine(destFolder, Constants.MQL4));
                }
            }

            return response;
        }

        private static string GetDropBoxPath()
        {
            var appDataPath = Environment.GetFolderPath(
                                               Environment.SpecialFolder.ApplicationData);
            var dbPath = Path.Combine(appDataPath, Constants.Dropbox);

            if (!File.Exists(dbPath))
                return null;

            var lines = File.ReadAllLines(dbPath);
            var dbBase64Text = Convert.FromBase64String(lines[1]);
            var folderPath = Encoding.UTF8.GetString(dbBase64Text);

            return folderPath;
        }

        private CopyResponse copyFolder(string sourceFolder, string destFolder)
        {
            CopyResponse response = new CopyResponse();
            response.Success = true;
            string[] files;


            if (destFolder[destFolder.Length - 1] != Path.DirectorySeparatorChar)
                destFolder += Path.DirectorySeparatorChar;

            if (Directory.Exists(destFolder))
                DeleteDirectory(destFolder, true);

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            files = Directory.GetFileSystemEntries(sourceFolder);

            try
            {
                foreach (string file in files)
                {
                    if (Directory.Exists(file))
                        copyFolder(file, Path.Combine(destFolder, Path.GetFileName(file)));
                    else
                        File.Copy(file, Path.Combine(destFolder, Path.GetFileName(file)), true);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.ToString();
                response.Success = false;
            }

            return response;
        }

        private static void DeleteDirectory(string path)
        {
            DeleteDirectory(path, false);
        }

        private static void DeleteDirectory(string path, bool recursive)
        {
            // Delete all files and sub-folders?
            if (recursive)
            {
                // Yep... Let's do this
                var subfolders = Directory.GetDirectories(path);
                foreach (var s in subfolders)
                {
                    DeleteDirectory(s, recursive);
                }
            }

            // Get all files of the folder
            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                // Get the attributes of the file
                var attr = File.GetAttributes(f);

                // Is this file marked as 'read-only'?
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // Yes... Remove the 'read-only' attribute, then
                    File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                }

                // Delete the file
                File.Delete(f);
            }

            // When we get here, all the files of the folder were
            // already deleted, so we just delete the empty folder
            Directory.Delete(path);

        }

        private void WriteLog(string path)
        {
            string message = string.Format("Last Copied: {0}", DateTime.Now.ToString());
            string fileName = Path.Combine(path, "Report.txt");

            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.Write(message);
            }
        }
    }
}
