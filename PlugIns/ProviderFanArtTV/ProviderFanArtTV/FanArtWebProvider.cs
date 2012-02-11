﻿using System;
using MusicBrowser.Engines.Logging;
using MusicBrowser.WebServices.Helper;
using MusicBrowser.WebServices.Interfaces;

/******************************************************************************
 * Home Theatre Backdrops access provider
 * ****************************************************************************
 * 
 * JJ - 16-DEC-2010
 * 
 * ***************************************************************************/

namespace MusicBrowser.WebServices.WebServiceProviders
{
    public class FanArtWebProvider : WebServiceProvider
    {
        public override bool ValidateParams()
        {
            return true;
        }

        public override void Execute()
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("FanArtWebProvider.Execute(" + base.URL + ")", "start");
#endif

            try
            {
                HttpProvider http = new HttpProvider();
                http.Url = URL;
                http.Method = HttpProvider.HttpMethod.Get;

                if (!String.IsNullOrEmpty(RequestBody))
                {
                    http.Body = RequestBody;
                    http.Method = HttpProvider.HttpMethod.Post;
                }

                ResponseStatus = "System Error";

                http.DoService();

                ResponseStatus = http.Status;
                ResponseBody = http.Response;
            }
            catch (Exception e)
            {
                LoggerEngineFactory.Error(e);
                ResponseStatus = "Unhandled and unexpected error";
                ResponseBody = string.Empty;
            }
        }
    }
}
