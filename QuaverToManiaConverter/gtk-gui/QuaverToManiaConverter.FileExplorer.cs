
// This file has been generated by the GUI designer. Do not modify.
namespace QuaverToManiaConverter
{
	public partial class FileExplorer
	{
		private global::Gtk.FileChooserWidget filechooserwidget1;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget QuaverToManiaConverter.FileExplorer
			this.Name = "QuaverToManiaConverter.FileExplorer";
			this.Title = global::Mono.Unix.Catalog.GetString("FileExplorer");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child QuaverToManiaConverter.FileExplorer.Gtk.Container+ContainerChild
			this.filechooserwidget1 = new global::Gtk.FileChooserWidget(((global::Gtk.FileChooserAction)(0)));
			this.filechooserwidget1.Name = "filechooserwidget1";
			this.Add(this.filechooserwidget1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 818;
			this.DefaultHeight = 519;
			this.Show();
			this.filechooserwidget1.FileActivated += new global::System.EventHandler(this.OnFilechooserwidget1FileActivated);
		}
	}
}
