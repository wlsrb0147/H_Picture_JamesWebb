// To activate the runtime extension sample, make sure the script
// is attached to a GameObject in the scene and set it active.

namespace Xarbrough.SelectionUtility.Samples
{
	using UnityEngine;
using Debug = DebugEx;

	/// <summary>
	/// Demonstrates that filters can also be added and removed dynamically
	/// to provide context-sensitive tooling in editor play mode.
	/// </summary>
	internal class CustomRuntimeFilter : MonoBehaviour
	{
		/// <summary>
		/// A minimal filter that lets all GameObjects pass.
		/// </summary>
		private readonly DataFilter myFilter = new("Test", _ => true);

		private void OnEnable()
		{
			SelectionPopupExtensions.FilterModifier = filters =>
			{
				if (filters.Contains(myFilter) == false)
					filters.Add(myFilter);

				return filters;
			};
		}

		private void OnDisable()
		{
			SelectionPopupExtensions.FilterModifier = filters =>
			{
				filters.Remove(myFilter);
				return filters;
			};
		}
	}
}
