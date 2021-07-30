using System.Windows.Forms;

namespace MinecraftClicker
{
    class ClickerConfigManager
    {
        public int savedBreakInterval = 0;
        public int savedBuildInterval = 0;
        public int savedReplaceInterval = 0;

        public Keys savedQuickBreakKey;
        public Keys savedQuickBuildKey;
        public Keys savedQuickReplaceKey;

        public ClickerConfigManager(int breakInterval, int buildInterval, int replaceInterval,
                           Keys quickBreakKey, Keys quickBulidKey, Keys quickReplaceKey)
        {
            savedQuickBreakKey = quickBreakKey;
            savedQuickBuildKey = quickBulidKey;
            savedQuickReplaceKey = quickReplaceKey;

            savedBreakInterval = breakInterval;
            savedBuildInterval = buildInterval;
            savedReplaceInterval = replaceInterval;
        }
    }
}
