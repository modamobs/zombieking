using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventZombie : MonoBehaviour
{
    [SerializeField] PlayerZombie pz;

    void Awake()
    {
        if (pz == null)
            pz = transform.parent.parent.GetComponent<PlayerZombie>();
    }

    public void EventGeneralAttack()
    {
        pz.TakeGeneralAttack();
    }

    public void EventGeneralAttackEnd()
    {

    }

    public void FootStep()
    {
        SoundManager.GetInstance().PlaySound("arena_footstep"); 
    }

    public void RootRot0()
    {
        //pz.transform.localEulerAngles = new Vector3(0, -25, 0);
    }

    public void RootRot25()
    {
        //pz.transform.eulerAngles = new Vector3(0, 25, 0);
    }

    // 버프 이벤트 
    public void EventSkillBuffEffect()
    {
        pz.igp.playerSkillAction.SkillBuffEffect();
    }

    // 공격 액션 후 버프 이벤트 체크 
    public void EventCheckingAttackEndBuff()
    {

    }

    //스킬 고고고 
    public void SkillEvent() // isBdbf => 1: 버프&디버프 적용 
    {
        pz.igp.playerSkillAction.SkillActivation();
    }

    public void SkillEnd()
    {

    }

    public void EventOnGameObject(string onObjName)
    {
        if(string.Equals(onObjName, "Bomb"))
        {
            pz.go_HandThrowBomb.SetActive(true);
        }
        else if(string.Equals(onObjName, "Smoke"))
        {
            pz.go_HandThrowSmoke.SetActive(true);
        }
    }
}
