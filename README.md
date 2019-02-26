# Godot Extension
MonoDevelop/Visual Studio for Mac Godot integration

At this moment extension only has 3 basic functionalities:
 * Launching game: https://youtu.be/vj-75tZjwz4 </br>
 The way this works it start `/Applications/Godot_mono.app/Contents/MacOS/Godot`(can be modified in preferences) inside same working directory as .csproj file. It also adds `--remote-debug` parameter which allows sending "reload_scripts". 
 * Attaching to existing game, launched from Godot Editor: https://youtu.be/3IsDl__Cpnk </br>
 If game was started inside GodotEditor, it's listening on port `23685` or whatever is written in `project.godot` `[mono] "mono/debugger_agent/port"` so we just connect to that port with debugger. `--remote-debug` is connected to Godot Editor, hence "reload_scripts" is not woring.
 * When game started by Launching from IDE and source code is re-built inside IDE it sends "reload_scritpts", aka. hot reloading: https://youtu.be/Krx1FhB68zs </br>
 Godot Mono integration does rest of the magic of reloading new .dll from hard drive and keeping existing values.
