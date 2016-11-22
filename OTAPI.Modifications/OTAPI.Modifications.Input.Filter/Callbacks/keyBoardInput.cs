using System.Windows.Forms;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class keyBoardInput
	{
		internal static IMessageFilter CreateMessageFilter()
		{
			IMessageFilter filter = Hooks.Input.CreateMessageFilter?.Invoke();

			return filter ?? new global::Terraria.keyBoardInput.inKey();
		}
	}
}