using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using GamePhase.Graphics;

namespace GamePhase.Utilities
{
  public class IO
  {
    public static T LoadClassInstance<T>(string fileName)
    {
      XmlSerializer deserializer = new XmlSerializer(typeof(T));
      TextReader textReader = new StreamReader(fileName);
      T instance = (T)deserializer.Deserialize(textReader);
      textReader.Close();
      return instance;
    }

    public static void SaveClassInstance<T>(T instance, string fileName)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      StreamWriter writer = new StreamWriter(fileName);
      serializer.Serialize(writer, instance);
      writer.Close();
    }

    public static void SaveSpriteStrip(SpriteStrip sprite, string fileName)
    {
      XmlElement elemParent, elem1, elem2;

      // Create an xml document
      XmlDocument doc = new XmlDocument();

      // Create the root element
      elemParent = doc.CreateElement("XnaContent");
      doc.AppendChild(elemParent);

      // Create the asset element
      elem1 = doc.CreateElement("Asset");
      elemParent.AppendChild(elem1);
      elem1.SetAttribute("Type", "GamePhase.Graphics.SpriteStrip");

      // Set elem1 as the parent element
      elemParent = elem1;

      // Name
      elemParent.AppendChild(doc.CreateElement("Name")).InnerText = sprite.Name;

      // Origin
      elem1 = doc.CreateElement("Origin");
      elem1.AppendChild(doc.CreateElement("X")).InnerText = sprite.Origin.X.ToString();
      elem1.AppendChild(doc.CreateElement("Y")).InnerText = sprite.Origin.Y.ToString();
      elemParent.AppendChild(elem1);

      // Bounding Box
      elem1 = doc.CreateElement("BoundingBox");
      elem1.AppendChild(doc.CreateElement("Left")).InnerText = sprite.BoundingBox.Left.ToString();
      elem1.AppendChild(doc.CreateElement("Right")).InnerText = sprite.BoundingBox.Right.ToString();
      elem1.AppendChild(doc.CreateElement("Top")).InnerText = sprite.BoundingBox.Top.ToString();
      elem1.AppendChild(doc.CreateElement("Bottom")).InnerText = sprite.BoundingBox.Bottom.ToString();
      elemParent.AppendChild(elem1);

      // Size
      elem1 = doc.CreateElement("Size");
      elem1.AppendChild(doc.CreateElement("Width")).InnerText = sprite.Size.Width.ToString();
      elem1.AppendChild(doc.CreateElement("Height")).InnerText = sprite.Size.Height.ToString();
      elemParent.AppendChild(elem1);

      // FrameCount
      elemParent.AppendChild(doc.CreateElement("FrameCount")).InnerText = sprite.FrameCount.ToString();

      // AnimationSpeed
      elemParent.AppendChild(doc.CreateElement("AnimationSpeed")).InnerText = sprite.AnimationSpeed.ToString();

      // FrameSpeeds
      elemParent.AppendChild(doc.CreateElement("FrameSpeeds")).InnerText = String.Join(" ", sprite.FrameSpeeds);

      // Anchors
      elem1 = doc.CreateElement("Anchors");
      elemParent.AppendChild(elem1);
      elemParent = elem1;
      foreach (SpriteAnchor anchor in sprite.Anchors)
      {
        elem1 = doc.CreateElement("Item");
        elemParent.AppendChild(elem1);
        elem1.AppendChild(doc.CreateElement("Name")).InnerText = anchor.Name;

        elem2 = doc.CreateElement("Frames");
        elem1.AppendChild(elem2);
        elem1 = elem2;
        // Frames
        foreach (SpriteAnchorFrame frame in anchor.Frames)
        {
          elem2 = doc.CreateElement("Item");
          elem1.AppendChild(elem2);
          elem2.AppendChild(doc.CreateElement("X1")).InnerText = frame.X1.ToString();
          elem2.AppendChild(doc.CreateElement("Y1")).InnerText = frame.Y1.ToString();
          elem2.AppendChild(doc.CreateElement("X2")).InnerText = frame.X2.ToString();
          elem2.AppendChild(doc.CreateElement("Y2")).InnerText = frame.Y2.ToString();
          elem2.AppendChild(doc.CreateElement("Angle")).InnerText = frame.Angle.ToString();
          elem2.AppendChild(doc.CreateElement("Length")).InnerText = frame.Length.ToString();
        }
      }

      // Save the XML document
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Encoding = Encoding.GetEncoding("utf-8");
      settings.Indent = true;
      XmlWriter writer = XmlWriter.Create(fileName, settings);
      doc.Save(writer);
    }

    /*public static SpriteAnim DeserializeSpriteAnim(string filename)
    {
      XmlReader reader = XmlReader.Create(filename);
      SpriteAnim sprite = IntermediateSerializer.Deserialize<SpriteAnim>(reader, "test.txt");
      return sprite;
    }*/

    /*public static void Serialize<T>(string filename, T data)
    {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      XmlWriter writer = XmlWriter.Create(filename, settings);
      IntermediateSerializer.Serialize<T>(writer, data, null);
      writer.Close();
    }*/
  }
}
