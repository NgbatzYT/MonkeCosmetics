using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MonkeCosmetics
{
    internal class CosmeticsNetworking : MonoBehaviourPunCallbacks
    {
        Hashtable LocalCosmetics;

        public static CosmeticsNetworking Instance;

        private const string MaterialKey = "MonkeCosmetics::Material";


        void Start() => Instance = this;
        public override void OnJoinedRoom()
        {
            if (Plugin.Instance.network.Value) return;
            SetProp();

            PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);

            foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers)
            {
                var e = GorillaGameManager.instance.FindPlayerVRRig(p);

                if (e.isLocal) { continue; }

                if (e.IsTagged()) { continue; }

                var matName = p.GetPlayerRef().CustomProperties[MaterialKey];

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

                    foreach (var mat in CustomCosmeticManager.materials)
                    {
                        if (mat.name != (string)matName) continue;
                        
                        Debug.Log($"[Monke Cosmetics] Setting material for {p.NickName}");
                        SetVRRigMaterial(mat, e);
                        return;
                    }
                }
            }

            base.OnJoinedRoom();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (Plugin.Instance.network.Value) return;

            if (PhotonNetwork.LocalPlayer.ActorNumber != targetPlayer.ActorNumber)
            {
                VRRig PlayerModel = GorillaGameManager.instance.FindPlayerVRRig(targetPlayer);
                ResetMaterial(PlayerModel);
                if (PlayerModel != null)
                {

                    if (PlayerModel.IsTagged()) { return; }

                    string matName = (string)changedProps[MaterialKey];

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
                        
                        foreach (var mat in CustomCosmeticManager.materials)
                        {
                            if (mat.name != (string)matName) continue;
                        
                            if(mat == null)
                            {
                                Debug.Log($"[Monke Cosmetics] Setting material for {targetPlayer.NickName} failed: you don't have their material installed.");
                            }

                            Debug.Log($"[Monke Cosmetics] Setting material for {targetPlayer.NickName}");
                            SetVRRigMaterial(mat, PlayerModel);
                            return;
                        }
                    }
                    catch (Exception e) { Debug.Log("[MonkeCosmetics] " + e); }
                }
                else
                {
                    Debug.LogWarning("[MonkeCosmetics] Failed to find player object");
                }
            }
        }

        public void SetVRRigMaterial(Material material, VRRig Rig)
        {
            if (Plugin.Instance.network.Value) return;
            if (material == null) return;
            var CCM = CustomCosmeticManager.instance;
            if (CCM.specialVariables.Any(s => string.Equals(s, CCM.CheckText(material.name), StringComparison.OrdinalIgnoreCase))) { material.color = new Color(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b, material.color.a); }

            Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = material;
        }

        private void SetProp()
        {
            if (Plugin.Instance.network.Value) return;
            LocalCosmetics = new Hashtable
            {
                { MaterialKey, CustomCosmeticManager.instance.currentMaterial.name ?? "" }
            };
        }

        public void ResetMaterial(VRRig Rig)
        {
            if(Rig == null) return;
            Debug.Log("[Monke Cosmetics] Started to reset material");
            if (Rig.isLocal)
            {
                CustomCosmeticManager.instance.currentMaterial = null;

                if (!Plugin.Instance.network.Value) SetProp();

                GameObject.Find("Player Objects").transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[Rig.setMatIndex];
                Debug.Log($"[Monke Cosmetics] Succesfully reset material");
            }
            else
            {
                if (Plugin.Instance.network.Value) return;
                SetVRRigMaterial(Rig.materialsToChangeTo[Rig.setMatIndex], Rig);
                Debug.Log($"[Monke Cosmetics] Reset material for {Rig.OwningNetPlayer.NickName}");
            }
        }
    }

    
}