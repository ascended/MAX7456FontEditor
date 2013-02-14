using System;
using Ookii.Dialogs.Wpf;

namespace MaxFontEditor.Services
{
    internal class DialogService : IDialogService
    {
        #region IDialogService Members

        public bool ShowConfirmation(string title, string text, string extra)
        {
            DialogMessageService service = new DialogMessageService(null)
            {
                Icon = DialogMessageIcon.Question,
                Buttons = DialogMessageButtons.Yes | DialogMessageButtons.No,
                Title = title,
                Text = text,
                Extra = extra
            };
            var result = service.Show();
            return result == DialogMessageResult.Yes;
        }

        public bool? ShowConfirmationWithCancel(string title, string text, string extra)
        {
            DialogMessageService service = new DialogMessageService(null)
            {
                Icon = DialogMessageIcon.Question,
                Buttons = DialogMessageButtons.Yes | DialogMessageButtons.No | DialogMessageButtons.Cancel,
                Title = title,
                Text = text,
                Extra = extra
            };

            DialogMessageResult result = service.Show();
            switch (result)
            {
                case DialogMessageResult.Yes:
                    return true;
                case DialogMessageResult.No:
                    return false;
                default:
                    return null;
            }
        }

        public void ShowMessage(string title, string text, string extra)
        {
            DialogMessageService service = new DialogMessageService(null) { Icon = DialogMessageIcon.Information, Buttons = DialogMessageButtons.Ok, Title = title, Text = text, Extra = extra };
            var result = service.Show();
        }

        public void ShowWarning(string title, string text, string extra)
        {
            DialogMessageService service = new DialogMessageService(null) { Icon = DialogMessageIcon.Warning, Buttons = DialogMessageButtons.Ok, Title = title, Text = text, Extra = extra };
            var result = service.Show();
        }

        public void ShowError(string title, string text, string extra)
        {
            DialogMessageService service = new DialogMessageService(null) { Icon = DialogMessageIcon.Error, Buttons = DialogMessageButtons.Ok, Title = title, Text = text, Extra = extra };
            var result = service.Show();
        }

        public string GetFileOpenPath(string title, string filter)
        {
            if (VistaOpenFileDialog.IsVistaFileDialogSupported)
            {
                VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog { Title = title, CheckFileExists = true, RestoreDirectory = true, Filter = filter };

                if (openFileDialog.ShowDialog() == true)
                    return openFileDialog.FileName;
            }
            else
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog { Title = title, CheckFileExists = true, RestoreDirectory = true, Filter = filter };

                if (ofd.ShowDialog() == true)
                    return ofd.FileName;
            }

            return "";
        }

        public string GetFileSavePath(string title, string defaultExt, string filter)
        {
            if (VistaSaveFileDialog.IsVistaFileDialogSupported)
            {
                VistaSaveFileDialog saveFileDialog = new VistaSaveFileDialog { Title = title, DefaultExt = defaultExt, CheckFileExists = false, RestoreDirectory = true, Filter = filter };

                if (saveFileDialog.ShowDialog() == true)
                    return saveFileDialog.FileName;
            }
            else
            {
                Microsoft.Win32.SaveFileDialog ofd = new Microsoft.Win32.SaveFileDialog { Title = title, DefaultExt = defaultExt, CheckFileExists = false, RestoreDirectory = true, Filter = filter };

                if (ofd.ShowDialog() == true)
                    return ofd.FileName;
            }

            return "";
        }
        #endregion
    }


    public interface IDialogService
    {
        bool ShowConfirmation(string title, string text, string extra);
        bool? ShowConfirmationWithCancel(string title, string text, string extra);

        void ShowMessage(string title, string text, string extra);
        void ShowWarning(string title, string text, string extra);
        void ShowError(string title, string text, string extra);

        string GetFileOpenPath(string title, string filter);
        string GetFileSavePath(string title, string defaultExt, string filter);
    }
}
