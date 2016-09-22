using UnityEngine;
using UnityEditor;
using System.Collections;
using BestHTTP;


public class Model : Editor {

    public string modelID;
    public string status;
    public string reviewerID;
    public string reviewerName;
    public string createTime;
    public string startMakeTime;
    public string finishMakeTime;
    public string lastReviewTime;
    public string predictMakeTime;
    public string businessUserID;
    public string businessUsername;
    public string modelName;
    public string genAssetName;

    public string keywords;
    
    public int pagesize;
    public int pagnum;

    AssetBundle model;

    public void InitModel()
    {
        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
