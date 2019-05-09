using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteToolBox
{
  class TextureSprite
  {
    public Texture2D Texture { get; set; }
    public int XCenter { get; set; }
    public int YCenter { get; set; }

    public TextureSprite(Texture2D texture)
    {
      Texture = texture;
      CalculateCenter();
    }

    public TextureSprite(GraphicsDevice graphics, Bitmap bitmap)
    {
      Texture = TextureSprite.GetTexture(graphics, bitmap);
      CalculateCenter();
    }

    private void CalculateCenter()
    {
      XCenter = Texture.Width / 2 + 1;
      YCenter = Texture.Height / 2 + 1;
      if (Texture.Width % 2 == 1)
        XCenter -= 1;
      if (Texture.Height % 2 == 1)
        YCenter -= 1;
    }

    public static Texture2D GetTexture(GraphicsDevice graphics, Bitmap bitmap)
    {
      Texture2D tex = null;
      using (MemoryStream s = new MemoryStream())
      {
        bitmap.Save(s, ImageFormat.Png);
        s.Seek(0, SeekOrigin.Begin);
        tex = Texture2D.FromStream(graphics, s);
      }
      return tex;
    }
  }
}
