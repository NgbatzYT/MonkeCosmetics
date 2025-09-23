using BepInEx.Configuration;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
namespace MonkeCosmetics
{
    internal class CosmeticsNetworking : MonoBehaviourPunCallbacks
    {
        Hashtable LocalCosmetics;

        public static CosmeticsNetworking Instance;
        private Material defaultMaterial;

        void Start() 
        {
            defaultMaterial = VRRig.LocalRig.materialsToChangeTo[0];
            Instance = this;
        }
        

        public override void OnJoinedLobby()
        {
            var CCM = CustomCosmeticManager.instance;
            if (CustomCosmeticManager.instance.currentMaterial != null)
            {
                LocalCosmetics = new Hashtable
                {
                    { "MonkeCosmetics::Material", CustomCosmeticManager.instance.currentMaterial.name }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);
            }
            else 
            {
                LocalCosmetics = new Hashtable
                {
                    { "MonkeCosmetics::Material", null }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);
            }

            foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers)
            {
                var e = GorillaGameManager.instance.FindPlayerVRRig(p);

                if (e.isLocal) { continue; }

                if (e.IsTagged()) { continue; }

                var matName = p.GetPlayerRef().CustomProperties["MonkeCosmetics::Material"];

                if (matName == null)
                {
                    if (CustomCosmeticManager.instance.currentMaterial == null) continue;
                    if (!Plugin.Instance.materialSet.Value) continue;
                    Debug.Log($"[Monke Cosmetics] Setting material for non-monke cosmetics user {p.NickName}");
                    SetVRRigMaterial(CustomCosmeticManager.instance.currentMaterial, e);
                }
                else
                {
                    if (!Plugin.Instance.materialSet.Value) continue;
                    foreach (var mate in CustomCosmeticManager.instance.materials)
                    {
                        if (mate.name == (string)matName)
                        {
                            Debug.Log($"[Monke Cosmetics] Setting material for {p.NickName}");
                            SetVRRigMaterial(mate, e);
                            continue;
                        }
                    }
                }
            }

            base.OnJoinedLobby();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == targetPlayer.ActorNumber)
            {
                return;
            }
            else
            {
                VRRig PlayerModel = GorillaGameManager.instance.FindPlayerVRRig(targetPlayer);
                ResetMaterial(PlayerModel);
                if (PlayerModel != null)
                {

                    if (PlayerModel.IsTagged()) { return; }

                    string matName = (string)changedProps["MonkeCosmetics::Material"];

                    if (string.IsNullOrEmpty(matName))
                    {
                        if (CustomCosmeticManager.instance.currentMaterial == null) return;
                        if (!Plugin.Instance.materialSet.Value) return;
                        SetVRRigMaterial(CustomCosmeticManager.instance.currentMaterial, PlayerModel);
                        Debug.Log($"[Monke Cosmetics] Setting material for non-monke cosmetics user {targetPlayer.NickName}");
                        return;
                    }
                    try
                    {
                        foreach (var mat in CustomCosmeticManager.instance.materials)
                        {
                            if (mat.name == matName)
                            {
                                Debug.Log($"[Monke Cosmetics] Setting material for {targetPlayer.NickName}");
                                SetVRRigMaterial(mat, PlayerModel);
                                return;
                            }
                        }
                    }
                    catch (Exception e) { Debug.Log("[MonkeCosmetics]" + e); }
                }
                else
                {
                    Debug.LogWarning("[MonkeCosmetics] Failed to find player object");
                }
            }
        }

        public void SetVRRigMaterial(Material material, VRRig Rig)
        {
            var CCM = CustomCosmeticManager.instance;
            if (CCM.specialVariables.Any(s => string.Equals(s, CCM.CheckText(material.name), StringComparison.OrdinalIgnoreCase))) { material.color = VRRig.LocalRig.playerColor; }
            
            Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = material;
        }

        public void ResetMaterial(VRRig Rig)
        {
            
            Debug.Log("[Monke Cosmetics] Started to reset material i think");
            if (Rig.isLocal)
            {
                CustomCosmeticManager.instance.currentMaterial = null;

                LocalCosmetics = new Hashtable
                {
                    { "MonkeCosmetics::Material", null }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);

                GameObject.Find("Player Objects").transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[0];
                Debug.Log($"[Monke Cosmetics] Succesfully reset material i think - CustomProps now are {VRRig.LocalRig}");
            }
            else
            {
                Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[0];
                Debug.Log($"[Monke Cosmetics] Reset material for {Rig.OwningNetPlayer.NickName}");
            }   
        }
    }

    public static class Extensions
    {
        public static bool IsTagged(this VRRig rig) // Thanks to HanSolo1000Falcon for providing this
        {
            bool isInfectionTagged = rig.setMatIndex == 2 || rig.setMatIndex == 11;
            bool isRockTagged = rig.setMatIndex == 1;

            return isInfectionTagged || isRockTagged;
        }
    }
}
