﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardJIL : IKeyboardHandler
    {
        public override void DoService()
        {
            foreach (baseEntity item in RawDataSet)
            {
                if (String.Compare(item.SortName, Value, true) > 0)
                {
                    Index = item.Index;
                    return;
                }
            }
            // if no match is found, go to the end of the list
            Index = RawDataSet.Count();
        }
    }
}