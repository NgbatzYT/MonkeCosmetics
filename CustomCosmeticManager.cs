using MonkeCosmetics.Cosmetic;
using MonkeCosmetics.Cosmetic.Pages;
using MonkeCosmetics.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeCosmetics
{
    public class CustomCosmeticManager : MonoBehaviour
    {
        public static CustomCosmeticManager instance;

        public static event Action OnInit;

        public static List<MonkeCosmeticPage> Pages = [];

        public static MonkeCosmeticPage CurrentPage;

        public List<GameObject> Buttons = [];

        public int Index;

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            AddPage(typeof(MaterialPage));
            OnInit?.Invoke();
        }

        public void AddPage(Type page)
        {
            if (page == null) return;
            if (!typeof(MonkeCosmeticPage).IsAssignableFrom(page)) return;

            var p = ScriptableObject.CreateInstance(page) as MonkeCosmeticPage;

            if(p == null) return;

            Pages.Add(p);
            p.OnMonkeCosmeticsIntialised();
            UpdateDisplay();
        }

        public void SetPage(MonkeCosmeticPage page)
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

            if (Pages[Index] != CurrentPage)
                SetPage(Pages[Index]);

            UpdateDisplay();
        }

        public void PageRightArrow()
        {
            if (Index < Pages.Count - 1) Index++;
            else Index = 0;

            if (Pages[Index] != CurrentPage)
                SetPage(Pages[Index]);

            UpdateDisplay();
        }
    }
    public static class Extensions
    {
        public static bool IsTagged(this VRRig rig) => rig.setMatIndex == 2 || rig.setMatIndex == 11 || rig.setMatIndex == 1;
    }
}
