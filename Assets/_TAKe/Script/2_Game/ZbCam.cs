using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ZbCam : MonoBehaviour
{
    public float smooth = 3f;
	bool bQuickSwitch = false;
	[SerializeField] GameObject goPz;
	[SerializeField] Transform standardPos;
	Transform thTransf;
	GameMng gMgr;

	[SerializeField] Animator aniCam;
	public Transform trCam;
	[SerializeField] Text txCamTypeNbr;

	void Awake()
	{
		thTransf = this.transform;
	}

	void Start()
    {
		gMgr = GameMng.GetInstance();
		StartCoroutine("Loop");
		PlayZombieCamera();
	}

	IEnumerator Loop()
	{
		yield return null;
		while (true)
		{
			if (standardPos == null)
			{
				if(gMgr.myPZ != null)
				{
					if (gMgr.myPZ.igp.state == IG.ZombieState.MOVE_POS || gMgr.myPZ.igp.state == IG.ZombieState.FIGHT)
					{
						goPz = gMgr.myPZ.gameObject;
						standardPos = gMgr.myPZ.tr.zbCamTarget;
						setCameraPositionNormalView(true);
					}
				}
			}
			else
			{
				if (goPz)
				{
					if (goPz.activeSelf == true)
					{
						if (GameMng.GetSqr(goPz.transform.position, standardPos.position) > 10)
							setCameraPositionNormalView(true);
						else setCameraPositionNormalView(false);
					}
					else
					{
						goPz = null;
						standardPos = null;
					}
				}
			}

			yield return null;
		}
	}

	void setCameraPositionNormalView(bool isInit)
	{
		if (isInit == false)
		{
			thTransf.position = Vector3.Lerp(thTransf.position, standardPos.position, Time.fixedDeltaTime * smooth);
			//transform.forward = Vector3.Lerp(thTransf.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
		}
		else
		{
			thTransf.position = standardPos.position;
			//thTransf.forward = standardPos.forward;
		}
	}

	private int zbcam_id = 1;
	public void PlayZombieCamera(int id = -1)
	{
		if (id == -1)
			id = PlayerPrefs.GetInt(PrefsKeys.key_zb_cam_type);
		else
		if (id > 3 || id <= 0)
			id = 1;
		
		zbcam_id = id;
		LogPrint.Print(" zbcam_id : " + zbcam_id);

		txCamTypeNbr.text = id.ToString();
		aniCam.Play(string.Format("CamZb_Type{0}", id));
	}

	/// <summary>
	/// 던전 입장시 카메라 
	/// </summary>
	public void DgCam() => aniCam.Play("CamZb_Type0");

	/// <summary>
	/// 버튼 : 카메라 시점 변경 
	/// </summary>
	public void Click_ZombieCameraType()
	{
		zbcam_id = PlayerPrefs.GetInt(PrefsKeys.key_zb_cam_type);
		zbcam_id++;
		if (zbcam_id > 3 || zbcam_id <= 0)
			zbcam_id = 1;

		PlayerPrefs.SetInt(PrefsKeys.key_zb_cam_type, zbcam_id);
		PlayZombieCamera(zbcam_id);
	}
}
