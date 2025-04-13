using Zorro.Settings;
using Zorro.Settings.DebugUI;

namespace SettingsLib.Settings.DebugUI;

public class DynamicSettingListUI : SettingUI
{
    public DynamicSettingListUI(DynamicSettingList setting, ISettingHandler settingHandler)
    {
        foreach (Setting subsetting in setting.GetSettings())
        {
            subsetting.GetDebugUI(settingHandler);
        }
    }
}
