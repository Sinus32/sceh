using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using s32.Sceh.DataStore;

namespace s32.Sceh.Code
{
    public class DataSerializer
    {
        private string _imageDirectoryFilePattern = "images{0}.xml";
        private int _maxBackups = 8;
        private string _steamProfilesFileName = "steamProfiles.xml";
        private XmlWriterSettings _xmlSettings;

        public DataSerializer()
        {
        }

        public void MakeBackup(string filePath, string backupsPath)
        {
            const string dateFormat = "yyyy-MM-dd-HH-mm-ss";

            if (!File.Exists(filePath))
                return;

            var namePart = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            if (Directory.Exists(backupsPath))
            {
                const string datePatternPart = @"\.([12][0-9]{3}-[012][0-9]-[0123][0-9]-[012][0-9]-[0-5][0-9]-[0-5][0-9])";
                var pattern = "^" + Regex.Escape(namePart) + datePatternPart + Regex.Escape(extension) + "$";
                var re = new Regex(pattern, RegexOptions.IgnoreCase);

                var currentFiles = new List<Tuple<string, DateTime>>(_maxBackups);

                var searchPattern = String.Concat(namePart, "*", extension);
                foreach (var filename in Directory.EnumerateFiles(backupsPath, searchPattern, SearchOption.TopDirectoryOnly))
                {
                    var match = re.Match(Path.GetFileName(filename));
                    if (match.Success)
                    {
                        var date = DateTime.ParseExact(match.Groups[1].Value, dateFormat, CultureInfo.CurrentCulture);
                        var item = Tuple.Create(filename, date);
                        if (currentFiles.Count < _maxBackups - 1)
                        {
                            currentFiles.Add(item);
                        }
                        else
                        {
                            var pos = 0;
                            var min = currentFiles[0];
                            for (int i = 1; i < currentFiles.Count; ++i)
                            {
                                var dt = currentFiles[i];
                                if (min.Item2 > dt.Item2)
                                {
                                    pos = i;
                                    min = dt;
                                }
                            }
                            File.Delete(min.Item1);
                            currentFiles[pos] = item;
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(backupsPath);
            }
            var backupFilename = String.Concat(namePart, '.', DateTime.Now.ToString(dateFormat), extension);
            var backupFilePath = Path.Combine(backupsPath, backupFilename);

            File.Move(filePath, backupFilePath);
        }

        public void SaveFiles(ScehDataFile scehDataFile, string directoryPath, string backupsPath)
        {
            var steamProfilesFilePath = Path.Combine(directoryPath, _steamProfilesFileName);
            MakeBackup(steamProfilesFilePath, backupsPath);
            SaveSteamProfilesFile(steamProfilesFilePath, scehDataFile.SteamProfiles);

            foreach (var imageDirectory in scehDataFile.ImageDirectories)
            {
                var imageDirectoryFileName = String.Format(_imageDirectoryFilePattern, imageDirectory.RelativePath);
                var imageDirectoryFilePath = Path.Combine(directoryPath, imageDirectoryFileName);
                MakeBackup(imageDirectoryFilePath, backupsPath);
                SaveImageDirectoryFile(imageDirectoryFilePath, imageDirectory);
            }
        }

        public void SaveImageDirectoryFile(string filePath, ImageDirectory imageDirectory)
        {
            var tmpFilePath = filePath + ".tmp";
            using (var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(stream, MakeXmlWriterSettings()))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("ImageDirectory", ScehData.NS_SCEH);

                writer.WriteStartAttribute("RelativePath");
                writer.WriteValue(imageDirectory.RelativePath);

                foreach (var image in imageDirectory.Images)
                {
                    writer.WriteStartElement("Image");

                    if (!String.IsNullOrEmpty(image.MimeType))
                    {
                        writer.WriteStartAttribute("MimeType");
                        writer.WriteValue(image.MimeType);
                    }

                    if (!String.IsNullOrEmpty(image.ETag))
                    {
                        writer.WriteStartAttribute("ETag");
                        writer.WriteValue(image.ETag);
                    }

                    writer.WriteStartAttribute("LastUpdate");
                    writer.WriteValue(image.LastUpdate);

                    writer.WriteStartElement("Filename");
                    writer.WriteValue(image.Filename);
                    writer.WriteEndElement();

                    writer.WriteStartElement("ImageUrl");
                    writer.WriteValue(image.ImageUrl);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            File.Move(tmpFilePath, filePath);
        }

        public void SaveSteamProfilesFile(string filePath, List<SteamProfile> steamProfiles)
        {
            var tmpFilePath = filePath + ".tmp";
            using (var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(stream, MakeXmlWriterSettings()))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("SteamProfiles", ScehData.NS_SCEH);

                foreach (var profile in steamProfiles)
                {
                    writer.WriteStartElement("SteamProfile");

                    writer.WriteStartAttribute("SteamId");
                    writer.WriteValue(profile.SteamId);

                    if (!String.IsNullOrEmpty(profile.CustomUrl))
                    {
                        writer.WriteStartAttribute("CustomUrl");
                        writer.WriteValue(profile.CustomUrl);
                    }

                    writer.WriteStartAttribute("LastUpdate");
                    writer.WriteValue(profile.LastUpdate);

                    writer.WriteStartElement("Name");
                    writer.WriteValue(profile.Name);
                    writer.WriteEndElement();

                    writer.WriteStartElement("LastUse");
                    writer.WriteValue(profile.LastUse);
                    writer.WriteEndElement();

                    writer.WriteStartElement("AvatarUrl");
                    writer.WriteElementString("Small", profile.AvatarSmallUrl);
                    writer.WriteElementString("Medium", profile.AvatarMediumUrl);
                    writer.WriteElementString("Full", profile.AvatarFullUrl);
                    writer.WriteEndElement();

                    if (profile.Note != null)
                    {
                        foreach (var note in profile.Note)
                        {
                            writer.WriteStartElement("Note");
                            writer.WriteValue(note);
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            File.Move(tmpFilePath, filePath);
        }

        private XmlWriterSettings MakeXmlWriterSettings()
        {
            var result = new XmlWriterSettings();
            result.ConformanceLevel = ConformanceLevel.Document;
            result.Indent = true;
            return result;
        }
    }
}