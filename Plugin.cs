using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;

[BepInPlugin("castro_war.miningboost", "Mining Boost", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static int Multiplier = 5;

    private void Awake()
    {
        Logger.LogInfo("MiningBoost iniciado!");

        Harmony harmony = new Harmony("castro_war.miningboost");
        harmony.PatchAll();

        Logger.LogInfo($"Mineração multiplicada por {Multiplier}x");
    }
}


[HarmonyPatch(
    typeof(ai.behaviours.BehExtractResourcesFromBuilding),
    "execute"
)]
public static class MiningPatch
{
    static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        MethodInfo target =
            AccessTools.Method(
                typeof(Actor),
                "addToInventory",
                new Type[]
                {
                    typeof(string),
                    typeof(int)
                });

        for (int i = 0; i < codes.Count; i++)
        {
            yield return codes[i];

            if (
                codes[i].opcode == OpCodes.Callvirt &&
                codes[i].operand is MethodInfo method &&
                method == target
            )
            {
                yield return new CodeInstruction(
                    OpCodes.Ldc_I4,
                    Plugin.Multiplier
                );

                yield return new CodeInstruction(
                    OpCodes.Mul
                );
            }
        }
    }
}