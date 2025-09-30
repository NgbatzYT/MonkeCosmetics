using BepInEx.Bootstrap;
using BepInEx.Configuration;
using ComputerInterface;
using ComputerInterface.Extensions;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeCosmetics.Scripts
{
    internal class MCconfigEntry : IComputerModEntry
    {
        public string EntryName => "Monke Cosmetics";
        public Type EntryViewType => typeof(ConfigView);
    }

    internal class ConfigView : ComputerView
    {
        internal class ConfEntry
        {
            public void Toggle()
            {
                if (Plugin.Instance.materialSet.Value)
                { 
                    Plugin.Instance.SaveMaterialSet(false);
                }
                else
                {
                    Plugin.Instance.SaveMaterialSet(true);
                }
            }
        }
        private readonly List<ConfEntry> _plugins;

        private readonly UIElementPageHandler<ConfEntry> _pageHandler;
        private readonly UISelectionHandler _selectionHandler;

        public ConfigView()
        { 
            _plugins = [];
            _plugins.Add(new ConfEntry());
            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter)
            {
                MaxIdx = _plugins.Count - 1
            };
            _selectionHandler.OnSelected += SelectMod;
            _selectionHandler.ConfigureSelectionIndicator($"<color=#{PrimaryColor}>> </color>", "", "  ", "");

            _pageHandler = new UIElementPageHandler<ConfEntry>
            {
                EntriesPerPage = 9
            };

            _pageHandler.SetElements([.. _plugins]);
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            Redraw();
        }

        private void Redraw()
        {
            StringBuilder builder = new();

            RedrawHeader(builder);
            DrawMods(builder);

            Text = builder.ToString();
        }

        private void RedrawHeader(StringBuilder str)
        {
            str.BeginColor("ffffff50").Append("== ").EndColor();
            str.Append($"Monke Cosmetics Config Editor").BeginColor("ffffff50").Append(" ==").EndColor().AppendLine();
        }

        private void DrawMods(StringBuilder str)
        {
            string enabledPrefix = "<color=#00ff00> + </color>";
            string disabledPrefix = "<color=#ff0000> - </color>";

            int lineIdx = _pageHandler.MovePageToIdx(_selectionHandler.CurrentSelectionIndex);

            _pageHandler.EnumarateElements((config, idx) =>
            {
                str.AppendLine();
                str.Append(Plugin.Instance.materialSet.Value ? enabledPrefix : disabledPrefix);
                str.Append(_selectionHandler.GetIndicatedText(idx, lineIdx, "Set your material for others"));
            });

            str.AppendLines(2);
            _pageHandler.AppendFooter(str);
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_selectionHandler.HandleKeypress(key))
            {
                Redraw();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
            }
        }

        private void SelectMod(int idx)
        {
            _plugins[idx].Toggle();
            Redraw();
        }
    }
}