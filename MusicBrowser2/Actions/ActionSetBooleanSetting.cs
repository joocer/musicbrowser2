﻿using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionSetBooleanSetting : baseActionCommand
    {
        private const string LABEL = "Set Boolean Setting";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        private string _key;
        private bool _value;

        public ActionSetBooleanSetting(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionSetBooleanSetting()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionSetSetting(entity);
        }

        public string Key 
        { 
            get { return _key; }
            set
            {
                _key = value;
                Value = Util.Config.GetInstance().GetBooleanSetting(_key);
                FirePropertyChanged("Value");
            }
        }

        public bool Value 
        { 
            get { return _value; } 
            set
            {
                _value = value;
                FirePropertyChanged("Value");
            } 
        }

        public override void DoAction(Entity entity)
        {
            Util.Config.GetInstance().SetSetting(Key, Value.ToString());
        }
    }
}
