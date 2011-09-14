﻿using System.Collections.Generic; 
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
 
// taken from the SDK example, I've changed the namespace and classname

namespace MusicBrowser 
{     
    public class EntryPoint : IAddInModule, IAddInEntryPoint     
    {         
        private static HistoryOrientedPageSession _sSession; 
         
        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)         
        { 
        }          
        
        public void Uninitialize()         
        {
            Providers.Transport.Transport.GetTransport().Close();
            if (Util.Config.GetInstance().GetBooleanSetting("LogStatsOnClose"))
            {
                Logging.Logger.Stats(Providers.Statistics.GetInstance());
#if DEBUG
                Logging.Logger.Verbose(Util.Helper.outputTypes(), "stats");
#endif
            }
            CacheEngine.NearLineCache.GetInstance().Save();
        }
        
        public void Launch(AddInHost host)         
        {
            if (host != null && host.ApplicationContext != null)
            {                 
                host.ApplicationContext.SingleInstance = true;
            }
            _sSession = new HistoryOrientedPageSession();
            Application app = new Application(_sSession, host);
            IEntity home = new Entities.Kinds.Home();

            // load the nearline cache
            CacheEngine.NearLineCache.GetInstance().Init();

            app.Navigate(home, new Breadcrumbs());

            //trigger the background caching tasks
            foreach (string path in MusicBrowser.Entities.Kinds.Home.Paths)
            {
                CommonTaskQueue.Enqueue(new BackgroundCacheProvider(path));
            }
        }     
    } 
} 