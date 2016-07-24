namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        internal static bool GetInputText(ref string result, string chatText)
        {
            if (Hooks.Input.GetText?.Invoke(ref chatText) == HookResult.Cancel)
            {
                //The method we use to inject the non void callback expects the
                //first parameter to be the result for when it's to return the
                //value from the vanilla method. It does not expect that the other
                //parameters may be what we care about.
                //Essentially this means the actual value needs to be applied...
                result = chatText;
                return false;
            }

            return true;
        }
    }
}