using BepInEx;
using BepInEx.Bootstrap;
using MonkeCosmetics.Cosmetic;
using MonkeCosmetics.Cosmetic.Pages;
using MonkeCosmetics.Scripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MonkeCosmetics
{
    public class CustomCosmeticManager : MonoBehaviour
    {
        public static CustomCosmeticManager instance;

        public static List<MonkeCosmeticPage> Pages = [];

        public static MonkeCosmeticPage CurrentPage;

        public List<GameObject> Buttons = [];

        public int Index;

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            StartAF();
        }

        private void StartAF()
        {
            Pages.Add(ScriptableObject.CreateInstance<MaterialPage>());            

            /*IEnumerable<Assembly> assemblies = Chainloader.PluginInfos.Values.Select(pluginInfo => pluginInfo.Instance.GetType().Assembly).Distinct();
            IEnumerable<MonkeCosmeticPage> found = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(foundEntry => typeof(MonkeCosmeticPage).IsAssignableFrom(foundEntry) && !foundEntry.IsInterface).Select(entryType => (MonkeCosmeticPage)Activator.CreateInstance(entryType)).Where(viewEntry => Pages.All(existingEntry => existingEntry.GetType() != viewEntry.GetType()));
            Pages.AddRange(found);*/

            foreach (var e in Pages) e.OnMonkeCosmeticsIntialised();

            SetPage(MaterialPage.instance);

            Buttons.AddRange([Plugin.Left, Plugin.Right, Plugin.Equip]);

            foreach (GameObject button in Buttons)
            {
                button.AddComponent<ButtonHandler>();
                button.layer = 18;
            }

            UpdateDisplay();
        }

        private void SetPage(MaterialPage page)
        {
            CurrentPage = page; 
            page.OnPageEntered();
        }

        void UpdateDisplay()
        {
            Plugin.PageLeft.texture = Pages[(Index - 1 + Pages.Count) % Pages.Count].Icon;
            Plugin.PageMain.texture = Pages[Index].Icon;
            Plugin.PageRight.texture = Pages[(Index + 1) % Pages.Count].Icon;

            CurrentPage.OnPageUpdate();
        }

        public void LeftArrow()
        {
            CurrentPage.OnLeftPress();

            UpdateDisplay();
        }

        public void RightArrow()
        {
            CurrentPage.OnRightPress();

            UpdateDisplay();
        }

        public void SelectPress()
        {
            CurrentPage.OnEquipPress();

            UpdateDisplay();
        }

        public void PageLeftArrow()
        {
            if (Index > 0) Index--;
            else Index = Pages.Count - 1;

            UpdateDisplay();
        }

        public void PageRightArrow()
        {
            if (Index < Pages.Count - 1) Index++;
            else Index = 0;

            UpdateDisplay();
        }

        public void PageSelectPress()
        {
            if (Pages[Index] != CurrentPage)
            {
                CurrentPage = Pages[Index];
                CurrentPage.OnPageEntered();
            }
            UpdateDisplay();
        }
    }
    public static class Extensions
    {
        public static bool IsTagged(this VRRig rig) => rig.setMatIndex == 2 || rig.setMatIndex == 11 || rig.setMatIndex == 1;
    }
}
