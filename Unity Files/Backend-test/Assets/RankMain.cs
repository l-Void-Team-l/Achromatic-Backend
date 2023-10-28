using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft;
using UnityEngine.UI;
using Newtonsoft.Json;

public class RankMain : MonoBehaviour
{
    public string host;
    public string port;
    public string idurl;
    public string top3url;
    public string posturl;
    public Text idtext;
    public Text scoretext;

    public Button idbtn;
    public Button top3btn;
    public Button postbtn;

    void Start()
    {
        this.top3btn.onClick.AddListener(() => {
            var url = string.Format("{0}:{1}/{2}", host, port, top3url);
            Debug.Log(url);

            StartCoroutine(this.GetTop3(url, (raw) =>
            {
                var res = JsonConvert.DeserializeObject<Protocols.Packets.res_scores_top3>(raw);
                Debug.LogFormat("{0}, {1}", res.cmd, res.result.Length);
                foreach (var user in res.result)
                {
                    Debug.LogFormat("{0} : {1}", user.id, user.score);
                }
            }));


        });
        this.idbtn.onClick.AddListener(() => {
            var url = string.Format("{0}:{1}/{2}/{3}", host, port, idurl, idtext.text);
            Debug.Log(url);

            StartCoroutine(this.GetId(url, (raw) => {

                var res = JsonConvert.DeserializeObject<Protocols.Packets.res_scores_id>(raw);
                Debug.LogFormat("{0}, {1}", res.result.id, res.result.score);

            }));
        });
        this.postbtn.onClick.AddListener(() => {
            var url = string.Format("{0}:{1}/{2}", host, port, posturl);
            Debug.Log(url); //http://localhost:3030/scores

            var req = new Protocols.Packets.req_scores();
            req.cmd = 1000; //(int)Protocols.eType.POST_SCORE;
            req.id = idtext.text;
            req.score = int.Parse(scoretext.text);
            //����ȭ  (������Ʈ -> ���ڿ�)
            var json = JsonConvert.SerializeObject(req);
            Debug.Log(json);
            //{"id":"hong@nate.com","score":100,"cmd":1000}

            StartCoroutine(this.PostScore(url, json, (raw) => {
                Protocols.Packets.res_scores res = JsonConvert.DeserializeObject<Protocols.Packets.res_scores>(raw);
                Debug.LogFormat("{0}, {1}", res.cmd, res.message);
            }));

        });
    }

    private IEnumerator GetTop3(string url, System.Action<string> callback)
    {

        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("��Ʈ��ũ ȯ���� �����Ƽ� ����� �Ҽ� �����ϴ�.");
        }
        else
        {
            callback(webRequest.downloadHandler.text);
        }
    }

    private IEnumerator GetId(string url, System.Action<string> callback)
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        Debug.Log("--->" + webRequest.downloadHandler.text);

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("��Ʈ��ũ ȯ���� �����Ƽ� ����� �Ҽ� �����ϴ�.");
        }
        else
        {
            callback(webRequest.downloadHandler.text);
        }
    }

    private IEnumerator PostScore(string url, string json, System.Action<string> callback)
    {

        var webRequest = new UnityWebRequest(url, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(json); //����ȭ (���ڿ� -> ����Ʈ �迭)

        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("��Ʈ��ũ ȯ���� �����Ƽ� ����� �Ҽ� �����ϴ�.");
        }
        else
        {
            Debug.LogFormat("{0}\n{1}\n{2}", webRequest.responseCode, webRequest.downloadHandler.data, webRequest.downloadHandler.text);
            callback(webRequest.downloadHandler.text);
        }
    }

}
