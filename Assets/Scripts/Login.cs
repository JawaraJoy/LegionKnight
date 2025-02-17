using AppsDaddyO.TimeMan;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//blum bisa dipakai
public class Login : MonoBehaviour
{
    //public LastDateClaim lastDateTime;
    //public bool wasLogin;
    //void Start()
    //{
    //    StartCoroutine(LoadData());
    //}

    //private IEnumerator LoadData()
    //{
    //    DataContainer dataLogin = DataManager.instance.LoadLoginData();
    //    DataContainer dataDate = DataManager.instance.loaddata();
    //    if (dataDate != null)
    //    {
    //        if (dataLogin != null)
    //        {
    //            lastDateTime.day = dataDate.day;
    //            lastDateTime.month = dataDate.month;
    //            lastDateTime.year = dataDate.year;
    //            wasLogin = dataLogin.login;
    //            if (TimeManager.theCurrentTime == null)
    //            {
    //                yield return new WaitForSeconds(0.5f);
    //                StartCoroutine(LoadData());
    //            }
    //            else
    //            {
    //                if (TimeManager.theCurrentTime.Day > lastDateTime.day || TimeManager.theCurrentTime.Month > lastDateTime.month || TimeManager.theCurrentTime.Year > lastDateTime.year)
    //                {
    //                    if (!wasLogin)
    //                    {
    //                        ActivityPoint.Instance.AddOrSubActivityPoint(1);
    //                        wasLogin = true;
    //                        DataManager.instance.SaveData(wasLogin);
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            ActivityPoint.Instance.AddOrSubActivityPoint(1);
    //            wasLogin = true;
    //            DataManager.instance.SaveData(wasLogin);
    //        }
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
