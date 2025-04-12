using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

namespace SettingsLib.Settings.Reusable;

public abstract class CustomDefaultFloatSetting : FloatSetting
{
    private float _default;

    public CustomDefaultFloatSetting(string prefix, float defaultValue)
        : base(prefix)
    {
        _default = defaultValue;
    }

    protected override float GetDefaultValue() => _default;
}

public abstract class FloatSetting : Zorro.Settings.FloatSetting
{
    private string _name;

    public FloatSetting(string prefix)
    {
        _name = $"{prefix}.{GetType().Name}";
        float2 minMaxValue = GetMinMaxValue();
        MinValue = minMaxValue.x;
        MaxValue = minMaxValue.y;
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        if (loader.TryLoadFloat(_name, out var value))
        {
            Value = value;
            return;
        }
        Debug.Log("Failed to load setting of type " + _name + " from PlayerPrefs.");
        Value = GetDefaultValue();
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saver.SaveFloat(_name, Value);
    }

    public override void ApplyValue() { }

    public string GetCategory() => "";
}
