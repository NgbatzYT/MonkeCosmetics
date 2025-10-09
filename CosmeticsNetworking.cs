using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MonkeCosmetics
{
    internal class CosmeticsNetworking : MonoBehaviourPunCallbacks
    {
        Hashtable LocalCosmetics;

        public static CosmeticsNetworking Instance;

        void Start() => Instance = this;

        public override void OnJoinedLobby()
        {
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
                NetworkMaterial(p, p.GetPlayerRef().CustomProperties);
            }

            base.OnJoinedLobby();
        }

        public void NetworkMaterial(NetPlayer player, Hashtable changedProps = null)
        {
            VRRig PlayerModel = GorillaGameManager.instance.FindPlayerVRRig(player);
            ResetMaterial(PlayerModel);
            if (PlayerModel != null)
            {
                if (PlayerModel.IsTagged()) { return; }

                string matName = null;

                if (changedProps != null) matName = (string)changedProps["MonkeCosmetics::Material"];

                if (string.IsNullOrEmpty(matName))
                {
                    if (CustomCosmeticManager.instance.currentMaterial == null) return;
                    if (!Plugin.Instance.materialSet.Value) return;
                    SetVRRigMaterial(CustomCosmeticManager.instance.currentMaterial, PlayerModel);
                    Debug.Log($"[Monke Cosmetics] Setting material for non-monke cosmetics user {player.NickName}");
                    return;
                }
                try
                {
                    foreach (var mat in CustomCosmeticManager.materials)
                    {
                        if (mat.name == matName)
                        {
                            Debug.Log($"[Monke Cosmetics] Setting material for {player.NickName}");
                            SetVRRigMaterial(mat, PlayerModel);
                            return;
                        }
                    }
                }
                catch (Exception e) { UnityEngine.Debug.Log("[MonkeCosmetics]" + e); }
            }
            else
            {
                Debug.LogWarning("[MonkeCosmetics] Failed to find player object");
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != targetPlayer.ActorNumber)
            {
                NetworkMaterial(targetPlayer, changedProps);
            }
        }

        public void SetVRRigMaterial(Material material, VRRig Rig)
        {
            var CCM = CustomCosmeticManager.instance;
            if (CCM.specialVariables.Any(s => string.Equals(s, CCM.CheckText(material.name), StringComparison.OrdinalIgnoreCase))) { material.color = new Color(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b, material.color.a); }

            Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = material;
        }

        public void ResetMaterial(VRRig Rig)
        {
            if (Rig.isLocal)
            {
                CustomCosmeticManager.instance.currentMaterial = null;

                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "MonkeCosmetics::Material", null } });

                GameObject.Find("Player Objects").transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[Rig.setMatIndex];
            }
            else
            {
                Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[Rig.setMatIndex];
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