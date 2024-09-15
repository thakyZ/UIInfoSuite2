using Microsoft.Xna.Framework;

using UIInfoSuite2.Api;

namespace UIInfoSuite2;
/// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
public interface IUIInfoSuite2Api
  : IUIInfoSuite2ApiWithObsoleteMethods // DELETE THIS LINE WHEN COPIED INTO YOUR MOD CODE
{
  /// <summary>Gets the next available position of an icon</summary>
  Point GetNewIconPosition();

  /// <summary>Checks if the screen is rendering normally.</summary>
  bool IsRenderingNormally();
}
