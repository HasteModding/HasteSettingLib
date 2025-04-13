using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

namespace SettingsLib.Settings;

public abstract class DynamicSettingList : Setting
{
    protected UI.DynamicSettingListUI? _uiElement;
    public List<Setting> Settings = [];

    public virtual List<Setting> GetSettings()
    {
        return Settings;
    }

    public void SetUIElement(UI.DynamicSettingListUI uiElement)
    {
        _uiElement = uiElement;
    }

    public virtual void UpdateUI()
    {
        _uiElement?.RefreshList();
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
        return new DebugUI.DynamicSettingListUI(this, settingHandler);
    }

    public override GameObject GetSettingUICell()
    {
        var obj = new GameObject(GetType().FullName);

        // Give the settings a little room to breath
        var vlg = obj.AddComponent<VerticalLayoutGroup>();
        vlg.padding.left = 10;
        vlg.padding.right = 10;
        obj.AddComponent<UI.DynamicSettingListUI>();

        return obj;
    }
}
