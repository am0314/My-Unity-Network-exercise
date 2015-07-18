using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectID : NetworkBehaviour {
	[SyncVar]public Vector3 pos;	//同步座標
	[SyncVar]public Color col;		//同步顏色
	[SyncVar]public string myid,pname;	//myid是自己待會要更名的id pname則是代表哪個玩家產生的
	public bool isbody;	//判斷自己是ind還是body 




	void Start(){
		gameObject.name = myid; //先給自己更名

		if(isbody){ //判斷自己是ind還是body後 把自己加入對方的參考
			GameObject.Find(pname).GetComponent<Play>().body=gameObject; 
		}else{
			GameObject.Find(pname).GetComponent<Play>().ind=gameObject;
		}
		transform.parent = GameObject.Find(pname).transform;//把自己加入對方的子物件

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
