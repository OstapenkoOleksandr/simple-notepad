using Microsoft.Win32;
using SimpleNotepad.windows;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace SimpleNotepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentDocumentPath { get; set; } = string.Empty;
        private double _defaultFontSize { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            _defaultFontSize = this.inputTextBox.FontSize;
        }

        #region File menu

        private void ConfigureSaveFileDialog(SaveFileDialog saveFileDialog)
        {
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Title = "Save file as...";
            saveFileDialog.Filter = "Text File (*.txt)|*.TXT|All files|*.*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.AddExtension = true;
        }

        private void ConfiugureOpenFileDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Title = "Select Text File..";
            openFileDialog.Filter = "Text Files (*.txt)|*.TXT|All files|*.*";
            openFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void ChangeWindowTitle(bool IsFile = false)
        {
            if (IsFile)
            {
                this.Title = _currentDocumentPath.Split('\\').Last();
            }
            else
            {
                this.Title = "*Not saved";
            }
        }

        private MessageBoxResult HandleUnsavedChanges(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("You have unsaved changes! Do you want to save them?", "Attention", MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveAsFileMenuItem_Click(sender, e);
                    break;
                case MessageBoxResult.No:
                    _currentDocumentPath = string.Empty;
                    this.inputTextBox.Text = string.Empty;
                    NewDocumentMenuItem_Click(sender, e);
                    break;
                case MessageBoxResult.None:
                case MessageBoxResult.Cancel:
                default:
                    break;
            }

            return result;
        }

        private void NewWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newMainWindow = new MainWindow();
            newMainWindow.Show();
        }

        private void NewDocumentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentDocumentPath) || !string.IsNullOrEmpty(this.inputTextBox.Text))
            {
                HandleUnsavedChanges(sender, e);
                return;
            }

            _currentDocumentPath = string.Empty;
            this.inputTextBox.Text = string.Empty;

            ChangeWindowTitle();
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentDocumentPath) || !string.IsNullOrEmpty(this.inputTextBox.Text))
            {
                HandleUnsavedChanges(sender, e);
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            ConfiugureOpenFileDialog(openFileDialog);
            openFileDialog.ShowDialog(this);

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                _currentDocumentPath = openFileDialog.FileName;

                using (var reader = new StreamReader(_currentDocumentPath))
                {
                    this.inputTextBox.Text = reader.ReadToEnd();
                    ChangeWindowTitle(true);
                }
            }
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentDocumentPath))
            {
                using (var writer = new StreamWriter(_currentDocumentPath))
                {
                    writer.Write(this.inputTextBox.Text);
                }
            }
            else
            {
                SaveAsFileMenuItem_Click(sender, e);
            }
        }

        private void SaveAsFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            ConfigureSaveFileDialog(saveFileDialog);

            saveFileDialog.ShowDialog(this);

            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                _currentDocumentPath = saveFileDialog.FileName;

                using (var writer = new StreamWriter(_currentDocumentPath))
                {
                    writer.WriteLine(this.inputTextBox.Text);
                }

                ChangeWindowTitle(true);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.inputTextBox.Text) || !string.IsNullOrEmpty(this.inputTextBox.Text))
            {
                HandleUnsavedChanges(sender, e);
            }

            this.Close();
        }

        #endregion

        #region Edit

        private void UndoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.Undo();
        }

        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.Cut();
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.Copy();
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.Focus();
            this.inputTextBox.Paste();
        }

        private void SelectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.Focus();
            this.inputTextBox.SelectAll();
        }

        private void RedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.Redo();
        }


        #endregion

        #region Format

        private void WordWrapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.TextWrapping = this.WordWrapMenuItem.IsChecked ?
                TextWrapping.Wrap :
                TextWrapping.NoWrap;

            this.inputTextBox.HorizontalScrollBarVisibility = this.WordWrapMenuItem.IsChecked ?
                System.Windows.Controls.ScrollBarVisibility.Disabled :
                System.Windows.Controls.ScrollBarVisibility.Visible;
        }

        #endregion

        #region View

        private void FontUpSizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.FontSize++;
        }

        private void FontDownSizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.inputTextBox.FontSize > 1)
            {
                this.inputTextBox.FontSize--;
            }
        }

        private void ResetFontSizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.inputTextBox.FontSize = _defaultFontSize;
        }

        #endregion

        #region About

        private void AboutMenuButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        #endregion


    }
}
