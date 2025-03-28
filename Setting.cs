using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace DetailedDescriptions;

[FileLocation($"ModsSettings/{nameof(DetailedDescriptions)}/{nameof(DetailedDescriptions)}")]
public class Setting : ModSetting
{
    public static Setting Instance;
    public Setting(IMod mod) : base(mod)
    {
    }

    public bool ShowLotSizeUnits = true;
    
    public override void SetDefaults()
    {
        
    }
}