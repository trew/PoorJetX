using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;

namespace PoorEngine.Helpers
{
    public static class FileHelper
    {
        #region CreateGameContentFile
        public static FileStream CreateGameContentFile(string relativeFilename, bool createNew)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFilename);
            return File.Open(fullPath,
                createNew ? FileMode.Create : FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.ReadWrite);
        }
        #endregion

        #region LoadGameContentFile
        public static FileStream LoadGameContentFile(string relativeFilename)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFilename);
            if (!File.Exists(fullPath))
                return null;
            else
                return File.Open(fullPath,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        #endregion

        #region SaveGameContentFile
        public static FileStream SaveGameContentFile(string relativeFilename)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFilename);
            return File.Open(fullPath,
                FileMode.Create, FileAccess.Write);
        }
        #endregion

        #region Get Text Lines
        static public string[] GetLines(string filename)
        {
            try
            {
                StreamReader reader = new StreamReader(
                    new FileStream(filename, FileMode.Open, FileAccess.Read),
                    System.Text.Encoding.UTF8);
                List<string> lines = new List<string>();
                do
                {
                    lines.Add(reader.ReadLine());
                } while (reader.Peek() > -1);
                reader.Close();
                return lines.ToArray();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        #endregion

        #region Write Helpers
        #endregion
    }
}
