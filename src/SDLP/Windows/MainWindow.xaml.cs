using Dev2Be.Toolkit;
using Dev2Be.Toolkit.Extensions;
using Fluent;
using Microsoft.Win32;
using MRULib.MRU.Interfaces;
using SDLL;
using SDLP.Helpers;
using SDLP.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private Activity activity;

        private bool isLogged;

        private string filePath;

        private OpenDocumentWindow OpenDocumentWindow;

        public TextRange DocumentTextRange;

        public bool IsLogged
        {
            get
            {
                AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);
                
                try
                {
                    var registryValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\"+assemblyInformation.Company+"\\"+assemblyInformation.Product+"\\Account", "IsLogged", false);

                    if (registryValue != null)
                        return Convert.ToBoolean(registryValue);
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }

                return false;
            }
            set
            {
                AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);

                RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\" + assemblyInformation.Company + "\\" + assemblyInformation.Product+"\\Account");

                registryKey.SetValue("IsLogged", isLogged);

                IsLogged = value;
            }
        }

        public MainWindow()
        {
            File.Create(Path.Combine(Path.GetTempPath() + "sdlp.sdlp"));

            InitializeComponent();

            DataContext = this;

            AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);

            DocumentTextRange = new TextRange(DocumentRichTextBox.Document.ContentStart, DocumentRichTextBox.Document.ContentEnd);

            WordsNumberTextBlock.Text = (DocumentTextRange.Text.Count() <= 1) ? DocumentTextRange.Text.Count() + " mot" : DocumentTextRange.Text.Count() + " mots";
            #region Bouton Click
            #region Blanck Activity
            BackstageNewActivityUserControl.BlanckActivityButton.Click += BlanckActivityButton_Click;
            StartScreenNewActivityUserControl.BlanckActivityButton.Click += BlanckActivityButton_Click;
            #endregion Blanck Activity

            #region Word Document
            BackstageNewActivityUserControl.WordDocumentButton.Click += WordDocumentButton_Click;
            StartScreenNewActivityUserControl.WordDocumentButton.Click += WordDocumentButton_Click;
            #endregion Word Document

            #region PDF Document
            BackstageNewActivityUserControl.PdfDocumentButton.Click += PdfDocumentButton_Click;
            StartScreenNewActivityUserControl.PdfDocumentButton.Click += PdfDocumentButton_Click;
            #endregion PDF Document
            #endregion Bouton Click

            Resources.Add("Icon", System.Drawing.Icon.ExtractAssociatedIcon(Path.Combine(Path.GetTempPath() + "sdlp.sdlp")).ToImageSource());
            
            StartScreenRecentFilesUserControl.RecentFilesMenuListView.PreviewMouseLeftButtonDown += RecentFilesMenuListView_MouseLeftButtonUp;
        }

        private void RecentFilesMenuListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (StartScreenRecentFilesUserControl.RecentFilesMenuListView.SelectedItem != null)
            {
                FluentBackastage.IsOpen = false;
                FluentStartScreen.IsOpen = false;

                var selectedProperty = (KeyValuePair<string, IMRUEntryViewModel>)((ListView)sender).SelectedItem;

                filePath = selectedProperty.Key;

                activity = Activity.Open(filePath);

                ActivityLayoutDocument.Title = activity.ActivityName;

                Title = activity.ActivityName + " - Le Studio des Langues";

                DocumentTextRange.Text = activity.Text;
                
                ProvidedWordsListBox.ItemsSource = activity.ProvidedWords;

                RecentFiles.UpdateEntry(filePath);
            }
        }

        private void BlanckActivityButton_Click(object sender, RoutedEventArgs e)
        {
            FluentBackastage.IsOpen = false;
            FluentStartScreen.IsOpen = false;

            activity = new Activity();

            ActivityLayoutDocument.Title = activity.ActivityName = "Activité 1";

            Title = activity.ActivityName + " - Le Studio des Langues";

            filePath = "";

            ProvidedWordsListBox.ItemsSource = activity.ProvidedWords;
        }

        private void WordDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Document Word|*.docx|Document Word 93-2003|*.doc" };

            if ((bool)openFileDialog.ShowDialog())
            {
                OpenDocumentWindow = new OpenDocumentWindow(Path.GetFileName(openFileDialog.FileName)) { Owner = this };

                Cursor = Cursors.Wait;

                Thread thread = new Thread(() =>
                {
                    MemoryStream memoryStream = OpenDocuments.GetTextFromWord(openFileDialog.FileName);

                    if (memoryStream != default(MemoryStream) && memoryStream != null)
                    {
                        Dispatcher.Invoke(() => DocumentTextRange.Load(memoryStream, DataFormats.Rtf));

                        Dispatcher.Invoke(() => FluentBackastage.IsOpen = false);
                        Dispatcher.Invoke(() => FluentStartScreen.IsOpen = false);

                        Dispatcher.Invoke(() => activity = new Activity() { Text = DocumentTextRange.Text, ActivityName = Path.GetFileNameWithoutExtension(openFileDialog.FileName) });

                        Dispatcher.Invoke(() => ActivityLayoutDocument.Title = activity.ActivityName);

                        Dispatcher.Invoke(() => Title = activity.ActivityName + " - Le Studio des Langues");

                        filePath = "";

                        Dispatcher.Invoke(() => ProvidedWordsListBox.ItemsSource = activity.ProvidedWords);
                    }

                    Dispatcher.Invoke(() => OpenDocumentWindow.Close());
                    Dispatcher.Invoke(() => Cursor = Cursors.Arrow);
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                OpenDocumentWindow.ShowDialog();

                RecentFiles.UpdateEntry(openFileDialog.FileName);
            }
        }

        private void PdfDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Fichiers PDF|*.pdf" };

            if ((bool)openFileDialog.ShowDialog())
            {
                OpenDocumentWindow = new OpenDocumentWindow(Path.GetFileName(openFileDialog.FileName)) { Owner = this };

                Cursor = Cursors.Wait;

                Thread thread = new Thread(() =>
                {
                        Dispatcher.Invoke(() => DocumentTextRange.Text = OpenDocuments.GetTextFromPdf(openFileDialog.FileName));

                        Dispatcher.Invoke(() => FluentBackastage.IsOpen = false);
                        Dispatcher.Invoke(() => FluentStartScreen.IsOpen = false);

                        activity = new Activity() { Text = DocumentTextRange.Text, ActivityName = Path.GetFileNameWithoutExtension(openFileDialog.FileName) };

                        Dispatcher.Invoke(() => ActivityLayoutDocument.Title = activity.ActivityName);

                        Dispatcher.Invoke(() => Title = activity.ActivityName + " - Le Studio des Langues");

                        filePath = "";

                        Dispatcher.Invoke(() => ProvidedWordsListBox.ItemsSource = activity.ProvidedWords);

                    Dispatcher.Invoke(() => OpenDocumentWindow.Close());
                    Dispatcher.Invoke(() => Cursor = Cursors.Arrow);
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                OpenDocumentWindow.ShowDialog();

                RecentFiles.UpdateEntry(openFileDialog.FileName);
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { DefaultExt = ".sdlp", Filter = "Fichier Studio des Langues - Professeur|*.sdlp" };

            if ((bool)saveFileDialog.ShowDialog())
            {
                filePath = saveFileDialog.FileName;

                activity.Save(saveFileDialog.FileName);
            }
        }

        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthentificationWindow().ShowDialog(this);
        }

        private void FluentBackstageTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExitBackstageTabItem.IsSelected)
                Close();
        }

        #region Application closing
        private void RibbonWindow_Closing(object sender, CancelEventArgs e) => System.Windows.Application.Current.Shutdown(0);

        private void RibbonWindow_Closed(object sender, EventArgs e) => RecentFiles.SaveMRU();
        #endregion Application closing

        private void ManageProvidedWordsButton_Click(object sender, RoutedEventArgs e)
        {
            ProvidedWordsLayoutAnchorable.IsVisible = true;
        }

        private void DocumentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WordsNumberTextBlock.Text = (DocumentTextRange.Text.Count() <= 1) ? DocumentTextRange.Text.Count() + " mot" : DocumentTextRange.Text.Count() + " mots";
        }

        #region Ajout des mots fournis
        private void AddProvidedWordsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentRichTextBox.Selection.Text != "")
                AddProvidedWords(DocumentRichTextBox.Selection.Text);

            ProvidedWordsListBox.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProvidedWordsWatermarkTextBox.Text != "")
                AddProvidedWords(ProvidedWordsWatermarkTextBox.Text);

            ProvidedWordsWatermarkTextBox.Text = "";
            ProvidedWordsListBox.Items.Refresh();
        }

        public void AddProvidedWords(string word)
        {
            CultureInfo cultureInfo = new CultureInfo("fr-Fr");

            if (cultureInfo.CompareInfo.IndexOf(DocumentTextRange.Text, word, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0)
            {
                string[] words = word.Split(Activity.SpecialCharacters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in words)
                {
                    if (item.IsFullWord(DocumentTextRange.Text, Dev2Be.Toolkit.Enumerations.StringComparison.IgnoreCaseAndDiacritics))
                    {
                        if (!activity.ProvidedWords.Contains(word, StringComparer.OrdinalIgnoreCase))
                            activity.ProvidedWords.Add(item);
                    }
                }
            }
        }
        #endregion Ajout des mots fournis
    }
}
