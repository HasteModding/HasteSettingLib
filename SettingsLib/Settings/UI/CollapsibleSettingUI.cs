using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Localization;
using Zorro.Settings;

namespace SettingsLib.Settings.UI;

public class CollapsibleSettingUI : SettingInputUICell
{
    // Track expanded state
    private bool _expanded = false;
    private CollapsibleSetting? _collapsibleSetting;

    // We no longer have a persistent titleObject; we create title objects on demand.
    private GameObject settingObject = new GameObject("SettingCell");
    private string collapseButtonText => _expanded ? "▼ Collapse" : "► Expand";

    public CollapsibleSettingUI()
        : base()
    {
        // Removed SetUpTitleObject call – title objects are now created as needed.
        SetUpSettingObject();
    }

    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting is CollapsibleSetting group)
        {
            _collapsibleSetting = group;

            ApplyEnclosingStyling();
            AddCollapseButton();

            // Run setup for all settings, except non-shown conditionals.
            foreach (var subsetting in group.GetSettings())
            {
                if (!(subsetting is IConditionalSetting conditional) || conditional.CanShow())
                {
                    AddSettingBlock(subsetting, settingHandler);
                }
            }
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
        _collapsibleSetting?.OnToggled(_expanded);
    }

    private void SetVisibility()
    {
        // Skip the first child (collapse button) when toggling visibility.
        for (var i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(_expanded);
        }
        // Trigger a layout update.
        gameObject.SendMessageUpwards("SetLayoutHorizontal");
        gameObject.SendMessageUpwards("SetLayoutVertical");
    }

    private void ApplyEnclosingStyling()
    {
        // Only run for a top-level CollapsibleSettingUICell.
        if (!transform.parent.parent.gameObject.TryGetComponent<SettingsUICell>(out var comp))
        {
            return;
        }

        // Make the top-level setting box's background and text ignore the layout.
        int idx = transform.parent.GetSiblingIndex();
        for (int i = 0; i < transform.parent.parent.childCount; i++)
        {
            if (i == idx)
                continue;
            var child = transform.parent.parent.GetChild(i);
            child.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        }

        // This is needed to force a layout.
        transform.parent.gameObject.AddComponent<VerticalLayoutGroup>();

        MakeSettingBoxAdaptable();
    }

    private void MakeSettingBoxAdaptable()
    {
        // Make the setting box as tall as necessary.
        var vlg = transform.parent.parent.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = false;
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.MiddleRight;
        // Give the bounding box some breathing room.
        vlg.padding.top = 15;
        vlg.padding.bottom = 15;
        transform.parent.parent.gameObject.AddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;
    }

    /// <summary>
    /// Adds a new setting block. If a title exists, it is added here.
    /// </summary>
    private void AddSettingBlock(Setting setting, ISettingHandler settingHandler)
    {
        var block = UnityEngine.Object.Instantiate(settingObject, transform);

        // Check if the setting has a title.
        LocalizedString? titleText = null;
        if (setting is IExposedSetting exposedSetting)
        {
            titleText = exposedSetting.GetDisplayName();
        }

        // Only add a title if it is non-null and non-empty.
        if (titleText != null && !titleText.IsEmpty)
        {
            AddSettingTitle(titleText, block.transform);
        }

        AddSetting(setting, block.transform, settingHandler);
    }

    /// <summary>
    /// Instantiates a title object and attaches it to the given parent.
    /// </summary>
    private void AddSettingTitle(LocalizedString titleText, Transform parentTransform)
    {
        var titleObj = new GameObject("SettingTitle");
        titleObj.transform.SetParent(parentTransform, false);
        var textMesh = titleObj.AddComponent<TextMeshProUGUI>();
        textMesh.enableAutoSizing = true;
        textMesh.alignment = TextAlignmentOptions.Center;
        var localizeUIText = titleObj.AddComponent<LocalizeUIText>();
        localizeUIText.SetString(titleText);
        var layout = titleObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 20;
    }

    private void AddSetting(Setting setting, Transform transform, ISettingHandler settingHandler)
    {
        var ui = UnityEngine.Object.Instantiate(setting.GetSettingUICell(), transform);

        // Ensure the element retains a preferred height so it isn’t collapsed by the layout.
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

    private void SetUpSettingObject()
    {
        var layout = settingObject.AddComponent<VerticalLayoutGroup>();
        // Add a little breathing room.
        layout.spacing = 2;
        layout.padding.top = 13;
        settingObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter
            .FitMode
            .PreferredSize;

        UnityEngine.Object.DontDestroyOnLoad(settingObject);
    }
}
