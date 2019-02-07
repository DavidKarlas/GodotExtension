using System;
using MonoDevelop.Core;

namespace Godot
{
    static class Options
    {
        public static readonly ConfigurationProperty<string> GodotExecutable = ConfigurationProperty.Create("Godot.GodotExecutable", "/Applications/Godot_mono.app/Contents/MacOS/Godot");
    }
}
