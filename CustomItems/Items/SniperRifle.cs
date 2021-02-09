using System.Collections.Generic;
using CustomItems.API;
using Exiled.Events.EventArgs;

namespace CustomItems.Items
{
    public class SniperRifle : CustomWeapon
    {
        public SniperRifle(ItemType type, int clipSize, int itemId) : base(type, clipSize, itemId)
        {
        }

        public override string Name { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.Name;
        public override Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = Plugin.Singleton.Config.ItemConfigs.GlCfg.SpawnLocations;
        public override string Description { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.Description;
        public override int SpawnLimit { get; set; } = Plugin.Singleton.Config.ItemConfigs.SniperCfg.SpawnLimit;
        protected override int ModBarrel { get; set; } = 3;
        protected override int ModSight { get; set; } = 4;

        
        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnloadEvents();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (CheckItem(ev.Attacker.CurrentItem))
                ev.Amount *= Plugin.Singleton.Config.ItemConfigs.SniperCfg.DamageMultiplier;
        }
    }
}