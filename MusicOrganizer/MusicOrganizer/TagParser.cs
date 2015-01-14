using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicOrganizer.Tag
{
    /// <summary>
    /// Classe abstraite qui définit un comportement de base que les classes filles doivent respecter
    /// C'est cette classe qui sera utilisé côté "utilisateur" afin de pouvoir changer de librarie sans impacter les fonctionnalités déjà présentes
    /// </summary>
    abstract class TagParser
    {
        protected string filename;

        public TagParser(String path)
        {
            this.filename = path;
        }

        public abstract string Album { get; }

        public abstract string Title { get; }

        public abstract string Track { get; }

        public abstract string Artist { get; }

        public abstract string Year { get; }

        public abstract string Genre { get; }

        public abstract string DiscNumber { get; }

        public string Path { get { return this.filename; } }

        /// <summary>
        /// Méthode qui va renvoyer un chemin complet pour un fichier donné selon un format de tag (pattern) donné
        /// </summary>
        /// <param name="tagFolderFormat">tags utlisés pour le générer un nom de dossier. Exemple <artist>\<album></param>
        /// <param name="tagFileFormat">tags utlisés pour le générer un nom de fichier. Exemple <track> - <title> </param>
        /// <param name="destFolder">dossier de destination utilisé comme racine pour les fichiers/dossiers générés</param>
        /// <returns>chemin et nom complet du fichier généré à partir des tags donnés</returns>
        public abstract string Parse(string tagFolderFormat, string tagFileFormat, string destFolder);
    }
}
