using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallbackPvpReward : MonoBehaviour
{
    [SerializeField] UI ui;
    [System.Serializable]
    class UI
    {
        public Image imIcon;
        public Image imRatingBg;
        public Text txRating;
        public Text txCount;
        public Text txName;
    }
    public void ScrollCellIndex(int idx)
    {
        // db.gch_type; -> it:item, win_score or lose_score : 점수, battle_coin : 배틀 코인 
        var db = PopUpMng.GetInstance().popUpPvpBattleResult.pvpRewards[idx];

        ui.txName.text = db.it_name;
        if (string.Equals(db.gch_type, "item"))
        {
            ui.imIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(db.it_type, db.it_rating);
            ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(db.it_rating);
            ui.txRating.text = GameDatabase.StringFormat.GetRatingColorText(db.it_rating, false);
            ui.txCount.text = string.Format("<color=#FFFFFF>x{0}</color>", db.it_cnt);
        }
        else if (string.Equals(db.gch_type, "battle_coin") || (string.Equals(db.gch_type, "win_score") || string.Equals(db.gch_type, "lose_score")))
        {
            ui.imIcon.sprite = SpriteAtlasMng.GetInstance().GetPvpRewarCellIconSprite(db.gch_type);
            ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            ui.txRating.text = "";

            switch (db.gch_type)
            {
                case "battle_coin":
                    ui.txCount.text = string.Format("<color=#FFFFFF>x{0}</color>", db.it_cnt);
                    break;
                case "win_score":
                    ui.txCount.text = string.Format("<color=#00FFFF>+{0}</color>", db.it_cnt);
                    break;
                case "lose_score":
                    ui.txCount.text = string.Format("<color=#FF0000>-{0}</color>", db.it_cnt);
                    break;
            }
        }
        else
        {
            ui.imIcon.sprite = SpriteAtlasMng.GetInstance().GetTransparency();
            ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            ui.txRating.text = "";
        }
    }
}
