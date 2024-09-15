using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley.Menus;

using StardewValley;

using UIInfoSuite2.Infrastructure;

namespace UIInfoSuite2.UIElements.Interfaces;
using Microsoft.Xna.Framework.Graphics;

[SuppressMessage("Blocker Code Smell", "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"")]
[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly")]
internal abstract class IconBase : IShowIcon {
#region Properties
  /// <summary>Getter property to determine if the icon should be shown.</summary>
  private          bool       ShouldShow                      { get; set; }

  /// <summary>Nullable hover text for the status icon.</summary>
  protected string? HoverText { get; set; }

  private readonly IModHelper _helper;
  private readonly IMonitor   _monitor;

  /// <summary>Instanced per screen icon of the Icon.</summary>
  protected        PerScreen<ClickableTextureComponent> Icon => new();

  /// <summary>Determines if the instance of this class is already disposed.</summary>
  private          bool                                 _disposedValue;

  /// <summary>The icon sheet that contains the icon we want to display.</summary>
  protected Texture2D? SpriteSheet { get; set; }
#endregion

#region Life cycle
  protected IconBase(IModHelper helper, IMonitor monitor)
  {
    _helper                              =  helper;
    _monitor                             =  monitor;
    _helper.Events.GameLoop.GameLaunched += OnGameLaunched;
  }

  protected virtual void Dispose(bool disposing) {}

  private void DisposeImpl(bool disposing) {
    if (_disposedValue) {
      return;
    }

    Dispose(disposing);
    ToggleOption(false);
    _helper.Events.GameLoop.GameLaunched -= OnGameLaunched;
    _disposedValue                           =  true;
  }

  ~IconBase() {
    DisposeImpl(disposing: false);
  }

  public void Dispose() {
    DisposeImpl(disposing: true);
    GC.SuppressFinalize(this);
  }

  public void ToggleOption(bool showQueenOfSauceIcon)
  {
    _helper.Events.Display.RenderingHud           -= OnRenderingHudImpl;
    _helper.Events.Display.RenderedHud            -= OnRenderedHudImpl;
    _helper.Events.GameLoop.DayStarted            -= OnDayStartedImpl;
    _helper.Events.GameLoop.UpdateTicked          -= OnUpdateTickedImpl;
    _helper.Events.GameLoop.SaveLoaded            -= OnSaveLoadedImpl;
    _helper.Events.GameLoop.OneSecondUpdateTicked -= OnOneSecondUpdateTickedImpl;

    if (showQueenOfSauceIcon)
    {
      UpdateStatusDataImpl();

      _helper.Events.GameLoop.DayStarted            += OnDayStartedImpl;
      _helper.Events.Display.RenderingHud           += OnRenderingHudImpl;
      _helper.Events.Display.RenderedHud            += OnRenderedHudImpl;
      _helper.Events.GameLoop.UpdateTicked          += OnUpdateTickedImpl;
      _helper.Events.GameLoop.SaveLoaded            += OnSaveLoadedImpl;
      _helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTickedImpl;
    }
  }
#endregion

#region Event subscriptions
  /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
    FindSpriteSheet();
  }

  /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  protected virtual void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {}

  /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e) {
    OnSaveLoaded(sender, e);
  }

  /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  protected virtual void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {}

  /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e) {
    OnUpdateTicked(sender, e);
  }

  /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  protected virtual void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e) {}

  /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e) {
    OnOneSecondUpdateTicked(sender, e);
    UpdateStatusDataImpl();
  }

  /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  protected virtual void OnDayStarted(object?   sender, DayStartedEventArgs   e) {}

  /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
  {
    OnDayStarted(sender, e);
    UpdateStatusDataImpl();
  }

  /// <inheritdoc cref="IDisplayEvents.RenderingHud"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  protected abstract void OnRenderingHud(object? sender, RenderingHudEventArgs e);

  /// <inheritdoc cref="IDisplayEvents.RenderingHud"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
  {
    OnRenderingHud(sender, e);
  }

  /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  protected virtual void OnRenderedHud(object? sender, RenderedHudEventArgs e) {}

  /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
  {
    OnRenderedHud(sender, e);

    // Show text on hover
    if (!ShouldShow || Icon.Value?.containsPoint(Game1.getMouseX(), Game1.getMouseY()) != true || string.IsNullOrEmpty(HoverText)) {
      return;
    }

    IClickableMenu.drawHoverText(Game1.spriteBatch, HoverText, Game1.dialogueFont);
  }
#endregion

#region Logic
  /// <summary>
  ///
  /// </summary>
  protected abstract Texture2D? FindSpriteSheet();

  /// <summary>
  /// Update status data. Please call the base
  /// </summary>
  protected abstract void       UpdateStatusData();

  private void UpdateStatusDataImpl() {
    FindSpriteSheetImpl();
    UpdateStatusData();
  }

  private void FindSpriteSheetImpl() {
    SpriteSheet ??= FindSpriteSheet();

    if (SpriteSheet is null) {
      _monitor.Log($"{GetType().Name}: Could not find Robin sprite sheet.", LogLevel.Warn);
    }
  }
#endregion
}
