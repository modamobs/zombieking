using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSweep : MonoBehaviour
{
    [SerializeField] IG.ModeType mdty;
    [SerializeField] int inNbr;
    float clrSec;

    [SerializeField] Text txTitle, txBtnSweepCount, txSweepCount;
    [SerializeField] Image imBtnStartBg, imBtnMinusBg, imBtnPlusBg;

    Sprite spBtnGray, spBtnBlue, spBtnPurple;

    void Awake()
    {
        spBtnGray = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");
        spBtnBlue = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue");
        spBtnPurple = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple");
    }

    public void SetData(IG.ModeType _mdTy, int _inNbr, float _clrSec, int _tikCnt)
    {
        mdty = _mdTy;
        inNbr = _inNbr;
        clrSec = _clrSec;

        swp_min = _tikCnt > 0 ? 1 : 0;
        swp_max = _tikCnt > max ? max : _tikCnt;
        swp_cnt = swp_min > 0 ? 1 : 0;

        MinMaxButton();
    }

    bool isPressOn = false, isPlus = false;
    int swp_min, swp_max, swp_cnt = 0, max = 50;
    public void UpPress_Minus() => isPressOn = false;
    public void DwPress_Minus()
    {
        if (swp_cnt > swp_min)
        {
            isPressOn = true;
            isPlus = false;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }

    public void UpPress_Plus() => isPressOn = false;
    public void DwPress_Plus()
    {
        if (swp_cnt >= swp_min && swp_cnt < swp_max)
        {
            isPressOn = true;
            isPlus = true;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }

    IEnumerator Routin_OnPress()
    {
        float press_time = 0.1f;
        yield return null;
        while (isPressOn)
        {
            int bCnt = swp_cnt;
            if (isPlus)
            {
                if (swp_cnt < swp_max)
                {
                    swp_cnt++;
                    MinMaxButton();
                }
            }
            else
            {
                if (swp_cnt > swp_min)
                {
                    swp_cnt--;
                    MinMaxButton();
                }
            }

            if (bCnt != swp_cnt)
            {
                MinMaxButton();
                yield return new WaitForSeconds(press_time);
                if (press_time > 0.05f)
                    press_time -= 0.2f;
                else if (press_time > 0.025f)
                    press_time -= 0.05f;
                else press_time = 0.025f;
            }
            else break;
        }
    }

    void MinMaxButton()
    {
        imBtnStartBg.sprite = swp_min > 0 && swp_cnt > 0 ? spBtnBlue: spBtnGray;
        imBtnMinusBg.sprite = swp_cnt > swp_min ? spBtnPurple : spBtnGray;
        imBtnPlusBg.sprite = swp_cnt < swp_max ? spBtnBlue : spBtnGray;

        txSweepCount.text = string.Format("{0}/{1}", swp_cnt, swp_max);
        txBtnSweepCount.text = string.Format("{0}회 일괄 소탕", swp_cnt);
    }

    public void Click_Start()
    {
        if(swp_min > 0 && swp_cnt > 0 && swp_max >= 1)
        {
            if (mdty == IG.ModeType.DUNGEON_TOP)
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr9, swp_cnt); // 일일미션, nbr9 던전 : 탑 진행하기! 
            else if (mdty == IG.ModeType.DUNGEON_MINE)
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr10, swp_cnt); // 일일미션, nbr10 던전 : 광산 진행하기! 
            else if (mdty == IG.ModeType.DUNGEON_RAID)
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr11, swp_cnt); // 일일미션, nbr11 던전 : 레이드 진행하기! 

            PopUpMng.GetInstance().RmvAllClosePop();
            GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.gameObject.SetActive(true);
            GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.SetData(mdty, inNbr, true, clrSec, true, swp_cnt);
        }
    }
}
