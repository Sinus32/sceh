using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace s32.Sceh.Data
{
    public class ScehData
    {
        public const string XML_NAMESPACE = "http://sinus32.net.pl/sceh/";

        public static string AppDataPath { get; private set; }
        public static ImageDirectory AvatarsDirectory { get; private set; }
        public static string AvatarsDirectoryPath { get; private set; }
        public static ImageDirectory CardsDirectory { get; private set; }
        public static string CardsDirectoryPath { get; private set; }
        public static ScehDataFile DataFile { get; private set; }
        public static string DataFilePath { get; private set; }

        public static SteamProfile AddOrUpdateProfile(SteamProfile profile)
        {
            if (profile.SteamId <= 0L)
                throw new ArgumentException("Steam profile is invalid", "profile");

            if (DataFile == null || DataFile.SteamProfiles == null)
                throw new InvalidOperationException("ScehData is not initialized");

            foreach (var dt in DataFile.SteamProfiles)
            {
                if (dt.SteamId == profile.SteamId)
                {
                    dt.Name = profile.Name;
                    dt.CustomURL = profile.CustomURL;
                    dt.AvatarSmall = profile.AvatarSmall;
                    dt.AvatarMedium = profile.AvatarMedium;
                    dt.AvatarFull = profile.AvatarFull;
                    dt.LastUpdate = profile.LastUpdate;
                    return dt;
                }
            }

            lock (DataFile.SteamProfiles)
                DataFile.SteamProfiles.Add(profile);
            return profile;
        }

        public static void Load()
        {
            var dataLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDataPath = Path.Combine(dataLocation, "Sceh");

            Directory.CreateDirectory(AppDataPath);

            DataFilePath = Path.Combine(AppDataPath, "scehdata.xml");
            if (File.Exists(DataFilePath))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(ScehDataFile));
                    using (var stream = File.OpenRead(DataFilePath))
                    using (var reader = new StreamReader(stream, true))
                        DataFile = (ScehDataFile)ser.Deserialize(stream);
                }
                catch (Exception)
                {
                    DataFile = new ScehDataFile();
                }
            }
            else
            {
                DataFile = new ScehDataFile();
            }

            if (DataFile.SteamProfiles == null)
                DataFile.SteamProfiles = new List<SteamProfile>();

            if (DataFile.ImageDirectories == null)
            {
                DataFile.ImageDirectories = new List<ImageDirectory>();
            }
            else
            {
                foreach (var dir in DataFile.ImageDirectories)
                    foreach (var img in dir.Images)
                        img.Directory = dir;
            }

            AvatarsDirectory = MatchImageDirectory(DataFile.ImageDirectories, "Avatars");
            CardsDirectory = MatchImageDirectory(DataFile.ImageDirectories, "Cards");

            AvatarsDirectoryPath = Path.Combine(AppDataPath, AvatarsDirectory.RelativePath);
            CardsDirectoryPath = Path.Combine(AppDataPath, CardsDirectory.RelativePath);

            Directory.CreateDirectory(AvatarsDirectoryPath);
            Directory.CreateDirectory(CardsDirectoryPath);
        }

        public static bool LocalFileExists(ImageFile image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (image.Directory == null)
                throw new ArgumentException("Image directory is null", "image");

            var filePath = Path.Combine(AppDataPath, image.Directory.RelativePath, image.Filename);

            return File.Exists(filePath);
        }

        public static void SaveFile()
        {
            if (DataFile == null || DataFilePath == null)
                return;

            var ser = new XmlSerializer(typeof(ScehDataFile));
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, XML_NAMESPACE);

            using (var stream = File.OpenWrite(DataFilePath))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var xml = XmlWriter.Create(writer, settings))
                ser.Serialize(xml, DataFile, namespaces);
        }

        private static ImageDirectory MatchImageDirectory(List<ImageDirectory> directories, string relativePath)
        {
            var result = directories.FirstOrDefault(q => q.RelativePath == relativePath);
            if (result == null)
            {
                result = new ImageDirectory(relativePath);
                directories.Add(result);
            }
            return result;
        }
    }
}