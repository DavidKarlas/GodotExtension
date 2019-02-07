using System;
using MonoDevelop.Ide.Gui.Dialogs;
using MonoDevelop.Components;
using MonoDevelop.Core;

namespace Godot
{
    public class GlobalOptionsPanel : OptionsPanel
    {
        FileEntry fileEntry = new FileEntry();

        public override Control CreatePanelWidget()
        {
            var vbox = new Gtk.VBox();
            vbox.Spacing = 6;

            var generalSectionLabel = new Gtk.Label("<b>" + GettextCatalog.GetString("General") + "</b>");
            generalSectionLabel.UseMarkup = true;
            generalSectionLabel.Xalign = 0;
            vbox.PackStart(generalSectionLabel, false, false, 0);

            var godotExecutableHBox = new Gtk.HBox();
            godotExecutableHBox.BorderWidth = 10;
            godotExecutableHBox.Spacing = 6;
            var outputDirectoryLabel = new Gtk.Label(GettextCatalog.GetString("Godot executable:"));
            outputDirectoryLabel.Xalign = 0;
            godotExecutableHBox.PackStart(outputDirectoryLabel, false, false, 0);
            fileEntry.Path = Options.GodotExecutable.Value;
            godotExecutableHBox.PackStart(fileEntry, true, true, 0);

            vbox.PackStart(godotExecutableHBox, false, false, 0);

            vbox.ShowAll();
            return vbox;
        }

        public override void ApplyChanges()
        {
            Options.GodotExecutable.Value = fileEntry.Path;
        }
    }
}
