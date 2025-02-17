using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBoss : MonoBehaviour
{
    public Image HealthBar;
    public float CurrentHealth;
    public float MaxHealth = 100f;
    public bool BossOneEye;

    public BossEye BossObj;
    // Start is called before the first frame update
    void Start()
    {
        HealthBar = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (BossOneEye == true){
            
            CurrentHealth = BossObj.GetComponent<BossEye>().HealthBossOneEye;
            HealthBar.fillAmount = CurrentHealth / MaxHealth;
        }

        if (BossOneEye == false){
            CurrentHealth = BossObj.GetComponent<BossEye>().HealthBoss;
            
            HealthBar.fillAmount = CurrentHealth / MaxHealth;
        }
    }
}
