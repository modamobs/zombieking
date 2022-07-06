using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPotionConversion : MonoBehaviour
{
    [SerializeField] InitOnStartPotionConversion initOnStartPotionConversion;

    public int n_rt = 0;

    [SerializeField] UI ui = new UI();

    [System.Serializable]
    [SerializeField]
    struct UI
    {
        public GameObject[] goSelectRatingBox;
    }

    Dictionary<int, int[]> afterRating = new Dictionary<int, int[]>()
    {
        { 1, new int[] { 2,3 } }, // 중급
        { 2, new int[] { 3 } }, // 고급
    };

    Dictionary<int, float[]> conversionRatioCount = new Dictionary<int, float[]>()
    {
        { 1, new float[] {    10,     100,    1000,   10000,  100000 } }, // 일반
        { 2, new float[] {    10,     100,    1000,   10000 } }, // 중급
        //{ 3, new float[] {    10,     100,    1000 } }, // 고급
    };


    public void SetSart(int rt)
    {
        n_rt = rt;
        for (int i = 0; i < ui.goSelectRatingBox.Length; i++)
        {
            if (ui.goSelectRatingBox[i] != null)
            {
                ui.goSelectRatingBox[i].SetActive(i == rt);
            }
        }
        initOnStartPotionConversion.SetInit(afterRating[rt].Length);
    }

    public int GetAfterRating(int n_rt, int cell_indx) => afterRating[n_rt][cell_indx];
    public float GetRatio(int n_rt, int cell_indx) => conversionRatioCount[n_rt][cell_indx];
    public void SetInitCellCall() => initOnStartPotionConversion.SetInit(afterRating[n_rt].Length);

    public void Click_ChangeRating(int p_rt) => SetSart(p_rt);
}
