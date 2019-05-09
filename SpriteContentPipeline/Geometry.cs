using System;
using System.Xml.Serialization;

namespace GamePhase.Graphics
{
  [Serializable]
  public class Position2D
  {
    [XmlAttribute("x")]
    public int X { get; set; }
    [XmlAttribute("y")]
    public int Y { get; set; }

    public Position2D()
    {
      X = Y = 0;
    }

    public Position2D(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  [Serializable]
  public class Size2D
  {
    [XmlAttribute("width")]
    public int Width { get; set; }
    [XmlAttribute("height")]
    public int Height { get; set; }

    public Size2D()
    {
      Width = Height = 0;
    }

    public Size2D(int width, int height)
    {
      Width = width;
      Height = height;
    }
  }

  [Serializable]
  public class Line2D
  {
    [XmlAttribute("x1")]
    public int X1 { get; set; }
    [XmlAttribute("y1")]
    public int Y1 { get; set; }
    [XmlAttribute("x2")]
    public int X2 { get; set; }
    [XmlAttribute("y2")]
    public int Y2 { get; set; }

    public Line2D()
    {
      X1 = Y1 = X2 = Y2 = 0;
    }

    public Line2D(int x1, int y1, int x2, int y2)
    {
      X1 = x1;
      Y1 = y1;
      X2 = x2;
      Y2 = y2;
    }
  }

  [Serializable]
  public class BoundingBox2D
  {
    [XmlAttribute("left")]
    public int Left { get; set; }
    [XmlAttribute("right")]
    public int Right { get; set; }
    [XmlAttribute("top")]
    public int Top { get; set; }
    [XmlAttribute("bottom")]
    public int Bottom { get; set; }

    public BoundingBox2D()
    {
      Left = Right = Top = Bottom = 0;
    }

    public BoundingBox2D(int left, int top, int right, int bottom)
    {
      Left = left;
      Right = right;
      Top = top;
      Bottom = bottom;
    }
  }
}
