using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoalScrip : MonoBehaviour
{
    [SerializeField] Soal[] soal; //soal dibuat array karna ada banyak
    [SerializeField] InputField kertasJawaban; // inputfield buat nulis jawaban
    
    //fungsi buat check apakah jawaban yang ada di inputfield itu ada kata yang sama dengan keyword yang ada dalam soal
    void Jawab()
    {
        string[] a = kertasJawaban.text.Split(" ");
        for (int i = 0; i < a.Length; i++)
        {
            try
            {
                a[i] = soal[0].keyword[i].keyWord;
            }
            catch
            {
                print("beda kata");
            }
        }
    }
}
//String keyword
[System.Serializable]
public class Keyword
{
    public string keyWord;
}
//deskripsi soal berisi string soal dan string array keyword
[System.Serializable]
public class Soal
{
    [TextArea(0, 3)]
    public string deskripsiSoal;
    public Keyword[] keyword;
}
