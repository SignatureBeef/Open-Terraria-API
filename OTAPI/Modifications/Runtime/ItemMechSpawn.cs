namespace OTAPI.Modifications.Runtime
{
    [Modification("Modifying Terraria.Item.MechSpawn")]
    class ItemMechSpawn : NpcMechSpawn
    {
        public ItemMechSpawn()
        {
            IL.Terraria.Item.MechSpawn += il => Modify(il, (bool returnValue, float x, float y, int type, int num, int num2, int num3, int i, Microsoft.Xna.Framework.Vector2 vector, float num6) =>
            {
                //Console.WriteLine($"Terraria.Item.MechSpawn returnValue={returnValue}, x={x}, y={y}, type={type}, num={num}, num2={num2}, num3={num3}, i={i}, vector={vector.X},{vector.Y}, num6={num6}");
                if (Hooks.Item.MechSpawn == null || Hooks.Item.MechSpawn(x, y, type, num, num2, num3, i, vector, num6) == HookResult.Continue)
                {
                    return returnValue;
                }
                return false;
            });
        }
    }
}
