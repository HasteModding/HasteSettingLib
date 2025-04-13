using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using Zorro.Core;
using Zorro.Localization;
using Zorro.Settings;

namespace SettingsLib.Settings.UI;

public class CollapsibleSettingUI : SettingInputUICell
{
    // Track expanded state
    private bool _expanded = false;
    private GameObject titleObject = new GameObject("SettingTitle");
    private GameObject settingObject = new GameObject("SettingCell");
    private string collapseButtonText
    {
        get => _expanded ? "▼ Collapse" : "► Expand";
    }

    public CollapsibleSettingUI()
        : base()
    {
        SetUpTitleObject();
        SetUpSettingObject();
    }

    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting is CollapsibleSetting group)
        {
            ApplyEnclosingStyling();
            AddCollapseButton();

            // Run setup for all settings, except non-shown conditionals
            foreach (var subsetting in group.GetSettings())
                if (!(subsetting is IConditionalSetting conditional) || conditional.CanShow())
                    AddSettingBlock(subsetting, settingHandler);

            SetVisibility();
        }
    }

    private void AddCollapseButton()
    {
        var ui = UnityEngine.Object.Instantiate(
            SingletonAsset<InputCellMapper>.Instance.ButtonSettingCell,
            transform
        );
        ui.AddComponent<LayoutElement>().preferredHeight = 55;
        var buttonUI = ui.GetComponent<Zorro.Settings.UI.ButtonSettingUI>();
        buttonUI.Label.text = collapseButtonText;
        buttonUI.Button.onClick.AddListener(() =>
        {
            OnToggled();
            buttonUI.Label.text = collapseButtonText;
        });
        ui.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void OnToggled()
    {
        _expanded = !_expanded;
        SetVisibility();
    }

    private void SetVisibility()
    {
        // Skip the first child, as it is the collapse button.
        for (var i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(_expanded);
        }
        // Trigger all ContentSizeFitters
        gameObject.SendMessageUpwards("SetLayoutHorizontal");
        gameObject.SendMessageUpwards("SetLayoutVertical");
    }

    private void ApplyEnclosingStyling()
    {
        // Only run for a top-level CollapsibleSettingUICell
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

        LocalizedString titleText = null!;
        if (setting is IExposedSetting exposedSetting)
        {
            titleText = exposedSetting.GetDisplayName();
        }

        // Find the title child object (it's the first child based on SetUpSettingObject)
        var titleChildTransform = block.transform.GetChild(0);

        // If the title is empty, destroy the title object. Otherwise, set its text.
        if (titleText == null || titleText.IsEmpty)
        {
            UnityEngine.Object.Destroy(titleChildTransform.gameObject);
        }
        else
        {
            titleChildTransform.GetComponentInChildren<LocalizeUIText>()?.SetString(titleText);
        }

        AddSetting(setting, block.transform, settingHandler);
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
        titleObject.transform.SetParent(settingObject.transform, false);

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
