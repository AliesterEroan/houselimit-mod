using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SunHavenHouseLimitMod
{
    [BepInPlugin("houselimit", "House Limit Mod", "1.0.0")]
    public class HouseLimitMod : BaseUnityPlugin
    {
        internal static ConfigEntry<int> configMaxHouses;
        internal static ManualLogSource logger;

        private void Awake()
        {
            try
            {
                logger = Logger;

                // Config setup
                configMaxHouses = Config.Bind(
                    "General",
                    "Max Houses",
                    999,
                    "Maximum number of houses allowed (default: 999)"
                );

                logger.LogInfo("House Limit Mod: Starting patch application...");

                // Apply Harmony patches manually using AccessTools
                var harmony = new Harmony("houselimit");

                // Try to find the PlayerHouse type
                Type playerHouseType = AccessTools.TypeByName("Wish.PlayerHouse");

                if (playerHouseType == null)
                {
                    logger.LogError("House Limit Mod: Could not find Wish.PlayerHouse type!");
                    return;
                }

                logger.LogInfo("House Limit Mod: Found Wish.PlayerHouse type");

                // Patch MaxNumber property getter
                MethodInfo maxNumberGetter = AccessTools.PropertyGetter(playerHouseType, "MaxNumber");
                if (maxNumberGetter != null)
                {
                    harmony.Patch(maxNumberGetter, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("MaxNumberTranspiler")));
                    logger.LogInfo("House Limit Mod: Patched MaxNumber getter");
                }
                else
                {
                    logger.LogError("House Limit Mod: Could not find MaxNumber getter");
                }

                // Patch SetMeta method
                MethodInfo setMetaMethod = AccessTools.Method(playerHouseType, "SetMeta");
                if (setMetaMethod != null)
                {
                    harmony.Patch(setMetaMethod, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace7Transpiler")));
                    logger.LogInfo("House Limit Mod: Patched SetMeta");
                }
                else
                {
                    logger.LogError("House Limit Mod: Could not find SetMeta");
                }

                // Patch Build method
                MethodInfo buildMethod = AccessTools.Method(playerHouseType, "Build");
                if (buildMethod != null)
                {
                    harmony.Patch(buildMethod, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace7Transpiler")));
                    logger.LogInfo("House Limit Mod: Patched Build");
                }
                else
                {
                    logger.LogError("House Limit Mod: Could not find Build");
                }

                // Patch SceneToLoad method
                MethodInfo sceneToLoadMethod = AccessTools.Method(playerHouseType, "SceneToLoad");
                if (sceneToLoadMethod != null)
                {
                    harmony.Patch(sceneToLoadMethod, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace7Transpiler")));
                    logger.LogInfo("House Limit Mod: Patched SceneToLoad");
                }
                else
                {
                    logger.LogError("House Limit Mod: Could not find SceneToLoad");
                }

                // Patch GetHouseSceneIndex method (private static)
                MethodInfo getHouseSceneIndexMethod = AccessTools.Method(playerHouseType, "GetHouseSceneIndex");
                if (getHouseSceneIndexMethod != null)
                {
                    harmony.Patch(getHouseSceneIndexMethod, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace7Transpiler")));
                    logger.LogInfo("House Limit Mod: Patched GetHouseSceneIndex");
                }
                else
                {
                    logger.LogError("House Limit Mod: Could not find GetHouseSceneIndex");
                }

                // Patch HousePlaceable methods - this is where the actual limit check happens
                Type housePlaceableType = AccessTools.TypeByName("Wish.HousePlaceable");
                if (housePlaceableType != null)
                {
                    // Patch PlaceDecoration method
                    MethodInfo placeDecorationMethod = AccessTools.Method(housePlaceableType, "PlaceDecoration");
                    if (placeDecorationMethod != null)
                    {
                        harmony.Patch(placeDecorationMethod, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace8Transpiler")));
                        logger.LogInfo("House Limit Mod: Patched HousePlaceable.PlaceDecoration");
                    }
                    else
                    {
                        logger.LogError("House Limit Mod: Could not find PlaceDecoration method");
                    }

                    // Patch Use1 method
                    MethodInfo use1Method = AccessTools.Method(housePlaceableType, "Use1");
                    if (use1Method != null)
                    {
                        harmony.Patch(use1Method, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace8Transpiler")));
                        logger.LogInfo("House Limit Mod: Patched HousePlaceable.Use1");
                    }
                    else
                    {
                        logger.LogError("House Limit Mod: Could not find Use1 method");
                    }

                    // Patch ModifyMeta method
                    MethodInfo modifyMetaMethod = AccessTools.Method(housePlaceableType, "ModifyMeta");
                    if (modifyMetaMethod != null)
                    {
                        harmony.Patch(modifyMetaMethod, transpiler: new HarmonyMethod(typeof(HouseLimitMod).GetMethod("Replace8Transpiler")));
                        logger.LogInfo("House Limit Mod: Patched HousePlaceable.ModifyMeta");
                    }
                    else
                    {
                        logger.LogError("House Limit Mod: Could not find ModifyMeta method");
                    }
                }
                else
                {
                    logger.LogError("House Limit Mod: Could not find Wish.HousePlaceable type");
                }

                logger.LogInfo($"House Limit Mod loaded successfully! Max houses set to: {configMaxHouses.Value}");
            }
            catch (Exception ex)
            {
                logger.LogError($"House Limit Mod: Error during initialization: {ex.Message}");
                logger.LogError(ex.StackTrace);
            }
        }

        // Transpiler for MaxNumber - replaces ldc.i4.8 with our config value
        public static IEnumerable<CodeInstruction> MaxNumberTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // Replace ldc.i4.8 (load 8) with our config value
                if (instruction.opcode == OpCodes.Ldc_I4_8)
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(HouseLimitMod).GetMethod("GetMaxHouses"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        // Transpiler for replacing ldc.i4.7 (7) with maxHouses-1
        public static IEnumerable<CodeInstruction> Replace7Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // Replace ldc.i4.7 (load 7) with our max-1 value
                if (instruction.opcode == OpCodes.Ldc_I4_7)
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(HouseLimitMod).GetMethod("GetMaxHousesMinusOne"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        // Transpiler for replacing ldc.i4.8 (8) with maxHouses
        public static IEnumerable<CodeInstruction> Replace8Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // Replace ldc.i4.8 (load 8) with our config value
                if (instruction.opcode == OpCodes.Ldc_I4_8)
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(HouseLimitMod).GetMethod("GetMaxHouses"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        // Helper method to get the configured max houses
        public static int GetMaxHouses()
        {
            return configMaxHouses.Value;
        }

        // Helper method to get max houses - 1 (for playerNumber clamping)
        public static int GetMaxHousesMinusOne()
        {
            return configMaxHouses.Value - 1;
        }
    }
}
