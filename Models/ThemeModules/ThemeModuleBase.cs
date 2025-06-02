using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using AudioVisualizer.Models.ThemeModules.Background;
using AudioVisualizer.Models.ThemeModules.Core;
using AudioVisualizer.Models.ThemeModules.Misc;
using AudioVisualizer.Services;
using AudioVisualizer.Services.Visualizer;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Ursa.Controls;

namespace AudioVisualizer.Models.ThemeModules;

// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism
[JsonDerivedType(typeof(ThemeModuleWaveLinear), typeDiscriminator: "waveLinear")]
[JsonDerivedType(typeof(ThemeModuleWaveRadial), typeDiscriminator: "waveRadial")]
[JsonDerivedType(typeof(ThemeModuleText), typeDiscriminator: "text")]
[JsonDerivedType(typeof(ThemeModuleEmpty), typeDiscriminator: "empty")]
[JsonDerivedType(typeof(ThemeModuleBackgroundSolid), typeDiscriminator: "bgSolid")]
[JsonDerivedType(typeof(ThemeModuleWaveform), typeDiscriminator: "waveform")]
[JsonDerivedType(typeof(ThemeModuleLevelMeter), typeDiscriminator: "levelMeter")]
[JsonDerivedType(typeof(ThemeModuleSpectrum), typeDiscriminator: "spectrum")]
[JsonDerivedType(typeof(ThemeModuleBox), typeDiscriminator: "box")]
public abstract class ThemeModuleBase : ObservableObject
{
    private const int DefaultFormFieldWidth = 200;

    [Browsable(false)]
    public abstract string Identifier { get; }
    [Browsable(false)]
    public abstract string Name { get; }
    [Browsable(false)]
    public abstract ThemeModuleCategory Category { get; }

    private bool _isVisible = true;
    [JsonRequired]
    [Browsable(false)]
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public virtual VisualizerBase? CreateVisualizer()
    {
        return null;
    }

    [JsonIgnore]
    [Browsable(false)]
    public virtual Control RenderedForm
    {
        get
        {
            var form = new Form()
            {
                DataContext = this,
                LabelPosition = Ursa.Common.Position.Left,
                LabelWidth = GridLength.Star
            };

            foreach (var prop in this.GetType().GetProperties() ?? [])
            {
                var browsableAttr = prop.GetCustomAttribute<BrowsableAttribute>();
                var readOnlyAttr = prop.GetCustomAttribute<ReadOnlyAttribute>();

                if ((browsableAttr != null && !browsableAttr.Browsable) ||
                    (readOnlyAttr != null && readOnlyAttr.IsReadOnly))
                {
                    continue;
                }

                Control editor;

                if (prop.PropertyType == typeof(string))
                {
                    editor = new TextBox()
                    {
                        Width = DefaultFormFieldWidth,
                        [!TextBox.TextProperty] = new Binding(prop.Name)
                    };
                }
                else if (prop.PropertyType == typeof(int))
                {
                    editor = new NumericUIntUpDown()
                    {
                        Width = DefaultFormFieldWidth,
                        [!NumericUIntUpDown.ValueProperty] = new Binding(prop.Name)
                    };
                }
                else if (prop.PropertyType == typeof(float))
                {
                    editor = new NumericFloatUpDown()
                    {
                        Width = DefaultFormFieldWidth,
                        [!NumericFloatUpDown.ValueProperty] = new Binding(prop.Name)
                    };
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    editor = new CheckBox()
                    {
                        [!CheckBox.IsCheckedProperty] = new Binding(prop.Name)
                    };
                }
                else if (prop.PropertyType == typeof(Avalonia.Media.Color))
                {
                    editor = new ColorPicker()
                    {
                        Width = DefaultFormFieldWidth,
                        [!ColorPicker.ColorProperty] = new Binding(prop.Name),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    };
                }
                else if (prop.PropertyType.IsEnum)
                {
                    editor = new EnumSelector()
                    {
                        Width = DefaultFormFieldWidth,
                        [EnumSelector.EnumTypeProperty] = prop.PropertyType,
                        [!EnumSelector.ValueProperty] = new Binding(prop.Name),
                    };
                }
                else if (prop.PropertyType == typeof(FontFamily))
                {
                    editor = new ComboBox()
                    {
                        Width = DefaultFormFieldWidth,
                        [!ComboBox.SelectedItemProperty] = new Binding(prop.Name),
                        ItemsSource = FontManager.Current.SystemFonts
                    };
                }
                // TODO
                // else if (prop.PropertyType == typeof(Vector2))
                // {
                //     editor = new Vector2Editor()
                //     {
                //         Width = 120,
                //         [!Vector2Editor.ValueProperty] = new Binding(prop.Name)
                //     };
                // }
                else
                {
                    Notifier.Warning($"Error showing input for {prop.Name}: unsupported type {prop.PropertyType}");
                    continue;
                }

                editor[FormItem.LabelProperty] = prop.Name;
                form.Items.Add(editor);
            }

            form.DataContext = this;
            return form;
        }
    }

    public override string ToString()
    {
        return Identifier;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ThemeModuleBase module)
        {
            return Identifier == module.Identifier && Name == module.Name && Category == module.Category &&
                   IsVisible == module.IsVisible;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode() ^ Name.GetHashCode() ^ Category.GetHashCode() ^ IsVisible.GetHashCode();
    }

    public ThemeModuleBase Clone()
    {
        return (ThemeModuleBase)MemberwiseClone();
    }
}


