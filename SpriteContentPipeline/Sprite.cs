using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GamePhase.Graphics
{
  public class Sprite
  {
    public Position2D Position { get; set; }
    public SpriteStrip Strip { get; set; }
    private int frameIndex = 0;
    public int FrameIndex {
      get
      {
        return frameIndex;
      }
      set
      {
        if(value >= 0 && value < Strip.FrameCount)
        {
          frameIndex = value;
          SetFrameDuration();
        }
      }
    }

    private float frameDuration;
    private float animationTimer;

    public Sprite(SpriteStrip strip)
    {
      Strip = strip;
      frameDuration = 1f / Strip.AnimationSpeed;
      animationTimer = 0f;
    }

    public void Update(float deltaTime)
    {
      animationTimer += deltaTime;
      if (animationTimer >= frameDuration)
        IncreaseFrame();
    }

    public void Draw()
    {
      // Draw the sprite
    }

    public void IncreaseFrame()
    {
      frameIndex++;
      if (frameIndex >= Strip.FrameCount)
        frameIndex = 0;
      SetFrameDuration();
    }

    public void DecreaseFrame()
    {
      frameIndex--;
      if (frameIndex < 0)
        frameIndex = Strip.FrameCount - 1;
      SetFrameDuration();
    }

    private void SetFrameDuration()
    {
      if (Strip.FrameSpeeds.Count > FrameIndex && Strip.FrameSpeeds[FrameIndex] > 0)
        frameDuration = 1f / Strip.FrameSpeeds[FrameIndex];
      else
        frameDuration = 1f / Strip.AnimationSpeed;
      // Reset animation timer
      animationTimer = 0f;
    }
  }

  [Serializable]
  [XmlRoot("spriteStrip")]
  public class SpriteStrip
  {
    [XmlElement("name")]
    public string Name { get; set; }

    [XmlElement("origin")]
    public Position2D Origin { get; set; }

    [XmlElement("boundingBox")]
    public BoundingBox2D BoundingBox { get; set; }

    [XmlElement("size")]
    public Size2D Size { get; set; }

    [XmlElement("frameCount")]
    public int FrameCount { get; set; }

    [XmlElement("animationSpeed")]
    public int AnimationSpeed { get; set; } = 15;

    [XmlArray("frameSpeeds")]
    [XmlArrayItem("frameSpeed")]
    public List<int> FrameSpeeds { get; set; }

    [XmlArray("anchors")]
    [XmlArrayItem("anchor")]
    public List<SpriteAnchor> Anchors { get; set; }

    // The dictionary is not used here (yet)
    //private Dictionary<string, SpriteAnchor> anchorsDict;

    public SpriteStrip()
    {
      Origin = new Position2D();
      Size = new Size2D();
      BoundingBox = new BoundingBox2D();
      FrameSpeeds = new List<int>();
      Anchors = new List<SpriteAnchor>();
      //anchorsDict = new Dictionary<string, SpriteAnchor>();
    }

    public void ResetFrameSpeeds()
    {
      if (FrameSpeeds != null)
        FrameSpeeds.Clear();
      else
        FrameSpeeds = new List<int>();
      for (int i = 0; i < FrameCount; i++)
      {
        FrameSpeeds.Add(0);
      }
    }

    public void ResetAnchors()
    {
      if (Anchors != null)
        Anchors.Clear();
      else
        Anchors = new List<SpriteAnchor>();
    }

    public bool AnchorExists(string name)
    {
      for (int i = 0; i < Anchors.Count; i++)
      {
        if (Anchors[i].Name == name)
          return true;
      }
      return false;
    }

  }

  [Serializable]
  public class SpriteAnchor
  {
    [XmlElement("name")]
    public string Name { get; set; }

    [XmlArray("frames")]
    [XmlArrayItem("frame")]
    public List<SpriteAnchorFrame> Frames { get; set; }

    public SpriteAnchor()
    {
      Name = "";
      Frames = new List<SpriteAnchorFrame>();
    }

    public SpriteAnchor(string name, int frameCount)
    {
      Name = name;
      Frames = new List<SpriteAnchorFrame>();
      for (int i = 0; i < frameCount; i++)
      {
        SpriteAnchorFrame frame = new SpriteAnchorFrame();
        Frames.Add(frame);
      }
    }
  }

  [Serializable]
  public class SpriteAnchorFrame
  {
    [XmlAttribute("x1")]
    public int X1 { get; set; } = 0;

    [XmlAttribute("y1")]
    public int Y1 { get; set; } = 0;

    [XmlAttribute("x2")]
    public int X2 { get; set; } = 0;

    [XmlAttribute("y2")]
    public int Y2 { get; set; } = 0;

    [XmlAttribute("angle")]
    public int Angle { get; set; } = 0;

    [XmlAttribute("length")]
    public int Length { get; set; } = 0;

    [XmlAttribute("visible")]
    public bool Visible { get; set; } = true;

    public SpriteAnchorFrame()
    {

    }

    public int CalculateLength()
    {
      // calculate delta x and delta y between the two points
      double deltaX = Math.Pow((X2 - X1), 2);
      double deltaY = Math.Pow((Y2 - Y1), 2);
      // pythagras theorem for distance
      Length = (int)Math.Sqrt(deltaY + deltaX);
      return Length;
    }

    public int CalculateAngle()
    {
      int xDiff = X2 - X1;
      int yDiff = Y1 - Y2;
      Angle = (int)(Math.Atan2((double)yDiff, (double)xDiff) * 180.0 / Math.PI);
      if (Angle < 0)
        Angle = Angle + 360;
      return Angle;
    }
  }
}
