using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zorro.Localization;
using Zorro.Settings;

namespace SettingsLib.Settings.UI
{
    public class DynamicSettingListUI : SettingInputUICell
    {
        // Track expanded state
        private bool _expanded = false;
        private GameObject settingObject = new GameObject("SettingCell");
        private DynamicSettingList? _setting;
        private ISettingHandler _settingHandler = GameHandler.Instance.SettingsHandler;
        private string collapseButtonText => _expanded ? "▼ Collapse" : "► Expand";

        public DynamicSettingListUI()
            : base()
        {
            SetUpSettingObject();
        }

        public override void Setup(Setting setting, ISettingHandler settingHandler)
        {
            if (setting is DynamicSettingList dynamicSetting)
            {
                ApplyEnclosingStyling();

                _setting = dynamicSetting;
                _setting.SetUIElement(this);
                _settingHandler = settingHandler;
                AddChildren();
            }
        }

        public void RefreshList()
        {
            RemoveChildren();
            AddChildren();

            // Trigger all ContentSizeFitters
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

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

            // Run setup for all settings, except non-shown conditionals.
            foreach (var subsetting in _setting.GetSettings())
            {
                if (!(subsetting is IConditionalSetting conditional) || conditional.CanShow())
                    AddSettingBlock(subsetting, _settingHandler);
            }
        }

        private void ApplyEnclosingStyling()
        {
            // Only run for a top-level DynamicSettingListUI.
            if (!transform.parent.parent.gameObject.TryGetComponent<SettingsUICell>(out var comp))
            {
                return;
            }

            // Make the top level setting box's background and text ignore the layout.
            int idx = transform.parent.GetSiblingIndex();
            for (int i = 0; i < transform.parent.parent.childCount; i++)
            {
                if (i == idx)
                    continue;
                var child = transform.parent.parent.GetChild(i);
                child.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            }

            // This is required to force the layout.
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

        private void AddSettingBlock(Setting setting, ISettingHandler settingHandler)
        {
            // Instantiate a new container using settingObject as template.
            var block = UnityEngine.Object.Instantiate(settingObject, transform);

            // Check if the setting defines a title.
            if (setting is IExposedSetting exposedSetting)
            {
                LocalizedString titleText = exposedSetting.GetDisplayName();
                if (titleText != null && !titleText.IsEmpty)
                {
                    AddSettingTitle(titleText, block.transform);
                }
            }

            AddSetting(setting, block.transform, settingHandler);
        }

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

        private void AddSetting(Setting setting, Transform parent, ISettingHandler settingHandler)
        {
            var ui = UnityEngine.Object.Instantiate(setting.GetSettingUICell(), parent);

            // Ensure the element gets a preferred height so it isn’t collapsed by the
            // layout.
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
            // Little breathing room.
            layout.spacing = 2;
            layout.padding.top = 13;
            settingObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter
                .FitMode
                .PreferredSize;

            UnityEngine.Object.DontDestroyOnLoad(settingObject);
        }
    }
}
