using System.ComponentModel.Composition;
using MaxFontEditor.Data;
using Caliburn.Micro;
using MaxFontEditor.Services;
using MaxFontEditor.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System;

namespace MaxFontEditor.Shell 
{

    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase, IShell 
    {
        private readonly IDialogService dialogService;
        
        private string openFilePath;

        public ShellViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            BlackBrushActive = true;
            SelectedGlyphs = new ObservableCollection<Glyph>();
            ExportScale = 2;
        }

        public void NewDocument()
        {
            CurrentGlyphSet = GlyphSet.CreateEmpty();
        }

        public void OpenDocument()
        {
            openFilePath = dialogService.GetFileOpenPath("Open MAX7456 Font File", "Font Files|*.mcm;*.h|MCM Files|*.mcm|Header Files|*.h");

            if (openFilePath == null)
                return;

            if (Path.GetExtension(openFilePath) == ".mcm")
            {
                CurrentGlyphSet = GlyphSet.OpenMcmFile(openFilePath);
            }
            else if (Path.GetExtension(openFilePath) == ".h")
            {
                CurrentGlyphSet = GlyphSet.OpenHFile(openFilePath);
            }
        }

        public void SaveDocument()
        {
            if (openFilePath == null)
            {
                SaveAsMCMDocument();
                return;
            }

            if (Path.GetExtension(openFilePath) == ".mcm")
            {
                CurrentGlyphSet.SaveMcmFile(openFilePath);
            }
            else if (Path.GetExtension(openFilePath) == ".h")
            {
                CurrentGlyphSet.SaveHFile(openFilePath);
            }
        }

        public void SaveAsMCMDocument()
        {
            string path = dialogService.GetFileSavePath("Save MCM Font File","mcm", "mcm File|*.mcm");

            if (path == null)
                return;

            if (openFilePath == null)
                openFilePath = path;

            CurrentGlyphSet.SaveMcmFile(path);
        }

        public void SaveAsHDocument()
        {
            string path = dialogService.GetFileSavePath("Save Header Font File", "h", "Header File|*.h");

            if (path == null)
                return;

            if (openFilePath == null)
                openFilePath = path;

            CurrentGlyphSet.SaveHFile(path);
        }

        public void EraseAllGlyphData()
        {
            foreach (var glyph in SelectedGlyphs)
            {
                glyph.EraseGlyph();
            }
        }

        public void ImportGlyphData()
        {
            string path = dialogService.GetFileOpenPath("Import Glyph from Image", "Image Files|*.bmp;*.jpg;*.png;*.gif|Icon Files|*.ico|Meta Files|*.wmf;*.emf");

            if (path == null)
                return;

            Glyph first = SelectedGlyphs.FirstOrDefault();

            if (first == null)
            {
                dialogService.ShowError("Import Error", "You must have a glyph selected before you can import!", null);
                return;
            }

            string import = first.ImportImage(path);

            if (import != null)
                dialogService.ShowError("Import Error", import, null);
        }

        public void ExportGlyphData()
        {
            if (openFilePath == null)
            {
                dialogService.ShowError("Export Error", "You must save the current font file first!", null);
                return;
            }

            string dir = Path.GetDirectoryName(openFilePath);

            foreach (Glyph glyph in SelectedGlyphs)
            {
                glyph.GenerateBitmap(ExportScale, Path.Combine(dir, String.Format("0x{0:x2}.png", glyph.Id)));
            }
        }

        int exportScale;
        public int ExportScale
        {
            get { return exportScale; }
            set
            {
                if (exportScale == value)
                    return;
                exportScale = value;
                NotifyOfPropertyChange(() => ExportScale);
            }
        }

        GlyphSet currentGlyphSet;
        public GlyphSet CurrentGlyphSet
        {
            get { return currentGlyphSet; }
            set
            {
                if (currentGlyphSet == value)
                    return;

                currentGlyphSet = value;

                NotifyOfPropertyChange("CurrentGlyphSet");
            }
        }

        bool transBrushActive;
        public bool TransBrushActive
        {
            get { return transBrushActive; }
            set
            {
                if (transBrushActive == value)
                    return;
                
                transBrushActive = value;

                if (value)
                {
                    WhiteBrushActive = false;
                    BlackBrushActive = false;
                }

                NotifyOfPropertyChange(() => TransBrushActive);
                NotifyOfPropertyChange(() => SelectedBrush);
            }
        }

        bool blackBrushActive;
        public bool BlackBrushActive
        {
            get { return blackBrushActive; }
            set
            {
                if (blackBrushActive == value)
                    return;

                blackBrushActive = value;

                if (value)
                {
                    WhiteBrushActive = false;
                    TransBrushActive = false;
                }

                NotifyOfPropertyChange(() => BlackBrushActive);
                NotifyOfPropertyChange(() => SelectedBrush);
            }
        }

        bool whiteBrushActive;
        public bool WhiteBrushActive
        {
            get { return whiteBrushActive; }
            set
            {
                if (whiteBrushActive == value)
                    return;

                whiteBrushActive = value;

                if (value)
                {
                    BlackBrushActive = false;
                    TransBrushActive = false;
                }

                NotifyOfPropertyChange(() => WhiteBrushActive);
                NotifyOfPropertyChange(() => SelectedBrush);
            }
        }

        public ActiveBrush SelectedBrush
        {
            get
            {
                if (TransBrushActive)
                    return ActiveBrush.Transparent;
                if (BlackBrushActive)
                    return ActiveBrush.Black;
                
                return ActiveBrush.White;
            }
        }

        ObservableCollection<Glyph> selectedGlyphs;
        public ObservableCollection<Glyph> SelectedGlyphs
        {
            get { return selectedGlyphs; }
            set
            {
                if (selectedGlyphs == value)
                    return;

                selectedGlyphs = value;

                NotifyOfPropertyChange(() => SelectedGlyphs);
            }
        }
    }
}
