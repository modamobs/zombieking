using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellPet : MonoBehaviour
{
    [SerializeField] GameDatabase.TableDB.Pet petDB;

    [SerializeField] UI ui = new UI();
    [System.Serializable]
    class UI
    {
        public Image imIcon, imRatingBg, imRatingOutline;
        public Text txRating, txLevel;
        public GameObject goUseLabel;
        public GameObject goSelect, goBlack;
        public GameObject goLck;
    }

    public void ScrollCellIndex(int idx)
    {
        var tapType = MainUI.GetInstance().tapPet.petTapType;
        if(tapType != PetTapType.Encyclopedia)
        {
            petDB = GameDatabase.GetInstance().tableDB.GetIndexPet(tapType, idx);
            
            ui.goUseLabel.SetActive(petDB.m_state == 1);
            ui.imIcon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(petDB.p_rt, petDB.p_id);
            ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(petDB.p_rt);
            ui.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(petDB.p_rt);
            ui.txRating.text = GameDatabase.StringFormat.GetRatingColorText(petDB.p_rt);
            ui.txLevel.text = petDB.p_rt >= 3 ? string.Format("Lv.{0}", petDB.p_lv) : string.Empty;

            ui.goLck.SetActive(int.Equals(petDB.m_lck, 1));
            if (tapType == PetTapType.Management)
            {
                ui.goSelect.SetActive(MainUI.GetInstance().tapPet.NowPetDB.aInUid == petDB.aInUid);
                ui.goBlack.SetActive(false);
            }
            else if (tapType == PetTapType.LevelUp)
            {
                ui.goSelect.SetActive(MainUI.GetInstance().tapPet.IsLevelUpSelect(petDB.aInUid));
                ui.goBlack.SetActive(false);
            }
            else if(tapType == PetTapType.OptionChange)
            {
                ui.goSelect.SetActive(MainUI.GetInstance().tapPet.IsOptionChangeSelect(petDB.aInUid));
                ui.goBlack.SetActive(false);
            }
            else if (tapType == PetTapType.Synthesis)
            {
                var _cdbPet = GameDatabase.GetInstance().chartDB.GetCdbPet(petDB.p_rt, petDB.p_id); // 옵션 
                ui.txLevel.text = string.Format("Lv.{0}/{1}", petDB.p_lv, _cdbPet.max_lv);
                if (MainUI.GetInstance().tapPet.isSynyMainSelect() == false) // 최대 레벨 메인 팻이 선택 되어있지 않다. 
                {
                    ui.goSelect.SetActive(false);
                    ui.goBlack.SetActive(petDB.p_lv < _cdbPet.max_lv);
                } 
                else // 최대 레벨의 메인 펫이 선택 되어있다. 
                {
                    ui.goSelect.SetActive(MainUI.GetInstance ().tapPet.isSynyMainSelectCheck(petDB.aInUid));
                    ui.goBlack.SetActive(MainUI.GetInstance().tapPet.isSynyMatSelectCheck(petDB.aInUid) || MainUI.GetInstance().tapPet.SyntSelectRating() != petDB.p_rt);
                }
            }
        }
        else
        {
            petDB = default;
            var cdbPet = GameDatabase.GetInstance().chartDB.GetCdbPet(idx);
            ui.goUseLabel.SetActive(false);
            ui.imIcon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(cdbPet.p_rt, cdbPet.p_id);
            ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(cdbPet.p_rt);
            ui.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(cdbPet.p_rt);
            ui.txRating.text = GameDatabase.StringFormat.GetRatingColorText(cdbPet.p_rt);
            ui.txLevel.text = string.Empty;
            ui.goBlack.SetActive(GameDatabase.GetInstance().tableDB.GetRatingIdPetEncy(cdbPet.p_rt, cdbPet.p_id).aInUid == 0);
            ui.goSelect.SetActive(false);
            ui.goLck.SetActive(false);
        }
    }

    public void Click_Select()
    {
        var tapType = MainUI.GetInstance().tapPet.petTapType;
        var _cdbPet = GameDatabase.GetInstance().chartDB.GetCdbPet(petDB.p_rt, petDB.p_id); // 옵션 

        if (tapType == PetTapType.Management)
        {
            MainUI.GetInstance().tapPet.UIManagement(petDB);
        }
        else if (tapType == PetTapType.LevelUp)
        {
            if(MainUI.GetInstance().tapPet.CellClick_LevelUpIsNowSelectState(petDB))
            {
                MainUI.GetInstance().tapPet.CellClick_LevelUpSelectIMat(petDB);
            }
            else
            {
                if (petDB.m_lck == 0)
                {
                    MainUI.GetInstance().tapPet.CellClick_LevelUpSelectIMat(petDB);
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("펫이 현재 잠금상태입니다.\n재료로 등록하시겠습니까?", AskConfirmLevelUpSelect);
                }
            }
        }
        else if (tapType == PetTapType.OptionChange)
        {
            MainUI.GetInstance().tapPet.CellClick_OpChangePetSelect(petDB);
        }
        else if (tapType == PetTapType.Synthesis)
        {
            if (MainUI.GetInstance().tapPet.isSynyMainSelect() == false)
            {
                if(petDB.p_lv >= _cdbPet.max_lv)
                {
                    MainUI.GetInstance().tapPet.CellClick_SyntSelectMain(petDB);
                } 
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("각 등급 최대 레벨인 펫만 메인으로 선택 할 수 있습니다.");
            }
            else
            {
                if(petDB.m_lck == 0) // 잠금 상태 아닐때 
                {
                    if(MainUI.GetInstance().tapPet.SyntSelectRating() == petDB.p_rt)
                        MainUI.GetInstance().tapPet.CellClick_SyntSelectMat(petDB);
                }
                else // 잠금 상태 일때 
                {
                    var usePet = GameDatabase.GetInstance().tableDB.GetUsePet();
                    if (usePet.aInUid == petDB.aInUid)
                    {
                        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("현재 동행중인 펫은 재료로 사용할 수 없습니다.");
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("펫이 현재 잠금상태입니다.\n재료로 등록하시겠습니까?", AskComfirmSynthesisSelect);
                    }
                }
            }
        }
        else if (tapType == PetTapType.Encyclopedia)
        {
           
        }
    }

    void AskConfirmLevelUpSelect()
    {
        MainUI.GetInstance().tapPet.CellClick_LevelUpSelectIMat(petDB);
    }

    void AskComfirmSynthesisSelect()
    {
        MainUI.GetInstance().tapPet.CellClick_SyntSelectMat(petDB);
    }
}
