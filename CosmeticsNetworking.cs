using ExitGames.Client.Photon;
using MonkeCosmetics.Editor.Cosmetic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MonkeCosmetics
{
    internal class CosmeticsNetworking : MonoBehaviourPunCallbacks
    {
        public static CosmeticsNetworking Instance;

        void Start() => Instance = this;
        public override void OnJoinedRoom()
        {
            foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers)
            {
                var e = GorillaGameManager.instance.FindPlayerVRRig(p);

                if (e.isLocal) { continue; }
                ResetMaterial(e);
                if (e.IsTagged()) { continue; }

                if (CustomCosmeticManager.instance.currentMaterial == null) continue;
                if (!Plugin.Instance.materialSet.Value) continue;
                Debug.Log($"[Monke Cosmetics] Setting material for non-monke cosmetics user {p.NickName}");
                SetVRRigMaterial(CustomCosmeticManager.instance.currentMaterial, e);
            }

            base.OnJoinedRoom();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != targetPlayer.ActorNumber)
            {
                VRRig PlayerModel = GorillaGameManager.instance.FindPlayerVRRig(targetPlayer);
                ResetMaterial(PlayerModel);
                if (PlayerModel != null)
                {

                    if (PlayerModel.IsTagged()) { return; }

                    if (CustomCosmeticManager.instance.currentMaterial == null) return;
                    if (!Plugin.Instance.materialSet.Value) return;
                    SetVRRigMaterial(CustomCosmeticManager.instance.currentMaterial, PlayerModel);
                    Debug.Log($"[Monke Cosmetics] Setting material for user {targetPlayer.NickName}");
                }
            }
        }

        public void SetVRRigMaterial(MonkeMaterial material, VRRig Rig)
        {
            if (material == null) return;
            if (material.customColours) { material.material.color = new Color(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b, material.material.color.a); }

            Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = material.material;
        }

        public void SetVRRigMaterial(Material material, VRRig Rig)
        {
            if (material == null) return;

            Rig.transform.root.Find("gorilla_new").GetComponent<SkinnedMeshRenderer>().material = material;
        }

        public void ResetMaterial(VRRig Rig)
        {
            if (Rig == null) return;
            Debug.Log("[Monke Cosmetics] Started to reset material");
            if (Rig.isLocal)
            {
                CustomCosmeticManager.instance.currentMaterial = null;

                GameObject.Find("Player Objects").transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[Rig.setMatIndex];
                Debug.Log($"[Monke Cosmetics] Succesfully reset material");
            }
            else
            {
                SetVRRigMaterial(Rig.materialsToChangeTo[Rig.setMatIndex], Rig);
                Debug.Log($"[Monke Cosmetics] Reset material for {Rig.Creator.NickName}");
            }
        }
    }
}