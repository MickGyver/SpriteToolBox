using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

using GamePhase.Graphics;
using GamePhase.Utilities;

namespace SpriteToolBox
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    Sprite sprite;
    SpriteStrip spriteStrip;
    string spritePath = "";

    bool ignoreOriginChange = false;
    bool ignoreBoundingBoxChange = false;
    bool ignoreFrameSpeedChange = false;
    bool resourcesLoaded = false;

    bool keySpace = false;
    int mouseXPrev = 0;
    int mouseYPrev = 0;

    public MainWindow()
    {
      InitializeComponent();

      System.Drawing.Color color = Properties.Settings.Default.BackgroundColor;
      System.Windows.Media.Color colorMedia = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
      colorDialog.SelectedColor = colorMedia;
      chkKeepData.IsChecked = Properties.Settings.Default.KeepData;
      spriteWindow.ZoomLevel = Properties.Settings.Default.ZoomLevel;
      spriteWindow.Playing = Properties.Settings.Default.Playing;
      spriteWindow.SetBackgroundColor(colorMedia);

      if (spriteWindow.Playing)
        btnPlay.Content = "Stop";

      valZoom.Text = (spriteWindow.ZoomLevel * 100).ToString() + "%";

      spriteStrip = new SpriteStrip();
      sprite = new Sprite(spriteStrip);
      spriteWindow.SpriteEdited = sprite;
    }

    private void LoadResources()
    {
      System.Drawing.Image img = Bitmap.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sprites", "cursor_normal.png"));
      Bitmap bitmap = new Bitmap(img);
      spriteWindow.SetCursor(bitmap);

      img = Bitmap.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sprites", "origin.png"));
      bitmap = new Bitmap(img);
      spriteWindow.SetOriginSprite(bitmap);

      img = Bitmap.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sprites", "anchor1.png"));
      bitmap = new Bitmap(img);
      img = Bitmap.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sprites", "anchor2.png"));
      Bitmap bitmap2 = new Bitmap(img);
      spriteWindow.SetAnchorSprites(bitmap, bitmap2);
    }

    private void addAnchor()
    {
      if (txtAnchorName.Text.Length > 0)
      {
        if (!sprite.Strip.AnchorExists(txtAnchorName.Text))
        {
          SpriteAnchor anchor = new SpriteAnchor(txtAnchorName.Text, sprite.Strip.FrameCount);
          sprite.Strip.Anchors.Add(anchor);
          lstAnchors.Items.Add(anchor.Name);
          lstAnchors.SelectedIndex = lstAnchors.Items.Count - 1;
          txtAnchorName.Text = "";
        }
      }
    }

    private void updateAnchorInfo()
    {
      if (spriteWindow.ShowAnchor >= 0)
      {
        SpriteAnchorFrame frame = sprite.Strip.Anchors[spriteWindow.ShowAnchor].Frames[sprite.FrameIndex];
        valDistance.Text = frame.Length.ToString() + "px";
        valAngle.Text = frame.Angle.ToString() + "°";
      }
    }

    private void UpdateBBOxGUIValues()
    {
      valBBoxSize.Text = (1 + sprite.Strip.BoundingBox.Right - sprite.Strip.BoundingBox.Left).ToString() + "," + (1 + sprite.Strip.BoundingBox.Bottom - sprite.Strip.BoundingBox.Top).ToString() + "px";
    }

    private bool LoadSpriteStrip(string fileName)
    {
      ignoreOriginChange = ignoreBoundingBoxChange = ignoreFrameSpeedChange = true;

      List<Bitmap> spriteFrames = new List<Bitmap>();
      FileInfo fInfo = new FileInfo(fileName);

      string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName).Replace(' ', '_');
      string strip = "";
      string spriteName = "";
      int frames = 0;

      strip = "";
      if (fInfo.Name.LastIndexOf("strip") > -1)
      {
        spriteName = fileNameNoExt.Substring(0, fInfo.Name.LastIndexOf("strip"));
        spriteName = spriteName.Trim(new char[] { ' ', '-', '_', '.', ',' });

        strip = fInfo.Name.Substring(fInfo.Name.LastIndexOf("strip") + 5);
        bool isDigit = true;
        int index = 0;
        int digit;
        while (isDigit == true)
        {
          if (!int.TryParse(strip.Substring(index, 1), out digit))
            isDigit = false;
          else
            index++;
        }
        strip = strip.Substring(0, index);
      }
      else
        spriteName = fileNameNoExt;

      // Set number of frames
      frames = 0;
      int.TryParse(strip, out frames);
      if (frames == 0)
        frames = 1;

      Bitmap bm = new Bitmap(fileName);
      FrameDimension dimension = new FrameDimension(bm.FrameDimensionsList[0]);
      Bitmap bm_cropped;
      Rectangle rect;
      int w = bm.Width / frames;
      int h = bm.Height;
      int framesGif = bm.GetFrameCount(dimension);

      valWidth.Text = w.ToString() + "px";
      valHeight.Text = h.ToString() + "px";

      if (bm.Width % frames == 0)
      {
        // Sprite frames
        // Animated GIF
        if (framesGif > 1 && frames <= 1)
        {
          for (int i = 0; i < framesGif; i++)
          {
            bm.SelectActiveFrame(dimension, i);
            bm_cropped = (Bitmap)bm.Clone();
            spriteFrames.Add(bm_cropped);
          }
          frames = framesGif;
        }
        // Strips etc. 
        else
        {
          for (int i = 0; i < frames; i++)
          {
            rect = new Rectangle(i * w, 0, w, h);
            bm_cropped = (Bitmap)bm.Clone(rect, bm.PixelFormat);
            spriteFrames.Add(bm_cropped);
          }
        }

        spritePath = fileName;
        valFrameCount.Text = frames.ToString();

      }
      else
      {
        return false;
        //logMessage.Comment = "Width / frames mismatch!";
      }

      string configPath = Path.Combine(fInfo.DirectoryName, spriteName + ".xsprite");
      if (File.Exists(configPath))
      {
        spriteStrip = IO.LoadClassInstance<SpriteStrip>(configPath);
      }
      else
      {
        BoundingBox2D bbox = new BoundingBox2D(spriteStrip.BoundingBox.Left, spriteStrip.BoundingBox.Top, spriteStrip.BoundingBox.Right, spriteStrip.BoundingBox.Bottom);
        Position2D origin = new Position2D(spriteStrip.Origin.X, spriteStrip.Origin.Y);
        spriteStrip = new SpriteStrip();
        if (chkKeepData.IsChecked ?? true)
        {
          spriteStrip.BoundingBox = bbox;
          spriteStrip.Origin = origin;
        }
        spriteStrip.Name = spriteName;
        spriteStrip.FrameCount = frames;
        spriteStrip.Size.Width = w;
        spriteStrip.Size.Height = h;
        spriteStrip.ResetFrameSpeeds();
      }
      sprite.Strip = spriteStrip;
      spriteWindow.SpriteEdited = sprite;
      spriteWindow.SetSprite(spriteFrames);
      spriteWindow.ShowAnchor = -1;
      populateFrameSpeedList();
      populateAnchorList();
      updateGUI();

      ignoreOriginChange = ignoreBoundingBoxChange = ignoreFrameSpeedChange = false;

      return true;
    }

    private void populateFrameSpeedList()
    {
      lstFrameSpeeds.Items.Clear();
      for (int i = 0; i < sprite.Strip.FrameCount; i++)
      {
        lstFrameSpeeds.Items.Add(sprite.Strip.FrameSpeeds[i].ToString());
      }
    }

    private void populateAnchorList()
    {
      lstAnchors.Items.Clear();
      for (int i = 0; i < sprite.Strip.Anchors.Count; i++)
      {
        lstAnchors.Items.Add(sprite.Strip.Anchors[i].Name);
      }
    }

    private void updateGUI()
    {
      numOriginX.Value = sprite.Strip.Origin.X;
      numOriginY.Value = sprite.Strip.Origin.Y;
      numBoundingBoxX1.Value = sprite.Strip.BoundingBox.Left;
      numBoundingBoxY1.Value = sprite.Strip.BoundingBox.Top;
      numBoundingBoxX2.Value = sprite.Strip.BoundingBox.Right;
      numBoundingBoxY2.Value = sprite.Strip.BoundingBox.Bottom;
      UpdateBBOxGUIValues();
      numAnimationSpeed.Value = sprite.Strip.AnimationSpeed;
    }

    private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
    {
      if (spriteWindow != null)
        spriteWindow.SetBackgroundColor((System.Windows.Media.Color)e.NewValue);
    }

    private void BtnLoadSprite_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Image files|*.png;*.gif";
      if (openFileDialog.ShowDialog() == true)
      {
        LoadSpriteStrip(openFileDialog.FileName);

        if (!resourcesLoaded)
          LoadResources();
      }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
      if (File.Exists(spritePath))
      {
        string folder = Path.GetDirectoryName(spritePath);
        string fileName = spriteStrip.Name + ".xsprite";
        IO.SaveClassInstance<SpriteStrip>(sprite.Strip, Path.Combine(folder, fileName));
      }
    }

    private void BtnPlay_Click(object sender, RoutedEventArgs e)
    {
      spriteWindow.Playing = !spriteWindow.Playing;
      if (spriteWindow.Playing)
        btnPlay.Content = "Stop";
      else
        btnPlay.Content = "Play";
    }

    private void NumAnimationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (sprite != null)
        sprite.Strip.AnimationSpeed = (int)numAnimationSpeed.Value;
    }

    private void NumFrameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (!ignoreFrameSpeedChange && lstFrameSpeeds != null)
      {
        if (lstFrameSpeeds.SelectedItems.Count > 0)
        {
          int index = lstFrameSpeeds.SelectedIndex;
          sprite.Strip.FrameSpeeds[lstFrameSpeeds.SelectedIndex] = (int)numFrameSpeed.Value;
          lstFrameSpeeds.Items[lstFrameSpeeds.SelectedIndex] = numFrameSpeed.Value.ToString();
          lstFrameSpeeds.SelectedIndex = index;
        }
      }
    }

    private void LstFrameSpeeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ignoreFrameSpeedChange = true;
      if (lstFrameSpeeds.SelectedItems.Count > 0)
      {
        int value = 0;
        string sVal = lstFrameSpeeds.Items[lstFrameSpeeds.SelectedIndex].ToString();

        if (int.TryParse(sVal, out value))
        {
          numFrameSpeed.Value = value;
        }
      }
      ignoreFrameSpeedChange = false;
    }

    private void TxtAnchorName_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        addAnchor();
    }

    private void BtnAddAnchor_Click(object sender, RoutedEventArgs e)
    {
      addAnchor();
    }

    private void BtnRemoveAnchor_Click(object sender, RoutedEventArgs e)
    {
      if (lstAnchors.SelectedItems.Count > 0)
      {
        int anchor = lstAnchors.SelectedIndex;
        if (spriteWindow.ShowAnchor == anchor)
        {
          spriteWindow.ShowAnchor = -1;
          valAngle.Text = "";
          valDistance.Text = "";
        }
        sprite.Strip.Anchors.RemoveAt(anchor);
        lstAnchors.Items.RemoveAt(anchor);
      }
    }

    private void LstAnchors_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (lstAnchors.SelectedItems.Count > 0)
      {
        // Hide origin when showing an anchor
        spriteWindow.ShowOrigin = false;
        btnShowOrigin.Content = "Show";
        // Show selected anchor
        spriteWindow.ShowAnchor = lstAnchors.SelectedIndex;
        updateAnchorInfo();
      }
    }

    private void NumOriginX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (!ignoreOriginChange && sprite != null)
      {
        sprite.Strip.Origin.Y = (int)numOriginX.Value;
        sprite.Strip.Origin.Y = (int)numOriginY.Value;
      }
    }

    private void BtnShowOrigin_Click(object sender, RoutedEventArgs e)
    {
      spriteWindow.ShowOrigin = !spriteWindow.ShowOrigin;
      if (spriteWindow.ShowOrigin)
      {
        btnShowOrigin.Content = "Hide";
        lstAnchors.UnselectAll();
        spriteWindow.ShowAnchor = -1;
        valAngle.Text = "";
        valDistance.Text = "";
      }
      else
        btnShowOrigin.Content = "Show";
    }

    private void NumBoundingBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (!ignoreBoundingBoxChange && sprite != null)
      {
        sprite.Strip.BoundingBox.Left = (int)numBoundingBoxX1.Value;
        sprite.Strip.BoundingBox.Top = (int)numBoundingBoxY1.Value;
        sprite.Strip.BoundingBox.Right = (int)numBoundingBoxX2.Value;
        sprite.Strip.BoundingBox.Bottom = (int)numBoundingBoxY2.Value;
        UpdateBBOxGUIValues();
      }
    }

    private void BtnShowBoundingBox_Click(object sender, RoutedEventArgs e)
    {
      spriteWindow.ShowBoundingBox = !spriteWindow.ShowBoundingBox;
      if (spriteWindow.ShowBoundingBox)
        btnShowBoundingBox.Content = "Hide";
      else
        btnShowBoundingBox.Content = "Show";
    }

    private void SpriteWindow_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
        e.Effects = DragDropEffects.Copy;
      else
        e.Effects = DragDropEffects.None;
    }

    private void SpriteWindow_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

        if (Directory.Exists(paths[0]))
        {
          // Directory
        }
        else if (File.Exists(paths[0]))
        {
          FileInfo fInfo = new FileInfo(paths[0]);
          if (fInfo.Extension.ToLower() == ".png" || fInfo.Extension.ToLower() == ".gif")
          {
            LoadSpriteStrip(paths[0]);
          }
        }

        if (!resourcesLoaded)
          LoadResources();
      }
    }

    private void SpriteWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Space)
        keySpace = true;
    }

    private void SpriteWindow_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Space)
        keySpace = false;
    }

    private void SpriteWindow_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
      {
        if (e.Delta < 0)
          spriteWindow.ZoomOut();
        else
          spriteWindow.ZoomIn();
        valZoom.Text = (spriteWindow.ZoomLevel * 100).ToString() + "%";
      }
      else
      {
        spriteWindow.Playing = false;
        btnPlay.Content = "Play";
        if (e.Delta < 0)
          sprite.DecreaseFrame();
        else
          sprite.IncreaseFrame();
        lstFrameSpeeds.SelectedIndex = sprite.FrameIndex;
        updateAnchorInfo();
      }
    }

    private void SpriteWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
      ignoreOriginChange = true;
      ignoreBoundingBoxChange = true;

      System.Windows.Point mousePos = e.GetPosition(spriteWindow);

      if (keySpace)
      {
        mouseXPrev = (int)mousePos.X;
        mouseYPrev = (int)mousePos.Y;
        if (e.RightButton == MouseButtonState.Pressed)
        {
          spriteWindow.OffsetX = 0;
          spriteWindow.OffsetY = 0;
        }
      }
      else
      {
        System.Drawing.Point point = new System.Drawing.Point(((int)mousePos.X - spriteWindow.OffsetX) / spriteWindow.ZoomLevel, ((int)mousePos.Y - spriteWindow.OffsetY) / spriteWindow.ZoomLevel);
        if (e.RightButton == MouseButtonState.Pressed)
        {
          sprite.Strip.BoundingBox.Left = point.X;
          sprite.Strip.BoundingBox.Right = point.X;
          sprite.Strip.BoundingBox.Top = point.Y;
          sprite.Strip.BoundingBox.Bottom = point.Y;
          numBoundingBoxX1.Value = point.X;
          numBoundingBoxY1.Value = point.Y;
          numBoundingBoxX2.Value = point.X;
          numBoundingBoxY2.Value = point.Y;
          UpdateBBOxGUIValues();
          spriteWindow.ShowBoundingBox = true;
          btnShowBoundingBox.Content = "Hide";
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
          if (spriteWindow.ShowOrigin)
          {
            sprite.Strip.Origin.X = point.X;
            sprite.Strip.Origin.Y = point.Y;
            numOriginX.Value = point.X;
            numOriginY.Value = point.Y;
          }
          else if (spriteWindow.ShowAnchor >= 0)
          {
            SpriteAnchorFrame frame = sprite.Strip.Anchors[spriteWindow.ShowAnchor].Frames[sprite.FrameIndex];
            frame.X1 = point.X;
            frame.Y1 = point.Y;
            frame.X2 = point.X;
            frame.Y2 = point.Y;
            frame.Angle = 0;
            frame.Length = 0;
            valDistance.Text = frame.Length.ToString() + "px";
            valAngle.Text = frame.Angle.ToString() + "°";
          }
        }
      }

      ignoreOriginChange = false;
      ignoreBoundingBoxChange = false;
    }

    private void SpriteWindow_MouseMove(object sender, MouseEventArgs e)
    {
      ignoreBoundingBoxChange = true;

      System.Windows.Point mousePos = e.GetPosition(spriteWindow);

      if (keySpace)
      {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
          spriteWindow.OffsetX += ((int)mousePos.X - mouseXPrev);
          spriteWindow.OffsetY += ((int)mousePos.Y - mouseYPrev);
          mouseXPrev = (int)mousePos.X;
          mouseYPrev = (int)mousePos.Y;
        }
      }
      else
      {
        System.Drawing.Point point = new System.Drawing.Point(((int)mousePos.X - spriteWindow.OffsetX) / spriteWindow.ZoomLevel, ((int)mousePos.Y - spriteWindow.OffsetY) / spriteWindow.ZoomLevel);
        valMouseX.Text = point.X.ToString();
        valMouseY.Text = point.Y.ToString();

        spriteWindow.MousePos = point;
        if (e.RightButton == MouseButtonState.Pressed)
        {
          sprite.Strip.BoundingBox.Right = point.X;
          sprite.Strip.BoundingBox.Bottom = point.Y;
          numBoundingBoxX2.Value = point.X;
          numBoundingBoxY2.Value = point.Y;
          UpdateBBOxGUIValues();
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
          if (spriteWindow.ShowAnchor >= 0)
          {
            SpriteAnchorFrame frame = sprite.Strip.Anchors[spriteWindow.ShowAnchor].Frames[sprite.FrameIndex];
            frame.X2 = point.X;
            frame.Y2 = point.Y;
            valDistance.Text = frame.CalculateLength().ToString() + "px";
            valAngle.Text = frame.CalculateAngle().ToString() + "°";
          }
        }
      }

      ignoreBoundingBoxChange = false;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      System.Windows.Media.Color color = (System.Windows.Media.Color)colorDialog.SelectedColor;
      System.Drawing.Color colorDrawing = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
      Properties.Settings.Default.BackgroundColor = colorDrawing;
      Properties.Settings.Default.ZoomLevel = spriteWindow.ZoomLevel;
      Properties.Settings.Default.Playing = spriteWindow.Playing;
      Properties.Settings.Default.KeepData = (bool)chkKeepData.IsChecked;
      Properties.Settings.Default.Save();
    }
  }
}
