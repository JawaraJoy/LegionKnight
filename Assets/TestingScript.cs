using CloudOnce;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public TextMeshProUGUI txtNama;
    public TextMeshProUGUI txtScore;

    private void Start()
    {
        Cloud.Initialize(true, true, true);
        Cloud.OnSignedInChanged += (x) =>
        {
            txtNama.text = Cloud.PlayerDisplayName;
            Leaderboards.LegionKnight.LoadScores((x) =>
                {
                    if (x == null) return;
                    if (x.Length == 0) return;

                    try
                    {

                        txtScore.text = (x.Where(y => y.userID.Equals(Cloud.PlayerID))).FirstOrDefault().value.ToString();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        txtScore.text = $"Error cause : {e}";
                    }


                });
        };
    }

    public void Login()
    {
        Cloud.SignIn(true);
    }

    public void Addscore()
    {
        Leaderboards.LegionKnight.SubmitScore(UnityEngine.Random.Range(0, 100), (x) =>
        {
            Leaderboards.LegionKnight.LoadScores((x) =>
            {
                if (x == null) return;
                if (x.Length == 0) return;

                try
                {

                    txtScore.text = (x.Where(y => y.userID.Equals(Cloud.PlayerID))).FirstOrDefault().value.ToString();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    txtScore.text = $"Error cause : {e}";
                }


            });
        });
    }
}
