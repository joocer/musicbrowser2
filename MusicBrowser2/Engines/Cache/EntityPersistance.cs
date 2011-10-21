﻿using MusicBrowser.Entities;
using ServiceStack.Text;
using System;

namespace MusicBrowser.Engines.Cache
{
    public static class EntityPersistance
    {

        public static string Serialize(Entity data)
        {
            return data.ToXml();
        }

        public static Entity Deserialize(string data)
        {
            if (String.IsNullOrEmpty(data)) { return null; }

            try
            {
                return XmlSerializer.DeserializeFromString<Entity>(data);
            }
            catch
            {
                return null;
            }
        }

        //public static IEntity Deserialize(string data, bool a)
        //{
        //    IEntity entity;

        //    try
        //    {
        //        XmlDocument xml = new XmlDocument();

        //        xml.LoadXml(data);

        //        switch (Helper.ReadXmlNode(xml, "EntityXML/@type").ToLower())
        //        {
        //            case "track":
        //                {
        //                    entity = new Track();
        //                    break;
        //                }
        //            case "album":
        //                {
        //                    entity = new Album();
        //                    break;
        //                }
        //            case "artist":
        //                {
        //                    entity = new Artist();
        //                    break;
        //                }
        //            case "genre":
        //                {
        //                    entity = new Genre();
        //                    break;
        //                }
        //            case "playlist":
        //                {
        //                    entity = new Playlist();
        //                    break;
        //                }
        //            case "folder":
        //                {
        //                    entity = new Folder();
        //                    break;
        //                }
        //            case "home":
        //                {
        //                    entity = new Home();
        //                    break;
        //                }
        //            default:
        //                {
        //                    throw new Exception("unknown type");
        //                }
        //        }

        //        // string reads
        //        entity.Title = Helper.ReadXmlNode(xml, "EntityXML/Title");
        //        entity.Summary = Helper.ReadXmlNode(xml, "EntityXML/Summary");
        //        entity.MusicBrainzID = Helper.ReadXmlNode(xml, "EntityXML/MusicBrainzID");
        //        entity.TrackName = Helper.ReadXmlNode(xml, "EntityXML/TrackName");
        //        entity.ArtistName = Helper.ReadXmlNode(xml, "EntityXML/ArtistName");
        //        entity.AlbumArtist = Helper.ReadXmlNode(xml, "EntityXML/AlbumArtist");
        //        entity.AlbumName = Helper.ReadXmlNode(xml, "EntityXML/AlbumName");
        //        entity.Lyrics = Helper.ReadXmlNode(xml, "EntityXML/Lyrics");
        //        entity.Codec = Helper.ReadXmlNode(xml, "EntityXML/Codec");
        //        entity.Channels = Helper.ReadXmlNode(xml, "EntityXML/Channels");
        //        entity.SampleRate = Helper.ReadXmlNode(xml, "EntityXML/SampleRate");
        //        entity.Resolution = Helper.ReadXmlNode(xml, "EntityXML/Resolution");
        //        entity.DiscId = Helper.ReadXmlNode(xml, "EntityXML/DiscId");

        //        //  number reads
        //        int i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Duration"), out i); entity.Duration = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Listeners"), out i); entity.Listeners = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/TotalPlays"), out i); entity.TotalPlays = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/PlayCount"), out i); entity.PlayCount = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Duration"), out i); entity.Duration = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Rating"), out i); entity.Rating = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/TrackNumber"), out i); entity.TrackNumber = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/DiscNumber"), out i); entity.DiscNumber = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Albums"), out i); entity.AlbumCount = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Artists"), out i); entity.ArtistCount = i;
        //        int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Tracks"), out i); entity.TrackCount = i;

        //        // date reads
        //        DateTime d;
        //        DateTime.TryParse(Helper.ReadXmlNode(xml, "EntityXML/ReleaseDate"), out d); entity.ReleaseDate = d;

        //        // path reads
        //        string backgroundImage = Helper.ReadXmlNode(xml, "EntityXML/BackgroundPath");
        //            if (File.Exists(backgroundImage)) { entity.BackgroundPath = backgroundImage; }
        //        string thumbImage = Helper.ReadXmlNode(xml, "EntityXML/IconPath"); 
        //            if (File.Exists(thumbImage)) { entity.IconPath = thumbImage; }

        //        // boolean reads
        //        entity.Favorite = Helper.ReadXmlNode(xml, "EntityXML/Favorite").ToLower() == "true";

        //        // list reads
        //        entity.Performers.AddRange(Helper.ReadXmlNode(xml, "ExntityXML/Performers").Split('|'));
        //        entity.Genres.AddRange(Helper.ReadXmlNode(xml, "ExntityXML/Genres").Split('|'));

        //        // complex reads
        //        foreach (XmlNode node in xml.SelectNodes("/EntityXML/ProviderTimeStamps/Provider"))
        //        {
        //            entity.ProviderTimeStamps[node.Attributes["name"].InnerText] = Convert.ToDateTime(node.Attributes["timestamp"].InnerText);
        //        }
        //    }
        //    catch
        //    {
        //        // there's been a problem
        //        return new Unknown();
        //    }
        //    return entity;
        //}
    }
}