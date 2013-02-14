using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace MaxFontEditor.Data
{
    public class Glyph : PropertyChangedBase
    {
        byte[] glyphData;

        int id = -1;

        public Glyph(int id)
        {
            thumbnail = new Bitmap(1, 1);
            this.id = id;
            EraseGlyph();
        }

        public Glyph(int id, byte[] data)
        {
            this.id = id;

            if (data.Length > 216)
                GlyphData = data.Take(216).ToArray();
            else
                GlyphData = data;
        }

        public byte[] GlyphData
        {
            get { return glyphData; }
            set
            {
                if (glyphData == value)
                    return;

                glyphData = value;

                NotifyOfPropertyChange(() => GlyphData);
                NotifyOfPropertyChange(() => GlyphString);
                GenerateBitmap();
            }
        }

        public void SetPixel(int x, int y, bool? value)
        {
            if (x > 11)
                x = 11;
            if (y > 17)
                y = 17;

            if (value == null)
                GlyphData[(y * 12) + x] = 1;
            else if (value == true)
                GlyphData[(y * 12) + x] = 2;
            else if (value == false)
                GlyphData[(y * 12) + x] = 0;

            NotifyOfPropertyChange(() => GlyphData);
            NotifyOfPropertyChange(() => GlyphString);
            GenerateBitmap();
        }

        public int Id
        {
            get { return id; }
        }

        public IEnumerable<bool?[]> Rows
        {
            get
            {
                bool?[] row = new bool?[12];
                byte pixel = 0;

                for (int i = 0; i < 18; i++)
                {
                    for (int x = 0; x < 12; x++)
                    {
                        pixel = glyphData[(i*12) + x];

                        if (pixel == 0)
                            row[x] = false;
                        else if (pixel == 2)
                            row[x] = true;
                        else
                            row[x] = null;
                    }

                    yield return row;
                }
            }
        }

        Bitmap thumbnail;
        public Bitmap Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail == value)
                    return;

                thumbnail = value;
                NotifyOfPropertyChange(() => Thumbnail);
            }
        }

        public string GlyphString
        {
            get
            {
                return CreateGlyphString(GlyphData);
            }
            set
            {
                GlyphData = ReadGlyphString(value);

                NotifyOfPropertyChange(() => GlyphString);
                GenerateBitmap();
            }
        }

        public void EraseGlyph()
        {
            byte[] erase = new byte[216];
            
            for (int i = 0; i < erase.Length; i++)
            {
                erase[i] = 1;
            }

            GlyphData = erase;
        }

        internal string ImportImage(string path)
        {
            Bitmap b = new Bitmap(path);

            if (b.Width != 12 && b.Height != 18)
                return "Image is not 12px wide by 18px high. This image cannot be imported!";

            Color pixel;
            byte[] data = new byte[216];

            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    pixel = b.GetPixel(x, y);
                    if (pixel == Color.FromArgb(255, 255, 255))
                    {
                        data[(y * 12) + x] = 2;
                    }
                    else if (pixel == Color.FromArgb(0, 0, 0))
                    {
                        data[(y * 12) + x] = 0;
                    }
                    else
                    {
                        data[(y * 12) + x] = 1;
                    }
                }
            }

            GlyphData = data;

            return null;
        }

        public void GenerateBitmap(int scale = 2, string path = null)
        {
            int x = 12;
            int y = 18;

            Bitmap b = new Bitmap(x*scale, y*scale);

            Graphics gfx = Graphics.FromImage(b);
            
            Color transparent = Color.FromArgb(0x11, 0x9E, 0xDA);
            Color white = Color.White;
            Color black = Color.Black;

            using (Brush whiteBrush = new SolidBrush(white))
            {
                using (Brush blackBrush = new SolidBrush(black))
                {
                    gfx.Clear(transparent);

                    int r = -1;
                    int c = -1;

                    foreach (IEnumerable<bool?> row in Rows)
                    {
                        r++;
                        c = -1;

                        foreach (bool? pixel in row)
                        {
                            c++;

                            if (pixel == null)
                                continue;

                            gfx.FillRectangle(pixel == true ? whiteBrush : blackBrush, c * scale, r * scale, scale, scale);
                        }
                    }
                }
            }

            if (path == null)
                Thumbnail = b;
            else
                b.Save(path, ImageFormat.Png);

            //b.Save(String.Format("render{0}.png", id), ImageFormat.Png);

        }

        public static string CreateGlyphString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append((char)(b + 48));
            }
            return sb.ToString();
        }

        public static byte[] ReadGlyphString(string value)
        {
            byte[] data = new byte[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                data[i] = (byte)(value[i] - 48);
            }

            return data;
        }

    }
}
