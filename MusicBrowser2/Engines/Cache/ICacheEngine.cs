﻿using System.Collections.Generic;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public interface ICacheEngine
    {
        void Delete(string key);
        baseEntity Fetch(string key);
        void Update(baseEntity entity);

        bool Exists(string key);

        void Scavenge();
        void Compress();
        void Clear();

        IEnumerable<string> Search(string kind, string criteria);
        Dictionary<string, int> HitsByType(string criteria);
    }
}
