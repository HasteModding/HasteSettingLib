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

### SettingsLib.Settings.Reusable

This namespace contains extensions of the base value setting classes, with
customisable storage names and defaults. These are made to work only within
the context of the CollapsibleSetting, and might lead to issues if used in
other contexts. For example, they have a default empty category string, as
any setting in a CollapsibleSetting falls under the CollapsibleSetting anyways,
and doesn't look at its category.

These classes are abstract because you do need to provide them a unique type
name to avoid conflicts in the settings. Besides this, you just need to provide
a prefix that will cause the Prefix+ClassName to be unique. The FullName of the
type of the CollapsibleSetting it is put under is recommended.

### Examples

#### Using the same type of setting multiple times in one CollapsibleSetting

```cs
using Landfall.Haste;
using SettingsLib.Settings;
using SettingsLib.Settings.Reusable;
using UnityEngine.Localization;

namespace SettingsLib;

// Only label the outer-most setting with `HasteSetting` to add it to the setting menu.
[HasteSetting]
public class CombinedSetting : CollapsibleSetting, IExposedSetting
{
    public TestStringSetting TestStringShape;
    public TestStringSetting TestStringFruit;
    public StackedCombinedSetting TestStackedCombinedSetting = new StackedCombinedSetting();

    public CombinedSetting()
    {
        TestStringShape = new TestStringSetting("Shape", GetType().FullName, "Circle");
        TestStringFruit = new TestStringSetting("Fruit", GetType().FullName, "Apple");
    }

    public string GetCategory() => "CollapsibleUI";

    public LocalizedString GetDisplayName() => new UnlocalizedString("TestCombinedSetting");
}

public class StackedCombinedSetting : CollapsibleSetting, IExposedSetting
{
    public TestStringSetting TestStringVehicle;
    public TestStringSetting TestStringAnimal;

    public StackedCombinedSetting()
        : this("") { }

    public StackedCombinedSetting(string prefix)
    {
        // If we want to use this class inside multiple other CombinedSettings, the normal
        // `GetType().FullName` prefix would cause setting conflicts
        prefix = $"{prefix}.{GetType().FullName}";
        TestStringVehicle = new TestStringSetting("Vehicle", prefix, "Car");
        TestStringAnimal = new TestStringSetting("Animal", prefix, "Cat");
    }

    public string GetCategory() => "Irrelevant";

    public LocalizedString GetDisplayName() => new UnlocalizedString("TestStackedCombinedSetting");
}

// To have multiple settings of the same type, override the `Load` and `Save`
// methods to use custom keys with the provided NamedType.
public class TestStringSetting : CustomDefaultStringSetting, IExposedSetting
{
    private LocalizedString _name;

    public TestStringSetting(string name, string prefix, string defaultValue)
        : this(new UnlocalizedString(name), prefix, defaultValue) { }

    public TestStringSetting(LocalizedString name, string prefix, string defaultValue)
        : base($"{prefix}.{name.ToString()}", defaultValue)
    {
        _name = name;
    }

    public LocalizedString GetDisplayName() => _name;
}
```

#### Reusing a CollapsibleSetting for multiple similar settings

```cs
using Landfall.Haste;
using SettingsLib.Settings;
using SettingsLib.Settings.Reusable;
using Unity.Mathematics;
using UnityEngine.Localization;

namespace SettingsLib;

[HasteSetting]
public class OnHitSetting : EventSettings, IExposedSetting
{
    protected override float DefaultPower => 0.6f;
    protected override float DefaultDuration => 2f;
    protected override float DefaultDropoff => 1f;

    public string GetCategory() => "CollapsibleUI";

    public LocalizedString GetDisplayName() => new UnlocalizedString("On Hit");
}

[HasteSetting]
public class OnDeathSetting : EventSettings, IExposedSetting
{
    protected override float DefaultPower => 0.6f;
    protected override float DefaultDuration => 2f;
    protected override float DefaultDropoff => 1f;

    public string GetCategory() => "CollapsibleUI";

    public LocalizedString GetDisplayName() => new UnlocalizedString("On Death");
}

public abstract class EventSettings : CollapsibleSetting
{
    protected abstract float DefaultPower { get; }
    protected abstract float DefaultDuration { get; }
    protected abstract float DefaultDropoff { get; }

    public PowerSetting Power;
    public DurationSetting Duration;
    public DropoffSetting Dropoff;

    public EventSettings()
    {
        Power = new PowerSetting(GetType().FullName, DefaultPower);
        Duration = new DurationSetting(GetType().FullName, DefaultDuration);
        Dropoff = new DropoffSetting(GetType().FullName, DefaultDropoff);
    }
}

public class PowerSetting : CustomDefaultFloatSetting, IExposedSetting
{
    public PowerSetting(string prefix, float defaultValue)
        : base(prefix, defaultValue) { }

    protected override float2 GetMinMaxValue() => new float2(0, 1);

    public LocalizedString GetDisplayName() => new UnlocalizedString("Power");
}

public class DurationSetting : CustomDefaultFloatSetting, IExposedSetting
{
    public DurationSetting(string prefix, float defaultValue)
        : base(prefix, defaultValue) { }

    protected override float2 GetMinMaxValue() => new float2(0, 10);

    public LocalizedString GetDisplayName() => new UnlocalizedString("Duration (s)");
}

public class DropoffSetting : CustomDefaultFloatSetting, IExposedSetting
{
    public DropoffSetting(string prefix, float defaultValue)
        : base(prefix, defaultValue) { }

    protected override float2 GetMinMaxValue() => new float2(0, 5);

    public LocalizedString GetDisplayName() => new UnlocalizedString("Dropoff");
}
```
