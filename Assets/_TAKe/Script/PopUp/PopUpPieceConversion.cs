using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopUpPieceConversion : MonoBehaviour
{
    [SerializeField] InitOnStartPieceConversion initOnStartPieceConversion;

    public bool n_is_acce = false;
    public int n_rt = 0;

    [SerializeField] UI ui = new UI();

    [System.Serializable]
    [SerializeField] struct UI
    {
        public GameObject goSelectEquipBox, goSelectAcceBox;
        public GameObject goRating5Btn, goRating6Btn;
        public GameObject[] goSelectRatingBox;
    }

    Dictionary<int, int[]> afterEquipRating = new Dictionary<int, int[]>()
    {
        { 2, new int[] { 3,4,5,6,7 } }, // 중급
        { 3, new int[] { 4,5,6,7 } }, // 고급
        { 4, new int[] { 5,6,7 } }, // 희귀
        { 5, new int[] { 6,7 } }, // 영웅
        { 6, new int[] { 7 } }, // 고대
    };

    Dictionary<int, int[]> afterAcceRating = new Dictionary<int, int[]>()
    {
        { 2, new int[] { 3,4,5 } }, // 중급
        { 3, new int[] { 4,5 } }, // 고급
        { 4, new int[] { 5 } }, // 희귀
    };

    Dictionary<int, float[]> conversionRatioCount = new Dictionary<int, float[]>()
    {
        { 2, new float[] {    10,     100,    1000,   10000,  100000 } }, // 중급
        { 3, new float[] {    10,     100,    1000,   10000 } }, // 고급
        { 4, new float[] {    10,     100,    1000 } }, // 희귀
        { 5, new float[] {    10,     100 } }, // 영웅
        { 6, new float[] {    10 } }, // 고대
    };

    public int GetAfterRating (int n_rt, int cell_indx)
    {
        if (n_is_acce)
            return afterAcceRating[n_rt][cell_indx];
        else return afterEquipRating[n_rt][cell_indx];
    }

    public float GetRatio(int n_rt, int cell_indx)
    {
        if (n_is_acce)
            return conversionRatioCount[n_rt][cell_indx] * 3;
        else return conversionRatioCount[n_rt][cell_indx];
    }

    public void SetData(bool piec_is_acce, int pice_rt)
    {
        n_is_acce = piec_is_acce;
        n_rt = pice_rt;
        ui.goSelectEquipBox.SetActive(!piec_is_acce);
        ui.goSelectAcceBox.SetActive(piec_is_acce);

        ui.goRating5Btn.SetActive(!n_is_acce);
        ui.goRating6Btn.SetActive(!n_is_acce);
        for (int i = 0; i < ui.goSelectRatingBox.Length; i++)
        {
            if(ui.goSelectRatingBox [i] != null)
            {
                ui.goSelectRatingBox[i].SetActive(i == pice_rt);
            }
        }

        if (piec_is_acce)
            initOnStartPieceConversion.SetInit(afterAcceRating[pice_rt].Length);
        else
            initOnStartPieceConversion.SetInit(afterEquipRating[pice_rt].Length);
    }

    public void SetInitCellCall()
    {
        if (n_is_acce)
            initOnStartPieceConversion.SetInit(afterAcceRating[n_rt].Length);
        else
            initOnStartPieceConversion.SetInit(afterEquipRating[n_rt].Length);
    }

    public void Click_ChangeType(bool piec_is_acce)
    {
        if (piec_is_acce && n_rt >= 5)
            n_rt = 4;

        SetData(piec_is_acce, n_rt);
    }
    public void Click_ChangeRating(int p_rt)
    {
        SetData(n_is_acce, p_rt);
    }
}
