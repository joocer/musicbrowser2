﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Album : Folder
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageAlbum"; }
        }

        public override string DefaultSort
        {
            get { return "[Track#:sort]"; }
        }

        public override string Information
        {
            get
            {
                return CalculateInformation("", "Track");
            }
        }

        public override void Play(bool queue, bool shuffle)
        {
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(Path);
            List<string> playlist = new List<string>();

            foreach (FileSystemItem item in items)
            {
                if (Util.Helper.getKnownType(item) == Util.Helper.knownType.Track)
                {
                    playlist.Add(item.FullPath);
                }
            }

            if (shuffle)
            {
                Util.Helper.ShuffleList<string>(playlist);
            }

            Engines.Transport.TransportEngineFactory.GetEngine().Play(queue, playlist); 
        }
    }
}
