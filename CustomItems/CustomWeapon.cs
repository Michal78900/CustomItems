using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace CustomItems
{
    public abstract class CustomWeapon : CustomItem
    {
        protected CustomWeapon(ItemType type, int clipSize, int itemId) : base(type, itemId)
        {
            ClipSize = clipSize;
        }

        public abstract override string ItemName { get; set; }
        protected abstract override string ItemDescription { get; set; }
        protected virtual int ClipSize { get; set; }
        
        protected virtual int ModBarrel { get; set; } = 0;
        protected virtual int ModSight { get; set; } = 0;
        protected virtual int ModOther { get; set; } = 0;

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;
            base.LoadEvents();
        }

        protected override void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;
            base.UnloadEvents();
        }

        protected virtual void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CheckItem(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;
                Log.Debug($"{ev.Player.Nickname} is reloading a {ItemName}!", Plugin.Singleton.Config.Debug);
                int remainingInClip = ClipSize - (int) ev.Player.CurrentItem.durability;
                int currentAmmoAmount = (int)ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType];
                int amountToReload = ClipSize - remainingInClip;
                if (currentAmmoAmount >= 0)
                {
                    ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();
                    Reload(ev.Player);
                    
                    int amountAfterReload = currentAmmoAmount - amountToReload;
                    if (amountAfterReload < 0)
                        ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = 0;
                    else
                        ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = (uint)(currentAmmoAmount - amountToReload);
                    
                    ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
                    Log.Debug($"{ev.Player.Nickname} - {ev.Player.CurrentItem.durability} - {ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType]}", Plugin.Singleton.Config.Debug);
                    Timing.CallDelayed(4.5f, () =>
                    {
                        Reload(ev.Player);
                    });
                }
            }
        }
        
        public override void SpawnItem(Vector3 position) => ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(ItemType, ClipSize, position, default, ModSight, ModBarrel, ModOther));
        
        public override void GiveItem(Player player)
        {
            ++Inventory._uniqId;
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = ClipSize,
                id = ItemType,
                uniq = Inventory._uniqId,
                modBarrel = ModBarrel,
                modSight = ModSight,
                modOther = ModOther
            };
            player.Inventory.items.Add(syncItemInfo);
            ItemIds.Add(syncItemInfo.uniq);
            ShowMessage(player);
            
            ItemGiven(player);
        }
        
        private void Reload(Player player) => player.ReferenceHub.weaponManager.RpcReload(player.ReferenceHub.weaponManager.curWeapon);
    }
}