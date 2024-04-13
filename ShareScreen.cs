using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShareScreen : MonoBehaviour
{
    public string ihadABaby = "I had a baby, I became a parent";
    public string myBaby = "My baby is already ";
    public string monthsOld = " months old";
    public string monthOld = " month old";
    private const string exitData3 = "exitData3";
    private bool isTimeAbuse = false;
    private bool isTimeReturn = false;

    int pastTimeInSeconds = 0;
    int secondsCountTemp = 0;
    [SerializeField] private AudioSource cameraSound;
    [SerializeField] private Text monthAge;
    [SerializeField] private Text childName;
    [SerializeField] private Text childNameOnFrame;
    [SerializeField] private Text screenSign;
    [SerializeField] private Image screeenImg;
    [SerializeField] private GameObject ScreenDisplay;
    [SerializeField] private GameObject plus30EnergyVisualComponent;
    [SerializeField] private List<GameObject> ui = new List<GameObject>();
    private List<GameObject> ActiveUi = new List<GameObject>();

    private DateTime dayOfRewarCollect;
    private const string dayOfRewarCollectSave = "dayOfRewarCollectSave";
    private const string canEnergyCollectForShare = "canEnergyCollectForShare";
    private const string url = "https://play.google.com/store/apps/details?id=com.DefaultCompany.BabyHost";
    private const int energyReward = 30;

    private const string ShareAPhoto = "ShareAPhoto";
    private tasksManager2 tasksManager_2;

#if UNITY_ANDROID || UNITY_IOS
    private bool isFocus = false;

    private string shareSubject, shareMessage;
    private bool isProcessing = false;
    private string screenshotName;

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(exitData3, DateTime.Now.ToString());
    }

#if UNITY_ANDROID || UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetString(exitData3, DateTime.Now.ToString());
        }
        else
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString(exitData3, DateTime.Now.ToString()));
            pastTimeInSeconds = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
        }
    }
#endif
    void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
#if UNITY_IOS
        if (!focus)
        {
            PlayerPrefs.SetString(exitData3, DateTime.Now.ToString());
        }
        else
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString(exitData3, DateTime.Now.ToString()));
            pastTimeInSeconds = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
        }
#endif
    }
    public void OnExitGame(TimeSpan ts1)
    {
        secondsCountTemp = ((ts1.Days * 24) + ts1.Hours) * 3600 + ts1.Minutes * 60 + ts1.Seconds;
        TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString(exitData3, DateTime.Now.ToString()));
        pastTimeInSeconds = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
        if (secondsCountTemp * 5 <= pastTimeInSeconds) //если время отсутствия по защищ таймеру в минимум 5 раз меньше чем по системному, то перемотка времени
        {
            isTimeAbuse = true;
            isTimeReturn = false;
        }
        else if (pastTimeInSeconds < 0)
        {
            isTimeReturn = true;
            isTimeAbuse = false;
        }
        else
        {
            isTimeAbuse = false;
            isTimeReturn = false;
        }
    }

    private void Start()
    {
        TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString(exitData3, DateTime.Now.ToString()));
        pastTimeInSeconds = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;

        if (PlayerPrefs.GetInt("Stage") == 2)
            tasksManager_2 = gameObject.GetComponent<tasksManager2>();

        if (PlayerPrefs.HasKey(dayOfRewarCollectSave))
            dayOfRewarCollect = DateTime.Parse(PlayerPrefs.GetString(dayOfRewarCollectSave));

        if (PlayerPrefs.GetInt(canEnergyCollectForShare) == 1)
            energyRewardPayAction();
    }

    public void closeScreenDisplay()
    {
        ScreenDisplay.SetActive(false);
    }

    public void MakeScrenshot()
    {
        if (tasksManager_2 != null)
        {
            if (tasksManager_2.tasksDisplay.activeSelf)
                tasksManager_2.tasksDisplay.SetActive(false);
        }

        screenshotName = "Screenshot_Best_Parent" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        shareSubject = " ";
        if (int.Parse(monthAge.text) == 0)
            shareMessage = ihadABaby;
        else if (int.Parse(monthAge.text) == 1)
            shareMessage = myBaby + monthAge.text + monthOld;
        else
            shareMessage = myBaby + monthAge.text + monthsOld;

        screenSign.text = "\"" + shareMessage + "\"";
        childNameOnFrame.text = childName.text;

        if (ActiveUi != null && ActiveUi.Count != 0)
            ActiveUi.Clear();
        for (int i = 0; i < ui.Count; i++)
        {
            if (ui[i].activeSelf)
                ActiveUi.Add(ui[i]);
        }
        for (int i = 0; i < ActiveUi.Count; i++)
        {
            ActiveUi[i].SetActive(false);
        }

        StartCoroutine(ScreenMaker());
    }

    private IEnumerator ScreenMaker()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture;
        if (PlayerPrefs.GetInt("Stage") == 1)
        {
            texture = new Texture2D((int)(Screen.width / 1.4f), Screen.height, TextureFormat.RGB24, false);

            texture.ReadPixels(new Rect((Screen.width - Screen.width / 1.4f) / 2 + 100, 0, Screen.width / 1.4f, Screen.height), 0, 0);
            texture.Apply();
        }
        else
        {
            float scaler = 1.6f;
            float scalerH = 1.2f;
            texture = new Texture2D((int)(Screen.width / scaler), (int)(Screen.height / scalerH), TextureFormat.RGB24, false);

            texture.ReadPixels(new Rect((Screen.width - Screen.width / scaler) / 2 - 70, 0, Screen.width / scaler, Screen.height / scalerH), 0, 0);
            texture.Apply();
        }

        //сохранение скриншота
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

        byte[] bytes = texture.EncodeToPNG();

        string fileLocation = Application.persistentDataPath + "/" + screenshotName;
        File.WriteAllBytes(fileLocation, bytes);
#elif !UNITY_EDITOR && UNITY_ANDROID
        //REFRESHING THE ANDROID PHONE PHOTO GALLERY IS BEGUN
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classMedia = new AndroidJavaClass("android.media.MediaScannerConnection");
        classMedia.CallStatic("scanFile", new object[4] { objActivity,
        new string[] { fileLocation },
        new string[] { "image/png" },
        null  });
        //REFRESHING THE ANDROID PHONE PHOTO GALLERY IS COMPLETE
#endif

        for (int i = 0; i < ActiveUi.Count; i++)
        {
            ActiveUi[i].SetActive(true);
        }

        cameraSound.Play();
        yield return new WaitForSeconds(1f);
        
        if (canRewardCollect())
            plus30EnergyVisualComponent.SetActive(true);
        else
            plus30EnergyVisualComponent.SetActive(false);

        ScreenDisplay.SetActive(true);

        //screeenImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(900f, 657.31f);//(float)texture.height / (float)texture.width * 900f);
        //screeenImg.sprite = Sprite.Create(texture, new Rect(0, 0f, texture.width, texture.height), new Vector2(0, 0));
        if (520f / 712f * texture.width <= texture.height)
            screeenImg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, 520f / 712f * texture.width), new Vector2(0, 0));
        else
            screeenImg.sprite = Sprite.Create(texture, new Rect((texture.width - 712f / 520f * texture.height) / 2, 0, 712f / 520f * texture.height, texture.height), new Vector2(0, 0));
    }    

    public void OnShareButtonClick()
    {

        ShareScreenshot();
    }

    private void ShareScreenshot()
    {
        if (!isProcessing)
        {
            StartCoroutine(ShareScreenshotInAnroid());
        }
    }


    public IEnumerator ShareScreenshotInAnroid()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(0.25f);

        string screenShotPath = Application.persistentDataPath + "/" + screenshotName;

        if (PlayerPrefs.GetInt("Stage") == 2)
        {
            tasksManager_2.TasksChecker();
            if (tasksManager_2.tasksList[9].activeSelf || tasksManager_2.tasksList[37].activeSelf || tasksManager_2.tasksList[71].activeSelf ||
                tasksManager_2.tasksList[102].activeSelf || tasksManager_2.tasksList[124].activeSelf)
                PlayerPrefs.SetInt(ShareAPhoto, PlayerPrefs.GetInt(ShareAPhoto) + 1);
            tasksManager_2.TasksChecker();
        }

        if (canRewardCollect())
        {
            StartCoroutine(collectReward());

            dayOfRewarCollect = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);
            PlayerPrefs.SetString(dayOfRewarCollectSave, dayOfRewarCollect.ToString());
        }

        yield return new WaitForSecondsRealtime(0.25f);

        if (!Application.isEditor)
        {
            new NativeShare().AddFile(screenShotPath)
        .SetSubject(shareSubject).SetText(shareMessage + ". Play with me -").SetUrl(url)
        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
            /*
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            //AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            //AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotPath);
            AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", screenShotPath);
            AndroidJavaClass fileProviderClass = new AndroidJavaClass("android.support.v4.content.FileProvider");
            object[] providerParams = new object[3];
            providerParams[0] = currentActivity;
            providerParams[1] = "com.DefaultCompany.BabyHost.provider";
            providerParams[2] = fileObject;
            AndroidJavaObject uriObject = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", providerParams);

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage + "\n" + url);

            intentObject.Call<AndroidJavaObject>("addFlags", intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));

            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "");
            currentActivity.Call("startActivity", chooser);
            */
        }

        yield return new WaitUntil(() => isFocus);
        ScreenDisplay.SetActive(false);
        isProcessing = false;
    }

    private bool canRewardCollect()
    {
        if (isTimeAbuse)
        {
            dayOfRewarCollect = dayOfRewarCollect.AddSeconds(pastTimeInSeconds - secondsCountTemp);
            PlayerPrefs.SetString(dayOfRewarCollectSave, dayOfRewarCollect.ToString());
            isTimeAbuse = false;
        }
        else if (isTimeReturn)
        {
            if (pastTimeInSeconds - secondsCountTemp < 0)
                dayOfRewarCollect = dayOfRewarCollect.AddSeconds(pastTimeInSeconds - secondsCountTemp);
            else
                dayOfRewarCollect = dayOfRewarCollect.AddSeconds(-(pastTimeInSeconds - secondsCountTemp));
            PlayerPrefs.SetString(dayOfRewarCollectSave, dayOfRewarCollect.ToString());
            isTimeReturn = false;
        }
        dayOfRewarCollect = new(dayOfRewarCollect.Year, dayOfRewarCollect.Month, dayOfRewarCollect.Day, 0, 0, 0, 0);

        TimeSpan ts = DateTime.Now - dayOfRewarCollect;
        int pastTimeInSeconds_ = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
        if (pastTimeInSeconds < 0)
            dayOfRewarCollect = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);

        DateTime dateNow = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);
        if (dateNow > dayOfRewarCollect || !PlayerPrefs.HasKey(dayOfRewarCollectSave))
        {
            return true;
        }

        return false;
    }

    private IEnumerator collectReward()
    {
        PlayerPrefs.SetInt(canEnergyCollectForShare, 1);
        yield return new WaitForSeconds(60f);

        energyRewardPayAction();
    }

    private void energyRewardPayAction()
    {
        GlobalVaribles.Energy += energyReward;
        PlayerPrefs.SetInt(canEnergyCollectForShare, 0);

        plus30EnergyVisualComponent.SetActive(false);
    }
#endif
}