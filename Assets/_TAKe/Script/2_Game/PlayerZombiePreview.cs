using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 탭 
/// </summary>
public class PlayerZombiePreview : MonoBehaviour
{
    string[] partsName = new string[] { /*"weapon_l",*/ "weapon_r", "shield_l", /*"shield_r",*/ "helmet", "shoulder_l", "shoulder_r", "armor", "arm", "pants", "boots" };
    public IG.Parts[] partsArray = new IG.Parts[9];
    [SerializeField] IG.PartsIdx parts = new IG.PartsIdx();

    public void SettingZombieEquipParts(IG.PartsIdx _parts) // -1 : all 
    {
        parts = _parts;

        // 착용 장비 세팅 
        foreach (var parItem in partsArray)
        {
            var parObj = parItem.parArray;
            int partsRat = 0, partsIdx = 0;
            switch (parItem.partsName)
            {
                //case "weapon_l":
                case "weapon_r":    partsRat = _parts.ty0_weapon_rt;        partsIdx = _parts.ty0_weapon_id; break;
                //case "shield_r":
                case "shield_l":    partsRat = _parts.ty1_shield_rt;        partsIdx = _parts.ty1_shield_id; break;
                case "helmet":      partsRat = _parts.ty2_helmet_rt;        partsIdx = _parts.ty2_helmet_id; break;
                case "shoulder_l":  partsRat = _parts.ty3_shoulder_r_rt;    partsIdx = _parts.ty3_shoulder_r_id; break;
                case "shoulder_r":  partsRat = _parts.ty3_shoulder_l_rt;    partsIdx = _parts.ty3_shoulder_l_id; break;
                case "armor":       partsRat = _parts.ty4_armor_rt;         partsIdx = _parts.ty4_armor_id; break;
                case "arm":         partsRat = _parts.ty5_arm_rt;           partsIdx = _parts.ty5_arm_id; break;
                case "pants":       partsRat = _parts.ty6_pants_rt;         partsIdx = _parts.ty6_pants_id; break;
                case "boots":       partsRat = _parts.ty7_boots_rt;         partsIdx = _parts.ty7_boots_id; break;
            }

            for (int i = 0; i < parObj.Count; i++)
            {
                var parObjs = parObj[i].partObj;
                for (int f = 0; f < parObjs.Count; f++)
                {
                    if (parObjs[f].mesh != null)
                    {
                        GameObject _eqGoMesh = parObjs[f].mesh.gameObject;
                        if (_eqGoMesh != null)
                        {
                            bool isActiveSelf = (i == partsRat && parObjs[f].idx == partsIdx);
                            if (isActiveSelf)
                            {
                                if (!_eqGoMesh.activeSelf)
                                    _eqGoMesh.SetActive(true);
                            }
                            else
                            {
                                if (_eqGoMesh.activeSelf)
                                    _eqGoMesh.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
