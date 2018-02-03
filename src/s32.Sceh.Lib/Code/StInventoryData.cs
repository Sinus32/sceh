using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using s32.Sceh.DataModel;

namespace s32.Sceh.Code
{
    public class StInventoryData : INotifyPropertyChanged
    {
        private Dictionary<long, StAppData> _data;
        private string _errorMessage;
        private bool _isLoaded;

        public StInventoryData()
        {
            _errorMessage = "Data not loaded yet";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<long, StAppData> Data
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

        public static Dictionary<long, StAppData> LoadStData(out string errorMessage)
        {
            const int BUFFER_SIZE = 0x4000;
            const string stCardsUrl = "http://cdn.steam.tools/data/set_data.json";
            const string origin = "http://steam.tools";
            const string referer = "http://steam.tools/cards/";

            try
            {
                var uri = new Uri(stCardsUrl);
                var request = SteamDataDownloader.PrepareRequest(uri, HttpMethod.Get, "application/json", referer);
                request.Headers.Add("Cache-Control", "max-age=0");
                request.Headers.Add("Origin", origin);

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

        public bool LoadStData()
        {
            string errorMessage;
            var result = LoadStData(out errorMessage);
            lock (this)
            {
                Data = result;
                ErrorMessage = errorMessage;
                IsLoaded = result != null && errorMessage == null;
            }
            return errorMessage == null;
        }

        private static Dictionary<long, StAppData> ReadResponse(JsonTextReader reader, out string errorMessage)
        {
            const int IGNORE = -1, BEGINNING = 0, MAIN = 1, SETS_ARRAY = 2, ST_APP_DATA = 3, NORMAL_CARD = 4;

            long gameCount = -1L;
            int state = BEGINNING;
            string propName = null;
            var stateStack = new Stack<int>(5);
            var result = new Dictionary<long, StAppData>();
            StAppData stApp = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        stateStack.Push(state);
                        switch (state)
                        {
                            case BEGINNING:
                                state = MAIN;
                                break;

                            case SETS_ARRAY:
                                stApp = new StAppData();
                                state = ST_APP_DATA;
                                break;

                            case ST_APP_DATA:
                                if (propName == "normal")
                                    state = NORMAL_CARD;
                                else
                                    state = IGNORE;
                                break;

                            default:
                                state = IGNORE;
                                break;
                        }
                        break;

                    case JsonToken.StartArray:
                        stateStack.Push(state);
                        if (state == MAIN && propName == "sets")
                            state = SETS_ARRAY;
                        else
                            state = IGNORE;
                        break;

                    case JsonToken.EndObject:
                        if (state == ST_APP_DATA && stApp.Id > 0)
                            result[stApp.Id] = stApp;
                        if (stateStack.Count == 0)
                        {
                            errorMessage = null;
                            return result;
                        }
                        state = stateStack.Pop();
                        break;

                    case JsonToken.EndArray:
                        if (stateStack.Count == 0)
                        {
                            errorMessage = null;
                            return result;
                        }
                        state = stateStack.Pop();
                        break;

                    case JsonToken.PropertyName:
                        propName = (string)reader.Value;
                        break;

                    case JsonToken.Integer:
                        if (state == MAIN && propName == "game_count")
                            gameCount = (long)reader.Value;
                        break;

                    case JsonToken.String:
                        switch (state)
                        {
                            case ST_APP_DATA:
                                switch (propName)
                                {
                                    case "appid":
                                        stApp.Id = Int64.Parse((string)reader.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                                        break;

                                    case "bprice":
                                        stApp.BoosterAvg = Double.Parse((string)reader.Value, NumberStyles.Number, CultureInfo.InvariantCulture);
                                        break;
                                }
                                break;

                            case NORMAL_CARD:
                                switch (propName)
                                {
                                    case "price":
                                        stApp.SetPrice = Double.Parse((string)reader.Value, NumberStyles.Number, CultureInfo.InvariantCulture);
                                        break;

                                    case "avg":
                                        stApp.CardAvg = Double.Parse((string)reader.Value, NumberStyles.Number, CultureInfo.InvariantCulture);
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }

            if (gameCount == result.Count)
                errorMessage = null;
            else
                errorMessage = String.Format("Steam tools data readed incorrectly: readed {0} games out of {1} in total", result.Count, gameCount);
            return result;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}