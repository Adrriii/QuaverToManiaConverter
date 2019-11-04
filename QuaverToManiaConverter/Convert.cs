using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Gtk;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using BMAPI.v1;
using BMAPI.v1.HitObjects;

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

            List<Beatmap> beatmaps = QuasToBeatmaps(toConvert);

            return true;
        }

        public static List<Beatmap> QuasToBeatmaps(List<Qua> quas)
        {
            List<Beatmap> result = new List<Beatmap>();

            foreach (Qua convert in quas)
            {
                result.Add(QuaToBeatmap(convert));
            }

            return result;
        }

        public static Beatmap QuaToBeatmap(Qua qua)
        {
            Beatmap beatmap = new Beatmap();

            beatmap.ApproachRate = 5;
            beatmap.Artist = qua.Artist;
            beatmap.ArtistUnicode = qua.Artist;
            beatmap.AudioFilename = qua.AudioFile;
            beatmap.AudioHash = "";
            beatmap.BeatmapHash = "";
            beatmap.Bookmarks = new List<int>();
            beatmap.CircleSize = qua.GetKeyCount();
            beatmap.ComboColours = new List<BMAPI.v1.Combo>();
            beatmap.Creator = qua.Creator;
            beatmap.EditorBookmarks = new List<int>();
            beatmap.Events = new List<BMAPI.v1.Events.EventBase>();
            beatmap.Filename = $"{ qua.Title } - { qua.Artist } [{ qua.DifficultyName }] ({ qua.Creator }).osu";
            beatmap.HitObjects = new List<BMAPI.v1.HitObjects.CircleObject>();

            foreach(HitObjectInfo quaHit in qua.HitObjects)
            {
                beatmap.HitObjects.Add(QuaHitToManiaHit(quaHit, qua.GetKeyCount()));
            }

            beatmap.HPDrainRate = 7;
            beatmap.Mode = GameMode.Mania;
            beatmap.OverallDifficulty = 7;
            beatmap.SampleSet = "";
            beatmap.SkinPreference = "";
            beatmap.SliderBorder = new BMAPI.Colour();
            beatmap.SliderMultiplier = 1;
            beatmap.SliderTickRate = 1;
            beatmap.Source = qua.Source;
            beatmap.Tags = new List<string>();
            beatmap.TimingPoints = new List<TimingPoint>();

            foreach (TimingPointInfo quaTP in qua.TimingPoints)
            {
                beatmap.TimingPoints.Add(QuaTPToManiaTP(quaTP));
            }

            foreach (SliderVelocityInfo quaSV in qua.SliderVelocities)
            {
                beatmap.TimingPoints.Add(QuaSVToManiaTP(quaSV));
            }

            beatmap.Title = qua.Title;
            beatmap.TitleUnicode = qua.Title;
            beatmap.Version = qua.DifficultyName;

            return beatmap;
        }

        public static CircleObject QuaHitToManiaHit(HitObjectInfo quaHit, int keyMode)
        {
            CircleObject maniaHit = new CircleObject();

            switch(quaHit.HitSound)
            {
                case Quaver.API.Enums.HitSounds.Clap:
                    maniaHit.Effect = EffectType.Clap;
                    break;
                case Quaver.API.Enums.HitSounds.Finish:
                    maniaHit.Effect = EffectType.Finish;
                    break;
                case Quaver.API.Enums.HitSounds.Whistle:
                    maniaHit.Effect = EffectType.Whistle;
                    break;
                default:
                    maniaHit.Effect = EffectType.None;
                    break;
            }

            int xfix = 512 / keyMode;
            // xpos =  (512 / keys) * (column - 1) - (256 / keys)
            maniaHit.Location = new BMAPI.Point2((quaHit.Lane - 1) * xfix - (xfix / 2), 192);
            maniaHit.Radius = 0f;
            maniaHit.StartTime = quaHit.StartTime;

            if(quaHit.IsLongNote)
            {
                maniaHit.Type = HitObjectType.Slider;
            }
            else
            {
                maniaHit.Type = HitObjectType.Circle;
            }

            return maniaHit;
        }

        public static TimingPoint QuaTPToManiaTP(TimingPointInfo quaTP)
        {
            TimingPoint maniaTP = new TimingPoint();

            maniaTP.BpmDelay = quaTP.MillisecondsPerBeat;
            maniaTP.CustomSampleSet = 0;
            maniaTP.InheritsBPM = false;
            maniaTP.SampleSet = 0;
            maniaTP.Time = quaTP.StartTime;
            switch (quaTP.Signature)
            {
                case Quaver.API.Enums.TimeSignature.Triple:
                    maniaTP.TimeSignature = 1;
                    break;
                default:
                    maniaTP.TimeSignature = 0;
                    break;
            }
            maniaTP.VisualOptions = TimingPointOptions.None;
            maniaTP.VolumePercentage = 100;

            return maniaTP;
        }

        public static TimingPoint QuaSVToManiaTP(SliderVelocityInfo quaSV)
        {
            TimingPoint maniaTP = new TimingPoint();

            maniaTP.BpmDelay = -1000 / (quaSV.Multiplier * 10);
            maniaTP.CustomSampleSet = 0;
            maniaTP.InheritsBPM = true;
            maniaTP.SampleSet = 0;
            maniaTP.Time = quaSV.StartTime;
            maniaTP.TimeSignature = 1;
            maniaTP.VisualOptions = TimingPointOptions.None;
            maniaTP.VolumePercentage = 100;

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
