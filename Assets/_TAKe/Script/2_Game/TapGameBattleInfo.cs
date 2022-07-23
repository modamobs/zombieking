using BackEnd.Game;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TapGameBattleInfo : MonoBehaviour
{
    [SerializeField]
    public BattleInfo myBattleInfo;
    [SerializeField]
    public BattleInfo orBattleInfo;

    public ParticleSystemRenderer[] psCardEfts;

    /// <summary>
    /// int[] => [0]:스킬번호, [1]:레벨, [2]:등급, [3]:발동포인트, [4]:착용중(0) or 랜덤(1)
    /// </summary>
    private Dictionary<int, int[]> temp_my_skill_data = new Dictionary<int, int[]>();
    private Dictionary<int, int[]> temp_or_skill_data = new Dictionary<int, int[]>();

    [SerializeField] GameObject myMiniInfo, orMiniInfo;
    [SerializeField] Text myMiniInfo_skName, orMiniInfo_skName;
    [SerializeField] Text myMiniInfo_skCont, orMiniInfo_skCont;

    public bool GetIsRandomSkillUse()
    {
        foreach (var k in temp_my_skill_data.Keys)
        {
            if(temp_my_skill_data[k][4] == 1)
                return true;
        }

        return false;
    }

    int[] GetMyNowMyBattleSkill ()
    {
        return new int[]
        {
            temp_my_skill_data[0][0],
            temp_my_skill_data[1][0],
            temp_my_skill_data[2][0],
            temp_my_skill_data[3][0],
            temp_my_skill_data[4][0],
            temp_my_skill_data[5][0],
        };
    }

    /// <summary>
    /// 나 : 스킬 슬롯 미니 팝업 정보 
    /// </summary>
    public void ClickMySelectMiniInfo(int slot)
    {
        int sk_idx = temp_my_skill_data[slot][0];
        int sk_lv = temp_my_skill_data[slot][1];
        int sk_rt = temp_my_skill_data[slot][2];

        GameDatabase.GetInstance().tableDB.GetUnusedSkillAll();

        //myMiniInfo.SetActive(true);
        //orMiniInfo.SetActive(false);
        //string str_sk_name = LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", sk_idx));
        //myMiniInfo_skName.text = string.Format("{0} Lv.{1}", "(" + sk_idx + ")" + str_sk_name, sk_lv);

        //string str_sk_dsc = LanguageGameData.GetInstance().GetString(string.Format("skill.description_{0}", sk_idx));
        //myMiniInfo_skCont.text = str_sk_dsc;

        if (sk_idx > 0)
            PopUpMng.GetInstance().PopUp_SkinnInfoMenu(new GameDatabase.TableDB.Skill() { indate = "", aInUid = 0, cliend_rating = sk_rt, idx = sk_idx, level = sk_lv, count = 0 }, true, true);
    }

    /// <summary>
    /// 적 : 스킬 슬롯 미니 팝업 정보 
    /// </summary>
    public void ClickOrSelectMiniInfo(int slot)
    {
        int sk_idx = temp_or_skill_data[slot][0];
        int sk_lv = temp_or_skill_data[slot][1];
        int sk_rt = temp_or_skill_data[slot][2];

        //orMiniInfo.SetActive(true);
        //myMiniInfo.SetActive(false);
        //string str_sk_name = LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", sk_idx));
        //orMiniInfo_skName.text = string.Format("{0} Lv.{1}", "(" + sk_idx + ")" + str_sk_name, sk_lv);

        //string str_sk_dsc = LanguageGameData.GetInstance().GetString(string.Format("skill.description_{0}", sk_idx));
        //orMiniInfo_skCont.text = str_sk_dsc;

        if (sk_idx > 0)
            PopUpMng.GetInstance().PopUp_SkinnInfoMenu(new GameDatabase.TableDB.Skill() { indate = "", aInUid = 0, cliend_rating = sk_rt, idx = sk_idx, level = sk_lv, count = 0 }, true, true);
    }

    /// <summary> 스킬 UI 정보 세팅 </summary>
    public void InitSetBattleInfo(bool _isMy, Dictionary<int, int[]> _skills)
    {
        var battleUIInfo = _isMy ? myBattleInfo : orBattleInfo;
        battleUIInfo.goCardRoot.SetActive(_skills.Count > 0);
        if (_isMy)
        {
            temp_my_skill_data.Clear();
            temp_my_skill_data = _skills;
        }
        else
        {
            temp_or_skill_data.Clear();
            temp_or_skill_data = _skills;
        }

        if (battleUIInfo.activeSkilLabel.go_Root.activeSelf)
            battleUIInfo.activeSkilLabel.go_Root.SetActive(false);

        if (battleUIInfo.goCardRoot.activeSelf)
        {
            var skills = _isMy == true ? temp_my_skill_data : temp_or_skill_data;
            for (int i = 0; i < skills.Count; i++)
            {
                int sk_id = skills[i][0]; // 번호 
                int sk_lv = skills[i][1]; // 레벨 
                int sk_rt = skills[i][2]; // 등급 
                int sk_pt = skills[i][3]; // 발동 포인트 
                int sk_rn = skills[i][4]; // 랜덤 지정 여부 (1) 

                Color ratingColor = ResourceDatabase.GetInstance().GetItemColor(sk_rt);
                battleUIInfo.cardsSkillInfo[i].image_SkillIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(sk_id);
                battleUIInfo.cardsSkillInfo[i].image_RatingOL.color = ratingColor;
                battleUIInfo.cardsSkillInfo[i].text_Rating.color = ratingColor;
                battleUIInfo.cardsSkillInfo[i].text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", sk_rt));
                battleUIInfo.cardsSkillInfo[i].text_Level.text = string.Format("LEVEL.{0}", sk_lv);
                battleUIInfo.cardsSkillInfo[i].go_RandomLabel.SetActive(sk_rn == 1);
                for (int c = 0; c < battleUIInfo.cardsSkillInfo[i].anis_Point.Length; c++)
                    battleUIInfo.cardsSkillInfo[i].anis_Point[c].gameObject.SetActive(c < sk_pt);
            }

            for (int i = 0; i < 6; i++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (battleUIInfo.cardsSkillInfo[i].anis_Point[y].gameObject.activeSelf)
                        battleUIInfo.cardsSkillInfo[i].anis_Point[y].Play("CardPoint_Default");

                    battleUIInfo.cardsSkillInfo[i].gos_PointOnEff[y].SetActive(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (battleUIInfo.anis_Point[i].gameObject.activeSelf)
                    battleUIInfo.anis_Point[i].Play("CardPoint_Default");

                battleUIInfo.gos_OnType0_PointEff[i].SetActive(false);
                battleUIInfo.gos_OnType1_PointEff[i].SetActive(false);
                battleUIInfo.gos_OnType2_PointEff[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 스킬 교체 대기중 표시 이미지 (현재 슬롯의 스킬과 변경한 스킬값을 비교해서 틀리면 로딩 띄움 
    /// </summary>
    public void SkillChangeWaiting (int chng_main_slot)
    {
        GameMng.GetInstance().myPZ.changeSkill = true;
        PlayerPrefs.SetInt(PrefsKeys.prefabKey_SkillUseMainSlot, chng_main_slot); // 사용할 매인 스킬 슬롯 채인지 
    }
    
    public Text GetHpText (bool _is_player)
    {
        if (_is_player)
        {
            return myBattleInfo.text_HpValue;
        }
        else
        {
            return orBattleInfo.text_HpValue;
        }
    }

    public Image GetHpAmount (bool _is_player)
    {
        if (_is_player)
        {
            return myBattleInfo.image_HpFillAmount;
        }
        else
        {
            return orBattleInfo.image_HpFillAmount;
        }
    }
    public Image GetHpBgAmount(bool _is_player)
    {
        if (_is_player)
        {
            return myBattleInfo.image_HpBgFillAmount;
        }
        else
        {
            return orBattleInfo.image_HpBgFillAmount;
        }
    }

    #region # ------------- 버프 & 디버프 ------------- #

    /// <summary> 스킬 발동시 버프 라벨 </summary>
    public async void ActiveSkillBuffLabel(bool _isMy, int _skNum)
    {
        var battleInfo = _isMy == true ? myBattleInfo : orBattleInfo;
        int active_slot_num = _isMy == true ? GameMng.GetInstance().myPZ.igp.playerSkillAction.playerSkill.readyActiveSkillSlot : GameMng.GetInstance().orPZ.igp.playerSkillAction.playerSkill.readyActiveSkillSlot;
        IG.SkillNumber active_sk_nbr = _isMy == true ? GameMng.GetInstance().myPZ.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].number : GameMng.GetInstance().orPZ.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].number;

        if (battleInfo.activeSkilLabel.go_Root.activeSelf)
            battleInfo.activeSkilLabel.go_Root.SetActive(false);

        battleInfo.activeSkilLabel.go_Root.SetActive(true);
        battleInfo.activeSkilLabel.ani["OnActiveSkillLabel"].speed = GameMng.GetInstance().GameSpeed;
        battleInfo.activeSkilLabel.txEng.text = LanguageGameData.GetInstance().GetString(string.Format("skill.active.en.label_{0}", (int)active_sk_nbr));
        battleInfo.activeSkilLabel.txInfo.text = LanguageGameData.GetInstance().GetString(string.Format("skill.active.kr.label_{0}", (int)active_sk_nbr));

        await Task.Delay((int)(1500 / GameMng.GetInstance().GameSpeed));
        battleInfo.activeSkilLabel.go_Root.SetActive(false);
    }

    /// <summary> 스킬 발동시 디버프 라벨 </summary>
    public void ActiveSkillDeBuffLabel(bool _isMy, int _skNum)
    {
        //if (_isMy)
        //{
        //    orBattleInfo.activeSkilLabel.go_Root.SetActive(true);
        //}
        //else
        //{
        //    myBattleInfo.activeSkilLabel.go_Root.SetActive(true);
        //}
    }

    /// <summary>
    /// 버프 & 디버프 초기화 
    /// </summary>
    public void  Init_BuffDebuff ()
    {
        for (int i = 0; i < myBattleInfo.buffAndDebuff.Count; i++)
        {
            var temp_my = myBattleInfo.buffAndDebuff[i];
            temp_my.val.i_skill_idx = 0;
            temp_my.val.i_end_atk_atv_cnt = 0;
            temp_my.val.l_set_time = 0;
            temp_my.val.owner = "";
            temp_my.val.bdf_type = "";
            temp_my.val.chk_bdf_atcker = "";
            myBattleInfo.buffAndDebuff[i] = temp_my;
            myBattleInfo.buffAndDebuff[i].go.go_Root.SetActive(false);
        }

        for (int i = 0; i < orBattleInfo.buffAndDebuff.Count; i++)
        {
            var temp_or = orBattleInfo.buffAndDebuff[i];
            temp_or.val.i_skill_idx = 0;
            temp_or.val.i_end_atk_atv_cnt = 0;
            temp_or.val.l_set_time = 0;
            temp_or.val.owner = "";
            orBattleInfo.buffAndDebuff[i] = temp_or;
            orBattleInfo.buffAndDebuff[i].go.go_Root.SetActive(false);
        }
    }

    /// <summary>
    /// 버프 & 디버프 데이터 세팅 
    /// </summary>
    public void AddBuffDebuff (string _owner, int _skIdx)
    {
        if (GameMng.GetInstance().myPZ == null || GameMng.GetInstance().orPZ == null)
            return;

        if(GameMng.GetInstance ().myPZ.GetNowHp() <= 0 || GameMng.GetInstance().orPZ.GetNowHp() <= 0)
            return;

        if (_skIdx > 0)
        {
            var chart = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(_skIdx);
            if (chart.s_chk_bdf_atcker != "null")
            {
                if (string.Equals(_owner, "player"))
                {
                    string bdfAtker = chart.s_chk_bdf_atcker;
                    int nw_atk_cnt = string.Equals(bdfAtker, "attacker") ? GameMng.GetInstance().myPZ.igp.etcZbData.attack_count : GameMng.GetInstance().orPZ.igp.etcZbData.attack_count;
                    if (string.Equals(chart.s_bdf_type, "bf")) // 폰 주인이 버프 스킬을 사용함 
                    {
                        int indx = myBattleInfo.buffAndDebuff.FindIndex(x => x.go.go_Root.activeSelf == false);
                        if (indx >= 0)
                        {
                            BattleInfo.BuffAndDebuff temp = myBattleInfo.buffAndDebuff[indx];
                            temp.val.i_skill_idx = _skIdx;
                            temp.val.owner = "player";
                            temp.val.bdf_type = "bf";
                            temp.val.chk_bdf_atcker = bdfAtker;
                            temp.val.i_end_atk_atv_cnt = nw_atk_cnt + chart.atk_atv_cnt;
                            temp.val.l_set_time = GameDatabase.GetInstance().GetUniqueIDX();
                            myBattleInfo.buffAndDebuff[indx] = temp;
                        }
                    }
                    else if (string.Equals(chart.s_bdf_type, "df")) // 폰 주인이 디버프 스킬을 사용함 
                    {
                        int indx = orBattleInfo.buffAndDebuff.FindIndex(x => x.go.go_Root.activeSelf == false);
                        if (indx >= 0)
                        {
                            BattleInfo.BuffAndDebuff temp = orBattleInfo.buffAndDebuff[indx];
                            temp.val.i_skill_idx = _skIdx;
                            temp.val.owner = "player";
                            temp.val.bdf_type = "df";
                            temp.val.chk_bdf_atcker = bdfAtker;
                            temp.val.i_end_atk_atv_cnt = nw_atk_cnt + chart.atk_atv_cnt;
                            temp.val.l_set_time = GameDatabase.GetInstance().GetUniqueIDX();
                            orBattleInfo.buffAndDebuff[indx] = temp;
                        }
                    }
                }
                else if (string.Equals(_owner, "enemy"))
                {
                    string bdfAtker = chart.s_chk_bdf_atcker;
                    int nw_atk_cnt = string.Equals(bdfAtker, "attacker") ? GameMng.GetInstance().orPZ.igp.etcZbData.attack_count : GameMng.GetInstance().myPZ.igp.etcZbData.attack_count;
                    if (string.Equals(chart.s_bdf_type, "bf")) // AI 버프 스킬을 사용함 
                    {
                        int indx = orBattleInfo.buffAndDebuff.FindIndex(x => x.go.go_Root.activeSelf == false);
                        if (indx >= 0)
                        {
                            BattleInfo.BuffAndDebuff temp = orBattleInfo.buffAndDebuff[indx];
                            temp.val.i_skill_idx = _skIdx;
                            temp.val.owner = "enemy";
                            temp.val.bdf_type = "bf";
                            temp.val.chk_bdf_atcker = bdfAtker;
                            temp.val.i_end_atk_atv_cnt = nw_atk_cnt + chart.atk_atv_cnt;
                            temp.val.l_set_time = GameDatabase.GetInstance().GetUniqueIDX();
                            orBattleInfo.buffAndDebuff[indx] = temp;
                        }
                    }
                    else if (string.Equals(chart.s_bdf_type, "df")) // AI 디버프 스킬을 사용함 
                    {
                        int indx = myBattleInfo.buffAndDebuff.FindIndex(x => x.go.go_Root.activeSelf == false);
                        if (indx >= 0)
                        {
                            BattleInfo.BuffAndDebuff temp = myBattleInfo.buffAndDebuff[indx];
                            temp.val.i_skill_idx = _skIdx;
                            temp.val.owner = "enemy";
                            temp.val.bdf_type = "df";
                            temp.val.chk_bdf_atcker = bdfAtker;
                            temp.val.i_end_atk_atv_cnt = nw_atk_cnt + chart.atk_atv_cnt;
                            temp.val.l_set_time = GameDatabase.GetInstance().GetUniqueIDX();
                            myBattleInfo.buffAndDebuff[indx] = temp;
                        }
                    }
                }
            }
        }

        RefreshBuff();
    }

    /// 
    /// <summary> 디버프 제거 </summary>
    /// 
    public List<IG.SkillNumber> RemoveDebuffAndReturnList (bool _is_player) //, List<int> _remv_sk_nums)
    {
        List<IG.SkillNumber> df_list = new List<IG.SkillNumber>();
        if(_is_player)
        {
            for (int i = 0; i < myBattleInfo.buffAndDebuff.Count; i++)
            {
                var temp = myBattleInfo.buffAndDebuff[i];
                if(temp.val.bdf_type == "df")
                {
                    df_list.Add((IG.SkillNumber)temp.val.i_skill_idx);
                    temp.val.i_end_atk_atv_cnt = 0;
                    myBattleInfo.buffAndDebuff[i] = temp;
                }
            }

            RefreshBuff();
        }
        else
        {
            for (int i = 0; i < orBattleInfo.buffAndDebuff.Count; i++)
            {
                var temp = orBattleInfo.buffAndDebuff[i];
                if (temp.val.bdf_type == "df")
                {
                    df_list.Add((IG.SkillNumber)temp.val.i_skill_idx);
                    temp.val.i_end_atk_atv_cnt = 0;
                    orBattleInfo.buffAndDebuff[i] = temp;
                }
            }

            RefreshBuff();
        }

        return df_list;
    }

    /// <summary> 버프 or 디버프 연장 </summary>
    public void ExtensionBuffAndDebuff (IG.SkillNumber _sn, int _exten_cnt, string target_player, string _bf_or_df)
    {
        if (GameMng.GetInstance().myPZ.GetNowHp() <= 0 || GameMng.GetInstance().myPZ.GetNowHp() <= 0)
            return;

        if (string.Equals(target_player, "player")) // 플레이어 유저가 
        {
            int indx = myBattleInfo.buffAndDebuff.FindIndex((BattleInfo.BuffAndDebuff v) => string.Equals(v.val.bdf_type, _bf_or_df) && v.val.i_skill_idx == (int)_sn);
            if (indx >= 0)
            {
                myBattleInfo.buffAndDebuff[indx].val.i_end_atk_atv_cnt += _exten_cnt;
            }
        }
        else if (string.Equals(target_player, "enemy"))
        {
            int indx = orBattleInfo.buffAndDebuff.FindIndex((BattleInfo.BuffAndDebuff v) => string.Equals(v.val.bdf_type, _bf_or_df) && v.val.i_skill_idx == (int)_sn);
            if (indx >= 0)
            {
                orBattleInfo.buffAndDebuff[indx].val.i_end_atk_atv_cnt += _exten_cnt;
            }
        }

        RefreshBuff();
    }

    /// <summary> 버프 & 디버프 아이콘 세팅 </summary>
    public void RefreshBuff()
    {
        if (GameMng.GetInstance().myPZ == null || GameMng.GetInstance().orPZ == null)
            return;

        if (GameMng.GetInstance().myPZ.GetNowHp() <= 0 || GameMng.GetInstance().orPZ.GetNowHp() <= 0)
            return;

        // 나 
        myBattleInfo.buffAndDebuff.Sort((x, y) => x.val.l_set_time < y.val.l_set_time && x.val.l_set_time > 0 ? -1 : 1);
        for (int i = 0; i < myBattleInfo.buffAndDebuff.Count; i++)
        {
            var temp = myBattleInfo.buffAndDebuff[i];
            if (temp.val.bdf_type == "bf")
            {
                int nw_atk_cnt = string.Equals(temp.val.chk_bdf_atcker, "attacker") ? GameMng.GetInstance().myPZ.igp.etcZbData.attack_count : GameMng.GetInstance().orPZ.igp.etcZbData.attack_count;
                temp.go.go_Root.SetActive(temp.val.i_end_atk_atv_cnt > nw_atk_cnt && temp.val.i_end_atk_atv_cnt > 0);
                if(temp.go.go_Root.activeSelf)
                {
                    temp.go.image_sk_icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(temp.val.i_skill_idx);
                    temp.go.image_BufDbuf_OL.color = string.Equals(temp.val.bdf_type, "bf") ? Color.black : Color.red;
                    temp.go.image_BdfAttacker.color = string.Equals(temp.val.chk_bdf_atcker, "attacker") ? Color.yellow : Color.red;

                    temp.go.text_atk_cnt.text = (temp.val.i_end_atk_atv_cnt - nw_atk_cnt).ToString();
                    temp.go.text_atk_cnt_OL.text = temp.go.text_atk_cnt.text.ToString();
                    temp.go.go_Root.SetActive(true);
                }
                else myBattleInfo.buffAndDebuff[i].val = new BattleInfo.BuffAndDebuff.Value();
            }
            else if(temp.val.bdf_type == "df")
            {
                int nw_atk_cnt = string.Equals(temp.val.chk_bdf_atcker, "attacker") ? GameMng.GetInstance().orPZ.igp.etcZbData.attack_count : GameMng.GetInstance().myPZ.igp.etcZbData.attack_count;
                temp.go.go_Root.SetActive(temp.val.i_end_atk_atv_cnt > nw_atk_cnt && temp.val.i_end_atk_atv_cnt > 0);
                if (temp.go.go_Root.activeSelf)
                {
                    temp.go.image_sk_icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(temp.val.i_skill_idx);
                    temp.go.image_BufDbuf_OL.color = string.Equals(temp.val.bdf_type, "bf") ? Color.black : Color.red;
                    temp.go.image_BdfAttacker.color = string.Equals(temp.val.chk_bdf_atcker, "defender") ? Color.yellow : Color.red;

                    temp.go.text_atk_cnt.text = (temp.val.i_end_atk_atv_cnt - nw_atk_cnt).ToString();
                    temp.go.text_atk_cnt_OL.text = temp.go.text_atk_cnt.text.ToString();
                    temp.go.go_Root.SetActive(true);
                }
                else myBattleInfo.buffAndDebuff[i].val = new BattleInfo.BuffAndDebuff.Value();
            }
        }

        // AI 
        orBattleInfo.buffAndDebuff.Sort((x, y) => x.val.l_set_time < y.val.l_set_time && x.val.l_set_time > 0 ? -1 : 1);
        for (int i = 0; i < orBattleInfo.buffAndDebuff.Count; i++)
        {
            var temp = orBattleInfo.buffAndDebuff[i];
            if (temp.val.bdf_type == "bf")
            {
                int nw_atk_cnt = string.Equals(temp.val.chk_bdf_atcker, "attacker") ? GameMng.GetInstance().orPZ.igp.etcZbData.attack_count : GameMng.GetInstance().myPZ.igp.etcZbData.attack_count;
                temp.go.go_Root.SetActive(temp.val.i_end_atk_atv_cnt > nw_atk_cnt && temp.val.i_end_atk_atv_cnt > 0);
                if (temp.go.go_Root.activeSelf)
                {
                    temp.go.image_sk_icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(temp.val.i_skill_idx);
                    temp.go.image_BufDbuf_OL.color = string.Equals(temp.val.bdf_type, "bf") ? Color.black : Color.red;
                    temp.go.image_BdfAttacker.color = string.Equals(temp.val.chk_bdf_atcker, "attacker") ? Color.yellow : Color.red;

                    temp.go.text_atk_cnt.text = (temp.val.i_end_atk_atv_cnt - nw_atk_cnt).ToString();
                    temp.go.text_atk_cnt_OL.text = temp.go.text_atk_cnt.text.ToString();
                    temp.go.go_Root.SetActive(true);
                }
                else orBattleInfo.buffAndDebuff[i].val = new BattleInfo.BuffAndDebuff.Value();
            }
            else if (temp.val.bdf_type == "df")
            {
                int nw_atk_cnt = string.Equals(temp.val.chk_bdf_atcker, "attacker") ? GameMng.GetInstance().myPZ.igp.etcZbData.attack_count : GameMng.GetInstance().orPZ.igp.etcZbData.attack_count;
                temp.go.go_Root.SetActive(temp.val.i_end_atk_atv_cnt > nw_atk_cnt && temp.val.i_end_atk_atv_cnt > 0);
                if (temp.go.go_Root.activeSelf)
                {
                    temp.go.image_sk_icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(temp.val.i_skill_idx);
                    temp.go.image_BufDbuf_OL.color = string.Equals(temp.val.bdf_type, "bf") ? Color.black : Color.red;
                    temp.go.image_BdfAttacker.color = string.Equals(temp.val.chk_bdf_atcker, "defender") ? Color.yellow : Color.red;

                    temp.go.text_atk_cnt.text = (temp.val.i_end_atk_atv_cnt - nw_atk_cnt).ToString();
                    temp.go.text_atk_cnt_OL.text = temp.go.text_atk_cnt.text.ToString();
                    temp.go.go_Root.SetActive(true);
                }
                else orBattleInfo.buffAndDebuff[i].val = new BattleInfo.BuffAndDebuff.Value();
            }
        }
    }
    #endregion

    /// <summary> 
    /// 스킬 포인트 증가 
    /// add_type > 0:일반공격으로 획득, 1:스킬로 획득, 2:장신구 전용 옵션 발동으로 획득  
    /// </summary>
    public void SkillPointUp(bool _isMine, int add_type, int rdy_card_slot, int last_bCnt, int nwPnt)
    {
        var btlInfo = _isMine == true ? myBattleInfo : orBattleInfo;
        int arrNum = nwPnt -= 1;
        if (arrNum >= 0 && arrNum < btlInfo.anis_Point.Length)
        {
            for (int i = last_bCnt; i <= arrNum; i++)
            {
                btlInfo.anis_Point[i].Play("CardPoint_Acquisition");
                // 스킬 포인트 
                if (add_type == 0) // 일반 공격으로 획득 스킬 포인트 Effect 
                {
                    btlInfo.gos_OnType0_PointEff[i].SetActive(true);
                }
                else if(add_type == 1) // 스킬 발동으로 획득 스킬 포인트 Effect 
                {
                    btlInfo.gos_OnType1_PointEff[i].SetActive(true);
                }
               
                else if (add_type == 2)  // 장신구 전용 옵션 발동으로 획득 스킬 포인트 Effect 
                {
                    btlInfo.gos_OnType2_PointEff[i].SetActive(true);
                }

                // 카드내 스킬 포인트 
                if (btlInfo.goCardRoot.activeSelf)
                {
                    btlInfo.cardsSkillInfo[rdy_card_slot].anis_Point[i].Play("CardPoint_Acquisition");
                    if (MainUI.GetInstance().isOnSkillEftObj() == true)
                    {
                        btlInfo.cardsSkillInfo[rdy_card_slot].gos_PointOnEff[i].SetActive(true);
                    }
                }
            }
        }
    }

    /// <summary> 스킬 발동시 현재 대기중인 카드 액션 및 스킬 발동 포인트 UI변경 </summary>
    public void SkillActiveBattleInfoUI(bool _isMine, int acv_card_slot, int now_sk_pnt, bool _req_by_owner)
    {
        var btlInfo = _isMine == true ? myBattleInfo : orBattleInfo;

        // 사용한 스킬 포인트를 없앰 (오른쪽 부터)
        for (int i = 0; i < 3; i++)
        {
            if (i >= now_sk_pnt)
                btlInfo.anis_Point[i].Play("CardPoint_Default");
        }

        if (MainUI.GetInstance().isOnSkillEftObj() == true)
        {
            StartCoroutine(DelayActiveEffectOn(btlInfo.cardsSkillInfo[acv_card_slot].anitor, btlInfo.cardsSkillInfo[acv_card_slot].go_ActiveEffect, btlInfo.cardsSkillInfo[acv_card_slot].anis_Point, _req_by_owner));
        }
        else
        {
            for (int i = 0; i < btlInfo.cardsSkillInfo.Length; i++)
            {
                foreach (var item in btlInfo.cardsSkillInfo[i].anis_Point)
                {
                    if (item.gameObject.activeSelf)
                    {
                        if (item.GetCurrentAnimatorStateInfo(0).IsName("CardPoint_Acquisition"))
                        {
                            item.Play("CardPoint_Default");
                            LogPrint.Print("<color=magenta> SkillActive acv_card_slot : " + acv_card_slot + " </color>");
                        }
                    }
                }
            }
        }
    }

    public void SkillPointInit(bool _isMine)
    {
        var btlInfo = _isMine == true ? myBattleInfo : orBattleInfo;
        for (int i = 0; i < 3; i++)
        {
            btlInfo.anis_Point[i].Play("CardPoint_Default");
        }
    }

    #region # ------------- 나 ------------- #

    /// <summary> 스킬 발동 준비 </summary>
    public void My_CardReady(int rdy_card_slot)
    {
        LogPrint.Print("<color=yellow> My_CardReady rdy_card_slot : " + rdy_card_slot + "</color>");
        var btlInfo = myBattleInfo;
        if (btlInfo.goCardRoot.activeSelf)
        {
            for (int i = 0; i < 6; i++)
            {
                var anis_point = btlInfo.cardsSkillInfo[rdy_card_slot].anis_Point;
                if (i == rdy_card_slot)
                {
                    PlayerZombie mypz = GameMng.GetInstance().myPZ;
                    btlInfo.cardsSkillInfo[i].anitor.Play("Card_Ready");
                    int skAtvPntCnt = mypz == null ? 0 : mypz.igp.playerSkillAction.playerSkill.nowBubbleCount;
                    for (int y = 0; y < anis_point.Length; y++)
                    {
                        if (anis_point[y].gameObject.activeSelf)
                        {
                            if (y < skAtvPntCnt)
                                anis_point[y].Play("CardPoint_Acquisition");
                            else
                                anis_point[y].Play("CardPoint_Default");
                        }
                    }
                }
                else
                {
                    btlInfo.cardsSkillInfo[i].anitor.Play("Card_Default");
                    for (int y = 0; y < 3; y++)
                    {
                        if (anis_point[y].gameObject.activeSelf)
                            anis_point[y].Play("CardPoint_Default");
                    }
                }
            }
        }
    }
    
    #endregion

    #region # ------------- 적 ------------- #
    /// <summary> 스킬 발동 준비 </summary>
    public void Or_CardReady(int rdy_card_slot)
    {
        if (GameMng.GetInstance().myPZ == null)
            return;

        var btlInfo = orBattleInfo;
        if (btlInfo.goCardRoot.activeSelf)
        {
            for (int i = 0; i < 6; i++)
            {
                var anis_point = btlInfo.cardsSkillInfo[rdy_card_slot].anis_Point;
                if (i == rdy_card_slot)
                {
                    PlayerZombie mypz = GameMng.GetInstance().myPZ;
                    btlInfo.cardsSkillInfo[i].anitor.Play("Card_Ready");
                    int skAtvPntCnt = mypz == null ? 0 : mypz.igp.playerSkillAction.playerSkill.nowBubbleCount;
                    for (int y = 0; y < anis_point.Length; y++)
                    {
                        if (anis_point[y].gameObject.activeSelf)
                        {
                            if (y < skAtvPntCnt)
                                anis_point[y].Play("CardPoint_Acquisition");
                            else
                                anis_point[y].Play("CardPoint_Default");
                        }
                    }
                }
                else
                {
                    btlInfo.cardsSkillInfo[i].anitor.Play("Card_Default");
                    for (int y = 0; y < 3; y++)
                    {
                        if (anis_point[y].gameObject.activeSelf)
                            anis_point[y].Play("CardPoint_Default");
                    }
                }
            }
        }
    }
    #endregion

    /// <summary> 카드 발동 Shiny 효과 끝나면 카드 가운데 이펙트 생성 및 카드의 포인트 없앰 </summary>
    IEnumerator DelayActiveEffectOn (Animator card_ani,  GameObject go, Animator[] anitor, bool _req_by_owner)
    {
        card_ani.Play("Card_Action");
        yield return null;
        while (card_ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            yield return null;

        if (go != null)
        {
            if (_req_by_owner)
                go.SetActive(true);
        }

        foreach (var item in anitor)
        {
            if (item.gameObject.activeSelf)
                item.Play("CardPoint_Default");
        }
    }
}
