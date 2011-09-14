﻿using System;
using System.Collections.Generic;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;

namespace MusicBrowser.Providers.Metadata
{
    public class TagSharpMetadataProvider : IDataProvider
    {
        private const string Name = "Tag#";

        public string FriendlyName() { return Name; }

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Logging.Logger.Verbose(Name + ": " + dto.Path, "start");
#endif
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            if (!Helper.IsSong(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a song: " + dto.Path };
                return dto;
            }

            #endregion

            Statistics.GetInstance().Hit(Name + ".hit");

            try
            {
                TagLib.File fileTag = TagLib.File.Create(dto.Path);

                if (!String.IsNullOrEmpty(fileTag.Tag.Title))
                {
                    dto.TrackName = fileTag.Tag.Title;
                    dto.AlbumName = fileTag.Tag.Album;
                    dto.ArtistName = fileTag.Tag.FirstPerformer;
                    dto.AlbumArtist = fileTag.Tag.FirstAlbumArtist;
                    dto.ReleaseDate = Convert.ToDateTime("01-JAN-" + fileTag.Tag.Year);
                    dto.DiscNumber = Convert.ToInt32(fileTag.Tag.Disc);
                    dto.TrackNumber = Convert.ToInt32(fileTag.Tag.Track);
                    dto.Codec = fileTag.MimeType.Substring(7).ToLower();
                    dto.Duration = Convert.ToInt32(fileTag.Properties.Duration.TotalSeconds);
                    dto.MusicBrainzId = fileTag.Tag.MusicBrainzTrackId;

                    if (fileTag.Tag.Performers != null) { dto.Performers.AddRange(fileTag.Tag.Performers); }
                    if (fileTag.Tag.Genres != null) { dto.Genres.AddRange(fileTag.Tag.Genres); }
                    if (string.IsNullOrEmpty(dto.AlbumArtist)) { dto.AlbumArtist = dto.ArtistName; }

                    // cache the thumb
                    foreach (TagLib.IPicture pic in fileTag.Tag.Pictures)
                    {
                        if (!pic.Data.IsEmpty)
                        {
                            dto.ThumbImage = ImageProvider.Convert(pic);
                            break;
                        }
                    }
                }
                else
                {
                    dto.Outcome = DataProviderOutcome.NotFound;
                    dto.Errors = new List<string> { "No data found in tags" };
                }
            }
            catch
            {
                dto.Outcome = DataProviderOutcome.SystemError;
                dto.Errors = new List<string> { "Error processing file" };
            }

            return dto;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "song");
        }

        public static void FetchLite(IEntity entity)
        {
            // this implements a cut-down version of the Tag# metadata fetcher
            // this is meant to be as quick as possible to not hold up the UI

            #region killer questions
            if (entity.ProviderTimeStamps.ContainsKey(Name)) { return; }
            if (!entity.Kind.Equals(EntityKind.Song)) { return; }
            #endregion

            try
            {
                TagLib.File fileTag = TagLib.File.Create(entity.Path);
                if (!String.IsNullOrEmpty(fileTag.Tag.Title))
                {
                    entity.Title = fileTag.Tag.Title;
                    entity.TrackName = entity.Title;
                    entity.DiscNumber = Convert.ToInt32(fileTag.Tag.Disc);
                    entity.TrackNumber = Convert.ToInt32(fileTag.Tag.Track);
                    entity.Duration = Convert.ToInt32(fileTag.Properties.Duration.TotalSeconds);
                    entity.ReleaseDate = Convert.ToDateTime("01-JAN-" + fileTag.Tag.Year);
                    entity.AlbumArtist = fileTag.Tag.FirstAlbumArtist;
                    entity.ArtistName = fileTag.Tag.FirstPerformer;

                    if (string.IsNullOrEmpty(entity.AlbumArtist)) { entity.AlbumArtist = entity.ArtistName; }
                }
            }
            catch { }
        }

        public bool isStale(DateTime lastAccess)
        {
            // this shouldn't need any updates
            return (lastAccess < DateTime.Parse("01-JAN-1000"));
        }
    }
}