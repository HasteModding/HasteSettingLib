using Zorro.Settings;
using Zorro.Settings.DebugUI;

namespace SettingsLib.Settings.DebugUI;

public class CollapsibleSettingUI : SettingUI
{
    public CollapsibleSettingUI(CollapsibleSetting setting, ISettingHandler settingHandler)
    {
        foreach (Setting subsetting in setting.GetSettings())
        {
            subsetting.GetDebugUI(settingHandler);
        }
    }
}
