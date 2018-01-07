using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
    public class DataSerializer
    {
        private string _imageDirectoryFilePattern = "images{0}.xml";
        private int _maxBackups = 8;
        private string _steamProfilesFileName = "steamProfiles.xml";

        public DataSerializer()
        {
        }

        public void LoadFiles(ScehData scehData)
        {
            var steamProfilesFilePath = Path.Combine(scehData.Paths.AppDataPath, _steamProfilesFileName);
            LoadSteamProfilesFile(steamProfilesFilePath, scehData.Profiles);

            foreach (var imageDirectory in scehData.ImageDirectories)
            {
                var imageDirectoryFileName = String.Format(_imageDirectoryFilePattern, imageDirectory.RelativePath);
                var imageDirectoryFilePath = Path.Combine(scehData.Paths.AppDataPath, imageDirectoryFileName);
                LoadImageDirectoryFile(imageDirectoryFilePath, imageDirectory);
            }
        }

        public void LoadImageDirectoryFile(string filePath, ImageDirectory imageDirectory)
        {
            if (!File.Exists(filePath))
                return;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = XmlReader.Create(stream, MakeXmlReaderSettings()))
            {
                const int IGNORE = -1, BEGINNING = 0, IMAGE_DIRECTORY = 1, IMAGE = 2, FILENAME = 3, IMAGE_URL = 4;

                var state = BEGINNING;
                var stack = new Stack<int>(5);
                ImageFile imageFile = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            int nextState = IGNORE;

                            switch (state)
                            {
                                case BEGINNING:
                                    switch (reader.LocalName)
                                    {
                                        case "ImageDirectory":
                                            imageDirectory.Images.Clear();
                                            nextState = IMAGE_DIRECTORY;
                                            break;
                                    }
                                    break;

                                case IMAGE_DIRECTORY:
                                    switch (reader.LocalName)
                                    {
                                        case "Image":
                                            imageFile = new ImageFile(imageDirectory);
                                            nextState = IMAGE;
                                            break;
                                    }
                                    break;

                                case IMAGE:
                                    switch (reader.LocalName)
                                    {
                                        case "Filename":
                                            nextState = FILENAME;
                                            break;

                                        case "ImageUrl":
                                            nextState = IMAGE_URL;
                                            break;
                                    }
                                    break;
                            }

                            if (!reader.IsEmptyElement)
                            {
                                stack.Push(state);
                                state = nextState;
                            }

                            while (reader.MoveToNextAttribute())
                            {
                                var attributeName = reader.LocalName;

                                if (!reader.ReadAttributeValue())
                                    continue;

                                switch (state)
                                {
                                    case IMAGE:
                                        switch (attributeName)
                                        {
                                            case "mimeType":
                                                imageFile.MimeType = reader.ReadContentAsString();
                                                break;

                                            case "eTag":
                                                imageFile.ETag = reader.ReadContentAsString();
                                                break;

                                            case "lastUpdate":
                                                imageFile.LastUpdate = reader.ReadContentAsDateTime();
                                                break;
                                        }
                                        break;
                                }
                            }

                            break;

                        case XmlNodeType.EndElement:
                            switch (state)
                            {
                                case IMAGE:
                                    if (imageFile.ImageUrl != null)
                                        imageDirectory.Images.Add(imageFile);
                                    break;
                            }

                            if (stack.Count > 0)
                                state = stack.Pop();
                            else
                                return;
                            break;

                        case XmlNodeType.Text:
                            switch (state)
                            {
                                case FILENAME:
                                    imageFile.Filename = reader.ReadContentAsString();
                                    break;

                                case IMAGE_URL:
                                    imageFile.ImageUrl = new Uri(reader.ReadContentAsString());
                                    break;
                            }

                            if (reader.NodeType == XmlNodeType.EndElement)
                                goto case XmlNodeType.EndElement;
                            break;
                    }
                }
            }
        }

        public void LoadSteamProfilesFile(string filePath, ProfilesData profiles)
        {
            if (!File.Exists(filePath))
                return;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = XmlReader.Create(stream, MakeXmlReaderSettings()))
            {
                const int IGNORE = -1, BEGINNING = 0, PROFILES = 1, LAST_PROFILE = 2, STEAM_PROFILE = 3, NAME = 4,
                    LAST_USE = 5, AVATAR_URL = 6, SMALL = 7, MEDIUM = 8, FULL = 9, TRADE_URL = 10, NOTE = 11;

                var state = BEGINNING;
                var stack = new Stack<int>(5);
                SteamProfile steamProfile = null;
                UserNote userNote = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            int nextState = IGNORE;

                            switch (state)
                            {
                                case BEGINNING:
                                    switch (reader.LocalName)
                                    {
                                        case "Profiles":
                                            profiles.SteamProfiles.Clear();
                                            nextState = PROFILES;
                                            break;
                                    }
                                    break;

                                case PROFILES:
                                    switch (reader.LocalName)
                                    {
                                        case "LastSteamProfileId":
                                            nextState = LAST_PROFILE;
                                            break;

                                        case "SteamProfile":
                                            steamProfile = new SteamProfile(0, null);
                                            nextState = STEAM_PROFILE;
                                            break;
                                    }
                                    break;

                                case STEAM_PROFILE:
                                    switch (reader.LocalName)
                                    {
                                        case "Name":
                                            nextState = NAME;
                                            break;

                                        case "LastUse":
                                            nextState = LAST_USE;
                                            break;

                                        case "AvatarUrl":
                                            nextState = AVATAR_URL;
                                            break;

                                        case "TradeUrl":
                                            nextState = TRADE_URL;
                                            break;

                                        case "Note":
                                            userNote = new UserNote();
                                            nextState = NOTE;
                                            break;
                                    }
                                    break;

                                case AVATAR_URL:
                                    switch (reader.LocalName)
                                    {
                                        case "Small":
                                            nextState = SMALL;
                                            break;

                                        case "Medium":
                                            nextState = MEDIUM;
                                            break;

                                        case "Full":
                                            nextState = FULL;
                                            break;
                                    }
                                    break;
                            }

                            if (!reader.IsEmptyElement)
                            {
                                stack.Push(state);
                                state = nextState;
                            }

                            while (reader.MoveToNextAttribute())
                            {
                                var attributeName = reader.LocalName;

                                if (!reader.ReadAttributeValue())
                                    continue;

                                switch (state)
                                {
                                    case LAST_PROFILE:
                                        switch (attributeName)
                                        {
                                            case "autoLogIn":
                                                profiles.AutoLogIn = reader.ReadContentAsBoolean();
                                                break;
                                        }
                                        break;

                                    case STEAM_PROFILE:
                                        switch (attributeName)
                                        {
                                            case "steamId":
                                                steamProfile.SteamId = reader.ReadContentAsLong();
                                                break;

                                            case "customUrl":
                                                steamProfile.CustomUrl = reader.ReadContentAsString();
                                                break;

                                            case "lastUpdate":
                                                steamProfile.LastUpdate = reader.ReadContentAsDateTime();
                                                break;
                                        }
                                        break;

                                    case NOTE:
                                        switch (attributeName)
                                        {
                                            case "score":
                                                userNote.Score = reader.ReadContentAsInt();
                                                break;
                                        }
                                        break;
                                }
                            }

                            break;

                        case XmlNodeType.EndElement:
                            switch (state)
                            {
                                case STEAM_PROFILE:
                                    if (steamProfile.SteamId > 0L)
                                        profiles.SteamProfiles.Add(steamProfile);
                                    break;

                                case NOTE:
                                    if (userNote.Score != 0 || userNote.Text != null)
                                        steamProfile.Notes.Add(userNote);
                                    break;
                            }

                            if (stack.Count > 0)
                                state = stack.Pop();
                            else
                                return;
                            break;

                        case XmlNodeType.Text:
                            switch (state)
                            {
                                case LAST_PROFILE:
                                    profiles.LastSteamProfileId = reader.ReadContentAsLong();
                                    break;

                                case NAME:
                                    steamProfile.Name = reader.ReadContentAsString();
                                    break;

                                case LAST_USE:
                                    steamProfile.LastUse = reader.ReadContentAsDateTime();
                                    break;

                                case SMALL:
                                    steamProfile.AvatarSmallUrl = new Uri(reader.ReadContentAsString());
                                    break;

                                case MEDIUM:
                                    steamProfile.AvatarMediumUrl = new Uri(reader.ReadContentAsString());
                                    break;

                                case FULL:
                                    steamProfile.AvatarFullUrl = new Uri(reader.ReadContentAsString());
                                    break;

                                case TRADE_URL:
                                    steamProfile.Notes.TradeUrl = new Uri(reader.ReadContentAsString());
                                    break;

                                case NOTE:
                                    userNote.Text = reader.ReadContentAsString();
                                    break;
                            }

                            if (reader.NodeType == XmlNodeType.EndElement)
                                goto case XmlNodeType.EndElement;
                            break;
                    }
                }
            }
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

        public void SaveFiles(ScehData scehData)
        {
            var steamProfilesFilePath = Path.Combine(scehData.Paths.AppDataPath, _steamProfilesFileName);
            MakeBackup(steamProfilesFilePath, scehData.Paths.BackupsPath);
            SaveSteamProfilesFile(steamProfilesFilePath, scehData.Profiles);

            foreach (var imageDirectory in scehData.ImageDirectories)
            {
                var imageDirectoryFileName = String.Format(_imageDirectoryFilePattern, imageDirectory.RelativePath);
                var imageDirectoryFilePath = Path.Combine(scehData.Paths.AppDataPath, imageDirectoryFileName);
                MakeBackup(imageDirectoryFilePath, scehData.Paths.BackupsPath);
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

                writer.WriteStartAttribute("relativePath");
                writer.WriteValue(imageDirectory.RelativePath);

                writer.WriteStartAttribute("xmlns");
                writer.WriteValue(ScehData.NS_SCEH);

                writer.WriteStartAttribute("xmlns", "xsi", null);
                writer.WriteValue(ScehData.NS_XSI);

                foreach (var image in imageDirectory.Images)
                {
                    writer.WriteStartElement("Image");

                    if (!String.IsNullOrEmpty(image.MimeType))
                    {
                        writer.WriteStartAttribute("mimeType");
                        writer.WriteValue(image.MimeType);
                    }

                    if (!String.IsNullOrEmpty(image.ETag))
                    {
                        writer.WriteStartAttribute("eTag");
                        writer.WriteValue(image.ETag);
                    }

                    writer.WriteStartAttribute("lastUpdate");
                    writer.WriteValue(image.LastUpdate);

                    writer.WriteStartElement("Filename");
                    writer.WriteValue(image.Filename);
                    writer.WriteEndElement();

                    writer.WriteStartElement("ImageUrl");
                    writer.WriteValue(image.ImageUrl.ToString());
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            File.Move(tmpFilePath, filePath);
        }

        public void SaveSteamProfilesFile(string filePath, ProfilesData profiles)
        {
            var tmpFilePath = filePath + ".tmp";
            using (var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = XmlWriter.Create(stream, MakeXmlWriterSettings()))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Profiles", ScehData.NS_SCEH);

                writer.WriteStartAttribute("xmlns");
                writer.WriteValue(ScehData.NS_SCEH);

                writer.WriteStartAttribute("xmlns", "xsi", null);
                writer.WriteValue(ScehData.NS_XSI);

                if (profiles.LastSteamProfileId > 0L)
                {
                    writer.WriteStartElement("LastSteamProfileId");

                    writer.WriteStartAttribute("autoLogIn");
                    writer.WriteValue(profiles.AutoLogIn);
                    writer.WriteEndAttribute();

                    writer.WriteValue(profiles.LastSteamProfileId);

                    writer.WriteEndElement();
                }

                foreach (var profile in profiles.SteamProfiles)
                {
                    writer.WriteStartElement("SteamProfile");

                    writer.WriteStartAttribute("steamId");
                    writer.WriteValue(profile.SteamId);

                    if (!String.IsNullOrEmpty(profile.CustomUrl))
                    {
                        writer.WriteStartAttribute("customUrl");
                        writer.WriteValue(profile.CustomUrl);
                    }

                    writer.WriteStartAttribute("lastUpdate");
                    writer.WriteValue(profile.LastUpdate);

                    writer.WriteStartElement("Name");
                    writer.WriteValue(profile.Name);
                    writer.WriteEndElement();

                    writer.WriteStartElement("LastUse");
                    writer.WriteValue(profile.LastUse);
                    writer.WriteEndElement();

                    writer.WriteStartElement("AvatarUrl");
                    writer.WriteElementString("Small", profile.AvatarSmallUrl.ToString());
                    writer.WriteElementString("Medium", profile.AvatarMediumUrl.ToString());
                    writer.WriteElementString("Full", profile.AvatarFullUrl.ToString());
                    writer.WriteEndElement();

                    if (profile.Notes.TradeUrl != null)
                    {
                        writer.WriteStartElement("TradeUrl");
                        writer.WriteValue(profile.Notes.TradeUrl.ToString());
                        writer.WriteEndElement();
                    }

                    foreach (var note in profile.Notes)
                    {
                        writer.WriteStartElement("Note");

                        if (note.Score != 0)
                        {
                            writer.WriteStartAttribute("score");
                            writer.WriteValue(note.Score);
                            writer.WriteEndAttribute();
                        }

                        writer.WriteValue(note.Text);

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            File.Move(tmpFilePath, filePath);
        }

        private XmlReaderSettings MakeXmlReaderSettings()
        {
            var result = new XmlReaderSettings();
            result.ConformanceLevel = ConformanceLevel.Document;
            result.DtdProcessing = DtdProcessing.Prohibit;
            result.IgnoreComments = true;
            result.IgnoreProcessingInstructions = true;
            result.IgnoreWhitespace = true;
            return result;
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