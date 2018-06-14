using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
    public class SceInventoryData : INotifyPropertyChanged
    {
        private Dictionary<long, SceAppData> _data;
        private string _errorMessage;
        private bool _isLoaded;

        public SceInventoryData()
        {
            _errorMessage = "Data not loaded yet";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<long, SceAppData> Data
        {
            get { return _data; }
            private set
            {
                if (_data != value)
                {
                    _data = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public static Dictionary<long, SceAppData> LoadSceData(out string errorMessage)
        {
            const int BUFFER_SIZE = 0x4000;
            const string sceInventoryUrl = "https://www.steamcardexchange.net/api/request.php?GetInventory";
            const string referer = "https://www.steamcardexchange.net/index.php?inventory";

            try
            {
                var uri = new Uri(sceInventoryUrl);
                var request = SteamDataDownloader.PrepareRequest(uri, HttpMethod.Get, "application/json", referer);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (!response.ContentType.StartsWith("application/json"))
                    {
                        errorMessage = String.Format("Cannot understand response content - type '{0}' is not supported", response.ContentType);
                        return null;
                    }

                    using (var sourceStream = response.GetResponseStream())
                    using (var sr = new StreamReader(sourceStream, Encoding.UTF8, true, BUFFER_SIZE))
                    using (var reader = new JsonTextReader(sr))
                    {
                        reader.CloseInput = false;
                        return ReadResponse(reader, out errorMessage);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                        errorMessage = String.Format("Cannot read data - http status: {0}", response.StatusCode);
                    else
                        errorMessage = String.Format("Cannot read data - no response");
                }
                else
                {
                    errorMessage = String.Format("Cannot read data - web exception status: {0}", ex.Status);
                }
            }
            catch (Exception ex)
            {
                errorMessage = String.Format("Cannot read data - {0}", ex.Message);
            }

            return null;
        }

        public static Dictionary<long, SceAppData> ReadResponse(JsonTextReader reader, out string errorMessage)
        {
            const string propertyNameErrorFormat = "Data format not recognized - property called '{0}' found, but '{1}' was expected";

            bool readed;

            if (!ReadToken(reader, JsonToken.StartObject, "start of data", out readed, out errorMessage))
                return null;

            if (!ReadToken(reader, JsonToken.PropertyName, "data label", out readed, out errorMessage))
                return null;

            if ((string)reader.Value != "data")
            {
                errorMessage = String.Format(propertyNameErrorFormat, reader.Value, "data");
                return null;
            }

            if (!ReadToken(reader, JsonToken.StartArray, "start of data array", out readed, out errorMessage))
                return null;

            var result = new Dictionary<long, SceAppData>();

            while (ReadToken(reader, JsonToken.StartArray, "start of item", out readed, out errorMessage))
            {
                var item = ReadSceAppData(reader, out readed, out errorMessage);
                if (errorMessage != null)
                    return null;

                if (!ReadToken(reader, JsonToken.EndArray, "end of item", out readed, out errorMessage))
                    return null;

                result.Add(item.Id, item);
            }

            if (readed && reader.TokenType == JsonToken.EndArray)
                errorMessage = null; // end of data array found
            else
                return null;

            if (!ReadToken(reader, JsonToken.EndObject, "end of data", out readed, out errorMessage))
                return null;

            return result;
        }

        public bool LoadSceData()
        {
            string errorMessage;
            var result = LoadSceData(out errorMessage);
            lock (this)
            {
                Data = result;
                ErrorMessage = errorMessage;
                IsLoaded = result != null && errorMessage == null;
            }
            return errorMessage == null;
        }

        private static SceAppData ReadSceAppData(JsonTextReader reader, out bool readed, out string errorMessage)
        {
            if (!ReadToken(reader, JsonToken.StartArray, "start of first tuple", out readed, out errorMessage))
                return null;

            if (!ReadToken(reader, JsonToken.Integer, "app id", out readed, out errorMessage))
                return null;

            var result = new SceAppData();
            result.Id = (long)reader.Value;

            if (!ReadToken(reader, JsonToken.String, "app name", out readed, out errorMessage))
                return null;

            result.Name = (string)reader.Value;

            if (!ReadToken(reader, JsonToken.Integer, "no cards / last card / normal", out readed, out errorMessage))
                return null;

            var number = (long)reader.Value;
            result.CardsAvailable = number > 0L;
            result.LastCard = number == 1L;

            if (!ReadToken(reader, JsonToken.Integer, "disabled / enabled", out readed, out errorMessage))
                return null;

            result.Enabled = (long)reader.Value != 0L;

            if (!ReadToken(reader, JsonToken.Integer, "non-marketable / marketable", out readed, out errorMessage))
                return null;

            result.Marketable = (long)reader.Value != 0L;

            if (!ReadToken(reader, JsonToken.EndArray, "end of first tuple", out readed, out errorMessage))
                return null;

            if (!ReadToken(reader, JsonToken.Integer, "worth", out readed, out errorMessage))
                return null;

            result.Worth = (int)(long)reader.Value;

            if (!ReadToken(reader, JsonToken.Integer, "cards in stock", out readed, out errorMessage))
                return null;

            result.CardsInStock = (int)(long)reader.Value;

            if (!ReadToken(reader, JsonToken.StartArray, "start of last tuple", out readed, out errorMessage))
                return null;

            if (!ReadToken(reader, JsonToken.Integer, "total cards in badge", out readed, out errorMessage))
                return null;

            result.TotalCards = (int)(long)reader.Value;

            if (!ReadToken(reader, JsonToken.Integer, "number of cards", out readed, out errorMessage))
                return null;

            // ignored

            if (!ReadToken(reader, JsonToken.Integer, "number of sets", out readed, out errorMessage))
                return null;

            // ignored

            if (!ReadToken(reader, JsonToken.EndArray, "end of last tuple", out readed, out errorMessage))
                return null;

            return result;
        }

        private static bool ReadToken(JsonTextReader reader, JsonToken token, string exprected, out bool readed, out string errorMessage)
        {
            const string errorFormat = "Data format not recognized - token '{0}' found, but '{1}' ({2}) was expected";

            if ((readed = reader.Read()) && reader.TokenType == token)
            {
                errorMessage = null;
                return true;
            }
            else
            {
                if (readed)
                    errorMessage = String.Format(errorFormat, reader.TokenType, token, exprected);
                else
                    errorMessage = String.Format(errorFormat, "end of stream", token, exprected);
                return false;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
