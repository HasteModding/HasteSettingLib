using UnityEngine;
using Zorro.Settings;

namespace SettingsLib.Settings.Reusable;

public abstract class CustomDefaultIntSetting : IntSetting
{
    private int _default;

    public CustomDefaultIntSetting(string prefix, int defaultValue)
        : base(prefix)
    {
        _default = defaultValue;
    }

    protected override int GetDefaultValue() => _default;
}

public abstract class IntSetting : Zorro.Settings.IntSetting
{
    private string _name;

    public IntSetting(string prefix)
    {
        _name = $"{prefix}.{GetType().Name}";
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        if (loader.TryLoadInt(_name, out var value))
        {
            Value = value;
            return;
        }
        Debug.Log("Failed to load setting of type " + _name + " from PlayerPrefs.");
        Value = GetDefaultValue();
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saver.SaveInt(_name, Value);
    }

    public override void ApplyValue() { }

    public string GetCategory() => "";
}
