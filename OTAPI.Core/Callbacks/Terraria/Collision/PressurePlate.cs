namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Collision
    {
        internal static void PressurePlate(int x, int y, global::Terraria.Entity entity)
        {
            if(Hooks.Collision.PressurePlate != null
                && Hooks.Collision.PressurePlate(ref x, ref y, ref entity) == HookResult.Cancel
            )
            {
                return;
            }

            //In the patcher the below code is removed so we must action the logic ourselves.
            global::Terraria.Wiring.HitSwitch(x, y);
            global::Terraria.NetMessage.SendData(59, number: x, number2: y);
        }
    }
}
