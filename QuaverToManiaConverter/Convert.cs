using System;
using System.IO;
using System.IO.Compression;
using Gtk;

namespace QuaverToManiaConverter
{
    public static class Convert
    {
        public static bool StartConvert(Window parent, string filename)
        {
            if (!CheckExtension(filename)) return false;

            ExtractQP(new FileInfo(filename));

            CleanWorkingDirectory();
            return true;
        }

        private static bool CheckExtension(string filename)
        {
            string file_extension = Path.GetExtension(filename);
            if (!file_extension.Equals(".qp"))
            {
                MessageDialog alert = new MessageDialog(null,
                    DialogFlags.DestroyWithParent,
                    MessageType.Error,
                    ButtonsType.Ok,
                    $"Format '{ file_extension }' not supported");
                alert.Run();
                alert.Destroy();
                return false;
            }
            return true;
        }

        private static bool ExtractQP(FileInfo file)
        {
            string destination = $"./QMCworking/{ Path.GetFileNameWithoutExtension(file.FullName) }";

            CleanWorkingDirectory();

            Directory.CreateDirectory(destination);

            ZipFile.ExtractToDirectory(file.FullName, destination);

            return true;
        }

        private static void CleanWorkingDirectory()
        {
            if (Directory.Exists("./QMCworking"))
            {
                Directory.Delete("./QMCworking", true);
            }
        }
    }
}
