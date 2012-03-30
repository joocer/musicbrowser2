﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Providers;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    [DataContract]
    class VideoCollection : Collection
    {
        public override string Information
        {
            get
            {
                return CalculateInformation("Video Collection", "Movie", "Show");
            }
        }

        public override void Play(bool queue, bool shuffle)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            MediaCollection collection = new MediaCollection();

            List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(Path)
                .FilterInternalFiles()
                .OrderBy(item => item.Name)
                .ToList();

            if (shuffle)
            {
                candidateitems = candidateitems.Shuffle().ToList();
            }

            foreach (FileSystemItem item in candidateitems)
            {
                if (Helper.GetKnownType(item) == Helper.KnownType.Video)
                {
                    collection.AddItem(item.FullPath);
                    collection[collection.Count - 1].FriendlyData.Add("Title", System.IO.Path.GetFileNameWithoutExtension(item.Name));
                }
            }

            mce.PlayMedia(MediaType.MediaCollection, collection, false);
            mce.MediaExperience.GoToFullScreen();
        }
    }
}
