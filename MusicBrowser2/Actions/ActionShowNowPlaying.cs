﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter.Hosting;

namespace MusicBrowser.Actions
{
    public class ActionShowNowPlaying : baseActionCommand
    {
        private const string LABEL = "Show Now Playing";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionShowNowPlaying(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionShowNowPlaying()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionShowKeyboard(entity);
        }

        public override void DoAction(Entity entity)
        {
            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
        }
    }
}