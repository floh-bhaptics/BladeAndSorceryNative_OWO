﻿using MyOWOVest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ThunderRoad;
using System.Runtime.CompilerServices;
using static ThunderRoad.GameData;
using System.Reflection;

namespace BladeAndSorcery_OWO
{
    public class BladeAndSorcery_OWO : ThunderScript
    {
        public static TactsuitVR tactsuitVr;
        private Harmony harmony;

        public override void ScriptLoaded(ModManager.ModData modData)
        {
            EventManager.OnPlayerPrefabSpawned += new EventManager.PlayerPrefabSpawnedEvent(Initialize);
            base.ScriptLoaded(modData);
        }

        public void Initialize()
        {
            tactsuitVr = new TactsuitVR();

            this.harmony = new Harmony("com.florianfahrenberger.owo");
            this.harmony.PatchAll();
        }

        [HarmonyPatch(typeof(BhapticsHandler), "PlayHapticInternal", new Type[] { typeof(float), typeof(float), typeof(BhapticsHandler.FeedbackType), typeof(float), typeof(bool), typeof(bool), typeof(float) })]
        public class bhaptics_PlayBhapticsEffectInternal
        {
            [HarmonyPrefix]
            public static void Prefix(BhapticsHandler __instance, float locationAngle, float locationHeight, BhapticsHandler.FeedbackType effect, float intensityMultiplier, bool reflected)
            {
                string pattern = effect.ToString();
                //tactsuitVr.LOG("Original pattern internal: " + pattern + " reflected: " + reflected.ToString() + " Intensity: " + intensityMultiplier.ToString());
                if (pattern == "NoFeedback") return;
                if (pattern == "HeartBeatFast") return;
                if (pattern == "HeartBeat") return;
                if (pattern == "DefaultDamage") pattern = "DamageVest";

                if (pattern.Contains("DamageRightArm"))
                {
                    pattern = "DamageRightArm";
                    if (reflected) pattern = pattern.Replace("RightArm", "LeftArm");
                }
                if (pattern.Contains("PlayerSpell"))
                {
                    pattern = "PlayerSpellRight";
                }
                if (pattern.Contains("PlayerTelekinesis"))
                {
                    pattern = "PlayerTelekinesisRight";
                }
                if ((pattern.Contains("Player")) || (pattern.Contains("Gauntlets")) || (pattern.Contains("Climbing")))
                    if (pattern.Contains("Right"))
                        if (reflected) pattern = pattern.Replace("Right", "Left");


                pattern = pattern.Replace("Blade", "");
                pattern = pattern.Replace("Other", "");
                pattern = pattern.Replace("Player", "");
                pattern = pattern.Replace("Arrow", "");

                pattern = pattern.Replace("Wood", "");
                pattern = pattern.Replace("Metal", "");
                pattern = pattern.Replace("Stone", "");
                pattern = pattern.Replace("Fabric", "");
                pattern = pattern.Replace("Flesh", "");

                pattern = pattern.Replace("Pierce", "");
                pattern = pattern.Replace("Slash", "");
                pattern = pattern.Replace("Blunt", "");

                pattern = pattern.Replace("Small", "");
                pattern = pattern.Replace("Large", "");

                pattern = pattern.Replace("LRD", "");
                pattern = pattern.Replace("LRU", "");
                pattern = pattern.Replace("RLD", "");
                pattern = pattern.Replace("RLU", "");

                // Melee feedback only triggers on the left hands right now, which is weird
                // So better shut it off overall.
                //if (pattern.Contains("Melee")) return;

                if (pattern.Contains("DamageVest"))
                {
                    pattern = "DamageVest";
                    tactsuitVr.PlayBackHit(pattern, locationAngle, locationHeight);
                }
                else tactsuitVr.PlayBackFeedback(pattern);
            }
        }

    }
}
