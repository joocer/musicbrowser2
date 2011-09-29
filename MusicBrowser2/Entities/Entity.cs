﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Models;
using MusicBrowser.Util;
using MusicBrowser.Providers;
using MusicBrowser.CacheEngine;
using System.Linq;

namespace MusicBrowser.Entities
{
    [DataContract]
    public enum EntityKind
    {
        Album,
        Artist,
        Folder,
        Home,
        Playlist,
        Track,
        Genre,
        Group,
        Virtual
    }

    [DataContract]
    public sealed class Entity : BaseModel
    {
        private string _sortName;
        private string _cacheKey;
        private string _path;

        public Entity()
        {
            Performers = new List<string>();
            ProviderTimeStamps = new Dictionary<string, DateTime>();
            BackgroundPaths = new List<string>();
        }

        [DataMember]
        public DateTime Added { get; set; }
        [DataMember]
        public DateTime CacheDate { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public string IconPath { get; set; }
        [DataMember]
        public List<string> BackgroundPaths { get; set; }
        [DataMember]
        public string MusicBrainzID { get; set; }
        [DataMember]
        public string DiscId { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string TrackName { get; set; }
        [DataMember]
        public string ArtistName { get; set; }
        [DataMember]
        public string AlbumArtist { get; set; }
        [DataMember]
        public string AlbumName { get; set; }
        [DataMember]
        public int TrackNumber { get; set; }
        [DataMember]
        public int DiscNumber { get; set; }
        [DataMember]
        public DateTime ReleaseDate { get; set; }
        [DataMember]
        public string Codec { get; set; }
        [DataMember]
        public string Channels { get; set; }
        [DataMember]
        public string SampleRate { get; set; }
        [DataMember]
        public string Resolution { get; set; }
        [DataMember]
        public int PlayCount { get; set; }
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public int Listeners { get; set; }
        [DataMember]
        public int TotalPlays { get; set; }
        [DataMember]
        public bool Favorite { get; set; }
        [DataMember]
        public List<string> Performers { get; set; }
        [DataMember]
        public string Genre { get; set; }
        [DataMember]
        public string Lyrics { get; set; }
        [DataMember]
        public Dictionary<string, DateTime> ProviderTimeStamps { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public int AlbumCount { get; set; }
        [DataMember]
        public int ArtistCount { get; set; }
        [DataMember]
        public int TrackCount { get; set; }

        // read only and have default values
        [DataMember]
        public EntityKind Kind { get; set; }
        public string KindName { get { return Kind.ToString(); } }
        public string DefaultBackgroundPath{ get { return string.Empty; } }

        public string DefaultIconPath
        { 
            get 
            {
                switch (Kind)
                {
                    case EntityKind.Track:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imageTrack";
                        }
                    case EntityKind.Playlist:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imagePlaylist";
                        }
                    case EntityKind.Group:
                    case EntityKind.Home:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2";
                        }
                    case EntityKind.Genre:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imageGenre";
                        }
                    case EntityKind.Folder:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder";
                        }
                    case EntityKind.Artist:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imageArtist";
                        }
                    case EntityKind.Album:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imageAlbum";
                        }
                    case EntityKind.Virtual:
                        {
                            return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder";
                        }
                }
                return string.Empty; 
            } 
        }

        // Calculated/transient
        public int Index { get; set; }
        public string SortName { get; set; }

        // Read Only values
        public string View { get { return Config.GetInstance().GetStringSetting(KindName + ".View").ToLower(); } }
        public bool Playable { get { return (Kind == EntityKind.Track || Kind == EntityKind.Playlist); } }

        // this allows the backgrounds to be cycled
        private int _backgroundID = 0;
        public void GetNextBackground()
        {
            if (BackgroundPaths.FirstOrDefault() != null)
            {
                _backgroundID++;
                if (_backgroundID >= BackgroundPaths.Count()) { _backgroundID = 0; }
                FirePropertyChanged("Background");
            }
        }

        public Image Background
        {
            get
            {
                // backgrounds are disabled when fan art is disabled
                if (!Config.GetInstance().GetBooleanSetting("EnableFanArt"))
                {
                    return GetImage(String.Empty);
                }
                else if (BackgroundPaths.FirstOrDefault() != null)
                {
                    return GetImage(BackgroundPaths[_backgroundID]);
                }
                else if (!String.IsNullOrEmpty(DefaultBackgroundPath))
                {
                    return GetImage(DefaultBackgroundPath);
                }
                return GetImage(String.Empty);
            }
        }
        public Image Icon
        {
            get
            {
                if (Kind == EntityKind.Virtual)
                {
                    string IBNPath = System.IO.Path.Combine(System.IO.Path.Combine(Util.Config.GetInstance().GetStringSetting("ImagesByName"), Label), Title);
                    IconPath = ImageProvider.LocateFanArt(IBNPath, ImageType.Thumb);
                }

                // implementing with a "DefaultIconPath" allows the true resx path of the default icon
                // to be hidden from things like the cache
                if (String.IsNullOrEmpty(IconPath) || !System.IO.File.Exists(IconPath)) 
                {
                    return GetImage(DefaultIconPath); 
                }
                return GetImage(IconPath);
            }
        }
        public string CacheKey
        {
            get
            {
                if (String.IsNullOrEmpty(_cacheKey))
                {
                    _cacheKey = Helper.GetCacheKey(Path);
                }
                return _cacheKey;
            }
        }

        public void UpdateValues()
        {
            if (Kind == EntityKind.Home) { Title = "MusicBrowser 2"; }

            FirePropertyChanged("ShortSummaryLine1");
            FirePropertyChanged("ShortSummaryLine2");
            FirePropertyChanged("OptionalArtistLine");
            FirePropertyChanged("Description");
            FirePropertyChanged("Background");
            FirePropertyChanged("Icon");
            FirePropertyChanged("Title");
            FirePropertyChanged("Summary");
        }

        public void CacheSortName()
        {
            // cache the sortname, we don't want to do this complex calc every time its accessed
            string sortName = MacroSubstitution(Config.GetInstance().GetStringSetting(KindName + ".SortOrder")).ToLower();
            sortName = Config.HandleIgnoreWords(sortName);
            Regex rgx = new Regex(@"[^#a-z0-9]+");
            SortName = rgx.Replace(sortName, " ").Trim();
        }

        private static Image GetImage(string path)
        {
            if (path.StartsWith("resx://"))
            {
                return new Image(path);
            }
            if (path.StartsWith("http://"))
            {
                return new Image(path);
            }
            if (System.IO.File.Exists(path))
            {
                return new Image("file://" + path);
            }
            return new Image("resx://MusicBrowser/MusicBrowser.Resources/nullImage");
        }

        public string ShortSummaryLine1 
        {
            get
            {
                switch (Kind)
                {
                    case EntityKind.Track:
                        {
                            StringBuilder sb = new StringBuilder();
                            if (Duration > 0)
                            {
                                TimeSpan t = TimeSpan.FromSeconds(Duration);
                                sb.Append(string.Format("{0}:{1:D2}  ", t.Minutes, t.Seconds));
                            }
                            if (!String.IsNullOrEmpty(Resolution)) { sb.Append(Resolution + "  "); }
                            if (!String.IsNullOrEmpty(Channels)) { sb.Append(Channels + "  "); }
                            if (!String.IsNullOrEmpty(SampleRate)) { sb.Append(SampleRate + "  "); }
                            if (!String.IsNullOrEmpty(Codec)) { sb.Append(Codec + "  "); }

                            if (sb.Length > 0) { return "Track  (" + sb.ToString().Trim() + ")"; }
                            break;
                        }
                    case EntityKind.Genre:
                    case EntityKind.Artist:
                    case EntityKind.Virtual:
                    case EntityKind.Album:
                        {
                            StringBuilder sb = new StringBuilder();

                            if (Kind == EntityKind.Genre)
                            {
                                if (ArtistCount == 1) { sb.Append("1 Artist  "); }
                                if (ArtistCount > 1) { sb.Append(ArtistCount + " Artists  "); }
                            }
                            if (Kind == EntityKind.Genre || Kind == EntityKind.Artist)
                            {
                                if (AlbumCount == 1) { sb.Append("1 Album  "); }
                                if (AlbumCount > 1) { sb.Append(AlbumCount + " Albums  "); }
                            }
                            if (TrackCount == 1) { sb.Append("1 Track  "); }
                            if (TrackCount > 1) { sb.Append(TrackCount + " Tracks  "); }

                            if (Duration > 0)
                            {
                                TimeSpan t = TimeSpan.FromSeconds(Duration);
                                if (t.Hours == 0)
                                {
                                    sb.Append(string.Format("{0}:{1:D2}  ", (Int32)Math.Floor(t.TotalMinutes), t.Seconds));
                                }
                                else
                                {
                                    sb.Append(string.Format("{0}:{1:D2}:{2:D2}  ", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds));
                                }
                            }

                            if (ReleaseDate > DateTime.Parse("01-JAN-1000")) { sb.Append(ReleaseDate.ToString("yyyy") + "  "); }

                            if (sb.Length > 0) { return KindName + "  (" + sb.ToString().Trim() + ")"; }
                            break;
                        }
                }
                return KindName;
            }
        }

        public string ShortSummaryLine2
        {
            get { return MacroSubstitution(Config.GetInstance().GetStringSetting(KindName + ".Summary")); }
        }

        public new string Description
        {
            get { return MacroSubstitution(Config.GetInstance().GetStringSetting(KindName + ".Format")); }
        }

        public string OptionalArtistLine
        {
            get
            {
                if (ArtistName != AlbumArtist && !String.IsNullOrEmpty(ArtistName) && !String.IsNullOrEmpty(AlbumArtist))
                {
                    return ArtistName;
                }
                if (Kind == EntityKind.Virtual)
                {
                    return AlbumArtist;
                }
                return string.Empty;
            }
        }

        public string MacroSubstitution(string input)
        {
            string output = input;

            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(input))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                switch (token)
                {
                    case "title":
                        output = output.Replace("[title]", Title); break;
                    case "description":
                        output = output.Replace("[description]", Description); break;
                    case "track":
                        if (Config.GetInstance().GetBooleanSetting("PutDiscInTrackNo"))
                        {
                            if (DiscNumber > 0)
                            {
                                output = output.Replace("[track]", DiscNumber.ToString() + "." + TrackNumber.ToString("D2")); break;
                            }
                        }
                        output = output.Replace("[track]", TrackNumber.ToString("D2")); break; 
                    case "trackname":
                        output = output.Replace("[trackname]", TrackName); break;
                    case "artist":
                        output = output.Replace("[artist]", ArtistName); break;
                    case "albumartist":
                        output = output.Replace("[albumartist]", AlbumArtist); break;
                    case "album":
                        output = output.Replace("[album]", AlbumName); break;
                    case "release.date":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000")) { output = output.Replace("[release.date]", ReleaseDate.ToString("dd mmmm yyyy")); break; }
                        output = output.Replace("[release.date]", ""); break;
                    case "release":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000")) { output = output.Replace("[release]", ReleaseDate.ToString("yyyy")); break; }
                        output = output.Replace("[release]", ""); break;
                    case "added":
                        if (Added > DateTime.Parse("01-JAN-1000")) { output = output.Replace("[added]", Added.ToString("yyyy-mm-dd")); break; }
                        output = output.Replace("[added]", ""); break;
                    case "channels":
                        output = output.Replace("[channels]", Channels); break;
                    case "samplerate":
                        output = output.Replace("[samplerate]", SampleRate); break;
                    case "resolution":
                        output = output.Replace("[resolution]", Resolution); break;
                    case "codec":
                        output = output.Replace("[codec]", Codec); break;
                    case "label":
                        output = output.Replace("[label]", Label); break;
                    case "playcount":
                        {
                            if (PlayCount > 0)
                            {
                                output = output.Replace("[playcount]", "Plays: " + PlayCount.ToString("N0"));
                                break;
                            }
                            output = output.Replace("[playcount]", String.Empty);
                            break;
                        }
                    case "listeners":
                        {
                            if (Listeners > 0)
                            {
                                output = output.Replace("[listeners]", "Listeners: " + Listeners.ToString("N0"));
                                break;
                            }
                            output = output.Replace("[listeners]", String.Empty);
                            break;
                        }
                    case "allplays":
                        {
                            if (TotalPlays > 0)
                            {
                                output = output.Replace("[allplays]", "Total Plays: " + TotalPlays.ToString("N0"));
                                break;
                            }
                            output = output.Replace("[allplays]", String.Empty);
                            break;
                        }
                    case "length":
                        TimeSpan t = TimeSpan.FromSeconds(Duration);
                        string length;
                        if (t.Hours == 0)
                        {
                            length = string.Format("{0}:{1:D2}", (Int32)Math.Floor(t.TotalMinutes), t.Seconds);
                        }
                        else
                        {
                            length = string.Format("{0}:{1:D2}:{2:D2}", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds);
                        }
                        output = output.Replace("[length]", length);
                        break;
                    case "kind":
                        output = output.Replace("[kind]", KindName); break;
                    case "tracks":
                        output = output.Replace("[tracks]", TrackCount.ToString("N0")); break;
                    case "rating":
                        output = output.Replace("[rating]", Rating.ToString()); break;
                    case "favorite":
                        if (Favorite) { output = output.Replace("[favorite]", "favorite"); break; }
                        output = output.Replace("[favorite]", ""); break;
                }

            }
            return output.Trim();
        }

        [DataMember]
        public string Path
        {
            get { return _path; }
            set
            {
                // don't waste time if nothing's changed
                if (_path == value) { return; }

                _path = value;

                if (Kind != EntityKind.Track && Kind != EntityKind.Playlist)
                {
                    if (string.IsNullOrEmpty(IconPath))
                    {
                        string temp = ImageProvider.LocateFanArt(_path, ImageType.Thumb);
                        if (!String.IsNullOrEmpty(temp))
                        {
                            IconPath = Util.Helper.ImageCacheFullName(CacheKey, "Thumbs");
                            ImageProvider.Save(
                                ImageProvider.Resize(
                                ImageProvider.Load(temp),
                                ImageType.Thumb),
                                IconPath);
                        }
                    }
                    if (BackgroundPaths.FirstOrDefault() == null)
                    {
                        List<string> backPaths = ImageProvider.LocateBackdropList(_path);
                        BackgroundPaths = backPaths;
                    }
                }
            }
        }

    }
}