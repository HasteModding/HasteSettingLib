using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Localization;
using Zorro.Settings;

namespace SettingsLib.Settings.UI;

public class DynamicSettingListUI : SettingInputUICell
{
    // Track expanded state
    private bool _expanded = false;
    private GameObject titleObject = new GameObject("SettingTitle");
    private GameObject settingObject = new GameObject("SettingCell");
    private DynamicSettingList? _setting;
    private ISettingHandler _settingHandler = GameHandler.Instance.SettingsHandler;
    private string collapseButtonText
    {
        get => _expanded ? "▼ Collapse" : "► Expand";
    }

    public DynamicSettingListUI()
        : base()
    {
        SetUpTitleObject();
        SetUpSettingObject();
    }

    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting is DynamicSettingList dynamicSetting)
        {
            ApplyEnclosingStyling();

            _setting = dynamicSetting;
            _settingHandler = settingHandler;
            AddChildren();
        }
    }

    public void RefreshList()
    {
        RemoveChildren();
        AddChildren();

        // Trigger all ContentSizeFitters
        gameObject.SendMessageUpwards("SetLayoutHorizontal");
        gameObject.SendMessageUpwards("SetLayoutVertical");
    }

    private void RemoveChildren()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    private void AddChildren()
    {
        if (_setting == null)
            return;

        // Run setup for all settings, except non-shown conditionals
        foreach (var subsetting in _setting.GetSettings())
            if (!(subsetting is IConditionalSetting conditional) || conditional.CanShow())
                AddSettingBlock(subsetting, _settingHandler);
    }

    private void ApplyEnclosingStyling()
    {
        // Only run for a top-level DynamicSettingListUICell
        if (!transform.parent.parent.gameObject.TryGetComponent<SettingsUICell>(out var comp))
        {
            return;
        }

        // Make the top level setting box's background and text ignore the Layout
        int idx = transform.parent.GetSiblingIndex();
        for (int i = 0; i < transform.parent.parent.childCount; i++)
        {
            if (i == idx)
                continue;
            var child = transform.parent.parent.GetChild(i);
            child.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        }

        // Breaks without this
        transform.parent.gameObject.AddComponent<VerticalLayoutGroup>();

        MakeSettingBoxAdaptable();
    }

    private void MakeSettingBoxAdaptable()
    {
        // Make the setting box as tall as necessary
        var vlg = transform.parent.parent.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = false;
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.MiddleRight;
        // Give the bounding box some breathing room
        vlg.padding.top = 15;
        vlg.padding.bottom = 15;
        transform.parent.parent.gameObject.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;
    }

    private void AddSettingBlock(Setting setting, ISettingHandler settingHandler)
    {
        var block = UnityEngine.Object.Instantiate(settingObject, transform);

        AddSettingTitle(setting, block.transform);
        AddSetting(setting, block.transform, settingHandler);
    }

    private void AddSettingTitle(Setting setting, Transform transform)
    {
        if (setting is IExposedSetting exposedSetting)
        {
            transform
                .GetComponentInChildren<LocalizeUIText>()
                ?.SetString(exposedSetting.GetDisplayName());
        }
    }

    private void AddSetting(Setting setting, Transform transform, ISettingHandler settingHandler)
    {
        var ui = UnityEngine.Object.Instantiate(setting.GetSettingUICell(), transform);

        // Give the field a height so it doesn't get destroyed by the layout
        var layoutElement = ui.GetComponent<ILayoutElement?>();
        if (layoutElement is null)
        {
            layoutElement = ui.AddComponent<LayoutElement>();
        }
        if (layoutElement.preferredHeight == -1 && layoutElement is LayoutElement elem)
        {
            elem.preferredHeight = setting switch
            {
                ButtonSetting set => 55,
                _ => 35,
            };
        }

        ui.GetComponent<SettingInputUICell>().Setup(setting, settingHandler);
    }

    private void SetUpTitleObject()
    {
        var textMesh = titleObject.AddComponent<TextMeshProUGUI>();
        textMesh.enableAutoSizing = true;
        textMesh.alignment = TextAlignmentOptions.Center;
        var text = titleObject.AddComponent<LocalizeUIText>();
        text.String = null;
        var layout = titleObject.AddComponent<LayoutElement>();
        layout.preferredHeight = 20;

        UnityEngine.Object.DontDestroyOnLoad(titleObject);
    }

    private void SetUpSettingObject()
    {
        titleObject.transform.parent = settingObject.transform;

        var layout = settingObject.AddComponent<VerticalLayoutGroup>();
        // Little breathing room
        layout.spacing = 2;
        layout.padding.top = 13;
        settingObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter
            .FitMode
            .PreferredSize;

        UnityEngine.Object.DontDestroyOnLoad(settingObject);
    }
}
