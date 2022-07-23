using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSkillAction : MonoBehaviour
{
    public bool isNowSkillAction = false;
    [SerializeField] PlayerZombie pz;
    public IG.PlayerSkill playerSkill;
    public ThrowCurve thc_ThrowBomb, thc_ThrowSmoke;

    public void InitSkill(Dictionary<int, int[]> dic_data) // dic_data : int[] => [0]:스킬번호, [1]:레벨, [2]:등급, [3]:발동포인트, [4]:착용중(0) or 랜덤(1)
    {
        playerSkill = new IG.PlayerSkill() { useSkill = new IG.PlayerSkill.Skill[6] };
        for (int i = 0; i < 6; i++)
        {
            if (dic_data.ContainsKey(i))
            {
                int sk_id = dic_data[i][0]; // 번호 
                int sk_lv = dic_data[i][1]; // 레벨 
                int sk_rt = dic_data[i][2]; // 등급 
                int sk_pt = dic_data[i][3]; // 발동 포인트 
                int sk_ur = dic_data[i][4]; // 착용중 or 랜덤 
                playerSkill.useSkill[i].activeState = IG.SkillActiveState.WAITING;
                playerSkill.useSkill[i].number = (IG.SkillNumber)sk_id;
                playerSkill.useSkill[i].stat.atvBubCount = sk_pt;
                playerSkill.useSkill[i].stat.skLv = sk_lv;
                playerSkill.useSkill[i].chart = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(sk_id);
            }
            else
            {

            }
        }
        
        // BubbleCountDeduction(3, true); // 초기화 
        ReadySkill(true);
        MainUI.GetInstance().tapGameBattleInfo.SkillPointInit(pz.zbType == IG.MonsterType.MINE);
    }

    public int GetNowReadySkillNumber() => pz.igp.playerSkillAction.playerSkill.readyActiveSkillSlot;
    public int GetNowReadySkillLevel() => pz.igp.playerSkillAction.playerSkill.useSkill[GetNowReadySkillNumber()].stat.skLv;
    public int GetFindUseSkillLevel(IG.SkillNumber _ig_sk_nbr)
    {
        for (int i = 0; i < 6; i++)
        {
            if(playerSkill.useSkill[i].number == _ig_sk_nbr)
            {
                return playerSkill.useSkill[i].stat.skLv;
            }
        }

        return -1;
    }

    #region #### 스킬 발동 대기 ####
    public void ReadySkill(bool init = false)
    {
        for (int i = 0; i < playerSkill.useSkill.Length; i++)
        {
            var us = playerSkill.useSkill[i];
            playerSkill.useSkill[i].activeState = playerSkill.readyActiveSkillSlot == i ? IG.SkillActiveState.READY_ACTIVE : IG.SkillActiveState.WAITING;
            if (playerSkill.useSkill[i].activeState == IG.SkillActiveState.READY_ACTIVE)
            {
                if (pz.zbType == IG.MonsterType.MINE)
                    MainUI.GetInstance().tapGameBattleInfo.My_CardReady(i);
                else
                    MainUI.GetInstance().tapGameBattleInfo.Or_CardReady(i);
            }
        }
    }

    public IG.PlayerSkill.Skill GetReadySkill()
    {
        IG.PlayerSkill.Skill s_val = new IG.PlayerSkill.Skill();
        s_val.number = IG.SkillNumber.EMPTY;
        for (int i = 0; i < playerSkill.useSkill.Length; i++)
        {
            if (playerSkill.useSkill[i].activeState == IG.SkillActiveState.READY_ACTIVE)
            {
                s_val = playerSkill.useSkill[i];
                break;
            }
        }

        return s_val;
    }

    // 스킬 발동하려는 스킬의 번호 
    private int GeReadySkillNumber()
    {
        int _atvSkillNum = 1;
        for (int i = 0; i < playerSkill.useSkill.Length; i++)
        {
            if (playerSkill.useSkill[i].activeState == IG.SkillActiveState.READY_ACTIVE)
            {
                _atvSkillNum = (int)playerSkill.useSkill[i].number;
            }
        }

        return _atvSkillNum;
    }

    // 스킬 발동하려는 버블의 갯수 
    public int GetReadySkillBubbleCount()
    {
        int _atvBubCount = 1;
        for (int i = 0; i < playerSkill.useSkill.Length; i++)
        {
            if (playerSkill.useSkill[i].activeState == IG.SkillActiveState.READY_ACTIVE)
            {
                _atvBubCount = playerSkill.useSkill[i].stat.atvBubCount;
            }
        }

        return _atvBubCount;
    }
    #endregion

    #region #### 스킬 발동 포인트 #####
    /// <summary>
    /// 스킬 발동 포인트 증가 
    /// add_type > 0:일반공격으로 획득, 1:스킬로 획득, 2:장신구 옵션으로 획득  
    /// </summary>
    public async void BubbleCountIncrease(int add_type, int inrCnt = 1)
    {
        int last_bCnt = playerSkill.nowBubbleCount;
        playerSkill.nowBubbleCount += inrCnt;
        if (playerSkill.nowBubbleCount >= 3)
            playerSkill.nowBubbleCount = 3;

        MainUI.GetInstance().tapGameBattleInfo.SkillPointUp(pz.zbType == IG.MonsterType.MINE, add_type, playerSkill.readyActiveSkillSlot, last_bCnt, playerSkill.nowBubbleCount);

        // #9.버블 추가 획득(확률)
        if (playerSkill.nowBubbleCount < 3 && pz.igp.statValue.sop9_val > 0)
        {
            if (int.Equals(add_type, 0) && pz.targetPz.GetNowHp() > 0)
            {
                if (GameDatabase.GetInstance().GetRandomPercent() < pz.igp.statValue.sop9_val)
                {
                    await Task.Delay(250);
                    playerSkill.nowBubbleCount++;
                    MainUI.GetInstance().tapGameBattleInfo.SkillPointUp(pz.zbType == IG.MonsterType.MINE, 2, playerSkill.readyActiveSkillSlot, last_bCnt, playerSkill.nowBubbleCount);
                }
            }
        }
    }

    /// <summary>
    /// 스킬 발동 포인트 차감  
    /// </summary>
    public void BubbleCountDeduction(int _dedCnt, bool _req_by_owner) // _req_by_owner true : 자신이 차감 시킴, false : 상대가 차감 시킴 
    {
        if (playerSkill.nowBubbleCount > 0)
        {
            if(pz.zbType != IG.MonsterType.MINE)
                LogPrint.Print("<color=magenta> 111 스킬 포인트 차감 : " + playerSkill.nowBubbleCount + ", isMine : " + pz.zbType == IG.MonsterType.MINE + "</color>");

            int acv_card_slot = playerSkill.readyActiveSkillSlot;
            playerSkill.nowBubbleCount -= _dedCnt;
            if (playerSkill.nowBubbleCount < 0)
                playerSkill.nowBubbleCount = 0;

            if (pz.zbType != IG.MonsterType.MINE)
                LogPrint.Print("<color=magenta> 222 스킬 포인트 차감 : " + playerSkill.nowBubbleCount + ", isMine : " + pz.zbType == IG.MonsterType.MINE + "</color>");

            MainUI.GetInstance().tapGameBattleInfo.SkillActiveBattleInfoUI(pz.zbType == IG.MonsterType.MINE, acv_card_slot, playerSkill.nowBubbleCount, _req_by_owner);
        }
    }
    #endregion

    #region ##### 스킬 발동 액션 #####
    public bool IsCheckActiveSkill ()
    {
        int acv_card_slot = playerSkill.readyActiveSkillSlot;
        if (playerSkill.useSkill[acv_card_slot].number == IG.SkillNumber.EMPTY)
            return false;

        int bucCnt = playerSkill.nowBubbleCount;
        return bucCnt >= GetReadySkillBubbleCount();
    }

    [SerializeField] Material onSkMat;

    public async Task<bool> ActiveSkill()
    {
        if (IsCheckActiveSkill() == true)
        {
            bool is_available = pz.targetPz.igp.activateSkills.FindIndex((IG.SkillNumber sn) => sn == IG.SkillNumber.NUMBER_1) == -1; // 상대가 침묵스킬을 발동이 아닐때 
            if (is_available)
            {
                isNowSkillAction = true;

                while (pz.igp.state != IG.ZombieState.FIGHT)
                    await Task.Delay(100);

                var rdySk = GetReadySkill();
                string skillName = string.Format("SKILL_{0}", (int)rdySk.number);

                if (onSkMat)
                {
                    int zSkNbr = (int)rdySk.number - 1;
                    onSkMat.SetTexture("_MainTex", SpriteAtlasMng.GetInstance ().texSkill[zSkNbr]);
                    pz.tr.goOnSkActive.SetActive(true);
                    pz.anim.Play(skillName); // 스킬 시전자의 스킬 애니를 플레이 시킨다.
                }
                
                BubbleCountDeduction((int)rdySk.stat.atvBubCount, true); // 사용할 스킬의 발동 소모량만큼 모인 포인튼를 차감하고, 카드 발동 애니를 실행 
                SoundManager.GetInstance().PlaySound("battle_skill_act");

                pz.targetPz.PauseAnimationSkill();// 스킬 발동전에 상대방 애니를 멈춘다.
                pz.anim.SetFloat("SkillSpeed", 0); // 스킬 시전자의 애니를 정지 시킨다.

                await Task.Delay(500);

                pz.anim.SetFloat("SkillSpeed", GameMng.GetInstance().GameSpeed);
                //while (pz.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
                while (pz.anim.GetCurrentAnimatorStateInfo(0).IsName(skillName) == true && pz.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
                    await Task.Delay(100);
                
                this.playerSkill.readyActiveSkillSlot++;
                if (this.playerSkill.readyActiveSkillSlot >= 6)
                    this.playerSkill.readyActiveSkillSlot = 0;

                ReadySkill();
                isNowSkillAction = false;
                return true;
            }
        }

        return false;
    }

    private IEnumerator CActionSkill()
    {
        while(pz.igp.state != IG.ZombieState.FIGHT)
            yield return null;
        
        pz.tr.goOnSkActive.SetActive(true);
        var usesk = GetReadySkill();

        string skillName = string.Format("SKILL_{0}", (int)usesk.number);
        pz.anim.Play(skillName); // 스킬 시전자의 스킬 애니를 플레이 시킨다.
        BubbleCountDeduction((int)usesk.stat.atvBubCount, true); // 사용할 스킬의 발동 소모량만큼 모인 포인튼를 차감하고, 카드 발동 애니를 실행 
        SoundManager.GetInstance().PlaySound("battle_skill_act");

        pz.targetPz.PauseAnimationSkill();// 스킬 발동전에 상대방 애니를 멈춘다.
        pz.anim.SetFloat("SkillSpeed", 0); // 스킬 시전자의 애니를 정지 시킨다.

        yield return new WaitForSeconds(1.0f);

        pz.anim.SetFloat("SkillSpeed", GameMng.GetInstance().GameSpeed);
        StartCoroutine("IESkillActionEnd");
        yield return null;
        while (pz.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
            yield return null;

        pz.anim.SetFloat("Speed", GameMng.GetInstance().GameSpeed);
        pz.anim.SetFloat("SkillSpeed", GameMng.GetInstance().GameSpeed);
        this.playerSkill.readyActiveSkillSlot++;
        if (this.playerSkill.readyActiveSkillSlot >= 6)
            this.playerSkill.readyActiveSkillSlot = 0;
                
        ReadySkill();
    }
    IEnumerator IESkillActionEnd()
    {
        while (pz.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.666f)
            yield return null;

        pz.igp.playerSkillAction.isNowSkillAction = false;
    }
    #endregion

    #region ##### 스킬 발동 #####

    /// <summary> 스킬 버프 이펙트 활성 </summary>
    public void SkillBuffEffect()
    {
        int active_slot_num = pz.igp.playerSkillAction.playerSkill.readyActiveSkillSlot;
        var chart_db = pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].chart;
        //if (string.Equals(chart_db.s_bdf_type, "bf"))
        //{
        int active_sk_num = (int)pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].number;
        MainUI.GetInstance().tapGameBattleInfo.ActiveSkillBuffLabel(pz.zbType == IG.MonsterType.MINE, active_sk_num);
        ObjectPool.GetInstance().PopFromPool(string.Format("opOnce_Sk{0}Ef_Buff", active_sk_num), this.transform.position);
    //    Debug.Log(string.Format("opOnce_Sk{0}Ef_Buff", active_sk_num));
        //}
    }

    /// <summary> 스킬 디버프 이펙트 활성 </summary>
    public void SkillDeBuffEffect()
    {
        int active_slot_num = pz.igp.playerSkillAction.playerSkill.readyActiveSkillSlot;
        int active_sk_num = (int)pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].number;
        //var chart_db = pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].chart;
        ObjectPool.GetInstance().PopFromPool(string.Format("opOnce_Sk{0}Ef_Buff", active_sk_num), pz.targetPz.transform.position);
        
    }

    /// <summary> 스킬 활성 </summary>
    public void SkillActivation()
    {
        int active_slot_num = pz.igp.playerSkillAction.playerSkill.readyActiveSkillSlot;
        var chart_db = pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].chart;
        IG.SkillNumber active_sk_nbr = pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].number;
        int active_sk_lv = (int)pz.igp.playerSkillAction.playerSkill.useSkill[active_slot_num].stat.skLv;
        if ((int)active_sk_nbr > 0)
        {
            long hit_sk_dmg = 0;
            if (chart_db.is_atv_hit_dmg == true) // 스킬 발동시 공격 대미지를 주는 스킬일 경우 
            {
                hit_sk_dmg = pz.GetAttackPower((int)active_sk_nbr, active_sk_lv);
                pz.Hit(false, hit_sk_dmg, false);
            }

            pz.igp.etcZbData.attack_count++;
            switch (active_sk_nbr)
            {
                case IG.SkillNumber.NUMBER_1:  StartCoroutine(CSkillActivation_Num1 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_2:  StartCoroutine(CSkillActivation_Num2 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_3:  StartCoroutine(CSkillActivation_Num3 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_4:  StartCoroutine(CSkillActivation_Num4 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_5:  StartCoroutine(CSkillActivation_Num5 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_6:  StartCoroutine(CSkillActivation_Num6 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_7:  StartCoroutine(CSkillActivation_Num7 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_8:  StartCoroutine(CSkillActivation_Num8 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_9:  StartCoroutine(CSkillActivation_Num9 (active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_10: StartCoroutine(CSkillActivation_Num10(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_11: StartCoroutine(CSkillActivation_Num11(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_12: StartCoroutine(CSkillActivation_Num12(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_13: StartCoroutine(CSkillActivation_Num13(active_sk_nbr, chart_db, hit_sk_dmg)); break;
                case IG.SkillNumber.NUMBER_14: StartCoroutine(CSkillActivation_Num14(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_15: StartCoroutine(CSkillActivation_Num15(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_16: StartCoroutine(CSkillActivation_Num16(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_17: StartCoroutine(CSkillActivation_Num17(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_18: StartCoroutine(CSkillActivation_Num18(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_19: StartCoroutine(CSkillActivation_Num19(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_20: StartCoroutine(CSkillActivation_Num20(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_21: StartCoroutine(CSkillActivation_Num21(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_22: StartCoroutine(CSkillActivation_Num22(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_23: StartCoroutine(CSkillActivation_Num23(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_24: StartCoroutine(CSkillActivation_Num24(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_25: StartCoroutine(CSkillActivation_Num25(active_sk_nbr, chart_db)); break;
                case IG.SkillNumber.NUMBER_26: StartCoroutine(CSkillActivation_Num26(active_sk_nbr, chart_db)); break;
            }

            MainUI.GetInstance().tapGameBattleInfo.RefreshBuff();
        }
    }


    /// <summary> 발동중인 스킬 + 버프or디버프- </summary>
    private void AddSkill (IG.SkillNumber _ig_sk_nbr)
    {
        if (pz.igp.playerSkillAction.GetCheckActivateCurrentSkill(_ig_sk_nbr) == -1)
        {
            pz.igp.activateSkills.Add(_ig_sk_nbr);
            MainUI.GetInstance().tapGameBattleInfo.AddBuffDebuff(pz.zbType == IG.MonsterType.MINE == true ? "player" : "enemy", (int)_ig_sk_nbr);
        }
    }

    /// <summary> 발동스킬 삭제 </summary>
    private bool ActiveSkillRemove(IG.SkillNumber _ig_sk_nbr)
    {
        int indx = GetCheckActivateCurrentSkill(_ig_sk_nbr);
        if (indx >= 0)
        {
            pz.igp.activateSkills.Remove(_ig_sk_nbr);
            return true;
        }

        return false;
    }

    /// <summary> 버프or디버프 카운트 연장 (상대가 디버프 스킬을 연장할 수도있고, 나자신의 버프 스킬을 연장할 수 있다) </summary>
    public void ExtensionBuffAndDebuff(bool _is_player, IG.SkillNumber _sn, int _exten_cnt, string _target_player, string _bdf)
    {
        if (_is_player && pz.igp.activateExtended.ContainsKey((int)_sn) == false)
            pz.igp.activateExtended.Add((int)_sn, _exten_cnt);
        else if (!_is_player && pz.targetPz.igp.activateExtended.ContainsKey((int)_sn) == false)
            pz.targetPz.igp.activateExtended.Add((int)_sn, _exten_cnt);

        MainUI.GetInstance().tapGameBattleInfo.ExtensionBuffAndDebuff(IG.SkillNumber.NUMBER_4, _exten_cnt, _target_player, _bdf);
    }

    /// <summary> 제거할 디버프 스킬의 번호 (상대가 나의 스킬을 제거할 스킬 번호를 추ㅡ가한다..) </summary>
    public void AddActiveSkillCancel (IG.SkillNumber _rmv_ig_sk_nbr)
    {
        if(!pz.targetPz.igp.activeSkillCancel.Contains(_rmv_ig_sk_nbr))
            pz.targetPz.igp.activeSkillCancel.Add(_rmv_ig_sk_nbr);
    }

    /// <summary> 캔슬을 하였는지 체크하여 맞다면 캔슬 스킬 리스트 삭제 </summary>
    private bool IsCheckActiveSkillCancel(IG.SkillNumber _ig_sk_nbr)
    {
        if (pz.igp.activeSkillCancel.Contains(_ig_sk_nbr))
        {
            pz.igp.activeSkillCancel.Remove(_ig_sk_nbr);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 스킬이 끝날때까지 기다린 후 발동된 리스트에서 삭제 
    /// </summary>
    IEnumerator WaitingGetSkillActive(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        int atv_cnt = _cdb.atk_atv_cnt;
        int las_my_attackCount = pz.igp.etcZbData.attack_count;
        int las_or_attackCount = pz.targetPz.igp.etcZbData.attack_count;
        bool isCancel = false;
        if (string.Equals(_cdb.s_chk_bdf_atcker, "attacker"))
        {
            yield return null;
            if (pz != null && pz.targetPz != null)
            {
                int anAtkCnt = pz.igp.etcZbData.attack_count;
                while (anAtkCnt < las_my_attackCount + atv_cnt && !isCancel)
                {
                    yield return null;
                    if (pz != null && pz.targetPz != null)
                    {
                        if (pz.GetNowHp() > 0 && pz.targetPz.GetNowHp() > 0)
                        {
                            if (pz.igp.activateExtended.ContainsKey((int)_ig_sk_nbr)) // 시전자 기준으로 발동중인 스킬을 연장시킬 때 
                            {
                                atv_cnt += (int)pz.igp.activateExtended[(int)_ig_sk_nbr];
                                pz.igp.activateExtended.Remove((int)_ig_sk_nbr);
                            }

                            if (pz.igp.activeSkillCancel.Contains(_ig_sk_nbr))  // 발동중인 디버프 스킬을 강제 취소 시킬때 
                                isCancel = true;

                            if (GameMng.GetInstance().mode_type == IG.ModeType.CHANGE_WAIT)
                                isCancel = true;

                            if (pz.igp.state == IG.ZombieState.LOSER || pz.igp.state == IG.ZombieState.WINNER)
                                isCancel = true;

                            anAtkCnt = pz.igp.etcZbData.attack_count;
                        }
                    }
                }
            }
        }
        else if (string.Equals(_cdb.s_chk_bdf_atcker, "defender"))
        {
            yield return null;
            if (pz != null && pz.targetPz != null)
            {
                int dnAtkCnt = pz.targetPz.igp.etcZbData.attack_count;
                while (dnAtkCnt < las_or_attackCount + atv_cnt && !isCancel)
                {
                    yield return null;
                    if (pz != null && pz.targetPz != null)
                    {
                        if (pz.GetNowHp() > 0 && pz.targetPz.GetNowHp() > 0)
                        {
                            if (pz.targetPz.igp.activateExtended.ContainsKey((int)_ig_sk_nbr))  // 방어자 기준으로 발동중인 스킬을 연장시킬 때 
                            {
                                atv_cnt += (int)pz.targetPz.igp.activateExtended[(int)_ig_sk_nbr];
                                pz.targetPz.igp.activateExtended.Remove((int)_ig_sk_nbr);
                            }

                            if (string.Equals(_cdb.s_bdf_type, "df"))
                            {

                            }

                            if (pz.igp.activeSkillCancel.Contains(_ig_sk_nbr)) // 발동중인 디버프 스킬을 강제 취소 시킬때 
                                isCancel = true;

                            if (GameMng.GetInstance().mode_type == IG.ModeType.CHANGE_WAIT)
                                isCancel = true;

                            if (pz.igp.state == IG.ZombieState.LOSER || pz.igp.state == IG.ZombieState.WINNER)
                                isCancel = true;

                            dnAtkCnt = pz.targetPz.igp.etcZbData.attack_count;
                        }
                    }
                }
            }
        }

        yield return null;
    }

    /// <summary> 현재 번호의 스킬이 발동중인가 </summary>
    public int GetCheckActivateCurrentSkill(IG.SkillNumber _sk_nbr)
    {
        return pz.igp.activateSkills.FindIndex((IG.SkillNumber sn) => sn == _sk_nbr);
    }

    #region -#- SKILL - 1 Corutin -#- 방어자 : 스킬 금지 
    IEnumerator CSkillActivation_Num1(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk1Ef_Aura", pz.targetPz.transform.position, default, pz.targetPz.transform);
        
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;
        
        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 2 Corutin -#- 
    IEnumerator CSkillActivation_Num2(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        pz.go_HandThrowBomb.SetActive(false);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk2Ef_Go", pz.go_HandThrowBomb.transform.position);
        thc_ThrowBomb = ol.GetComponent<ThrowCurve>();
        thc_ThrowBomb.SetStart(pz.targetPz.tr.throw_end_point, pz.targetPz.tr.throw_curve_point, 3);
        StartCoroutine(CSkillChecking_Num2(_cdb.atk_atv_cnt));

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        if (!IsCheckActiveSkillCancel(_ig_sk_nbr) && pz.GetNowHp() > 0 && pz.targetPz.GetNowHp() > 0)
        {
            pz.targetPz.PauseAnimation();
            yield return new WaitForSeconds(0.25f);

            int sk2_lv = pz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_2);
            long bomb_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility2_BombDamage(sk2_lv, pz.GetAttackPower());
            pz.targetPz.EtcTakeDamage(bomb_dmg, ResourceDatabase.GetInstance().hitColor_Bomb, "opOnce_Sk2Ef_Hit", "arena_hit", pz.targetPz.tr.throw_end_point.position);
            pz.targetPz.ResumAnimation();
        }

        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }

    IEnumerator CSkillChecking_Num2(int _atk_atv_cnt)
    {
        int atv_cnt = _atk_atv_cnt;
        int nwMy_atk_cnt = pz.igp.etcZbData.attack_count;
        while (thc_ThrowBomb)
        {
            yield return null;
            if (atv_cnt <= 0 || pz.GetNowHp() <= 0)  // if (GetisActiveSkill(2) == -1 || pz.GetHp() <= 0)
            {
                thc_ThrowBomb = null;
            }
            else if (nwMy_atk_cnt != pz.igp.etcZbData.attack_count)
            {
                nwMy_atk_cnt = pz.igp.etcZbData.attack_count;
                atv_cnt--;
                thc_ThrowBomb.SetCount(atv_cnt);
            }
        }
    }
    #endregion

    #region -#- SKILL - 3 Corutin -#- 시전자 : 보호막 생성 + 상대로 부터 기절, 경직 100% 저항 
    IEnumerator CSkillActivation_Num3(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        //ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk3Ef_Aura", pz.transform.position, default, pz.etcGameObject.hip_j);

        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk3Ef_Aura", this.transform.position);

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 4 Corutin -#- 방어자 : 확률로 상대 기절 시킴 
    IEnumerator CSkillActivation_Num4(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        yield return null;
        // 스킬 발동 확률이 true , 상대방이 기절 저항중이 아닐때  
        if (GameDatabase.GetInstance ().chartDB.GetSkillAbility4_Stunned(pz.igp.playerSkillAction.GetNowReadySkillLevel()) && pz.targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_3) == -1)
        {
            AddSkill(_ig_sk_nbr);
            pz.targetPz.anim.Play("Stunned");
            ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk4Ef_Aura", pz.targetPz.etcGameObject.head.position, default, pz.targetPz.etcGameObject.head);

            yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
            yield return null;

            ol.VoLife();
            pz.targetPz.anim.Play("Idle");
        }
        
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 5 Corutin -#- 시전자 : 경직 100% 저항하고, 받은 피해를 반사 
    IEnumerator CSkillActivation_Num5(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);

        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk5Ef_Aura", this.transform.position);

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;


        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 6 Corutin -#- 방어자 : 출혈 
  /*  IEnumerator CSkillActivation_Num6(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        if(pz.targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_6) == -1) // 방어자가 출혈에 저항 스킬 발동 상태가 아닐 때
        {
            AddSkill(_ig_sk_nbr);
            ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk6Ef_Aura", pz.targetPz.etcGameObject.bottom.position, default);

            yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
            yield return null;

            ol.VoLife();
            ActiveSkillRemove(_ig_sk_nbr);
        }
    }*/
    IEnumerator CSkillActivation_Num6(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        if (pz.targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_6) == -1) // 방어자가 출혈에 저항 스킬 발동 상태가 아닐 때
        {
            AddSkill(_ig_sk_nbr);
            ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk6Ef_Aura", pz.targetPz.etcGameObject.bottom.position, default);


            yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
            yield return null;

            ol.VoLife();
            ActiveSkillRemove(_ig_sk_nbr);
        }
    }

    #endregion

    #region -#- SKILL - 7 Corutin -#- 방어자 : 공격전 경직 상태에 걸림 
    IEnumerator CSkillActivation_Num7(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk7Ef_Aura", pz.targetPz.etcGameObject.bottom.position, default);

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 8 Corutin -#- 시전자 : 현재 남은 체력 기준으로 일정량 체력 회복 
    IEnumerator CSkillActivation_Num8(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opOnce_Sk8Ef_Aura", this.transform.position);
        int sk8_lv = pz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_8);
        long recv_hp = GameDatabase.GetInstance().chartDB.GetSkillAbility8_RecoveryHealth(sk8_lv, pz.GetNowHp());
        pz.RecoveryHealth(recv_hp);
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 9 Corutin -#- 시전자 : 공격력 증가 
    IEnumerator CSkillActivation_Num9(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk9Ef_Aura", this.transform.position);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 10 Corutin -#- 시전자 : 명중률 증가 
    IEnumerator CSkillActivation_Num10(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife oblf = ObjectPool.GetInstance().PopFromPool("opLoop_Sk10Ef_Aura", this.transform.position);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        if(oblf != null)
        {
            oblf.VoLife();
        }

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 11 Corutin -#- 시전자 : 공격 속도 증가 
    IEnumerator CSkillActivation_Num11(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk11Ef_Aura", this.transform.position);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 12 Corutin -#- 시전자 : 상대방이 기절상태일 경우 추가 대미지 
    IEnumerator CSkillActivation_Num12(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        bool is_stuned = GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_4) >= 0;
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        yield return new WaitForSeconds(0.2f);
        if (is_stuned && pz.GetNowHp() > 0 && pz.targetPz.GetNowHp() > 0) // this 기절 스킬이 발동되어 있는지  
        {
            pz.targetPz.PauseAnimation();
            yield return new WaitForSeconds(0.2f);

            int sk12_lv = pz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_12);
            long val_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility12_StunnedBonusDamage(sk12_lv, pz.igp.statValue.p0_wea_attackPower);
            //pz.targetPz.EtcTakeDamage(val_dmg, ResourceDatabase.GetInstance().hitColor_Bonus, "opOnce_Sk12Ef_Hit", "arena_hit", pz.targetPz.etcGameObject.hip_j.position);
            pz.targetPz.EtcTakeDamage(val_dmg, ResourceDatabase.GetInstance().hitColor_Bonus, "opOnce_Sk12Ef_Hit", "arena_hit", pz.targetPz.etcGameObject.bottom.position);
            
            pz.targetPz.ResumAnimation();
        }

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 13 Corutin -#- 시전자 : 상대에게 타격한 대미지의 일정량을 체력으로 회복 
    IEnumerator CSkillActivation_Num13(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb, long _hit_dmg)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opOnce_Sk13Ef_Aura", this.transform.position);
        int sk13_lv = pz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_13);
        long recv_hp = GameDatabase.GetInstance().chartDB.GetSkillAbility13_RecoveryHealth(sk13_lv, _hit_dmg);
        pz.RecoveryHealth(recv_hp);
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 14, 15, 16 Corutin -#- 방어자 : 14방어, 15공격, 16명중 감소 
    IEnumerator CSkillActivation_Num14(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    IEnumerator CSkillActivation_Num15(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    IEnumerator CSkillActivation_Num16(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 17 Corutin -#- 시전자 : 치명타 확률 100% 치명타대미지 증가 
    IEnumerator CSkillActivation_Num17(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk17Ef_Aura", pz.targetPz.etcGameObject.bottom.position, default);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

       // ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 18 Corutin -#- 시전자 : 공격시 최대체력의 % 체력 회복 
    IEnumerator CSkillActivation_Num18(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk18Ef_Aura", this.transform.position);
        //ObjectLife ol1 = ObjectPool.GetInstance().PopFromPool("opOnce_Sk18Ef_Aura", this.transform.position);//추가 했는데 맞는지 모르겠음

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ol.VoLife();

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 19 Corutin -#- 방어자 : 치명타 확률 50%감소, 치명타 대미지 감소 
    IEnumerator CSkillActivation_Num19(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 20 Corutin -#- 방어자 : 공격 속도 감소 
    IEnumerator CSkillActivation_Num20(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk20Ef_Aura", pz.targetPz.etcGameObject.bottom.position, default);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ol.VoLife();
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 21 Corutin -#- 시전자 : 상대방의 스킬 포인트를 모두 빼앗아 온다 
    IEnumerator CSkillActivation_Num21(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        if(pz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_6) >= 0) // 스킬 6번 : 상대에게 출혈 발동 중일 경우
        {
            int orSkAtvCnt = pz.targetPz.igp.playerSkillAction.playerSkill.nowBubbleCount;
            pz.targetPz.igp.playerSkillAction.BubbleCountDeduction(orSkAtvCnt, false); // 상대 스킬 발동 포인트 리셋 
            pz.igp.playerSkillAction.BubbleCountIncrease(1, orSkAtvCnt); // 상대의 스킬 발동 포인트를 내것으로 
        }
        
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 22 Corutin -#- 시전자 : 자신에게 걸린 모든 효과를 제거, 이전에 상대로 부터 받은 대미지의 ?%를 나의 체력으로 회복 
    IEnumerator CSkillActivation_Num22(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        // 상대가 디버프 스킬을 사용중인것들을 모두 제거 (기절, 경직, 출혈, 스탯 감소 등등....)
        List<IG.SkillNumber> _rmv_ig_sk_nbrs = MainUI.GetInstance().tapGameBattleInfo.RemoveDebuffAndReturnList(pz.zbType == IG.MonsterType.MINE);
        foreach (var item in _rmv_ig_sk_nbrs)
        {
            AddActiveSkillCancel(item);
        }

        // 마지막으로 받은 대미지를 일정 퍼센트로 나의 체력회복 
        ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opOnce_Sk22Ef_Aura", this.transform.position);
        int sk22_lv = pz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_22);
        int recv_hp = GameDatabase.GetInstance().chartDB.GetSkillAbility22_GetLastDamageHpUp(sk22_lv, pz.igp.etcZbData.last_taken_damage);
        pz.RecoveryHealth(recv_hp);
        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 23 Corutin -#- 시전자 : 자신의 체력이 30% 이하일때 자신의 방어, 회피, 치명타 방어력이 증가 
    IEnumerator CSkillActivation_Num23(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 24 Corutin -#- 시전자 : 상대가 기절 상태일 경우 3회 더 연장시킵니다.
    IEnumerator CSkillActivation_Num24(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        if (pz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_4) >= 0) // 스킬 4번 : 상대를 기절 발동되어있을 때 
        {
            int sk24_lv = pz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_24);
            string target_playter = pz.zbType == IG.MonsterType.MINE ? "enemy" : "player";
            ExtensionBuffAndDebuff(pz.zbType == IG.MonsterType.MINE, IG.SkillNumber.NUMBER_4, GameDatabase.GetInstance().chartDB.GetSkillAbility24_StunnedExtension(), target_playter, "df");
        }

        AddSkill(_ig_sk_nbr);

        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 25 Corutin -#- 시전자 : 출혈에 100% 저항합니다.
    IEnumerator CSkillActivation_Num25(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion

    #region -#- SKILL - 26 Corutin -#- 방어자 : 스킬 능력 무력화, 스킬 대미지 피해 감소 
    IEnumerator CSkillActivation_Num26(IG.SkillNumber _ig_sk_nbr, cdb_stat_skill _cdb)
    {
        AddSkill(_ig_sk_nbr);
        yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));
        yield return null;

        ActiveSkillRemove(_ig_sk_nbr);


        //AddSkill(_ig_sk_nbr);
        //ObjectLife oblf = ObjectPool.GetInstance().PopFromPool("opLoop_Sk26Ef_Go", pz.go_HandThrowSmoke.transform.position);
        //pz.go_HandThrowSmoke.SetActive(false);
        //thc_ThrowSmoke = oblf.GetComponent<ThrowCurve>();
        //thc_ThrowSmoke.SetStart(pz.targetPz.tr.throw_end_point, pz.targetPz.tr.throw_curve_point, 3);
        //ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opLoop_Sk26Ef_Aura", pz.targetPz.tr.throw_curve_point.position);
        //yield return StartCoroutine(WaitingGetSkillActive(_ig_sk_nbr, _cdb));

        //thc_ThrowSmoke = null;
        //oblf.VoLife();
        //ol.VoLife();
        //ActiveSkillRemove(_ig_sk_nbr);
    }
    #endregion


    #endregion
}
