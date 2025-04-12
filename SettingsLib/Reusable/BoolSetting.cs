using UnityEngine;
using Zorro.Settings;

namespace SettingsLib.Settings.Reusable;

public abstract class CustomDefaultBoolSetting : BoolSetting
{
    private bool _default;

    public CustomDefaultBoolSetting(string prefix, bool defaultValue)
        : base(prefix)
    {
        _default = defaultValue;
    }

    protected override bool GetDefaultValue() => _default;
}

public abstract class BoolSetting : Zorro.Settings.BoolSetting
{
    private string _name;

    public BoolSetting(string prefix)
    {
        _name = $"{prefix}.{GetType().Name}";
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        if (loader.TryLoadBool(_name, out var value))
        {
            Value = value;
            return;
        }
        Debug.Log("Failed to load setting of type " + _name + " from PlayerPrefs.");
        Value = GetDefaultValue();
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saver.SaveBool(_name, Value);
    }

    public override void ApplyValue() { }

    public string GetCategory() => "";
}
