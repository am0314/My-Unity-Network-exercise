using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectID : NetworkBehaviour {
	[SyncVar]public Vector3 pos;	//同步座標
	[SyncVar]public Color col;		//同部顏色
	[SyncVar]public string myid,pname;
	public bool isbody;




	void Start(){
		gameObject.name = myid;

		if(isbody){
			GameObject.Find(pname).GetComponent<Play>().body=gameObject;
		}else{
			GameObject.Find(pname).GetComponent<Play>().ind=gameObject;
		}
		transform.parent = GameObject.Find(pname).transform;

	}
	
	// 這些值的更新由'Play'這個腳本執行
	void Update () {
		if(isServer){//伺服器端負責改變SyncVar的值,因此伺服器要以自己為基準更新SyncVar的值 isServer則代表你是host的話
			pos=gameObject.transform.position;
			col=gameObject.GetComponent<Renderer>().material.color;
		}else{//客戶端則用SyncVar來更新自己的顯示
			gameObject.transform.position=pos;
			gameObject.GetComponent<Renderer>().material.color=col;
		}
	}
}
