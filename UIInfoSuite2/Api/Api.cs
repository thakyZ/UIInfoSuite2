using System;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using UIInfoSuite2.Infrastructure;
using UIInfoSuite2.UIElements;

namespace UIInfoSuite2.Api;

public class Api : IUIInfoSuite2Api {
  private readonly IManifest      mod;
  private readonly Action<string> DeprecationWarner;

  internal Api(IManifest mod, Action<string> DeprecationWarner)
  {
    this.mod               = mod;
    this.DeprecationWarner = DeprecationWarner;
  }

  public Point GetNewIconPosition() {
    return IconHandler.Handler.GetNewIconPosition();
  }

  public bool IsRenderingNormally() {
    return UIElementUtils.IsRenderingNormally();
  }
}
