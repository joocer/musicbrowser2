﻿using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Actions
{
    static class Factory
    {
        private struct ActionConfigEntry
        {
            public baseActionCommand OnRecord;
            public baseActionCommand OnEnter;
            public baseActionCommand OnPlay;
            public baseActionCommand OnStar;

            public IList<baseActionCommand> MenuOptions;
        }

        private static readonly IEnumerable<baseActionCommand> _availableActions = GetAvailableActions();
        private static readonly IDictionary<String, ActionConfigEntry> _actionConfig = GetActionConfig();

        public static baseActionCommand ActionFactory(String name)
        {
            foreach (baseActionCommand action in _availableActions)
            {
                if (action.ToString().EndsWith(".action" + name, StringComparison.OrdinalIgnoreCase))
                {
                    return action;
                }
            }

            return new ActionNoOperation();
        }

        public static baseActionCommand ActionFactory(XmlNode node)
        {
            if (node != null)
            {
                return ActionFactory(node.InnerText);
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetEnterAction(baseEntity entity)
        {
            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnEnter.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnEnter.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetPlayAction(baseEntity entity)
        {
            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnPlay.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnPlay.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetStarAction(baseEntity entity)
        {
            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnStar.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnStar.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static baseActionCommand GetRecordAction(baseEntity entity)
        {
            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].OnRecord.GetType() != typeof(ActionNoOperation))
                    {
                        return _actionConfig[leaf].OnRecord.NewInstance(entity);
                    }
                }
            }
            return new ActionNoOperation();
        }

        public static List<baseActionCommand> GetActionList(baseEntity entity)
        {
            IEnumerable<string> tree = entity.Tree();
            foreach (string leaf in tree)
            {
                if (_actionConfig.ContainsKey(leaf))
                {
                    if (_actionConfig[leaf].MenuOptions.Count > 0)
                    {
                        List<baseActionCommand> ret = new List<baseActionCommand>();
                        foreach (baseActionCommand action in _actionConfig[leaf].MenuOptions)
                        {
                            ret.Add(action.NewInstance(entity));
                        }
                        return ret;
                    }
                }
            }
            return null;
        }

        private static IEnumerable<baseActionCommand> GetAvailableActions()
        {
            List<baseActionCommand> ret = new List<baseActionCommand>();

            ret.Add(new ActionCloseMenu());
            ret.Add(new ActionCycleViews());
            ret.Add(new ActionNoOperation()); // AKA Do Nothing
            ret.Add(new ActionOpen());
            ret.Add(new ActionPause());
            ret.Add(new ActionPlay());
            ret.Add(new ActionPlayEntireLibrary());
            ret.Add(new ActionPlayFavourites());
            ret.Add(new ActionPlayNewlyAdded());
            ret.Add(new ActionPlayMostPopular());
            ret.Add(new ActionPlayPopularLastFM());
            ret.Add(new ActionPlayRandomPopular());
            ret.Add(new ActionPlayRandomPopularLastFM());
            ret.Add(new ActionPlaySimilarTracks());
            ret.Add(new ActionPreviousPage());
            ret.Add(new ActionQueue());
            ret.Add(new ActionRefreshMetadata());
            ret.Add(new ActionResume());
            ret.Add(new ActionShowActions());
            ret.Add(new ActionShowKeyboard());
            ret.Add(new ActionShowSearch());
            ret.Add(new ActionShowSettings());
            ret.Add(new ActionShuffle());
            ret.Add(new ActionSkipBack());
            ret.Add(new ActionSkipForward());
            ret.Add(new ActionStop());

            return ret;
        }

        private static IDictionary<String, ActionConfigEntry> GetActionConfig()
        {
            IDictionary<String, ActionConfigEntry> actions = new Dictionary<String, ActionConfigEntry>();

            string configFile = Path.Combine(Util.Helper.AppFolder, "actions.config");
            XmlDocument xml = new XmlDocument();
            try
            {
                if (File.Exists(configFile))
                {
                    LoggerEngineFactory.Debug("using actions override file");
                    xml.Load(configFile);
                }
                else
                {
                    xml.LoadXml(Resources.ActionConfig);
                }
            }
            catch (Exception e)
            {
                LoggerEngineFactory.Error(e);
                throw e;
            }

            XmlNodeList nodes = xml.SelectNodes("ActionConfig/Entity");
            foreach(XmlNode node in nodes)
            {
                try
                {
                    ActionConfigEntry entry = new ActionConfigEntry();
                    entry.MenuOptions = new List<baseActionCommand>();

                    entry.OnEnter = ActionFactory(node.SelectSingleNode("OnEnter"));
                    entry.OnPlay = ActionFactory(node.SelectSingleNode("OnPlay"));
                    entry.OnRecord = ActionFactory(node.SelectSingleNode("OnRecord"));
                    entry.OnStar = ActionFactory(node.SelectSingleNode("OnStar"));

                    foreach (XmlNode item in node.SelectNodes("MenuItems/Item"))
                    {
                        entry.MenuOptions.Add(ActionFactory(item.InnerText));
                    }
                    entry.MenuOptions.Add(new ActionCloseMenu());

                    actions.Add(node.Attributes["name"].InnerText, entry);
                }
                catch (Exception e)
                {
                    LoggerEngineFactory.Error(e);
                    throw e;
                }
            }

            return actions;
        }
    }
}