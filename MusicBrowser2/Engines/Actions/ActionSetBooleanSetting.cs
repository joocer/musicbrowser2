﻿using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionSetBooleanSetting : baseActionCommand
    {
        private const string LABEL = "Set Boolean Setting";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        private string _key;
        private bool _value;

        public ActionSetBooleanSetting(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSetBooleanSetting(entity);
        }

        public string Key 
        { 
            get { return _key; }
            set
            {
                _key = value;
                Value = Util.Config.GetBooleanSetting(_key);
                FirePropertyChanged("Value");
            }
        }

        public bool Value 
        { 
            get { return _value; } 
            set
            {
                if (value != _value)
                {
                    _value = value;
                    FirePropertyChanged("Value");
                }
            } 
        }

        public override void DoAction(baseEntity entity)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                Util.Config.SetSetting(Key, Value.ToString());
            }
        }
    }
}
