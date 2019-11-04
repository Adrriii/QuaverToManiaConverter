using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Gtk;
using Quaver.API.Maps;
using Quaver.API.Helpers;

namespace QuaverToManiaConverter
{
    public static class Convert
    {
        public static bool StartConvert(Window parent, string filename)
        {
            if (!CheckExtension(filename)) return false;

            string destination = $"./QMCworking/{ Path.GetFileNameWithoutExtension(filename) }";

            CleanWorkingDirectory();

            if(!ExtractQP(new FileInfo(filename), destination)) return false;

            List<Qua> toConvert = GetQuaFromDirectory(destination);

            CleanWorkingDirectory();

            return true;
        }

        private static List<Qua> GetQuaFromDirectory(string dirname)
        {
            List<Qua> maps = new List<Qua>();

            foreach (string file in Directory.GetFiles(dirname, "*.qua"))
            {
                try
                {
                    maps.Add(Qua.Parse(File.ReadAllBytes(file), true));
                } catch (Exception e)
                {
                    Alert($"Invalid map '{ Path.GetFileNameWithoutExtension(file) }'. ({ e.ToString() })");
                }
            }

            return maps;
        }

        private static bool CheckExtension(string filename)
        {
            string file_extension = Path.GetExtension(filename);
            if (!file_extension.Equals(".qp"))
            {
                Alert($"Format '{ file_extension }' not supported");
                return false;
            }
            return true;
        }

        private static bool ExtractQP(FileInfo file, string destination)
        {
            try
            {
                Directory.CreateDirectory(destination);

                ZipFile.ExtractToDirectory(file.FullName, destination);

            } catch (Exception e)
            {
                Alert("Failed to extract the .qp file.");
                return false;
            }
            return true;
        }

        private static void CleanWorkingDirectory()
        {
            if (Directory.Exists("./QMCworking"))
            {
                Directory.Delete("./QMCworking", true);
            }
        }

        private static void Alert(string message)
        {
            MessageDialog alert = new MessageDialog(null,
               DialogFlags.DestroyWithParent,
               MessageType.Error,
               ButtonsType.Ok,
               message);
            alert.Run();
            alert.Destroy();
        }
    }
}
