using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

namespace SettingsLib.Settings;

public abstract class CollapsibleSetting : Setting
{
    public virtual List<Setting> GetSettings()
    {
        var settings = new List<Setting>();

        foreach (FieldInfo property in this.GetType().GetFields())
        {
            if (property.FieldType.IsSubclassOf(typeof(Setting)))
            {
                settings.Add((Setting)property.GetValue(this));
            }
        }

        return settings;
    }

		public virtual bool OnToggled()
		{
			return false;
		}

    public override void ApplyValue()
    {
        foreach (Setting setting in GetSettings())
        {
            setting.ApplyValue();
        }
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        foreach (Setting setting in GetSettings())
        {
            setting.Load(loader);
        }
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        foreach (Setting setting in GetSettings())
        {
            setting.Save(saver);
        }
    }

    public override SettingUI GetDebugUI(ISettingHandler settingHandler)
    {
        return new DebugUI.CollapsibleSettingUI(this, settingHandler);
    }

    public override GameObject GetSettingUICell()
    {
        var obj = new GameObject(GetType().FullName);

        // Give the settings a little room to breath
        var vlg = obj.AddComponent<VerticalLayoutGroup>();
        vlg.padding.left = 10;
        vlg.padding.right = 10;
        obj.AddComponent<UI.CollapsibleSettingUI>();

        return obj;
    }
}
