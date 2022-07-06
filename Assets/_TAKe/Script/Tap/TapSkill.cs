using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapSkill : MonoBehaviour
{
    [SerializeField] TapObject tapObject;

    public WearCell wearCell = new WearCell();
    public ListCell listCell = new ListCell();
    public int nowSelectMainSlot = 0;

    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
    }

    public void Init (int useMainSlot)
    {
        nowSelectMainSlot = useMainSlot;
        wearCell.SetWearingData();
        listCell.SetListData(useMainSlot);
    }

    
    /// <summary>
    /// 매인 슬롯을 변경 
    /// </summary>
    public void ClickWearing_SkillMainSlotChange(int number)
    {
        if (nowSelectMainSlot != number)
        {
            nowSelectMainSlot = number;
            wearCell.SetWearingData();
            listCell.SetListData(number);
        }

        MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowSelectMainSlot);
    }
    
    [System.Serializable]
    public struct WearCell
    {
        #region # 착용중인 슬롯 정보 #
        public CellSkill[] useSlotCellSkill;
        public Image[] image_SlotBtnLine;
        public Text[] text_SlotBtn;
        public void SetWearingData()
        {
            WearingSlot();
            WearingChangeButton();
        }

        /// <summary>
        /// 장착한 스킬 슬롯 정보 세팅 
        /// </summary>
        private void WearingSlot()
        {
            int n_mSlotNum = MainUI.GetInstance ().tapSkill.nowSelectMainSlot;
            var data_m_slot = GameDatabase.GetInstance().tableDB.GetSkillSlot(n_mSlotNum);
            for (int i = 0; i < data_m_slot.slot.Length; i++)
            {
                useSlotCellSkill[i].SetWearingIdx(i);
                long uid = data_m_slot.slot[i].aInUid;
                if (uid > 0)
                {
                    var data_skill = GameDatabase.GetInstance().tableDB.GetFindSkill(uid);
                    var cdb_sk_all = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(data_skill.idx);
                    useSlotCellSkill[i].SetData(cdb_sk_all, false);
                }
                else useSlotCellSkill[i].SetData(default, false);
            }
        }

        private void WearingChangeButton()
        {
            int useSlot = MainUI.GetInstance().tapSkill.nowSelectMainSlot;
            for (int i = 0; i < image_SlotBtnLine.Length; i++)
            {
                image_SlotBtnLine[i].color = i == useSlot ? Color.white : Color.black;
                text_SlotBtn[i].color = i == useSlot ? Color.white : Color.gray;
            }
        }
        #endregion
    }

   
    [System.Serializable]
    public struct ListCell
    {
        #region # 보유중인 스킬 리스트 #
        public List<CellSkill> cellsSkill;

        public void SetListData(int useMainSlot)
        {
            var cdb_sk_all = GameDatabase.GetInstance().chartDB.GetChartSkill_DataAll();
            int fi = 0;
            foreach (var item in cdb_sk_all)
            {
                cellsSkill[fi].SetData(item, true, useMainSlot);
                fi++;
            }
        }
        #endregion
    }
}
