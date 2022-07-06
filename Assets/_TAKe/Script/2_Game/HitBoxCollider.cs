using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxCollider : MonoBehaviour
{
    [SerializeField] PlayerZombie pz;
    void OnTriggerEnter (Collider col)
    {
        return;
        //if(!pz.igp.playerSkillAction.isNowSkillAction)
        //{
        //    if (pz != null)
        //    {
        //        if (col.gameObject != pz.gameObject)
        //        {
        //            if (col.CompareTag("Player"))
        //            {
        //                //if(pz._animator.GetCurrentAnimatorStateInfo(0).IsName("GeneralAttack1"))
        //                //{
        //                Debug.LogError("OnTriggerEnter col.gameObject : " + col.gameObject.name);
        //                Vector3 pos = new Vector3(transform.position.x, transform.position.y, col.transform.position.z);
        //                ObjectPool.GetInstance().PopFromPool(ObjectPool.OP_Once_GeneralBeHit, pos);
        //                SoundManager.GetInstance().PlaySound("arena_hit");
        //                pz.Hit(pos, true);
        //                //}
        //            }
        //        }
        //    }
        //}
    }
}
