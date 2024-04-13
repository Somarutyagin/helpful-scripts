using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CheckTime : MonoBehaviour
{
    private bool isState = false;

    private void resetTimers()
    {
        if (DateTime.Now < DateTime.Parse(PlayerPrefs.GetString("lastSessionOff", DateTime.Now.ToString())))
        {
            PlayerPrefs.DeleteKey("lastSessionOff");
            PlayerPrefs.DeleteKey("lastSessionOn");
            PlayerPrefs.DeleteKey("lastSession");
        }
    }

    public void OnApplicationQuit()
    {
        DateTime dateTimeOn;
        DateTime dateTimeOff;
        checkInternetConnection((isConnected) => {
            if (isConnected)
            {
                //возвращает время при выходе со включенным интернетом
                dateTimeOn = CheckGlobalTime();
                PlayerPrefs.SetString("lastSessionOn", dateTimeOn.ToString());

                PlayerPrefs.SetInt("lastSession", 1);


                //возвращает время при выключенном интернете при выходе со включенным интернетом
                dateTimeOff = DateTime.Now;
                PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());
            }
            else
            {
                //возвращает время при выходе с выключенным интернетом
                dateTimeOff = DateTime.Now;
                PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());

                PlayerPrefs.SetInt("lastSession", 0);
            }
        });
    }
    public void Awake()
    {
        resetTimers();
        isState = false; 

        DateTime dateTimeOn;
        DateTime dateTimeOff;
        TimeSpan ts;
        checkInternetConnection((isConnected) => {
            if (isConnected)
            {
                if (PlayerPrefs.GetInt("lastSession") == 1)
                {
                    //возвращает разницу во времени при входе со включенным интернетом
                    dateTimeOn = CheckGlobalTime();
                    //возвращает разницу во времени при входе с выключенным интернетом
                    dateTimeOff = DateTime.Now;
                    if (PlayerPrefs.HasKey("lastSessionOn"))
                    {
                        ts = dateTimeOn - DateTime.Parse(PlayerPrefs.GetString("lastSessionOn"));
                        //операции с переводом времени отсутствия в характеристики
                        StateUpdater.OnExitGame(ts);
                        EventGenerator.OnExitGame(ts);
                        adMobRewardedAd.OnExitGame(ts);
                        bonus_Wheel.OnExitGame(ts);
                        shareScreen.OnExitGame(ts);
                        isCalledOnExitGame = true;
                        isState = true;

                        print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                    }
                }
                else
                {
                    //возвращает разницу во времени при входе с выключенным интернетом
                    dateTimeOff = DateTime.Now;
                    if (PlayerPrefs.HasKey("lastSessionOff"))
                    {
                        ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));
                        //операции с переводом времени отсутствия в характеристики
                        StateUpdater.OnExitGame(ts);
                        EventGenerator.OnExitGame(ts);
                        adMobRewardedAd.OnExitGame(ts);
                        bonus_Wheel.OnExitGame(ts);
                        shareScreen.OnExitGame(ts);
                        isCalledOnExitGame = true;
                        isState = true;

                        print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                    }
                }
            }
            else
            {
                //возвращает разницу во времени при входе с выключенным интернетом
                dateTimeOff = DateTime.Now;
                if (PlayerPrefs.HasKey("lastSessionOff"))
                {
                    ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));
                    //операции с переводом времени отсутствия в характеристики
                    StateUpdater.OnExitGame(ts);
                    EventGenerator.OnExitGame(ts);
                    adMobRewardedAd.OnExitGame(ts);
                    bonus_Wheel.OnExitGame(ts);
                    shareScreen.OnExitGame(ts);
                    isCalledOnExitGame = true;
                    isState = true;

                    print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                }
            }
        });
#if PLATFORM_IOS
        isState = false;
#endif
    }

#if UNITY_ANDROID || UNITY_EDITOR
    public void OnApplicationPause(bool pause)
    {
        DateTime dateTimeOn;
        DateTime dateTimeOff;
        TimeSpan ts;
        if (pause)
        {
            checkInternetConnection((isConnected) => {
                if (isConnected)
                {
                    //возвращает время при выходе со включенным интернетом
                    dateTimeOn = CheckGlobalTime();
                    PlayerPrefs.SetString("lastSessionOn", dateTimeOn.ToString());

                    PlayerPrefs.SetInt("lastSession", 1);


                    //возвращает время при выключенном интернете при выходе со включенным интернетом
                    dateTimeOff = DateTime.Now;
                    PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());
                }
                else
                {
                    //возвращает время при выходе с выключенным интернетом
                    dateTimeOff = DateTime.Now;
                    PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());

                    PlayerPrefs.SetInt("lastSession", 0);
                }
            });
        }
        else
        {
            if (!isOnColldown)
            {
            checkInternetConnection((isConnected) => {
                if (isConnected)
                {
                    if (PlayerPrefs.GetInt("lastSession") == 1)
                    {
                        //возвращает разницу во времени при входе со включенным интернетом
                        dateTimeOn = CheckGlobalTime();
                        //возвращает разницу во времени при входе с выключенным интернетом
                        dateTimeOff = DateTime.Now;
                        if (PlayerPrefs.HasKey("lastSessionOn") && isState != true)
                        {
                            ts = dateTimeOn - DateTime.Parse(PlayerPrefs.GetString("lastSessionOn"));

                            print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                        }
                        else if (PlayerPrefs.HasKey("lastSessionOn") && isState == true)
                        {
                            isState = false;
                        }
                    }
                    else
                    {
                        //возвращает разницу во времени при входе с выключенным интернетом
                        dateTimeOff = DateTime.Now;
                        if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                        {
                            ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                            print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                        }
                        else if (PlayerPrefs.HasKey("lastSessionOff") && isState == true)
                        {
                            isState = false;
                        }
                    }
                }
                else
                {
                    //возвращает разницу во времени при входе с выключенным интернетом
                    dateTimeOff = DateTime.Now;
                    if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                    {
                        ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                        print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                    }
                    else if (PlayerPrefs.HasKey("lastSessionOff") && isState == true)
                    {
                        isState = false;
                    }
                }
            });
            }
        }
    }
#elif UNITY_IOS
    public void OnApplicationFocus(bool focus)
    {
        DateTime dateTimeOn;
        DateTime dateTimeOff;
        TimeSpan ts;
        if (!focus)
        {
            checkInternetConnection((isConnected) => {
                if (isConnected)
                {
                    //возвращает время при выходе со включенным интернетом
                    dateTimeOn = CheckGlobalTime();
                    PlayerPrefs.SetString("lastSessionOn", dateTimeOn.ToString());

                    PlayerPrefs.SetInt("lastSession", 1);


                    //возвращает время при выключенном интернете при выходе со включенным интернетом
                    dateTimeOff = DateTime.Now;
                    PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());
                }
                else
                {
                    //возвращает время при выходе с выключенным интернетом
                    dateTimeOff = DateTime.Now;
                    PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());

                    PlayerPrefs.SetInt("lastSession", 0);
                }
            });
        }
        else
        {
            if (!isOnColldown)
            {
                checkInternetConnection((isConnected) =>
                {
                    if (isConnected)
                    {
                        if (PlayerPrefs.GetInt("lastSession") == 1)
                        {
                            //возвращает разницу во времени при входе со включенным интернетом
                            dateTimeOn = CheckGlobalTime();
                            if (PlayerPrefs.HasKey("lastSessionOn") && isState != true)
                            {
                                ts = dateTimeOn - DateTime.Parse(PlayerPrefs.GetString("lastSessionOn"));

                                print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                            }
                            else if (PlayerPrefs.HasKey("lastSessionOn") && isState == true)
                            {
                                isState = false;
                            }
                        }
                        else
                        {
                            //возвращает разницу во времени при входе с выключенным интернетом
                            dateTimeOff = DateTime.Now;
                            if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                            {
                                ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                                print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                            }
                            else if (PlayerPrefs.HasKey("lastSessionOff") && isState == true)
                            {
                                isState = false;
                            }
                        }
                    }
                    else
                    {
                        //возвращает разницу во времени при входе с выключенным интернетом
                        dateTimeOff = DateTime.Now;
                        if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                        {
                            ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                            print(string.Format("Вы отсутствовали: {0} дней, {1} часов, {2} минут, {3} секунд", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                        }
                        else if (PlayerPrefs.HasKey("lastSessionOff") && isState == true)
                        {
                            isState = false;
                        }
                    }
                });
            }
        }
    }
#endif

    //check online time
    public DateTime CheckGlobalTime()
    {
        var www = new WWW("https://google.com");
        while (!www.isDone && www.error == null)
            Thread.Sleep(1);

        var str = www.responseHeaders["Date"];
        DateTime dateTime;

        if (!DateTime.TryParse(str, out dateTime))
            return DateTime.MinValue;

        return dateTime.ToUniversalTime();
    }

    public void checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        if (www.error != null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                action(false);
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                action(true);
            }
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                action(false);
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                action(true);
            }
        }
    }
}
