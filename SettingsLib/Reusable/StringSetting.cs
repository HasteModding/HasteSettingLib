using UnityEngine;
using Zorro.Settings;

namespace SettingsLib.Settings.Reusable;

public abstract class CustomDefaultStringSetting : StringSetting
{
    private string _default;

    public CustomDefaultStringSetting(string prefix, string defaultValue)
        : base(prefix)
    {
        _default = defaultValue;
    }

    protected override string GetDefaultValue() => _default;
}

public abstract class StringSetting : Zorro.Settings.StringSetting
{
    private string _name;

    public StringSetting(string prefix)
    {
        _name = $"{prefix}.{GetType().Name}";
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        if (loader.TryLoadString(_name, out var value))
        {
            Value = value;
            return;
        }
        Debug.Log("Failed to load setting of type " + _name + " from PlayerPrefs.");
        Value = GetDefaultValue();
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saver.SaveString(_name, Value);
    }

    public override void ApplyValue() { }

    public string GetCategory() => "";
}
