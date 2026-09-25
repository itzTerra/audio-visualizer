# Audio Visualizer

> Final personal project for the course PV178 Introduction to C#/.NET on FI MUNI.

## Used libs

- [Avalonia](https://docs.avaloniaui.net/docs/basics/user-interface/controls/builtin-controls)
- [Ursa](https://github.com/irihitech/Ursa.Avalonia) [(Demo)](https://irihitech.github.io/Ursa.Avalonia/)
  - [Semi Theme](https://docs.irihi.tech/semi/en/docs/)
- [Icons.Avalonia](https://github.com/Projektanker/Icons.Avalonia)
- [SoundFlow](https://github.com/LSXPrime/SoundFlow)

## Future Goals

- **Main goal: a theme that uses AI audio embedding vectors (e.g. from [VGGish](https://github.com/tensorflow/models/blob/master/research/audioset/vggish/README.md)) as the source of information for rendering video**
- Project mode — the app can open a directory that then acts as a project
  - outputs are saved into the project folder, and the project folder is the default directory for FileInput inputs
  - ability to save a Theme locally within the project instead of system-wide
- Creating custom themes with custom parameters and associated rendering methods
- Ability to export only a selected part of the timeline
- Ability to map a different theme onto different parts of the audio, or to have multiple graphical outputs for the same part (layers + opacity could be nice)
- Loading and working with multiple audio tracks

## Known Issues
- Video export unsupported
- Theme module forms are not validated against nonsense inputs
- Theme Explorer double-click to select works only on the text part of a node
- Theme Explorer doesn't show a drop location indicator when hovering over an edge of a category (style issue)
- App displays playing sound icon in the taskbar all the time, even when not playing sound
- Theme module groups are not used in UI yet
