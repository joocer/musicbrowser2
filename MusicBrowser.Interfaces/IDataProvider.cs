﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

/******************************************************************************
 * 
 *  Contains all of the interfaces, enums and structs for DataProoviders
 * 
 * ***************************************************************************/


namespace MusicBrowser.Interfaces
{

    public enum  DataProviderOutcome
    {
        Success,        // everything was as expected, data is available
        SystemError,    // something went wrong, more info should be in the Errors collection
        NoData,         // the query worked but there's no data or no new data
        NotFound,       // the query worked but there item couldn't be found (e.g. an artist was looked for that doesn't exist)
        InvalidInput
    }

    public enum DataTypes
    {
        Song,
        Album,
        Artist,
        Playlist,
        Disc
    }

    /// <summary>
    /// DTO for passing data to and from data providers
    /// </summary>
    public struct DataProviderDTO
    {
        // in
        public DataTypes DataType;
        public string Path;
        public string DiscId;

        // out
        public DataProviderOutcome Outcome;
        public IEnumerable<string> Errors;

        // in and out
        public string TrackName;
        public string ArtistName;
        public string AlbumArtist;
        public string AlbumName;

        public int TrackNumber;
        public int DiscNumber;
        public DateTime ReleaseDate;

        public string Codec;
        public string Channels;
        public string Duration;
        public int SampleRate;
        public string Resolution;

        public int PlayCount;
        public int Rating;
        public int Listeners;
        public int TotalPlays;
        public bool Favorite;

        public Bitmap ThumbImage;
        public Bitmap BackImage;

        public string MusicBrainzId;

        public IEnumerable<string> Performers;
        public IEnumerable<string> Genres;

        public string Lyrics;

        public string Summary;
    }

    /// <summary>
    /// Provides a common interface for data providers to provide.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Populates the DTO with data
        /// </summary>
        /// <param name="input">DTO</param>
        /// <returns>Populated DTO</returns>
        DataProviderDTO Fetch(DataProviderDTO input);
    }
}
