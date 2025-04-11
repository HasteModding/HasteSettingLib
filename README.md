# SettingsLib

This is a library of additional Setting UI elements for Haste: Broken Worlds.
It currently only contains the CollapsibleSetting

## CollapsibleSetting

The CollapsibleSetting is a setting which will automatically detect its own
setting fields, and render them all collapsed under one setting, with an
expand/collapse button.

It supports nesting, and should work fine with all existing setting types.

As settings by default assume their class to be the setting's key, this can
cause conflicts with a CollapsibleSetting that contains multiple fields of the
same type. See the example for a method to resolve this.

### Example

```cs
using Landfall.Haste;
using Landfall.Modding;
using SettingsLib;
using SettingsLib.Settings;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Only label the outer-most setting with `HasteSetting` to add it to the setting menu.
[HasteSetting]
public class CombinedSetting : CollapsibleSetting, IExposedSetting
{
    public TestStringSetting TestStringShape = new TestStringSetting("Shape", "Circle");
    public StackedCombinedSetting TestStackedCombinedSetting = new StackedCombinedSetting();
    public TestStringSetting TestStringFruit = new TestStringSetting("Fruit", "Apple");
    public TestFloatSetting TestFloat = new TestFloatSetting();
    public TestButtonSetting TestButton = new TestButtonSetting();

    public string GetCategory() => "CollapsibleUI";

    public LocalizedString GetDisplayName() => new UnlocalizedString("TestCombinedSetting");
}

public class StackedCombinedSetting : CollapsibleSetting, IExposedSetting
{
    public TestStringSetting TestStringVehicle = new TestStringSetting("Vehicle", "Car");
    public TestStringSetting TestStringAnimal = new TestStringSetting("Animal", "Cat");

    public string GetCategory() => "Irrelevant";

    public LocalizedString GetDisplayName() => new UnlocalizedString("TestStackedCombinedSetting");
}

// To have multiple settings of the same type, override the `Load` and `Save`
// methods to use custom keys with the provided NamedType.
public class TestStringSetting : StringSetting, IExposedSetting
{
    private string Name;
    private string DefaultValue;

    public TestStringSetting(string name, string defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        if (loader.TryLoadString(new NamedType(GetType().FullName, Name), out var value))
        {
            Value = value;
            return;
        }
        Debug.Log("Failed to load setting of type " + GetType().FullName + " from PlayerPrefs.");
        Value = GetDefaultValue();
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saver.SaveString(new NamedType(GetType().FullName, Name), Value);
    }

    public override void ApplyValue() { }

    public string GetCategory() => "General";

    public LocalizedString GetDisplayName() => new UnlocalizedString(Name);

    protected override string GetDefaultValue() => DefaultValue;
}

public class TestButtonSetting : ButtonSetting, IExposedSetting
{
    public override void ApplyValue() { }

    public override string GetButtonText() => "Stacked Button!";

    public string GetCategory() => "General";

    public LocalizedString GetDisplayName() => new UnlocalizedString("TestIntSetting");

    public override void OnClicked(ISettingHandler settingHandler)
    {
        Debug.Log("Clicked stacked button");
    }
}

public class TestFloatSetting : FloatSetting, IExposedSetting
{
    public override void ApplyValue() { }

    public string GetCategory() => "General";

    public LocalizedString GetDisplayName() => new UnlocalizedString("TestFloatSetting");

    protected override float GetDefaultValue() => 0.5f;

    protected override float2 GetMinMaxValue() => new float2(0, 1);
}
```
