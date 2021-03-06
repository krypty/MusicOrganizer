﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using MusicOrganizer.Tag;

namespace MusicOrganizer
{
    public partial class MainWindow : Window
    {
        FolderScanner folderScanner;
        public MainWindow()
        {
            InitializeComponent();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            // On utilise un modèle personalisé arborescent qui liste hiérarchiquement les fichiers musicaux d'un dossier donné
            folderScanner = new FolderScanner(path);

            // treeViewFolder est un treeview classique auquel on a rajouté un checkbox bindé sur la propriété IsChecked du modèle FolderScanner
            treeViewFolders.DataContext = folderScanner.Items;

            lblSourceFolder.Content = path;
        }

        private void btnSourceFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            String path = dialog.SelectedPath;

            if (path != "")
            {
                if (path == lblDestFolder.Content.ToString())
                {
                    MessageBox.Show("Veuillez sélectionner un dossier de destination différent que le dossier de destination.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    lblSourceFolder.Content = path;

                    folderScanner = new FolderScanner(path);
                    treeViewFolders.DataContext = folderScanner.Items;
                }
            }
        }

        private void btnDestFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            String path = dialog.SelectedPath;

            if (path != "")
            {
                if (path == lblSourceFolder.Content.ToString())
                {
                    MessageBox.Show("Veuillez sélectionner un dossier de destination différent que le dossier source.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    lblDestFolder.Content = path;
                    lblDestFolder.FontWeight = FontWeights.Normal;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadAvailableTags();
        }

        private void loadAvailableTags()
        {
            List<TagItem> availableTags = TagParserTools.GetAvailableTags();
            availableTags.ForEach(t => lbxTags.Items.Add(t));
        }

        # region DragAndDrop pour les tags
        private void lbxTags_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox lbx = (ListBox)sender;

                if (lbx.SelectedItem == null)
                {
                    return;
                }
                TagItem tagItem = (TagItem)lbx.SelectedItem;

                DataObject dataObj = new DataObject();
                dataObj.SetData(typeof(TagItem), tagItem);
                DragDropEffects effects = DragDrop.DoDragDrop(this.lbxTags, dataObj, DragDropEffects.Copy);
            }
        }

        private void previewDragEnterTag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagItem)))
            {
                e.Effects = DragDropEffects.Copy;
            }

            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void dropTag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TagItem)))
            {
                e.Effects = DragDropEffects.Copy;
                TagItem tagItem = (TagItem)e.Data.GetData(typeof(TagItem));

                ComboBox cbx = (ComboBox)sender;
                cbx.Text = cbx.Text + tagItem.TagValue;
            }

            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        #endregion

        /// <summary>
        /// Méthode qui va mettre à jour l'aperçu.
        /// Cet aperçu prend un fichier sélectionné et montre à quoi il ressemblera une fois converti
        /// </summary>
        private void updateFilePreview()
        {
            bool isDestFolderValid = Directory.Exists(lblDestFolder.Content.ToString());
            bool isOneFileSelected = this.treeViewFolders.SelectedItem != null && ((FolderItem)this.treeViewFolders.SelectedItem).IsFolder == false;
            bool isTagFileFormatValid = !String.IsNullOrEmpty(this.tbxFileFormat.Text.ToString());

            if (!(isDestFolderValid && isOneFileSelected && isTagFileFormatValid))
            {
                lblPreviewFile.Content = "Indiquer un dossier de destination, un format de fichier valide et d'avoir sélectionner un fichier";
            }
            else
            {
                FolderItem selectedFolderItem = (FolderItem)this.treeViewFolders.SelectedItem;
                string originalFilename = selectedFolderItem.Path;

                string tagFileFormat = tbxFileFormat.Text.ToString();
                string tagFolderFormat = tbxFolderFormat.Text.ToString();
                string destFolder = lblDestFolder.Content.ToString();

                TagParser parser = TagParserTools.Create(originalFilename);
                string tagPreview = parser.Parse(tagFolderFormat, tagFileFormat, destFolder);

                lblPreviewFile.Content = tagPreview;
            }
        }

        // TODO: fusionner tbxFolderFormat_TextChanged et tbxFileFormat_TextChanged
        private void tbxFolderFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateFilePreview();
        }

        private void tbxFileFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateFilePreview();
        }

        private void treeViewFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // on ne met à jour que s'il s'agit d'un enfant, donc d'un fichier
            FolderItem selectedFolderItem = (FolderItem)this.treeViewFolders.SelectedItem;
            if (selectedFolderItem.ChildFolderItem.Count == 0)
            {
                updateFilePreview();
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            bool isDestFolderValid = Directory.Exists(lblDestFolder.Content.ToString());
            bool isTagFileFormatValid = !String.IsNullOrEmpty(this.tbxFileFormat.Text.ToString());
            bool isTagFolderFormatValid = !String.IsNullOrEmpty(this.tbxFolderFormat.Text.ToString());

            if (isDestFolderValid && isTagFileFormatValid && isTagFolderFormatValid)
            {
                if (userHasConfirm())
                {
                    processCopy();
                }
            }
            else
            {
                MessageBox.Show("Veuillez vérifier que:\n- Vous avez un dossier de destination valide\n- Le format saisi des fichiers est valide\n- Le format saisi des dossiers est valide", "Informations manquantes ou invalides", MessageBoxButton.OK);
            }
        }

        private void processCopy()
        {
            String tagFolderFormat = tbxFolderFormat.Text;
            String tagFileFormat = tbxFileFormat.Text;
            String destFolder = lblDestFolder.Content.ToString();

            // Lancement d'une tâche de copie asynchrone
            WorkInProgressWindow window = new WorkInProgressWindow(folderScanner, tagFolderFormat, tagFileFormat, destFolder);
            window.ShowAndRun();
        }

        private bool userHasConfirm()
        {
            MessageBoxResult mbxResult = System.Windows.MessageBox.Show("En cas de doublons les fichiers présents seront écrasés dans le dossier de destination.\nLes éventuels caractères invalides seront remplacés par un équivalent valide. Exemple: AC/DC --> AC_DC\nEtes-vous sûr de vouloir continuer ?", "Ecrasement possible de fichiers existants", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return mbxResult == MessageBoxResult.Yes;
        }

        private void miClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
