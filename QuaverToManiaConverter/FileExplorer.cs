using System;
using System.Collections.Generic;
using System.IO;
using Gdk;
using Gtk;

namespace QuaverToManiaConverter
{
    public partial class FileExplorer : Gtk.Window
    {
        public FileExplorer() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }

        protected void OnFilechooserwidget1FileActivated(object sender, EventArgs e)
        {
            this.Hide();
            foreach(string file in ((FileChooser)sender).Filenames)
            {
                Console.WriteLine(file);
            }
            this.Destroy();
        }
    }
}
