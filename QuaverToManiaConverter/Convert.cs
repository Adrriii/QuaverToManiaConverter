using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Gtk;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;

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

            List<ManiaBeatmap> beatmaps = QuasToBeatmaps(toConvert);

            string dir_save = null;
            do
            {
                if(dir_save != null)
                {
                    Alert("Please select a directory");
                }

                Gtk.FileChooserDialog filechooser =
                    new Gtk.FileChooserDialog("Choose a folder to save the map",
                        parent,
                        FileChooserAction.CreateFolder,
                        "Cancel", ResponseType.Cancel,
                        "Chose", ResponseType.Accept);

                if (filechooser.Run() == (int)ResponseType.Accept)
                {
                    dir_save = filechooser.Filename;
                }
                else
                {
                    return false;
                }

                filechooser.Destroy();
            } while (!Directory.Exists(dir_save));

            foreach(ManiaBeatmap b in beatmaps)
            {
                b.Save(dir_save + "/" + b.Filename);
            }

            return true;
        }

        public static List<ManiaBeatmap> QuasToBeatmaps(List<Qua> quas)
        {
            List<ManiaBeatmap> result = new List<ManiaBeatmap>();

            foreach (Qua convert in quas)
            {
                result.Add(QuaToBeatmap(convert));
            }

            return result;
        }

        public static ManiaBeatmap QuaToBeatmap(Qua qua)
        {
            ManiaBeatmap beatmap = new ManiaBeatmap();


            foreach (HitObjectInfo quaHit in qua.HitObjects)
            {
                beatmap.HitObjects.Add(QuaHitToManiaHit(quaHit, qua.GetKeyCount()));
            }


            beatmap.Artist = qua.Artist;
            beatmap.AudioFileName = qua.AudioFile;
            beatmap.CircleSize = qua.GetKeyCount();
            beatmap.Creator = qua.Creator;
            beatmap.Source = qua.Source;
            beatmap.BackgroundFile = qua.BackgroundFile;

            foreach (TimingPointInfo quaTP in qua.TimingPoints)
            {
                beatmap.TimingPoints.Add(QuaTPToManiaTP(quaTP));
            }

            foreach (SliderVelocityInfo quaSV in qua.SliderVelocities)
            {
                beatmap.TimingPoints.Add(QuaSVToManiaTP(quaSV));
            }

            beatmap.Title = qua.Title;
            beatmap.Version = qua.DifficultyName;
            beatmap.PreviewTime = qua.SongPreviewTime;

            beatmap.RefreshFilename();

            return beatmap;
        }

        public static HitObject QuaHitToManiaHit(HitObjectInfo quaHit, int keyMode)
        {
            HitObject maniaHit;
            int xfix = 512 / keyMode;

            if (quaHit.IsLongNote)
            {
                maniaHit = new HitObject((quaHit.Lane) * xfix - (xfix / 2), quaHit.StartTime, true, quaHit.EndTime);
            }
            else
            {
                maniaHit = new HitObject((quaHit.Lane) * xfix - (xfix / 2), quaHit.StartTime);
            }

            return maniaHit;
        }

        public static TimingPoint QuaTPToManiaTP(TimingPointInfo quaTP)
        {

            TimingPoint maniaTP = new TimingPoint((int) quaTP.StartTime, quaTP.MillisecondsPerBeat);

            return maniaTP;
        }

        public static TimingPoint QuaSVToManiaTP(SliderVelocityInfo quaSV)
        {

            TimingPoint maniaTP = new TimingPoint((int)quaSV.StartTime, -1000 / (quaSV.Multiplier * 10));

            return maniaTP;
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
