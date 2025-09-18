using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
namespace MonkeCosmetics
{
    internal class CosmeticsNetworking : MonoBehaviourPunCallbacks
    {
        string otherplayermeshdir = "gorilla_new";

        Hashtable LocalCosmetics = new Hashtable();
        public override void OnJoinedLobby()
        {
            var CCM = GameObject.Find("MatSwitcherStand(Clone)").GetComponent<CustomCosmeticManager>();
            LocalCosmetics.Add("MonkeCosmetics::Material", CCM.materials[CCM.index].name);
            PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);
            base.OnJoinedLobby();
        }

        private GameObject FindAvatarByActorNumber(int actorNumber)
        {

            PhotonView[] views = FindObjectsOfType<PhotonView>();
            foreach (var v in views)
            {
                if (v.Owner != null && v.Owner.ActorNumber == actorNumber)
                {
                    return v.gameObject;
                }
            }
            return null;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == targetPlayer.ActorNumber)
            {
                Debug.Log("Same Player Found");
            }
            else
            {

                GameObject PlayerModel = FindAvatarByActorNumber(targetPlayer.ActorNumber);
                if (PlayerModel != null)
                {
                    string matName = (string)changedProps["MonkeCosmetics::Material"];
                    var mat = MonkeCosmetics.Plugin.LoadMat(matName);
                    if (mat != null)
                    {
                        PlayerModel.transform.Find(otherplayermeshdir).gameObject.GetComponent<MeshRenderer>().material = mat;
                    }
                    else
                    {
                        Debug.Log("Did no have other player Material");
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to find player object");
                }
            }
        }

    }
}
