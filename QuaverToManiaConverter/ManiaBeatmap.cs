using System;
using System.Collections.Generic;
using System.IO;

namespace QuaverToManiaConverter
{
    public class ManiaBeatmap
    {
        public string AudioFileName;
        public string Title;
        public string Artist;
        public string Creator;
        public string Version;
        public string BackgroundFile;
        public string Source;
        public string Filename;
        public int CircleSize;

        public List<TimingPoint> TimingPoints;
        public List<HitObject> HitObjects;

        public ManiaBeatmap() {
            TimingPoints = new List<TimingPoint>();
            HitObjects = new List<HitObject>();
        }

        public void Save(string to)
        {
            if(AudioFileName == null ||
                Title == null ||
                Artist == null ||
                Creator == null ||
                Version == null ||
                BackgroundFile == null ||
                Source == null
                )
            {
                Console.WriteLine("Can't write " + to + ", missing property.");
                return;
            }

            StreamWriter writer = new StreamWriter(to);

            writer.WriteLine("osu file format v14");
            writer.WriteLine("");
            writer.WriteLine("[General]");
            writer.WriteLine($"AudioFileName: { AudioFileName }");
            writer.WriteLine("AudioLeadIn: 0");
            writer.WriteLine("PreviewTime: -1");
            writer.WriteLine("Countdown: 0");
            writer.WriteLine("SampleSet: None");
            writer.WriteLine("StackLeniency: 1");
            writer.WriteLine("Mode: 3");
            writer.WriteLine("LetterboxInBreaks: 0");
            writer.WriteLine("SpecialStyle: 0");
            writer.WriteLine("WidescreenStoryboard: 0");
            writer.WriteLine("");
            writer.WriteLine("[Editor]");
            writer.WriteLine("DistanceSpacing: 1");
            writer.WriteLine("BeatDivisor: 4");
            writer.WriteLine("GridSize: 16");
            writer.WriteLine("TimelineZoom: 1");
            writer.WriteLine("");
            writer.WriteLine("[Metadata]");
            writer.WriteLine($"Title: { Title }");
            writer.WriteLine($"TitleUnicode: { Title }");
            writer.WriteLine($"Artist: { Artist }");
            writer.WriteLine($"ArtistUnicode: { Artist }");
            writer.WriteLine($"Creator: { Creator }");
            writer.WriteLine($"Version: { Version }");
            writer.WriteLine($"Source: { Source }");
            writer.WriteLine("Tags: ");
            writer.WriteLine("");
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("HPDrainRate: 7");
            writer.WriteLine($"CircleSize: { CircleSize }");
            writer.WriteLine("OverallDifficulty: 7");
            writer.WriteLine("ApproachRate: 7");
            writer.WriteLine("SliderMultiplier: 1.4");
            writer.WriteLine("SliderTickRate: 1");
            writer.WriteLine("");
            writer.WriteLine("[Events]");
            writer.WriteLine("//Background and Video events");
            if (BackgroundFile != null)
            {
                writer.WriteLine($"0,0,\"bg.png\",0,0: { BackgroundFile }");
            }
            writer.WriteLine("");
            writer.WriteLine("[TimingPoints]");

            foreach(TimingPoint timingPoint in TimingPoints) {
                writer.WriteLine(timingPoint);
            }

            writer.WriteLine("");
            writer.WriteLine("[HitObjects]");

            foreach (HitObject hitObject in HitObjects) {
                writer.WriteLine(hitObject);
            }

            writer.Close();
        }

        public void RefreshFilename()
        {
            Filename = $"{ Title } - { Artist } [{ Version }] ({ Creator }).osu";
        }
    }

    public class HitObject
    {
        public int x;
        public int y;
        public int time;
        public int end_time;
        public bool ln;

        public HitObject(int x, int time, bool ln = false, int end_time = 0, int y = 192)
        {
            this.x = x;
            this.y = y;
            this.time = time;
            this.ln = ln;
            this.end_time = end_time;
        }

        
        override public string ToString()
        {
            return $"{ x },{ y },{ time },{ (ln ? 128 : 1) },{ end_time }:0:0:0:";
        }
    }

    public class TimingPoint
    {
        public int time;
        public double val;
        public int type;

        public TimingPoint(int time, double val)
        {
            this.time = time;
            this.val = val;
            this.type = val >= 0 ? 0 : 1;
        }

        override public string ToString()
        {
            return $"{ time },{ val },4,1,0,100,{ type },0";
        }
    }
}
