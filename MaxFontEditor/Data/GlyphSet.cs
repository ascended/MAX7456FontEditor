using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.IO;
using System.Collections.ObjectModel;

namespace MaxFontEditor.Data
{
    public class GlyphSet : PropertyChangedBase
    {
        public GlyphSet()
        {
            glyphs = new ObservableCollection<Glyph>();
        }

        public static GlyphSet CreateEmpty()
        {
            GlyphSet set = new GlyphSet();
            for (int i = 0; i < 256; i++)
            {
                set.Glyphs.Add(new Glyph(i));
            }

            return set;
        }

        public static GlyphSet OpenHFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (StreamReader sr = new StreamReader(filePath))
            {
                if (sr.ReadLine() != "PROGMEM const byte fontdata[16384] = {")
                    return null;

                GlyphSet set = new GlyphSet();

                byte[] gdata = new byte[256];
                string line = "";
                for (int g = 0; g < 256; g++)
                {
                    gdata = new byte[256];

                    line = sr.ReadLine().Trim() + sr.ReadLine().Trim().TrimEnd(',');
                    string[] values = line.Split(',');

                    for (int i = 0; i < values.Length; i++)
                    {
                    	int hx = Int32.Parse(values[i].Substring(2,2), System.Globalization.NumberStyles.HexNumber);

                        gdata[(i * 4)] = (byte)(hx >> 6 & 3);
                        gdata[(i * 4) + 1] = (byte)(hx >> 4 & 3);
                        gdata[(i * 4) + 2] = (byte)(hx >> 2 & 3);
                        gdata[(i * 4) + 3] = (byte)(hx & 3);
                    }

                    set.Glyphs.Add(new Glyph(g, gdata));
                }

                sr.Close();

                return set;
            }
        }

        public static GlyphSet OpenMcmFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (StreamReader sr = new StreamReader(filePath))
            {
                if (sr.ReadLine() != "MAX7456")
                    return null;

                GlyphSet set = new GlyphSet();

                byte[] gdata = new byte[256];
                string line = "";
                for (int g = 0; g < 256; g++)
                {
                    gdata = new byte[256];

                    for (int i = 0; i < 64; i++)
                    {
                        line = sr.ReadLine();

                        for (int c = 0; c < 4; c++)
                        {
                            gdata[(i * 4) + c] |= (byte)((line[c * 2] == '1' ? 1 : 0) << 1);
                            gdata[(i * 4) + c] |= (byte)((line[(c * 2) + 1] == '1' ? 1 : 0));
                        }
                    }

                    set.Glyphs.Add(new Glyph(g, gdata));
                }

                sr.Close();

                return set;
            }
        }

        public void SaveHFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write("PROGMEM const byte fontdata[16384] = {");

                var glyphList = Glyphs.OrderBy(g => g.Id);
                var last = glyphList.LastOrDefault();

                foreach (var glyph in glyphList)
                {
                    int glyphGlyphDataLength = glyph.GlyphData.Length/4;

                    byte c = 0x00;

                    for (int i = 0; i < 64; i++)
                    {
                        if (i != 0)
                            sw.Write(",");

                        if (i % 32 == 0)
                            sw.Write("\n  ");

                        if (i >= glyphGlyphDataLength)
                        {
                            sw.Write("0x55");
                        }
                        else
                        {
                            c = 0;
                            c |= (byte)(glyph.GlyphData[(i * 4)] << 6);
                            c |= (byte)(glyph.GlyphData[(i * 4) + 1] << 4);
                            c |= (byte)(glyph.GlyphData[(i * 4) + 2] << 2);
                            c |= (byte)(glyph.GlyphData[(i * 4) + 3]);

                            sw.Write(String.Format("0x{0:x2}", c));
                        }
                    }

                    if (glyph != last)
                        sw.Write(",");
                }

                sw.Write("};\n");
                sw.Close();
            }
        }

        public void SaveMcmFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write("MAX7456");

                foreach (var glyph in Glyphs.OrderBy(g => g.Id))
                {
                    int glyphGlyphDataLength = glyph.GlyphData.Length;
                    for (int i = 0; i < 256; i++)
                    {
                        if (i % 4 == 0)
                            sw.Write("\n");

                        if (i >= glyphGlyphDataLength)
                        {
                            sw.Write("01");
                            continue;
                        }

                        sw.Write(String.Format("{0}{1}", (glyph.GlyphData[i] >> 1) & 1, glyph.GlyphData[i] & 1));
                    }
                }
                sw.Close();
            }
        }

        ObservableCollection<Glyph> glyphs;
        public ObservableCollection<Glyph> Glyphs
        {
            get { return glyphs; }
            set
            {
                if (glyphs == value)
                    return;

                glyphs = value;

                NotifyOfPropertyChange(() => Glyphs);
            }
        }
    }
}
