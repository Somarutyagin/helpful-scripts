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
                //���������� ����� ��� ������ �� ���������� ����������
                dateTimeOn = CheckGlobalTime();
                PlayerPrefs.SetString("lastSessionOn", dateTimeOn.ToString());

                PlayerPrefs.SetInt("lastSession", 1);


                //���������� ����� ��� ����������� ��������� ��� ������ �� ���������� ����������
                dateTimeOff = DateTime.Now;
                PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());
            }
            else
            {
                //���������� ����� ��� ������ � ����������� ����������
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
                    //���������� ������� �� ������� ��� ����� �� ���������� ����������
                    dateTimeOn = CheckGlobalTime();
                    //���������� ������� �� ������� ��� ����� � ����������� ����������
                    dateTimeOff = DateTime.Now;
                    if (PlayerPrefs.HasKey("lastSessionOn"))
                    {
                        ts = dateTimeOn - DateTime.Parse(PlayerPrefs.GetString("lastSessionOn"));
                        //�������� � ��������� ������� ���������� � ��������������
                        StateUpdater.OnExitGame(ts);
                        EventGenerator.OnExitGame(ts);
                        adMobRewardedAd.OnExitGame(ts);
                        bonus_Wheel.OnExitGame(ts);
                        shareScreen.OnExitGame(ts);
                        isCalledOnExitGame = true;
                        isState = true;

                        print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                    }
                }
                else
                {
                    //���������� ������� �� ������� ��� ����� � ����������� ����������
                    dateTimeOff = DateTime.Now;
                    if (PlayerPrefs.HasKey("lastSessionOff"))
                    {
                        ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));
                        //�������� � ��������� ������� ���������� � ��������������
                        StateUpdater.OnExitGame(ts);
                        EventGenerator.OnExitGame(ts);
                        adMobRewardedAd.OnExitGame(ts);
                        bonus_Wheel.OnExitGame(ts);
                        shareScreen.OnExitGame(ts);
                        isCalledOnExitGame = true;
                        isState = true;

                        print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                    }
                }
            }
            else
            {
                //���������� ������� �� ������� ��� ����� � ����������� ����������
                dateTimeOff = DateTime.Now;
                if (PlayerPrefs.HasKey("lastSessionOff"))
                {
                    ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));
                    //�������� � ��������� ������� ���������� � ��������������
                    StateUpdater.OnExitGame(ts);
                    EventGenerator.OnExitGame(ts);
                    adMobRewardedAd.OnExitGame(ts);
                    bonus_Wheel.OnExitGame(ts);
                    shareScreen.OnExitGame(ts);
                    isCalledOnExitGame = true;
                    isState = true;

                    print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
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
                    //���������� ����� ��� ������ �� ���������� ����������
                    dateTimeOn = CheckGlobalTime();
                    PlayerPrefs.SetString("lastSessionOn", dateTimeOn.ToString());

                    PlayerPrefs.SetInt("lastSession", 1);


                    //���������� ����� ��� ����������� ��������� ��� ������ �� ���������� ����������
                    dateTimeOff = DateTime.Now;
                    PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());
                }
                else
                {
                    //���������� ����� ��� ������ � ����������� ����������
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
                        //���������� ������� �� ������� ��� ����� �� ���������� ����������
                        dateTimeOn = CheckGlobalTime();
                        //���������� ������� �� ������� ��� ����� � ����������� ����������
                        dateTimeOff = DateTime.Now;
                        if (PlayerPrefs.HasKey("lastSessionOn") && isState != true)
                        {
                            ts = dateTimeOn - DateTime.Parse(PlayerPrefs.GetString("lastSessionOn"));

                            print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                        }
                        else if (PlayerPrefs.HasKey("lastSessionOn") && isState == true)
                        {
                            isState = false;
                        }
                    }
                    else
                    {
                        //���������� ������� �� ������� ��� ����� � ����������� ����������
                        dateTimeOff = DateTime.Now;
                        if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                        {
                            ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                            print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                        }
                        else if (PlayerPrefs.HasKey("lastSessionOff") && isState == true)
                        {
                            isState = false;
                        }
                    }
                }
                else
                {
                    //���������� ������� �� ������� ��� ����� � ����������� ����������
                    dateTimeOff = DateTime.Now;
                    if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                    {
                        ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                        print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
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
                    //���������� ����� ��� ������ �� ���������� ����������
                    dateTimeOn = CheckGlobalTime();
                    PlayerPrefs.SetString("lastSessionOn", dateTimeOn.ToString());

                    PlayerPrefs.SetInt("lastSession", 1);


                    //���������� ����� ��� ����������� ��������� ��� ������ �� ���������� ����������
                    dateTimeOff = DateTime.Now;
                    PlayerPrefs.SetString("lastSessionOff", dateTimeOff.ToString());
                }
                else
                {
                    //���������� ����� ��� ������ � ����������� ����������
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
                            //���������� ������� �� ������� ��� ����� �� ���������� ����������
                            dateTimeOn = CheckGlobalTime();
                            if (PlayerPrefs.HasKey("lastSessionOn") && isState != true)
                            {
                                ts = dateTimeOn - DateTime.Parse(PlayerPrefs.GetString("lastSessionOn"));

                                print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                            }
                            else if (PlayerPrefs.HasKey("lastSessionOn") && isState == true)
                            {
                                isState = false;
                            }
                        }
                        else
                        {
                            //���������� ������� �� ������� ��� ����� � ����������� ����������
                            dateTimeOff = DateTime.Now;
                            if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                            {
                                ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                                print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
                            }
                            else if (PlayerPrefs.HasKey("lastSessionOff") && isState == true)
                            {
                                isState = false;
                            }
                        }
                    }
                    else
                    {
                        //���������� ������� �� ������� ��� ����� � ����������� ����������
                        dateTimeOff = DateTime.Now;
                        if (PlayerPrefs.HasKey("lastSessionOff") && isState != true)
                        {
                            ts = dateTimeOff - DateTime.Parse(PlayerPrefs.GetString("lastSessionOff"));

                            print(string.Format("�� �������������: {0} ����, {1} �����, {2} �����, {3} ������", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
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
