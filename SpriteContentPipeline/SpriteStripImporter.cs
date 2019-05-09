using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using GamePhase.Graphics;

[ContentImporter(".xsprite", DisplayName = "Sprite Strip Importer", DefaultProcessor = "SpriteStripProcessor")]
public class SpriteStripImporter : ContentImporter<string>
{
  public override string Import(string filename, ContentImporterContext context)
  {
    string file = File.ReadAllText(filename);
    return file;
  }
}

[ContentProcessor(DisplayName = "Sprite Strip Processor")]
public class SpriteStripProcessor : ContentProcessor<string, SpriteStrip>
{
  public override SpriteStrip Process(string input, ContentProcessorContext context)
  {
    try
    {
      context.Logger.LogMessage("Processing Sprite Xml");
      TextReader inputReader = new StringReader(input);
      var serializer = new XmlSerializer(typeof(SpriteStrip));
      SpriteStrip sprite = (SpriteStrip)serializer.Deserialize(inputReader);
      return sprite;
    }
    catch (Exception ex)
    {
      context.Logger.LogMessage("Error {0}", ex);
      throw;
    }
  }
}

[ContentTypeWriter()]
public class SpriteStripWriter : ContentTypeWriter<SpriteStrip>
{
  // Content version, increase this if content structure changes
  const int _version = 1;

  protected override void Write(ContentWriter output, SpriteStrip value)
  {
    // Version
    output.Write(_version);

    output.Write(value.Name);
    output.Write(value.Origin.X);
    output.Write(value.Origin.Y);
    output.Write(value.BoundingBox.Left);
    output.Write(value.BoundingBox.Right);
    output.Write(value.BoundingBox.Top);
    output.Write(value.BoundingBox.Bottom);
    output.Write(value.Size.Width);
    output.Write(value.Size.Height);
    output.Write(value.FrameCount);
    output.Write(value.AnimationSpeed);
    // Framespeeds
    int frameCount = value.FrameSpeeds.Count;
    output.Write(frameCount);
    for (int i = 0; i < frameCount; i++)
      output.Write(value.FrameSpeeds[i]);
    // Anchors
    int anchorCount = value.Anchors.Count;
    output.Write(anchorCount);
    for (int i = 0; i < anchorCount; i++)
    {
      SpriteAnchor anchor = value.Anchors[i];
      output.Write(anchor.Name);
      // Frames
      int anchorFrameCount = anchor.Frames.Count;
      for (int j = 0; j < anchorFrameCount; j++)
      {
        output.Write(anchor.Frames[j].X1);
        output.Write(anchor.Frames[j].Y1);
        output.Write(anchor.Frames[j].X2);
        output.Write(anchor.Frames[j].Y2);
        output.Write(anchor.Frames[j].Angle);
        output.Write(anchor.Frames[j].Length);
        output.Write(anchor.Frames[j].Visible);
      }
    }
  }

  public override string GetRuntimeReader(TargetPlatform targetPlatform)
  {
    return typeof(SpriteStripReader).AssemblyQualifiedName;
  }
}

public class SpriteStripReader : ContentTypeReader<SpriteStrip>
{
  protected override SpriteStrip Read(ContentReader input, SpriteStrip sprite)
  {
    if (sprite == null)
      sprite = new SpriteStrip();

    // Get version, useful if the content structure changes
    int version = input.ReadInt32();

    sprite.Name = input.ReadString();
    sprite.Origin.X = input.ReadInt32();
    sprite.Origin.Y = input.ReadInt32();
    sprite.BoundingBox.Left = input.ReadInt32();
    sprite.BoundingBox.Right = input.ReadInt32();
    sprite.BoundingBox.Top = input.ReadInt32();
    sprite.BoundingBox.Bottom = input.ReadInt32();
    sprite.Size.Width = input.ReadInt32();
    sprite.Size.Height = input.ReadInt32();
    sprite.FrameCount = input.ReadInt32();
    sprite.AnimationSpeed = input.ReadInt32();
    // Framespeeds
    int frameCount = input.ReadInt32();
    for (int i = 0; i < frameCount; i++)
    {
      sprite.FrameSpeeds.Add(input.ReadInt32());
    }
    // Anchors
    int anchorCount = input.ReadInt32();
    for (int i = 0; i < anchorCount; i++)
    {
      SpriteAnchor anchor = new SpriteAnchor();
      anchor.Name = input.ReadString();
      int anchorFrameCount = input.ReadInt32();
      for (int j = 0; j < anchorFrameCount; j++)
      {
        SpriteAnchorFrame frame = new SpriteAnchorFrame();
        frame.X1 = input.ReadInt32();
        frame.Y1 = input.ReadInt32();
        frame.X2 = input.ReadInt32();
        frame.Y2 = input.ReadInt32();
        frame.Angle = input.ReadInt32();
        frame.Length = input.ReadInt32();
        frame.Visible = input.ReadBoolean();
        anchor.Frames.Add(frame);
      }
      sprite.Anchors.Add(anchor);
    }
    return sprite;
  }
}