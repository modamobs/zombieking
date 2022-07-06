using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TopUI : MonoBehaviour
{
    BackendReturnObject bro1 = null;
    [SerializeField] UI ui = new UI();
    [System.Serializable]
    struct UI
    {
        public Text txNickName;
        public Text txWhiteDiaTBC;
        public Text txGold;
        public Text txBlueDia;
        public Text txEther;
        public Text txRuby;
    }

    void Start()
    {
        //ui.txNickName.text = BackendGpgsMng.backendUserInfo.m_nickname.ToString();
    }

    public void SetGoodsView()
    {
        var goods = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        ui.txGold.text = string.Format("{0:#,0}", goods.m_gold);
        ui.txBlueDia.text = goods.m_dia == 0 ? "0" : string.Format("{0:#,0}", goods.m_dia);
        ui.txRuby.text = goods.m_ruby == 0 ? "0" : string.Format("{0:#,0}", goods.m_ruby);
        ui.txEther.text = goods.m_ether == 0 ? "0" : string.Format("{0:#,0}", goods.m_ether);
    }

    public async void GetInfoViewTBC()
    {
        int itbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
        InfoViewTBC(itbc);
    }

    public void InfoViewTBC(int count) => ui.txWhiteDiaTBC.text = count == 0 ? "0" : string.Format("{0:#,0}",  count);
}
