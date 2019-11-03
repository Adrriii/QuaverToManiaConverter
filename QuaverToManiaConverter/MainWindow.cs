using System;
using Gtk;
using QuaverToManiaConverter;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnFileButtonDragDrop(object o, DragDropArgs args)
    {
    }

    protected void OnFileButtonClicked(object sender, EventArgs e)
    {
        FileExplorer filepick = new FileExplorer();
        filepick.Show();
    }
}
