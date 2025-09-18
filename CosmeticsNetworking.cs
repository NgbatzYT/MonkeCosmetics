using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
namespace MonkeCosmetics
{
    internal class CosmeticsNetworking : MonoBehaviourPunCallbacks
    {
        string otherplayermeshdir = "gorilla_new";

        Hashtable LocalCosmetics;

        public override void OnJoinedLobby()
        {
            var CCM = CustomCosmeticManager.instance;
            LocalCosmetics = new Hashtable
            {
                { "MonkeCosmetics::Material", CCM.materials[CCM.index].name }
            };
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
                return;
            }
            else
            {

                GameObject PlayerModel = FindAvatarByActorNumber(targetPlayer.ActorNumber);
                if (PlayerModel != null)
                {
                    string matName = (string)changedProps["MonkeCosmetics::Material"];

                    if (string.IsNullOrEmpty(matName)) return;
                    try 
                    { 
                        var mat = Plugin.Instance.bundle.LoadAsset<Material>(matName);
                        if (mat != null)
                        {
                            PlayerModel.transform.Find(otherplayermeshdir).GetComponent<MeshRenderer>().material = mat;
                        }
                        else
                        {
                            Debug.Log("[MonkeCosmetics] You don't have the other players material.");
                        }
                    } catch (Exception e) { Debug.Log("[MonkeCosmetics]" + e); }                  
                }
                else
                {
                    Debug.LogWarning("[MonkeCosmetics] Failed to find player object");
                }
            }
        }

    }
}
