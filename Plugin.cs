using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine.InputSystem;

namespace Seed_Changer
{
    [BepInPlugin("com.meds.seedchanger", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new("com.meds.seedchanger");
        internal static ManualLogSource Log;
        private void Awake()
        {
            // Plugin startup logic
            Log = Logger;
            harmony.PatchAll();
            Logger.LogInfo($"Seed Changer has loaded!");
        }
    }
    [HarmonyPatch]
    public class SeedChanger
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputController), "DoKeyBinding")]
        public static bool DoKeyBindingPrefix(ref InputAction.CallbackContext _context)
        {
            if ((UnityEngine.Object)MapManager.Instance != (UnityEngine.Object)null && Keyboard.current != null && _context.control == Keyboard.current[Key.F2])
            {
                AlertManager.buttonClickDelegate = new AlertManager.OnButtonClickDelegate(ActuallyChangeSeed);
                AlertManager.Instance.AlertInput(Texts.Instance.GetText("gameSeedInput"), Texts.Instance.GetText("accept").ToUpper());
                return false;
            }
            return true;
        }
        public static void ActuallyChangeSeed()
        {
            AlertManager.buttonClickDelegate -= new AlertManager.OnButtonClickDelegate(ActuallyChangeSeed);
            if (AlertManager.Instance.GetInputValue() == null)
                return;
            string upper = AlertManager.Instance.GetInputValue().ToUpper();
            if (!(upper.Trim() != ""))
                return;
            AtOManager.Instance.SetGameId(upper);
            AtOManager.Instance.SaveGame();
            OptionsManager.Instance.DoExit();
        }
    }
}
