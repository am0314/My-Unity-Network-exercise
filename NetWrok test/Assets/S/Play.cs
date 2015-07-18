using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Play : NetworkBehaviour {
	public GameObject body,ind;	//要使用的參考物件 body是代表玩家的位置 ind則是告知其他人自己要前往哪裡的指標
	[SerializeField]public GameObject pbody,pind;//參考預製物件

	public Camera mycam;
	public NetworkIdentity mynetid;
	[SyncVar]public string playid;

	

	// Use this for initialization
	public override void OnStartLocalPlayer () {//這是只有本地玩家才會執行的部份 他比Start還早執行
		
		mycam = transform.Find("Camera").GetComponent<Camera>();//獲得自己的攝影機參考
		GameObject.Find("Main Camera").SetActive(false);//在本地關閉場景攝影機
		mycam.enabled=true;//為了避免有新玩家加入的時候 新玩家的攝影機會把其他人的攝影機優先蓋掉
		//因此玩家的攝影機在預製中的時候是關閉的 在這裡只有本地才打開
	}

	void Start () {

		mynetid = gameObject.GetComponent<NetworkIdentity>();
		playid = mynetid.netId.ToString();//獲得自己的netId

		gameObject.name = "play"+playid;//給自己改名 在start 這樣在unity編輯中方便辨識 和給新物件名字

		CallObj(pbody,gameObject,transform.position+=new Vector3(0,20,0),playid+"b",gameObject.name);//創造物件 並給物件名字 例如1b 2b 等 方便待會搜尋找
		CallObj(pind,gameObject,transform.position+=new Vector3(0,20,0),playid+"i",gameObject.name);
		
	}


	
	// Update is called once per frame
	void Update () {
		if(isLocalPlayer){//這是為了在所有同樣的物件中區別玩家的角色,避免操作會導致所有物件都做出同樣的動作
			if(Input.GetKey(KeyCode.W))
				mycam.transform.position += new Vector3(0,0,5*Time.deltaTime);
			if(Input.GetKey(KeyCode.S))
				mycam.transform.position += new Vector3(0,0,-5*Time.deltaTime);
			if(Input.GetKey(KeyCode.A))
				mycam.transform.position += new Vector3(-5*Time.deltaTime,0,0);
			if(Input.GetKey(KeyCode.D))
				mycam.transform.position += new Vector3(5*Time.deltaTime,0,0);

			if(Input.GetMouseButtonDown(0)){
				RaycastHit hit;
				Ray ray = mycam.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit)){
					CallMove(ind,hit.point);//呼叫移動的方法
				}
			}
			
			


			if(Input.GetMouseButtonDown(1)){
				CallMove(body,ind.transform.position);//呼叫移動的方法
			}

			
		}
	}

	void OnGUI(){
		if(isLocalPlayer){
			if(GUI.Button (new Rect(10,130,120,30),"紅")){
				CallColor(body,Color.red);//呼叫改變顏色的方法
				CallColor(ind,Color.red);
			}
			
			if(GUI.Button (new Rect(10,160,120,30),"白")){
				CallColor(body,Color.white);//呼叫改變顏色的方法
				CallColor(ind,Color.white);
			}
			
			if(GUI.Button (new Rect(10,190,120,30),"綠")){
				CallColor(body,Color.green);//呼叫改變顏色的方法
				CallColor(ind,Color.green);
			}
			if(GUI.Button (new Rect(10,220,120,30),"藍")){
				CallColor(body,Color.blue);//呼叫改變顏色的方法
				CallColor(ind,Color.blue);
			}
			
			if(GUI.Button (new Rect(10,250,120,30),"黑")){
				CallColor(body,Color.black);//呼叫改變顏色的方法
				CallColor(ind,Color.black);
			}
		}
	}
	


	//以下為創造物件
	[Client]
	void CallObj(GameObject obj,GameObject meobj,Vector3 pos,string snedid,string pn){ //obj是要創造的物件 meobj是自己 pos是座標 sendid是要給予的名字
		CmdlObj(obj,meobj,pos,snedid,pn);
	}

	[Command]
	void CmdlObj(GameObject obj,GameObject meobj,Vector3 pos,string snedid,string pn){
		GameObject nobj = (GameObject)Instantiate(obj,pos,transform.rotation);
		nobj.transform.parent = meobj.transform;//在server端將兩個物件變成發出請求玩家的子物件 這樣在玩家斷線時會一併刪除
		nobj.GetComponent<ObjectID>().myid = snedid;
		nobj.GetComponent<ObjectID>().pname = pn;
		NetworkServer.Spawn(nobj);
	}

	//以下為更新物件座標(移動)
	[Client]
	void CallMove(GameObject obj,Vector3 pos){
		CmdMove(obj,pos);
	}

	[Command]//用Command的話 方法前面必須有Cmd
	void CmdMove(GameObject obj,Vector3 pos){
		obj.transform.position = pos;
	}


	//以下為更新顏色的部份
	[Client]//客戶端發出
	void CallColor(GameObject obj,Color col){	//第一個參數是要改變的目標 第二個參數是要給予的顏色
		CmdColor(obj,col);
	}

	[Command]//請球伺服器執行的內容
	void CmdColor(GameObject obj,Color col){
		obj.GetComponent<Renderer>().material.color = col;//請Server端從那邊改變目標的顏色
	}

}
