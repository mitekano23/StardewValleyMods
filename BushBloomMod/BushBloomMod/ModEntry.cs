﻿using BushBloomMod.Patches;
using BushBloomMod.Patches.Integrations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace BushBloomMod {
    internal sealed class ModEntry : Mod {
        public static ModEntry Instance { get; private set; }

        public Configuration Config;

        public override void Entry(IModHelper helper) {
            Instance = this;
            Config = Configuration.Register(helper);
            Bushes.Register();
            //Almanac.Register(this.ModManifest);
            Automate.Register(this.ModManifest);
            helper.Events.GameLoop.GameLaunched += (s, e) => Schedule.ReloadEntries(9);
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Content.AssetRequested += Schedule.Content_AssetRequested;
        }

        // INFO: We need to manually re-trigger bush.dayupdate() in order to allow CP to apply
        // content updates which would normally only happen after the base game has already
        // called that function.
        [EventPriority(EventPriority.Low)]
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e) {
            // grab newest content, with any CP updates
            Schedule.ReloadEntries();
            // find all bushes and update for real
            Bushes.UpdateAllBushes();
        }

        public override object GetApi() => new Api();
    }
}