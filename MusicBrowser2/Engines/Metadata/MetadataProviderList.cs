﻿//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using MusicBrowser.Engines.Cache;
//using MusicBrowser.Engines.Logging;
//using MusicBrowser.Entities;
//using MusicBrowser.Interfaces;
//using MusicBrowser.Providers.Background;

//namespace MusicBrowser.Providers.Metadata
//{
//    class MetadataProviderList : IBackgroundTaskable
//    {
//        private static object obj = new object();
//        private static IList<IDataProvider> _providers = null;

//        private const int MinDaysBetweenHits = 180;
//        private const int MaxDaysBetweenHits = 360;
//        private const int RefreshPercentage = 25;

//        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

//        public static IEnumerable<IDataProvider> GetProviders()
//        {
//            if (_providers == null)
//            {
//                lock (obj)
//                {
//                    if (_providers == null)
//                    {
//                        _providers = new List<IDataProvider>();

//                        _providers.Add(new MetadataFileProvider());
//                        _providers.Add(new TagSharpMetadataProvider());
//                        _providers.Add(new MediaInfoMetadataProvider());
//                        _providers.Add(new InheritanceMetadataProvider());
//                        _providers.Add(new HTBackdropMetadataProvider());
//                        _providers.Add(new LastFMMetadataProvider());
//                        _providers.Add(new FileSystemMetadataProvider());
//                        _providers.Add(new IconMetadataProvider());
//                        _providers.Add(new VideoFilenameMetadataProvider());
//                        _providers.Add(new CoreMetadataProvider());
//                    }
//                }
//            }
//            return _providers;
//        }

//        public static void ProcessEntity(baseEntity entity, bool Forced)
//        {
//#if DEBUG
//            Engines.Logging.LoggerEngineFactory.Verbose("ProcessEntity(" + entity.Path + ", <providers>, " + Forced + ")", "start");
//#endif
//            // only waste time triggering cache updates if the content has changed
//            bool requiresUpdate = false;
//            // only do the slow providers if we've not done the fast Provider marker
//            bool onlyFastProviders = !entity.ProviderTimeStamps.ContainsKey("CoreMetadataProvider");

//            // put the data into the DTO for the providers to read
//            DataProviderDTO dto = PopulateDTO(entity);

//            foreach (IDataProvider provider in GetProviders())
//            {
//                try
//                {
//                    DateTime lastAccess = entity.ProviderTimeStamps.ContainsKey(provider.FriendlyName()) ? entity.ProviderTimeStamps[provider.FriendlyName()] : DateTime.MinValue;
//                    if (!Forced && onlyFastProviders && provider.Type == ProviderType.Peripheral) { continue; }
//                    if (!provider.CompatibleWith(entity.KindName)) { continue; }
//                    if (!Forced && !provider.isStale(lastAccess)) { continue; }

//                    // execute the payload
//                    dto = provider.Fetch(dto);

//                    if (dto.Outcome == DataProviderOutcome.Success)
//                    {
//                        entity = PopulateEntity(entity, dto);
//                        requiresUpdate = true;
//                        entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
//                    }
//                    else if (dto.Outcome != DataProviderOutcome.NoData) // no data is a warning, ignore it and move on
//                    {
//                        LoggerEngineFactory.Debug(dto.Outcome.ToString() + " " + dto.Errors[0]);
//                        entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
//                    }
//                }
//                catch (Exception e)
//                {
//#if DEBUG
//                    Engines.Logging.LoggerEngineFactory.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType().ToString(), entity.Path), e));
//#endif
//                }
//            }
//            if (requiresUpdate)
//            {
//                entity.UpdateValues();
//                InMemoryCache.GetInstance().Update(entity);
//                CacheEngineFactory.GetEngine().Update(entity);
//            }

//            // go through the refresh again, this time it'll pick up the slow providers
//            if (!Forced && onlyFastProviders)
//            {
//                CommonTaskQueue.Enqueue(new MetadataProviderList(entity));
//            }
//        }

//        private static DataProviderDTO PopulateDTO(baseEntity entity)
//        {
//            DataProviderDTO dto = new DataProviderDTO();

//            dto.AlbumArtist = entity.AlbumArtist;
//            dto.AlbumName = entity.AlbumName;
//            dto.ArtistName = entity.ArtistName;
//            dto.Channels = entity.Channels;
//            dto.Codec = entity.Codec;
//            dto.DiscId = entity.DiscId;
//            dto.DiscNumber = entity.DiscNumber;
//            dto.Duration = entity.Duration;
//            dto.Favorite = entity.Favorite;
//            dto.Genre = entity.Genre;
//            dto.Listeners = entity.Listeners;
//            dto.Lyrics = entity.Lyrics;
//            dto.MusicBrainzId = entity.MusicBrainzID;
//            dto.Path = entity.Path;
//            dto.Performers = entity.Performers;
//            dto.PlayCount = entity.PlayCount;
//            dto.Rating = entity.Rating;
//            dto.ReleaseDate = entity.ReleaseDate;
//            dto.Resolution = entity.Resolution;
//            dto.SampleRate = entity.SampleRate;
//            dto.Summary = entity.Summary;
//            dto.TotalPlays = entity.TotalPlays;
//            dto.TrackNumber = entity.TrackNumber;

//            dto.TrackCount = entity.TrackCount;
//            dto.Title = entity.Title;
//            dto.Label = entity.Label;
//            dto.AlbumCount = entity.AlbumCount;
//            dto.ArtistCount = entity.ArtistCount;

//            dto.hasThumbImage = !String.IsNullOrEmpty(entity.IconPath);
//            dto.hasBackImage = !(entity.BackgroundPaths.FirstOrDefault() == null);
//            dto.BackImages = new List<Bitmap>();

//            dto.Episode = entity.Episode;
//            dto.Season = entity.Season;

//            dto.ProviderTimeStamps = entity.ProviderTimeStamps;

//            switch (entity.Kind)
//            {
//                    case EntityKind.Album:
//                    {
//                        dto.DataType = DataTypes.Album;
//                        dto.AlbumName = entity.Title;
//                        break;
//                    }
//                    case EntityKind.Artist:
//                    {
//                        dto.DataType = DataTypes.Artist;
//                        dto.ArtistName = entity.Title;
//                        dto.AlbumArtist = entity.Title;
//                        break;
//                    }
//                    case EntityKind.Playlist:
//                    {
//                        dto.DataType = DataTypes.Playlist;
//                        break;
//                    }
//                    case EntityKind.Track:
//                    {
//                        dto.DataType = DataTypes.Track;
//                        dto.TrackName = entity.Title;
//                        break;
//                    }
//                    case EntityKind.Genre:
//                    {
//                        dto.DataType = DataTypes.Genre;
//                        break;
//                    }
//                    case EntityKind.Folder:
//                    {
//                        dto.DataType = DataTypes.Folder;
//                        break;
//                    }
//                    case EntityKind.Episode:
//                    {
//                        dto.DataType = DataTypes.Episode;
//                        break;
//                    }
//                    case EntityKind.Movie:
//                    {
//                        dto.DataType = DataTypes.Movie;
//                        break;
//                    }
//                    case EntityKind.Photo:
//                    {
//                        dto.DataType = DataTypes.Photo;
//                        break;
//                    }
//                    case EntityKind.PhotoAlbum:
//                    {
//                        dto.DataType = DataTypes.PhotoGallery;
//                        break;
//                    }
//                    case EntityKind.Video:
//                    {
//                        dto.DataType = DataTypes.Video;
//                        break;
//                    }
//                    default:
//                    {
//                        dto.DataType = DataTypes.Other;
//                        break;
//                    }
//            }

//            return dto;
//        }

//        private static baseEntity PopulateEntity(baseEntity entity, DataProviderDTO dto)
//        {

//            if (!String.IsNullOrEmpty(dto.AlbumArtist)) { entity.AlbumArtist = dto.AlbumArtist; }
//            if (!String.IsNullOrEmpty(dto.AlbumName)) { entity.AlbumName = dto.AlbumName; }
//            if (!String.IsNullOrEmpty(dto.ArtistName)) { entity.ArtistName = dto.ArtistName; }
//            if (!String.IsNullOrEmpty(dto.Channels)) { entity.Channels = dto.Channels; }
//            if (!String.IsNullOrEmpty(dto.Codec)) { entity.Codec = dto.Codec; }
//            if (!String.IsNullOrEmpty(dto.DiscId)) { entity.DiscId = dto.DiscId; }
//            if (dto.DiscNumber > 0) { entity.DiscNumber = dto.DiscNumber; }
//            if (dto.Duration > 0) { entity.Duration = dto.Duration; }
//            entity.Favorite = dto.Favorite;
//            if (dto.Genre != null) { entity.Genre = dto.Genre; }
//            if (!String.IsNullOrEmpty(dto.Label)) { entity.Label = dto.Label; }
//            if (!String.IsNullOrEmpty(dto.Lyrics)) { entity.Lyrics = dto.Lyrics; }
//            if (!String.IsNullOrEmpty(dto.MusicBrainzId)) { entity.MusicBrainzID = dto.MusicBrainzId; }
//            if (!String.IsNullOrEmpty(dto.Path)) { entity.Path = dto.Path; }
//            if (dto.Performers != null) { entity.Performers = dto.Performers; }
//            if (dto.Rating > 0) { entity.Rating = dto.Rating; }
//            if (entity.ReleaseDate < DateTime.Parse("01-JAN-1000")) 
//            { 
//                entity.ReleaseDate = dto.ReleaseDate;
//            } 
//            if (!String.IsNullOrEmpty(dto.Resolution)) { entity.Resolution = dto.Resolution; }
//            if (!String.IsNullOrEmpty(dto.SampleRate)) { entity.SampleRate = dto.SampleRate; }
//            if (!String.IsNullOrEmpty(dto.Summary)) { entity.Summary = dto.Summary; }
//            if (!String.IsNullOrEmpty(dto.Title)) { entity.Title = dto.Title; }
//            if (dto.TotalPlays > 0) { entity.TotalPlays = dto.TotalPlays; }
//            if (dto.TrackNumber > 0) { entity.TrackNumber = dto.TrackNumber; }
//            if (dto.AlbumCount > 0) { entity.AlbumCount = dto.AlbumCount; }
//            if (dto.ArtistCount > 0) { entity.ArtistCount = dto.ArtistCount; }
//            if (dto.TrackCount > 0) { entity.TrackCount = dto.TrackCount; }
//            if (dto.Listeners > 0) { entity.Listeners = dto.Listeners; }
//            if (dto.PlayCount > entity.PlayCount) { entity.PlayCount = dto.PlayCount; }

//            if (dto.Episode > 0) { entity.Episode = dto.Episode; }
//            if (dto.Season > 0) { entity.Season = dto.Season; }

//            if (dto.ThumbImage != null)
//            {
//                string iconPath = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
//                if (ImageProvider.Save(ImageProvider.Resize(dto.ThumbImage, ImageType.Thumb), iconPath))
//                {
//                    entity.IconPath = iconPath;
//                }
//            }

//            if (dto.BackImages != null && dto.BackImages.Count > 0)
//            {
//                for (int i = 0; i < dto.BackImages.Count; i++)
//                {
//                    string backgroundPath = Util.Helper.ImageCacheFullName(entity.CacheKey + "[" + i + "]", "Backgrounds");
//                    if (ImageProvider.Save(ImageProvider.Resize(dto.BackImages[i], ImageType.Backdrop), backgroundPath))
//                    {
//                        entity.BackgroundPaths.Add(backgroundPath);
//                    }
//                }
//            }

//            switch (dto.DataType)
//            {
//                case DataTypes.Album:
//                    {
//                        entity.Kind = EntityKind.Album;
//                        if (!String.IsNullOrEmpty(dto.AlbumName)) 
//                        {
//                            entity.Title = dto.AlbumName;  
//                        }
//                        break;
//                    }
//                case DataTypes.Artist:
//                    {
//                        entity.Kind = EntityKind.Artist;
//                        if (!String.IsNullOrEmpty(dto.ArtistName))
//                        {
//                            entity.Title = dto.ArtistName;
//                        }
//                        break;
//                    }
//                case DataTypes.Track:
//                    {
//                        entity.Kind = EntityKind.Track;
//                        if (!String.IsNullOrEmpty(dto.TrackName))
//                        {
//                            entity.Title = dto.TrackName;
//                        }
//                        break;
//                    }
//                case DataTypes.Genre:
//                    {
//                        entity.Kind = EntityKind.Genre;
//                        break;
//                    }
//                case DataTypes.Folder:
//                    {
//                        entity.Kind = EntityKind.Folder;
//                        break;
//                    }
//                case DataTypes.Episode:
//                    {
//                        entity.Kind = EntityKind.Episode;
//                        break;
//                    }
//                case DataTypes.Movie:
//                    {
//                        entity.Kind = EntityKind.Movie;
//                        break;
//                    }
//                case DataTypes.Video:
//                    {
//                        entity.Kind = EntityKind.Video;
//                        break;
//                    }
//                case DataTypes.Photo:
//                    {
//                        entity.Kind = EntityKind.Photo;
//                        break;
//                    }
//                case DataTypes.PhotoGallery:
//                    {
//                        entity.Kind = EntityKind.PhotoAlbum;
//                        break;
//                    }
//                case DataTypes.Other:
//                    {
//                        break;
//                    }
//            }
            
//            return entity;
//        }

//        private readonly baseEntity _entity;
//        private readonly bool _forced;

//        public MetadataProviderList(baseEntity entity)
//        {
//            _entity = entity;
//            _forced = false;
//        }

//        public MetadataProviderList(baseEntity entity, bool forced)
//        {
//            _entity = entity;
//            _forced = forced;
//        }

//        #region IBackgroundTaskable Members

//        public string Title
//        {
//            get { return "MetadataProviderList(" + _entity.Path + ")"; }
//        }

//        public void Execute()
//        {
//            try
//            {
//                ProcessEntity(_entity, _forced);
//            }
//            catch (Exception e)
//            {
//                LoggerEngineFactory.Error(new Exception(string.Format("MetadataProviderList failed for {0}\r", _entity.Path), e));
//            }

//        }

//        #endregion
//    }
//}