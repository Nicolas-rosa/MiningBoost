using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "castro_war.miningboost";
    public const string PluginName = "Mining Boost";
    public const string PluginVersion = "1.2.0";

    private const int DefaultMultiplier = 100;
    private const int MaximumMultiplier = 10000;
    private const int DefaultChaosChancePercent = 5;

    private static ConfigEntry<int> multiplier;
    private static ConfigEntry<bool> miningChaosEnabled;
    private static ConfigEntry<int> chaosChancePercent;
    private static ManualLogSource log;
    private static bool overflowWarningLogged;
    private Harmony harmony;

    private static readonly string[] MiningChaosTraits =
    {
        "madness",
        "plague",
        "evil"
    };

    private void Awake()
    {
        log = Logger;
        multiplier = Config.Bind(
            "General",
            "Multiplier",
            DefaultMultiplier,
            new ConfigDescription(
                "Multiplier applied to resources extracted from buildings.",
                new AcceptableValueRange<int>(1, MaximumMultiplier)));
        miningChaosEnabled = Config.Bind(
            "Mining Chaos",
            "Enabled",
            true,
            "Allows mining to corrupt workers with powerful random traits.");
        chaosChancePercent = Config.Bind(
            "Mining Chaos",
            "ChancePercent",
            DefaultChaosChancePercent,
            new ConfigDescription(
                "Chance per extraction for a miner to receive madness, plague, or evil.",
                new AcceptableValueRange<int>(0, 100)));

        harmony = new Harmony(PluginGuid);
        harmony.PatchAll();

        Logger.LogInfo(
            $"Mining Boost loaded with multiplier {multiplier.Value}x and mining chaos " +
            $"at {chaosChancePercent.Value}%.");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }

    internal static int MultiplyAmount(int amount)
    {
        long boostedAmount = (long)amount * multiplier.Value;

        if (boostedAmount > int.MaxValue)
        {
            if (!overflowWarningLogged)
            {
                log.LogWarning("The mining result exceeded Int32.MaxValue and was capped.");
                overflowWarningLogged = true;
            }

            return int.MaxValue;
        }

        if (boostedAmount < int.MinValue)
        {
            if (!overflowWarningLogged)
            {
                log.LogWarning("The mining result was below Int32.MinValue and was capped.");
                overflowWarningLogged = true;
            }

            return int.MinValue;
        }

        return (int)boostedAmount;
    }

    internal static void AddMiningResources(Actor actor, string resourceId, int amount)
    {
        actor.addToInventory(resourceId, MultiplyAmount(amount));
        TryTriggerMiningChaos(actor);
    }

    private static void TryTriggerMiningChaos(Actor actor)
    {
        if (!miningChaosEnabled.Value ||
            !Randy.randomChance(chaosChancePercent.Value / 100f))
        {
            return;
        }

        string traitId = MiningChaosTraits[
            Randy.randomInt(0, MiningChaosTraits.Length)];

        if (actor.addTrait(traitId))
        {
            log.LogDebug($"Mining instability gave {actor.getName()} the {traitId} trait.");
        }
    }

    internal static void LogPatchResult(int replacements)
    {
        if (replacements == 1)
        {
            log.LogDebug("Mining extraction patch applied.");
            return;
        }

        log.LogWarning(
            $"Mining extraction patch changed {replacements} calls to addToInventory; expected 1. " +
            "The WorldBox version may require an update to this mod.");
    }
}

[HarmonyPatch(typeof(ai.behaviours.BehExtractResourcesFromBuilding), "execute")]
public static class MiningPatch
{
    private static readonly MethodInfo AddToInventoryMethod = AccessTools.Method(
        typeof(Actor),
        "addToInventory",
        new[] { typeof(string), typeof(int) });

    private static readonly MethodInfo AddMiningResourcesMethod = AccessTools.Method(
        typeof(Plugin),
        nameof(Plugin.AddMiningResources));

    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        int replacements = 0;

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Calls(AddToInventoryMethod))
            {
                replacements++;

                // Replace the original call while preserving its IL labels and blocks.
                instruction.opcode = OpCodes.Call;
                instruction.operand = AddMiningResourcesMethod;
            }

            yield return instruction;
        }

        Plugin.LogPatchResult(replacements);
    }
}
