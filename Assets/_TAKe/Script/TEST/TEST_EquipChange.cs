using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TEST_EquipChange : MonoBehaviour
{
    [SerializeField] ToggleGroup toggles_eating;
    [SerializeField] Text txEqID, txMstLv, txNorLv, txEnhLv;
    [SerializeField] Scrollbar sbEqID, sbMst, sbNor, sbEnh;

    int toggle_rt = 1;
    int eqID = 1, mstLV = 1, nor = 1, enh = 1;
    int[] eqIDMax = { 0, 1, 2, 3, 4, 4, 3, 2};

    void OnEnable()
    {
        int rt_frh = 0;
        int rt = 0;
        Toggle[] tog = toggles_eating.transform.GetComponentsInChildren<Toggle>();
        foreach (var item in tog)
        {
            item.isOn = rt_frh == rt;
            rt_frh++;
        }
        toggle_rt = 1;
        ToggleRt();
    }

    void Update()
    {
        eqID = (int)(sbEqID.value * eqIDMax [toggle_rt]);
        if (eqID == 0)
            eqID = 1;
        txEqID.text = eqID.ToString();

        mstLV = (int)(sbMst.value * 10);
        if (mstLV == 0)
            mstLV = 1;
        txMstLv.text = mstLV.ToString();

        nor = (int)(sbNor.value * 100);
        if (nor == 0)
            nor = 1;
        txNorLv.text = nor.ToString();

        enh = (int)(sbEnh.value * 35);
        if (enh == 0)
            enh = 1;
        txEnhLv.text = enh.ToString();
    }

    public void ToggleRt()
    {
        toggle_rt = 1;
        foreach (Toggle toggle in toggles_eating.ActiveToggles())
        {
            Text t = toggle.GetComponentInChildren<Text>();
            for (int i = 1; i < 8; i++)
            {
                string srtRt = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", i));
                if (string.Equals(srtRt, t.text))
                {
                    toggle_rt = i;
                    break;
                }
            }
        }
    }

    public void ChangeAsync()
    {
        ToggleRt();
        for (int f_ty = 0; f_ty < 11; f_ty++)
        {
            var wearEqDb = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(f_ty);
            var change_eqDB = GameDatabase.GetInstance ().tableDB.GetNewEquipmentData(wearEqDb.eq_ty, toggle_rt, eqID);
             
            change_eqDB.m_norm_lv = nor;
            change_eqDB.m_ehnt_lv = enh;

            change_eqDB.indate = wearEqDb.indate;
            change_eqDB.aInUid = wearEqDb.aInUid;
            change_eqDB.ma_st_id = wearEqDb.ma_st_id;
            change_eqDB.ma_st_rlv = mstLV;
            change_eqDB.st_op = wearEqDb.st_op;
            change_eqDB.st_ms = wearEqDb.st_ms;
            change_eqDB.m_lck = wearEqDb.m_lck;
            change_eqDB.m_state = wearEqDb.m_state;
            change_eqDB.st_sop_ac = wearEqDb.st_sop_ac;
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(change_eqDB);

            //var eq_ty_finds = equip_all.FindAll(e => e.eq_ty == f_ty && e.eq_rt > 0 && e.m_state == 0);
            //if (eq_ty_finds.Count > 0)
            //{
            //    eq_ty_finds.Sort((GameDatabase.TableDB.Equipment x, GameDatabase.TableDB.Equipment y) => GameDatabase.GetInstance().tableDB.GetEquipCombatPower(y, "total").CompareTo(GameDatabase.GetInstance().tableDB.GetEquipCombatPower(x, "total")));
            //    var wearEqDb = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(f_ty);
            //    int nowWear_combat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(wearEqDb, "total");
            //    var highEqDb = eq_ty_finds[0];
            //    int high_combat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eq_ty_finds[0], "total");
            //    if (nowWear_combat < high_combat)
            //    {
            //        wearEqDb.m_state = 0;
            //        highEqDb.m_state = 1;
            //        if (string.IsNullOrEmpty(highEqDb.indate) == true)
            //        {
            //            highEqDb.indate = wearEqDb.indate;
            //            wearEqDb.indate = string.Empty;
            //        }

            //        if (string.IsNullOrEmpty(highEqDb.indate) == false)
            //        {
            //            BackendReturnObject bro1 = null, bro2 = null;
            //            Param wear_parm = string.IsNullOrEmpty(highEqDb.indate) == false && string.IsNullOrEmpty(wearEqDb.indate) == true ?
            //                    GameDatabase.GetInstance().tableDB.EquipParamCollection(highEqDb) : ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = highEqDb.m_state } });

            //            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, highEqDb.indate, wear_parm, callback => { bro1 = callback; });
            //            if (string.IsNullOrEmpty(wearEqDb.indate) == false)
            //                SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, wearEqDb.indate, ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = 0 } }), callback => { bro2 = callback; });
            //            else bro2 = new BackendReturnObject();

            //            while (Loading.Bottom(bro1, bro2) == false) { await Task.Delay(100); }

            //            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(highEqDb);
            //            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(wearEqDb);
            //            GameDatabase.GetInstance().tableDB.SetTempEquipDbChange(highEqDb, wearEqDb);
            //            isChange = true;
            //        }
            //        else PopUpMng.GetInstance().Open_MessageNotif("장비의 데이터가 오류입니다. 문제가 지속된다면 게임을 재실행 해주시기 바랍니다.");
            //    }
            //}
        }

        //if(findChangeEquip.Count > 0)
        //{
        //    await BackendGpgsMng.AWaitTask();
        //    foreach (var item in findChangeEquip)
        //    {
        //        if (!string.IsNullOrEmpty(item.eq.indate))
        //            GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(item.unused_uid);
        //    }

        //    MainUI.GetInstance().RefreshGameStatViewInfo(-1);
        //}

        GameDatabase.GetInstance().characterDB.SetPlayerStatValue();
        MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(-1);
        MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
        GameMng.GetInstance().myPZ.igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯 
        GameMng.GetInstance().myPZ.SettingParts(-1); // 필드 좀비
        gameObject.SetActive(false);
    }
}
