﻿using System;
using System.Drawing;
using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.FanArt;
using MusicBrowser.WebServices.WebServiceProviders;
using MusicBrowser.Providers;

namespace MusicBrowser.Engines.Metadata
{
    public class ArtistFanArtMetadataProvider : baseMetadataProvider
    {
        public ArtistFanArtMetadataProvider()
        {
            Name = "ArtistFanArtMetadataProvider";
            MinDaysBetweenHits = 7;
            MaxDaysBetweenHits = 14;
            RefreshPercentage = 25;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return dto.InheritsFrom<Artist>();
        }

        public override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (String.IsNullOrEmpty(((Artist)dto).MusicBrainzID)) { return false; }
            if (!Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders")) { return false; }
            if (!Util.Config.GetInstance().GetBooleanSetting("EnableFanArt")) { return false; }
            if ((dto.ThumbPath != dto.DefaultThumbPath) && (dto.BackgroundPaths.Count > 0)) { return false; }
            if (String.IsNullOrEmpty(dto.Title)) { return false; }
            return true;
        }

        public override ProviderOutcome DoWork(baseEntity dto)
        {
            // set up the web service classes
            ArtistImageServiceDTO serviceDTO = new ArtistImageServiceDTO();
            ArtistImageService service = new ArtistImageService();
            bool doneSomething = false;

            // set up the search criteria
            serviceDTO.ArtistName = dto.Title;
            serviceDTO.MusicBrainzID = ((Artist)dto).MusicBrainzID;

            // use the HTTP provider to execute the web service
            WebServiceProvider webProvider = new FanArtWebProvider();
            service.SetProvider(webProvider);
            service.Fetch(serviceDTO);

            // handle error back from the provider
            if (serviceDTO.Status == WebServiceStatus.Error)
            {
                return ProviderOutcome.SystemError;
            }

            // handle the response
            if ((serviceDTO.ClearLogoList.Count > 0) && (!dto.LogoExists))
            {
                Bitmap thumb = ImageProvider.Download(serviceDTO.ClearLogoList.First(), ImageType.Thumb);
                dto.ThumbPath = Util.Helper.ImageCacheFullName(dto.CacheKey, ImageType.Logo, -1);
                ImageProvider.Save(thumb, dto.LogoPath);
                doneSomething = true;
            }

            if ((serviceDTO.BackdropList.Count > 0) && (dto.BackgroundPaths.Count == 0))
            {
                // limit to 10 backdrops
                foreach (string img in serviceDTO.BackdropList.Take(10))
                {
                    // wrap in a try, if one fails we don't want them all to fail
                    try
                    {
                        Bitmap i = ImageProvider.Download(img, ImageType.Backdrop);
                        if (i != null)
                        {
                            string path = Util.Helper.ImageCacheFullName(dto.CacheKey, ImageType.Backdrop, dto.BackgroundPaths.Count);
                            ImageProvider.Save(i, path);
                            dto.BackgroundPaths.Add(path);
                            doneSomething = true;
                        }
                    }
                    catch { }
                }
            }

            if (!doneSomething)
            {
                return ProviderOutcome.NoData;
            }

            return ProviderOutcome.Success;

        }
    }
}
