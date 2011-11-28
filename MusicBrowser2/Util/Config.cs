﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Util
{
    public class Config
    {
        private readonly XmlDocument _xml;
        private readonly string[,] _defaults = { 

                { "EnableFanArt", true.ToString() },
                { "UseFolderImageForTracks", true.ToString() },
                { "ShowClock", true.ToString() },
                { "AutoLoadNowPlaying", false.ToString() },
                { "AutoPlaylistSize", "50" },
                { "SortReplaceWords", "the|a|an" },
                { "PutDiscInTrackNo", true.ToString() },
                { "ImagesByName", Path.Combine(Helper.AppFolder, "IBN") },

                { "Log.Level", "error" },
                { "Log.Destination", "file" },
                { "Log.File", Path.Combine(Helper.AppLogFolder, "MusicBrowser2.log") },

                { "Internet.UseProviders", true.ToString() } ,
                { "Internet.LastFMUserName", String.Empty },

                { "Experimental.VideoSupport", false.ToString() },
                { "Experimental.PhotoSupport", false.ToString() },

                { "Collections.Folder",  Path.Combine(Helper.AppFolder, "Collections") },

                { "Cache.Path", Helper.CachePath },
                { "Cache.Enable", true.ToString() },
                { "Cache.Engine", "SQLite" },
                                        
                { "Entity.Home.View", "Thumb" },
                { "Entity.Home.Format", "MusicBrowser 2" },
                { "Entity.Home.SortOrder", "[title]" },
                { "Entity.Home.Summary", "" },

                { "Entity.Artist.View", "List" },
                { "Entity.Artist.Format", "[title]" },
                { "Entity.Artist.SortOrder", "[title]" },
                { "Entity.Artist.Summary", "[playcount]  [listeners]  [allplays]" },

                { "Entity.Album.View", "List" },
                { "Entity.Album.Format", "([release]) [title]" },
                { "Entity.Album.SortOrder", "[release][title]" },
                { "Entity.Album.Summary", "[playcount]  [listeners]  [allplays]" },

                { "Entity.Genre.View", "List" },
                { "Entity.Genre.Format", "[title]" },
                { "Entity.Genre.SortOrder", "[title]" },
                { "Entity.Genre.SearchSummary", "[kind] [title]" },
                { "Entity.Genre.Summary", "" },

                { "Entity.Track.View", "List" },
                { "Entity.Track.Format", "[track] - [title]" },
                { "Entity.Track.SortOrder", "[track][title]" },
                { "Entity.Track.Summary", "[playcount]  [listeners]  [allplays]" },

                { "Entity.Playlist.View", "" },
                { "Entity.Playlist.Format", "[title]" },
                { "Entity.Playlist.SortOrder", "" },
                { "Entity.Playlist.Summary", "" },

                { "Entity.Playlist.View", "" },
                { "Entity.Playlist.Format", "[title]" },
                { "Entity.Playlist.SortOrder", "" },
                { "Entity.Playlist.Summary", "" },

                { "Entity.Collection.View", "List" },
                { "Entity.Collection.Format", "[title]" },
                { "Entity.Collection.SortOrder", "[title]" },
                { "Entity.Collection.Summary", "" },

                { "Entity.Video.View", "List" },
                { "Entity.Video.Format", "[title]" },
                { "Entity.Video.SortOrder", "[title]" },
                { "Entity.Video.Summary", "" },

                { "Entity.Photo.View", "List" },
                { "Entity.Photo.Format", "[title]" },
                { "Entity.Photo.SortOrder", "[title]" },
                { "Entity.Photo.Summary", "" },

                { "Entity.PhotoAlbum.View", "List" },
                { "Entity.PhotoAlbum.Format", "[title]" },
                { "Entity.PhotoAlbum.SortOrder", "[title]" },
                { "Entity.PhotoAlbum.Summary", "" },

                { "Entity.Group.View", "List" },
                { "Entity.Group.Format", "[title]" },
                { "Entity.Group.SortOrder", "[title]" },
                { "Entity.Group.Summary", "" },

                { "Entity.Virtual.View", "List" },
                { "Entity.Virtual.Format", "[title]" },
                { "Entity.Virtual.SortOrder", "[title]" },
                { "Entity.Virtual.Summary", "" },

                { "Entity.Episode.View", "List" },
                { "Entity.Episode.Format", "[episode] - [title]" },
                { "Entity.Episode.SortOrder", "[episode]" },
                { "Entity.Episode.Summary", "[season] [episode]" },
                                        
                { "Telemetry.Participate", false.ToString() },
                { "Telemetry.ID", Guid.NewGuid().ToString() },

                { "Player.Engine", "MediaCentre" },
                { "Player.Paths.foobar2000", (Is64Bit ? "C:\\Program Files (x86)" : "C:\\Program Files") + "\\foobar2000\\foobar2000.exe" },
                { "Player.Paths.VLC", (Is64Bit ? "C:\\Program Files (x86)" : "C:\\Program Files") + "\\VideoLAN\\VLC\\vlc.exe" },

                { "Extensions.Playlist", ".wpl|.m3u|.asx" },
                { "Extensions.Ignore", ".xml|.cue|.txt|.nfo" },
                { "Extensions.Image", ".png|.jpg|.jpeg" },
 
                { "Views.Thumbs.IsHorizontal", true.ToString() },
                { "Views.List.ShowSummary", true.ToString() },
                { "Views.Strip.ShowSummary", true.ToString() }

        };

        #region singleton
        static Config _instance;

        Config()
        {
            string configFile = Helper.AppConfigFile;
            try
            {
                _xml = new XmlDocument();
                _xml.Load(configFile);
            }
            catch (Exception e) // there's been an error, delete the file and reset the config
            {
                try
                {
                    if (File.Exists(configFile))
                    {
                        LoggerEngineFactory.Error(new Exception("Error reading config file, file is being reset, all settings will be lost.", e));
                        File.Delete(configFile);
                    }
                    File.WriteAllText(configFile, Resources.BlankSettings);
                    _xml = new XmlDocument();
                    _xml.Load(configFile);
                }
                catch (Exception) { }
            }
        }

        public static Config GetInstance()
        {
            if (_instance != null) return _instance;
            _instance = new Config();
            return _instance;
        }
        #endregion

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        readonly Dictionary<string, string> _settingCache = new Dictionary<string, string>();

        public string GetSetting(string key)
        {
            // see if we've already cached the setting
            if (_settingCache.ContainsKey(key))
            {
                return _settingCache[key];
            }

            string retval = string.Empty;
            try
            {
                string xpathString = string.Format("Settings/{0}", key.Replace('.', '/'));
                retval = _xml.SelectSingleNode(xpathString).InnerText;
            }
            catch { }
            // if we've not got a value from the XML (usually the first run) get the default value
            if (String.IsNullOrEmpty(retval))
            {
                bool found = false;
                for (int x = 0; x < _defaults.GetLength(0); x++ )
                {
                    if (_defaults[x, 0] == key)
                    {
                        //save the value to the XML
                        retval = _defaults[x, 1];
                        SetSetting(key, retval);
                        found = true;
                        break;
                    }
                }
                if (!found) { LoggerEngineFactory.Error(new InvalidDataException("No setting found for '" + key + "'")); }
            }
            // cache the setting on read
            _settingCache[key] = retval;
            return retval;
        }

        public bool GetBooleanSetting(string key)
        {
            try
            {
                return (GetSetting(key).ToLower() == "true");
            }
            catch
            {
                return false;
            }
        }

        public int GetIntSetting(string key)
        {
            int value;
            if (int.TryParse(GetSetting(key), out value)) { return value; }
            return 0;
        }

        public string GetStringSetting(string key)
        {
            return GetSetting(key);
        }

        public IEnumerable<string> GetListSetting(string key)
        {
            return GetSetting(key).ToLower().Split('|');
        }

        public void SetSetting(string key, string value)
        {
            string configFile = Helper.AppConfigFile;
            string xpathString = string.Format("Settings/{0}", key.Replace('.', '/'));

            // update the cache
            _settingCache[key] = value;
            XmlNode node = _xml.SelectSingleNode(xpathString);
            if (node == null)
            {
                if (key.Contains("."))
                {
                    string[] parts = key.Split('.');
                    string path = "Settings";

                    for (int i = 0; i < parts.Length; i++)
                    {
                        string parent = path;
                        // we build the path as we go
                        path = path + "/" + parts[i];
                        // if the part of the path we're looking at doesn't exist, create it
                        if (_xml.SelectSingleNode(path) == null)
                        {
                            XmlNode newNode = _xml.CreateNode(XmlNodeType.Element, parts[i], string.Empty);
                            // if this is the last item, save the "value"
                            if (i == (parts.Length - 1))
                            {
                                newNode.InnerText = value;
                            }
                            _xml.SelectSingleNode(parent).AppendChild(newNode);
                        }
                    }
                }
                else
                {
                    node = _xml.CreateNode(XmlNodeType.Element, key, string.Empty);
                    node.InnerText = value;
                    _xml.FirstChild.AppendChild(node);
                }
            }
            else
            {
                node.InnerText = value;
            }
            _xml.Save(configFile);

            if (!(OnSettingUpdate == null))
            {
                OnSettingUpdate(key);
            }
        }

        public delegate void SettingsChangedHandler(String Key);
        public static event SettingsChangedHandler OnSettingUpdate;

        public void ResetSettings()
        {
            string configFile = Helper.AppConfigFile;
            if (File.Exists(configFile))
            {
                File.Delete(configFile);
            }
            // reset the cache too
            _settingCache.Clear();
        }

        // this pushes the intelligence involved with some settings to the config manager
        static IEnumerable<string> _sortIgnore;
        public static string HandleIgnoreWords(string value)
        {
            if (_sortIgnore == null) { _sortIgnore = _instance.GetListSetting("SortReplaceWords"); }

            foreach (string item in _sortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1); }
            }
            return value;
        }
    }
}
