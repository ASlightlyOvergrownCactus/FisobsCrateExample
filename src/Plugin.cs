using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Fisobs.Core;

using BepInEx;
using System.Security.Permissions;

// IMPORTANT
// This requires Fisobs to work!
// Big thx to Dual-Iron (on github) for help with Fisobs!
// Big thx to (insert discord name here if they are okay with it) for help with getting the collision to work!
// This code was based off of Dual-Iron's Centishield as practice, I didn't make parts of this! (Probably add more details on that later)

#pragma warning disable CS0618 // Do not remove the following line.
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TestMod
    {

// Add target_game_version and youtube_trailer_id to modinfo.json if applicable.
// See https://rainworldmodding.miraheze.org/wiki/Downpour_Reference/Mod_Directories

[BepInPlugin("cactus.testMod", "Test Mod - Crate", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public void OnEnable()
    {
            // How to make a hook:
            /*
        On.RainWorld.OnModsInit += Init;

            // Register hooks

            On.Rock.ctor += Rock_ctor;
            */

            Content.Register(new CrateFisobs());
    }



        // Rock bounce thingy (DO NOT)
    /*private void Rock_ctor(On.Rock.orig_ctor orig, Rock self, AbstractPhysicalObject abstractPhysicalObject, World world)
        {
            orig(self, abstractPhysicalObject, world);
            self.gravity = 0.1f;
            self.bounce = 3.0f;
        }*/


    private void Init(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        Logger.LogDebug("Hello world!");
    }
}

}