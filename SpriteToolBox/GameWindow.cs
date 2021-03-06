﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;

using GamePhase.Graphics;

namespace SpriteToolBox
{
  public class GameWindow : WpfGame
  {
    private WpfGraphicsDeviceService graphicsDeviceManager;
    private SpriteBatch spriteBatch;

    private WpfKeyboard keyboard;
    private WpfMouse mouse;

    public Sprite SpriteEdited { get; set; }
    private List<Texture2D> spriteFrames;

    TextureSprite texRectangle = null;
    TextureSprite texPixel = null;
    TextureSprite texCursor = null;
    TextureSprite texOrigin = null;
    TextureSprite texAnchor1 = null;
    TextureSprite texAnchor2 = null;

    public System.Drawing.Point MousePos { get; set; }

    public int ShowAnchor { get; set; } = -1;
    public bool ShowOrigin { get; set; }
    public bool ShowBoundingBox { get; set; }
    public bool Playing { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }

    private Color bgColor = Color.Black;

    // Scale
    private int zoomLevel = 6;
    public int ZoomLevel
    {
      get
      {
        return zoomLevel;
      }
      set
      {
        if ((value > 0) && (value < 40))
        {
          zoomLevel = value;
        }
      }
    }
    public void ZoomIn()
    {
      if (zoomLevel < 40)
        zoomLevel++;
    }
    public void ZoomOut()
    {
      if (zoomLevel > 1)
        zoomLevel--;
    }

    public GraphicsDevice GetGraphicsDevice()
    {
      return GraphicsDevice;
    }

    public WpfGraphicsDeviceService GetGraphicsDeviceManager()
    {
      return graphicsDeviceManager;
    }

    public void SetBackgroundColor(System.Windows.Media.Color color)
    {
      bgColor = new Color((int)color.R, (int)color.G, (int)color.B, 255);
    }

    protected override void Initialize()
    {
      // must be initialized. required by Content loading and rendering (will add itself to the Services)
      // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
      // be called inside Initialize (before base.Initialize())
      graphicsDeviceManager = new WpfGraphicsDeviceService(this);

      // must be called after the WpfGraphicsDeviceService instance was created
      base.Initialize();

      // Create a spritebatch for drawing sprites
      spriteBatch = new SpriteBatch(GraphicsDevice);

      //graphicsDeviceManager.PreferMultiSampling = false;
      //graphicsDeviceManager.ApplyChanges();

      // wpf and keyboard need reference to the host control in order to receive input
      // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
      keyboard = new WpfKeyboard(this);
      mouse = new WpfMouse(this);

      spriteFrames = new List<Texture2D>();

      Texture2D tex = new Texture2D(GraphicsDevice, 1, 1);
      tex.SetData(new Color[] { Color.Red });
      texRectangle = new TextureSprite(tex);

      tex = new Texture2D(GraphicsDevice, 1, 1);
      tex.SetData(new Color[] { Color.LightBlue });
      texPixel = new TextureSprite(tex);

      // content loading now possible
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      //gameShip = Content.Load<Model>("Blender2p63");
    }

    protected override void Update(GameTime gameTime)
    {
      // every update we can now query the keyboard & mouse for our WpfGame
      var mouseState = mouse.GetState();
      var keyboardState = keyboard.GetState();

      if (Playing)
        SpriteEdited.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

      base.Update(gameTime);
    }

    private void DrawSprite(Texture2D sprite, int x, int y, int scale)
    {
      spriteBatch.Draw(sprite, new Rectangle(x, y, sprite.Width * zoomLevel, sprite.Height * zoomLevel), new Rectangle(0,0,sprite.Width,sprite.Height), new Color(255, 255, 255, 255));
    }

    protected override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);

      GraphicsDevice.Clear(bgColor);

      spriteBatch.Begin(samplerState: SamplerState.PointClamp);
      //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
      //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

      if (SpriteEdited != null)
      {
        if (spriteFrames.Count > SpriteEdited.FrameIndex)
          DrawSprite(spriteFrames[SpriteEdited.FrameIndex], OffsetX, OffsetY, zoomLevel);

        if (ShowBoundingBox)
          spriteBatch.Draw(texRectangle.Texture, new Rectangle(OffsetX + ScreenValue(SpriteEdited.Strip.BoundingBox.Left), OffsetY + ScreenValue(SpriteEdited.Strip.BoundingBox.Top), ScreenValue(SpriteEdited.Strip.BoundingBox.Right) - ScreenValue(SpriteEdited.Strip.BoundingBox.Left) + zoomLevel, ScreenValue(SpriteEdited.Strip.BoundingBox.Bottom) - ScreenValue(SpriteEdited.Strip.BoundingBox.Top) + zoomLevel), new Color(255, 255, 255, 255) * 0.3f);

        if (texOrigin != null && ShowOrigin)
          spriteBatch.Draw(texOrigin.Texture, new Rectangle(OffsetX + ScreenValue(SpriteEdited.Strip.Origin.X) - texOrigin.XCenter * zoomLevel, OffsetY + ScreenValue(SpriteEdited.Strip.Origin.Y) - texOrigin.YCenter * zoomLevel, texOrigin.Texture.Width * zoomLevel, texOrigin.Texture.Height * zoomLevel), new Color(255, 255, 255, 255) * 0.6f);

        if (texAnchor1 != null && ShowAnchor >= 0)
        {
          SpriteAnchorFrame frame = SpriteEdited.Strip.Anchors[ShowAnchor].Frames[SpriteEdited.FrameIndex];
          spriteBatch.Draw(texAnchor2.Texture, new Rectangle(OffsetX + ScreenValue(frame.X2) - texAnchor2.XCenter * zoomLevel, OffsetY + ScreenValue(frame.Y2) - texAnchor2.YCenter * zoomLevel, texAnchor2.Texture.Width * zoomLevel, texAnchor2.Texture.Height * zoomLevel), new Color(255, 255, 255, 255) * 0.6f);
          spriteBatch.Draw(texAnchor1.Texture, new Rectangle(OffsetX + ScreenValue(frame.X1) - texAnchor1.XCenter * zoomLevel, OffsetY + ScreenValue(frame.Y1) - texAnchor2.YCenter * zoomLevel, texAnchor1.Texture.Width * zoomLevel, texAnchor1.Texture.Height * zoomLevel), new Color(255, 255, 255, 255) * 0.6f);
        }

        if (texCursor != null)
          DrawSprite(texCursor.Texture, OffsetX + ScreenValue(MousePos.X) - texCursor.XCenter * zoomLevel, OffsetY + ScreenValue(MousePos.Y) - texCursor.YCenter * zoomLevel, zoomLevel);
          //spriteBatch.Draw(texCursor.Texture, new Rectangle(OffsetX + ScreenValue(MousePos.X) - texCursor.XCenter * zoomLevel, OffsetY + ScreenValue(MousePos.Y) - texCursor.YCenter * zoomLevel, texCursor.Texture.Width * zoomLevel, texCursor.Texture.Height * zoomLevel), new Color(255, 255, 255, 255));
      }

      spriteBatch.End();
    }

    public void SetCursor(System.Drawing.Bitmap bitmap)
    {
      texCursor = new TextureSprite(GraphicsDevice, bitmap);
    }

    public void SetOriginSprite(System.Drawing.Bitmap bitmap)
    {
      texOrigin = new TextureSprite(GraphicsDevice, bitmap);
    }

    public void SetAnchorSprites(System.Drawing.Bitmap bitmap1, System.Drawing.Bitmap bitmap2)
    {
      texAnchor1 = new TextureSprite(GraphicsDevice, bitmap1);
      texAnchor2 = new TextureSprite(GraphicsDevice, bitmap2);
    }

    public void SetSprite(List<System.Drawing.Bitmap> bitmaps)
    {
      if (bitmaps.Count > 0)
      {
        // Clear current list of frames
        foreach (Texture2D texture in spriteFrames)
        {
          // Delete texture
        }
        spriteFrames.Clear();

        // Add all frames
        foreach (System.Drawing.Bitmap bitmap in bitmaps)
        {
          Texture2D tex = TextureSprite.GetTexture(GraphicsDevice, bitmap);
          spriteFrames.Add(tex);
        }
      }
    }

    private int ScreenValue(int x)
    {
      return x * zoomLevel;
    }
  }
}
