using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodhomeQoL.Settings
{
    public sealed class LocalSettings : SettingBase<LocalSettingAttribute>
    {
        public string GearSwitcherLastPreset { get; set; } = "FullGear";

        public Dictionary<string, bool>? PerSaveModules { get; set; }

        public int DefaultsResetVersion { get; set; }
    }
}
