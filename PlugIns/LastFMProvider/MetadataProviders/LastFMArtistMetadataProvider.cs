﻿//using System;
//using MusicBrowser.Entities;
//using MusicBrowser.WebServices.Interfaces;
//using MusicBrowser.WebServices.Services.LastFM;
//using MusicBrowser.WebServices.WebServiceProviders;
//using MusicBrowser.Interfaces;
//using MusicBrowser.Engines.Metadata;

//namespace MusicBrowser.Providers.Metadata
//{
//    class LastFMMetadataProvider : IProvider
//    {
//        private const string Name = "LastFMMetadataProvider";

//        private const int MinDaysBetweenHits = 5;
//        private const int MaxDaysBetweenHits = 100;
//        private const int RefreshPercentage = 95;

//        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

//        public ProviderOutcome Fetch(baseEntity dto)
//        {
//#if DEBUG
//            Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
//#endif
//            ProviderOutcome outcome = ProviderOutcome.Success;

//            #region killer questions

//            if (!Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders"))
//            {
//                outcome = ProviderOutcome.NoData;
//            }

//            #endregion

//            Statistics.Hit(Name + ".hit");

//            WebServiceProvider lfmProvider = new LastFMWebProvider();

//            return outcome;
//        }


//        //public void dead
//        //{
//        //    switch (dto.DataType)
//        //    {
//        //        case DataTypes.Album:
//        //            {
//        //                #region killer questions

//        //                if (string.IsNullOrEmpty(dto.AlbumName))
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.InvalidInput;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Missing album data: Album name [" + dto.Path + "]" };
//        //                    return dto;
//        //                }

//        //                if (string.IsNullOrEmpty(dto.AlbumArtist))
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.InvalidInput;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Missing data: Artist name [" + dto.Path + "]" };
//        //                    return dto;
//        //                }

//        //                #endregion

//        //                AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
//        //                albumDTO.Album = dto.AlbumName;
//        //                albumDTO.MusicBrainzID = dto.MusicBrainzId;
//        //                albumDTO.Artist = dto.AlbumArtist;
//        //                albumDTO.Username = (Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
//        //                if (dto.ProviderTimeStamps.ContainsKey(FriendlyName()))
//        //                {
//        //                    albumDTO.lastAccessed = dto.ProviderTimeStamps[FriendlyName()];
//        //                }
//        //                else { albumDTO.lastAccessed = DateTime.Parse("01-01-1000"); }

//        //                AlbumInfoService albumService = new AlbumInfoService();
//        //                albumService.SetProvider(lfmProvider);
//        //                albumService.Fetch(albumDTO);

//        //                // handle error back from the provider
//        //                if (albumDTO.Status == WebServiceStatus.Error)
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.SystemError;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + albumDTO.Error + ") Artist (" + albumDTO.Artist + ") Album (" + albumDTO.Album + ")" };
//        //                    return dto;
//        //                }
//        //                if (albumDTO.Status == WebServiceStatus.Warning)
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.NoData;
//        //                    return dto;
//        //                }

//        //                dto.PlayCount = albumDTO.Plays;
//        //                dto.Listeners = albumDTO.Listeners;
//        //                dto.TotalPlays = albumDTO.TotalPlays;

//        //                dto.Title = albumDTO.Album;
//        //                dto.AlbumName = dto.Title;
//        //                dto.Summary = albumDTO.Summary;
//        //                dto.MusicBrainzId = albumDTO.MusicBrainzID;

//        //                dto.ReleaseDate = albumDTO.Release;

//        //                if (!dto.hasThumbImage && !string.IsNullOrEmpty(albumDTO.Image))
//        //                {
//        //                    dto.ThumbImage = ImageProvider.Download(albumDTO.Image, ImageType.Thumb);
//        //                }

//        //                break;
//        //            }
//        //        case DataTypes.Artist:
//        //            {
//        //                #region killer questions

//        //                if (string.IsNullOrEmpty(dto.ArtistName))
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.InvalidInput;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Missing artist data: Artist name [" + dto.Path + "]" };
//        //                    return dto;
//        //                }

//        //                #endregion

//        //                ArtistInfoServiceDTO artistDTO = new ArtistInfoServiceDTO();
//        //                artistDTO.Artist = dto.ArtistName;
//        //                artistDTO.MusicBrainzID = dto.MusicBrainzId;
//        //                artistDTO.Username = (Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
//        //                if (dto.ProviderTimeStamps.ContainsKey(FriendlyName()))
//        //                {
//        //                    artistDTO.lastAccessed = dto.ProviderTimeStamps[FriendlyName()];
//        //                }
//        //                else { artistDTO.lastAccessed = DateTime.Parse("01-01-1000"); }

//        //                ArtistInfoService artistService = new ArtistInfoService();
//        //                artistService.SetProvider(lfmProvider);
//        //                artistService.Fetch(artistDTO);

//        //                // handle error back from the provider
//        //                if (artistDTO.Status == WebServiceStatus.Error)
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.SystemError;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + artistDTO.Error + ") Artist (" + artistDTO.Artist + ")" };
//        //                    return dto;
//        //                }
//        //                if (artistDTO.Status == WebServiceStatus.Warning)
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.NoData;
//        //                    return dto;
//        //                }

//        //                dto.PlayCount = artistDTO.Plays;
//        //                dto.Listeners = artistDTO.Listeners;
//        //                dto.TotalPlays = artistDTO.TotalPlays;

//        //                dto.Title = artistDTO.Artist;
//        //                dto.ArtistName = dto.Title;
//        //                dto.MusicBrainzId = artistDTO.MusicBrainzID;
//        //                dto.Summary = artistDTO.Summary;

//        //                if (!dto.hasThumbImage && !string.IsNullOrEmpty(artistDTO.Image))
//        //                {
//        //                    dto.ThumbImage = ImageProvider.Download(artistDTO.Image, ImageType.Thumb);
//        //                }

//        //                break;
//        //            }
//        //        case DataTypes.Track:
//        //            {
//        //                #region killer questions

//        //                if (string.IsNullOrEmpty(dto.ArtistName))
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.InvalidInput;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Missing track data: Artist name [" + dto.Path + "]" };
//        //                    return dto;
//        //                }

//        //                #endregion

//        //                TrackInfoDTO trackDTO = new TrackInfoDTO();
//        //                trackDTO.Track = dto.TrackName;
//        //                trackDTO.Artist = dto.ArtistName;
//        //                trackDTO.MusicBrainzID = dto.MusicBrainzId;
//        //                trackDTO.Username = (Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
//        //                if (dto.ProviderTimeStamps.ContainsKey(FriendlyName()))
//        //                {
//        //                    trackDTO.lastAccessed = dto.ProviderTimeStamps[FriendlyName()];
//        //                }
//        //                else { trackDTO.lastAccessed = DateTime.Parse("01-01-1000"); }

//        //                TrackInfoService trackService = new TrackInfoService();
//        //                trackService.SetProvider(lfmProvider);
//        //                trackService.Fetch(trackDTO);

//        //                // handle error back from the provider
//        //                if (trackDTO.Status == WebServiceStatus.Error)
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.SystemError;
//        //                    dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + trackDTO.Error + ") Artist (" + trackDTO.Artist + ") Track (" + trackDTO.Track + ")" };
//        //                    return dto;
//        //                }
//        //                if (trackDTO.Status == WebServiceStatus.Warning)
//        //                {
//        //                    dto.Outcome = DataProviderOutcome.NoData;
//        //                    return dto;
//        //                }

//        //                dto.PlayCount = trackDTO.Plays;
//        //                dto.Listeners = trackDTO.Listeners;
//        //                dto.TotalPlays = trackDTO.TotalPlays;

//        //                dto.Title = trackDTO.Track;
//        //                dto.TrackName = dto.Title;
//        //                dto.Summary = trackDTO.Summary;
//        //                dto.MusicBrainzId = trackDTO.MusicBrainzID;
//        //                dto.Favorite = trackDTO.Loved;

//        //                break;
//        //            }
//        //    }

//        //    return dto;
//        //}

//        public string FriendlyName()
//        {
//            return Name;
//        }

//        public bool CompatibleWith(string type)
//        {
//            return (type.ToLower() == "artist") || (type.ToLower() == "album") || (type.ToLower() == "track");
//        }

//        /// <summary>
//        /// refresh requests between the min and max refresh period have 5% chance of refreshing
//        /// </summary>
//        private static bool RandomlyRefreshData(DateTime stamp)
//        {
//            // if it's never refreshed, refresh it
//            if (stamp < DateTime.Parse("01-JAN-1000")) { return true; }

//            // if it's less then the min, don't refresh if it's older than the max then do refresh
//            int dataAge = (DateTime.Today.Subtract(stamp)).Days;
//            if (dataAge <= MinDaysBetweenHits) { return false; }
//            if (dataAge >= MaxDaysBetweenHits) { return true; }

//            // otherwise refresh randomly (95% don't refresh each run)
//            return (Rnd.Next(100) >= RefreshPercentage);
//        }

//        public bool isStale(DateTime lastAccess)
//        {
//            // refresh strategy for Last.fm is complicated, this first bit is just to randomly 
//            return RandomlyRefreshData(lastAccess);
//        }
//    }
//}