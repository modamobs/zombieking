using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BackEnd;
using LitJson;
using System.Threading.Tasks;

public class PopUpUserInfo : MonoBehaviour
{
    [SerializeField] UI ui;
    [System.Serializable]
    struct UI
    {
        public bool isInfoOn;
        public Animator ani;
        public Text txUserNickName;
        public GameObject goBtnBlock, goBtnUnBlock;
        public GameObject goBtnUserInfo, goBtnReport;

        public WearingEquipmentInfo tapWearEquipInfo;
        public TapCharacter tapCharacterStatInfo;

        public CellSkill[] cellSkill;
    }

    public async void SetData(string nickName, bool isRankOrChat, bool isBlock, string gamerInDate = "") // isBlock true : 차단된 상태 
    {
        ui.isInfoOn = false;
        ui.txUserNickName.text = nickName;
        ui.goBtnBlock.SetActive(!isBlock && !isRankOrChat);
        ui.goBtnUnBlock.SetActive(isBlock && !isRankOrChat);
        ui.goBtnUserInfo.SetActive(!isRankOrChat);
        ui.goBtnReport.SetActive(!isRankOrChat);

        ui.ani.Play("PopUpUserInfo_Default");
        await Task.Delay(100);
        if (isRankOrChat)
        {
            GetViewUserData(gamerInDate);
        }
    }

    private async void GetViewUserData(string gamerInDate)
    {
        if (ui.isInfoOn)
            return;

        BackendReturnObject bro1 = null;
        if (string.IsNullOrEmpty(gamerInDate))
        {
            SendQueue.Enqueue(Backend.Social.GetGamerIndateByNickname, ui.txUserNickName.text, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            if (bro1.IsSuccess())
            {
                JsonData rows = bro1.GetReturnValuetoJSON()["rows"];
                if(rows.Count > 0)
                    gamerInDate = RowPaser.StrPaser(rows[0], "inDate");
            }

            if (string.IsNullOrEmpty(gamerInDate))
            {
                ui.ani.Play("PopUpUserInfo_NoData");
                return;
            }
        }


        bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPublicContentsByGamerIndate, BackendGpgsMng.tableName_Pub_NowCharData, gamerInDate, callback => { bro1 = callback; });
        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            JsonData rows = bro1.GetReturnValuetoJSON()["rows"];
            if (rows == null || rows.Count == 0)
            {
                ui.ani.Play("PopUpUserInfo_NoData");
            }
            else
            {
                JsonData row = rows[0];
                JSONObject j_equip = JSONObject.Create(RowPaser.StrPaser(row, "useEquip")); // 유저 장비 
                if (j_equip["rows"] != null)
                {
                    for (int i = 0; i < j_equip["rows"].Count; i++)
                    {
                        var eq_psr = JsonUtility.FromJson<GameDatabase.PublicContentDB.PubDB_NowChar_Equip>(j_equip["rows"][i].ToString());
                        //LogPrint.EditorPrint("유저 장비 : " + j_equip["rows"][i] + "\n유저 장비 2 : " + JsonUtility.ToJson(eq_psr));
                        ui.tapWearEquipInfo.SetWearEquipView
                        (
                            eq_psr.eq_ty,
                            true,
                            new GameDatabase.TableDB.Equipment()
                            {
                                eq_ty = eq_psr.eq_ty,
                                eq_rt = eq_psr.eq_rt,
                                eq_id = eq_psr.eq_id,
                                eq_legend = eq_psr.eq_legend,
                                eq_legend_sop_id = eq_psr.eq_legend_sop_id,
                                eq_legend_sop_rlv = eq_psr.eq_legend_sop_rlv,
                                ma_st_id = eq_psr.ma_st_id,
                                ma_st_rlv = eq_psr.ma_st_rlv,
                                m_norm_lv = eq_psr.eq_nor_lv,
                                m_ehnt_lv = eq_psr.eq_ehn_lv,
                                st_sop_ac = eq_psr.st_sop_ac,
                                st_op = eq_psr.st_op,
                            }
                        );
                    }
                }

                // 유저 장비 rt, id 
                var parts = JsonUtility.FromJson<IG.PartsIdx>(RowPaser.StrPaser(row, "parts"));

                // 유저 스킬 id, rlv 
                var pubDb_skill = JsonUtility.FromJson<GameDatabase.PublicContentDB.PubDB_ClearChapterChar_UseSkill>(RowPaser.StrPaser(row, "useSkill"));
                //LogPrint.EditorPrint("유저 스킬 : " + JsonUtility.ToJson(pubDb_skill));
                ui.cellSkill[0].SetOthers(pubDb_skill.slot1_id, pubDb_skill.slot1_lv);
                ui.cellSkill[1].SetOthers(pubDb_skill.slot2_id, pubDb_skill.slot2_lv);
                ui.cellSkill[2].SetOthers(pubDb_skill.slot3_id, pubDb_skill.slot3_lv);
                ui.cellSkill[3].SetOthers(pubDb_skill.slot4_id, pubDb_skill.slot4_lv);
                ui.cellSkill[4].SetOthers(pubDb_skill.slot5_id, pubDb_skill.slot5_lv);
                ui.cellSkill[5].SetOthers(pubDb_skill.slot6_id, pubDb_skill.slot6_lv);

                // 유저 캐릭터 스탯 
                GameDatabase.CharacterDB.StatValue stat_value = JsonUtility.FromJson<GameDatabase.CharacterDB.StatValue>(RowPaser.StrPaser(row, "statValue"));
                ui.tapCharacterStatInfo.SetStatView(stat_value);
                ui.ani.Play("PopUpUserInfo_SizeUp");
                ui.isInfoOn = true;
            }
        }
        else
        {
            ui.ani.Play("PopUpUserInfo_NoData");
        }
    }

    public void Click_ViewUserInfo()
    {
        GetViewUserData(string.Empty);
    }

    public void Click_UserBlock()
    {
        if (!string.IsNullOrEmpty(ui.txUserNickName.text))
            ChatScript.Instance().BlockUser(ui.txUserNickName.text);
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("닉네임이 잘못되었습니다.");

        gameObject.SetActive(false);
    }

    public void Click_UnUserBlock()
    {
        if (!string.IsNullOrEmpty(ui.txUserNickName.text))
            ChatScript.Instance().UnBlockUser(ui.txUserNickName.text);
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("닉네임이 잘못되었습니다.");

        gameObject.SetActive(false);
    }

    public void Click_Report()
    {
        if(ui.txUserNickName.text.Length <= 0)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("유저의 닉네임을 찾을 수 없습니다.");
            return;
        }

        PopUpMng.GetInstance().Open_ChatReport(ui.txUserNickName.text);
    }
}
