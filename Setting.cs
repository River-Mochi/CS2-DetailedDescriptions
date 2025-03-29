using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace DetailedDescriptions
{
    //[FileLocation($"ModsSettings/{nameof(DetailedDescriptions)}/{nameof(DetailedDescriptions)}")]
    [FileLocation("ModsSettings/DetailedDescriptions/DetailedDescriptions")]
    [SettingsUIGroupOrder(kSettingsGroup, kDescriptionsGroup)]
    [SettingsUIShowGroupName(kSettingsGroup, kDescriptionsGroup)]
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public const string kMainSection = "Settings";
        public const string kSettingsGroup = "GeneralSettings";
        public const string kDescriptionsGroup = "DescriptionTypes";
        public Setting(IMod mod) : base(mod)
        {
        }

        #region General Settings
        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ShowLotSizeUnits { get; set; } = true;

        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ApplyChanges
        {
            set
            {
                Mod.ApplySettingsChanges();
                Mod.ReloadActiveLocale();
            }
        }
    
        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ReloadActiveLocale
        {
            set
            {
                Mod.ReloadActiveLocale();
            }
        }

        #endregion
    
        #region Description Types
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowBuildingLotSizes { get; set; } = true;
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowZoneLotSizes { get; set; } = true;
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowBuildingWorkplaces { get; set; } = true;
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowRoadSpeedLimit { get; set; } = true;
    
        #endregion

    
        public override void SetDefaults()
        {
        
        }
    }
}