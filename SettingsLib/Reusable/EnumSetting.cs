using UnityEngine;
using Zorro.Settings;

namespace SettingsLib.Settings.Reusable;

public abstract class CustomDefaultEnumSetting<T> : EnumSetting<T>
    where T : unmanaged, Enum
{
    private T _default;

    public CustomDefaultEnumSetting(string prefix, T defaultValue)
        : base(prefix)
    {
        _default = defaultValue;
    }

    protected override T GetDefaultValue() => _default;
}

public abstract class EnumSetting<T> : Zorro.Settings.EnumSetting<T>
    where T : unmanaged, Enum
{
    private string _name;

    public EnumSetting(string prefix)
    {
        _name = $"{prefix}.{GetType().Name}";
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        if (loader.TryLoadString(_name, out var value))
        {
            try
            {
                Value = (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception ex)
            {
                if (!(ex is ArgumentException))
                {
                    throw;
                }
                Debug.LogError(
                    "Failed to parse setting of type " + GetType().FullName + " from PlayerPrefs."
                );
                Value = GetDefaultValue();
            }
            return;
        }
        Debug.Log("Failed to load setting of type " + _name + " from PlayerPrefs.");
        Value = GetDefaultValue();
        PostLoadCheckIfValueIsValid();
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saver.SaveString(_name, Value.ToString());
    }

    public override void ApplyValue() { }

    public string GetCategory() => "";
}
