using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
public class FortuneWheelManager : MonoBehaviour
{
    public string tipeShop;
    public TimedReward tr;
    public WheelType wheelType;
    private bool _isStarted;
    public List<float> _sectorsAngles;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    public Button TurnButton;
    public GameObject KurangDuitButton, LuckySpinObj;
    public GameObject Circle; 			// Rotatable Object with rewards

    public int PreviousCoinsAmount;		// For wasted coins animation
    public RewardType priceToPay;
    public int CurrentCoinsAmount, TurnCost;
    public int[] IncreaseCost;

    public List<probabilityChance> allProbability;
    public GameObject sliceObject;
    public int amountSlice;
    public Transform theCircle;

    public List<RewardSpin> UnrandommedList;
    public List<RewardSpin> AllRewardList;
    private float degreePerSlice;

    public GameObject rewardAppears;
    public Image rewardImage;
    public Text rewardTextUI;

    public GameManager gm;

    public int DuplicateDapatCoin;

    //public Text BuyText;
    private void Awake ()
    {
        DuplicateDapatCoin = TurnCost / 2;
        degreePerSlice = 360f / amountSlice;
        //tr = FindObjectOfType<TimedReward>();
        Debug.Log(degreePerSlice);
        int amount = UnrandommedList.Count;
        for(int i = 0; i < amount; i++)
        {
            int random = UnityEngine.Random.Range(0, UnrandommedList.Count);
            AllRewardList.Add(UnrandommedList[random]);
            UnrandommedList.RemoveAt(random);
        }
        for (int i = 0; i < amountSlice; i++)
        {
            float degree = (i) * degreePerSlice;
            _sectorsAngles.Add(degree);
            GameObject slice = Instantiate(sliceObject, theCircle);
            slice.transform.rotation = Quaternion.Euler(0, 0, degree);
            RewardSlices rSlice = slice.GetComponent<RewardSlices>();
            rSlice.theReward = AllRewardList[i];
            rSlice.degree = degree;
            rSlice.RefreshReward();
            allProbability[((int)rSlice.theReward.rewardRarity)].listDegree.Add(degree);
        }
        if (priceToPay == RewardType.coin)
        {
            CurrentCoinsAmount = gm.Coins;
        }
        else if (priceToPay == RewardType.diamond)
        {
            CurrentCoinsAmount = gm.Diamond;
        }
        else if(priceToPay == RewardType.key)
        {
            CurrentCoinsAmount = gm.Key;
        }
    }
    private void OnEnable()
    {

        TurnButton.interactable = true;
    }
    public void TurnWheel ()
    {
        TurnButton.interactable = false;
        // Player has enough money to turn the wheel
        if (priceToPay == RewardType.coin)
        {
            CurrentCoinsAmount = gm.Coins;
            gm.SaveCoin();
                
        }
        else if(priceToPay == RewardType.diamond)
        {
            CurrentCoinsAmount = gm.Diamond;
            gm.SaveCoin();
        }

        if (CurrentCoinsAmount >= TurnCost) {
            TurnButton.interactable = false;
            TurnButton.GetComponent<Image>().color = new Color(0, 100, 87, 100f);
            StartCoroutine(ButtonAktif());
            Debug.Log("Button Non Aktif");

            
            _currentLerpRotationTime = 0f;

            // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
            float randomAngle=0;
                


            List<Vector2> probabilityList = new List<Vector2>();
            int probabilityCounter=0;
            for(int i=0; i<allProbability.Count; i++)
            {
                probabilityList.Add(new Vector2(probabilityCounter + 1, probabilityCounter + allProbability[i].chance));
                probabilityCounter += allProbability[i].chance;
            }

            int random = UnityEngine.Random.Range(1, probabilityCounter+1);

            for (int i = 0; i < allProbability.Count; i++)
            {
                if (random >= probabilityList[i].x && random <= probabilityList[i].y)
                {
                    randomAngle = allProbability[i].listDegree[UnityEngine.Random.Range(0, allProbability[i].listDegree.Count)];
                    break;
                }
            } 
            Debug.Log(random);
            int fullCircles = 5;
            float randomFinalAngle = randomAngle;
            
            // Here we set up how many circles our wheel should rotate before stop
            _finalAngle = -(fullCircles * 360 + randomFinalAngle);
            _isStarted = true;
            
            PreviousCoinsAmount = CurrentCoinsAmount;
            
            // Decrease money for the turn
            CurrentCoinsAmount -= TurnCost;
            if (priceToPay == RewardType.coin)
            {
                gm.Coins-=TurnCost;
                gm.SaveCoin();
            }
            else if (priceToPay == RewardType.diamond)
            {
                gm.Diamond-=TurnCost;
                gm.SaveCoin();
            }
            tr.ResetUI();

        }
        
    }
    private void GiveAwardByAngle ()
    {
        float angleResult = _startAngle / degreePerSlice;
        int result = Mathf.Abs(Convert.ToInt32(angleResult));
        Debug.Log(AllRewardList[result]);
        GetReward(AllRewardList[result]);
        tr.ResetUI();
    }

    /*public void KurangDuitOn() {
        KurangDuitButton.SetActive(true);
    }

    public void KurangDuitOff()
    {
        KurangDuitButton.SetActive(false);
    }*/

    public void BukaLuckySpin(){
        if (gm.Coins < TurnCost && wheelType == WheelType.Buy) {
            KurangDuitButton.SetActive(true);
        }
        else if(gm.Coins >= TurnCost && wheelType == WheelType.Buy){
            LuckySpinObj.SetActive(true);
        }

        
    }

    public void BukaLuckySpinDiamond(){
        if (gm.Diamond < TurnCost && wheelType == WheelType.Buy) {
            KurangDuitButton.SetActive(true);
        }
        else if(gm.Diamond >= TurnCost && wheelType == WheelType.Buy){
            LuckySpinObj.SetActive(true);
        }

        
    }

    void Update ()
    {
        CurrentCoinsAmount = gm.Coins;

        if (!_isStarted)
    	    return;

    	float maxLerpRotationTime = 4f;
    
    	// increment timer once per frame
    	_currentLerpRotationTime += Time.deltaTime;
    	if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
    	    _currentLerpRotationTime = maxLerpRotationTime;
    	    _isStarted = false;
    	    _startAngle = _finalAngle % 360;
    
    	    GiveAwardByAngle ();
    	    StartCoroutine(HideCoinsDelta ());
    	}
    
    	// Calculate current position using linear interpolation
    	float t = _currentLerpRotationTime / maxLerpRotationTime;
    
    	// This formulae allows to speed up at start and speed down at the end of rotation.
    	// Try to change this values to customize the speed
    	t = t * t * t * (t * (6f * t - 15f) + 10f);
    
    	float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
    	Circle.transform.eulerAngles = new Vector3 (0, 0, angle);

        
    }

    private void RewardCoins (int awardCoins)
    {
        CurrentCoinsAmount += awardCoins;
        StartCoroutine (UpdateCoinsAmount ());
    }

    private IEnumerator HideCoinsDelta ()
    {
        yield return new WaitForSeconds (1f);

    }

    private IEnumerator UpdateCoinsAmount ()
    {
    	// Animation for increasing and decreasing of coins amount
    	const float seconds = 0.5f;
    	float elapsedTime = 0;
    
    	while (elapsedTime < seconds) {
    	    elapsedTime += Time.deltaTime;
    
    	    yield return new WaitForEndOfFrame ();
        }
    
    	PreviousCoinsAmount = CurrentCoinsAmount;
    }

    private void GetReward(RewardSpin reward)
    {
        string rewardNameUI = reward.rewardName;
        if(reward.rewardType == RewardType.coin)
        {
            gm.Coins += reward.amount;
            rewardNameUI = reward.amount + " Coins";
            gm.SaveCoin();
        }
        if(reward.rewardType == RewardType.diamond) 
        {
            gm.Diamond += reward.amount;
            rewardNameUI = reward.amount + " Diamonds";
            gm.SaveCoin();

        }
        if (reward.rewardType == RewardType.character)
        {
            string characterAvailable = CharacterDatabase.instance.GetOwnedCharacterID.Find((x) => x == reward.character.CharacterID);         
            if(characterAvailable == reward.character.CharacterID)
            {
                DuplicateDapatCoin = TurnCost / 2;
                int rewardDuplicate = DuplicateDapatCoin;
                gm.Coins += DuplicateDapatCoin;
                
                rewardNameUI = "Duplicate!\n" + "You got " + rewardDuplicate + " "+priceToPay;
            }  
            else
            {
                CharacterDatabase.instance.GetOwnedCharacterID.Add(reward.character.CharacterID);
                Debug.Log(characterAvailable);
                int r = UnityEngine.Random.Range(0, IncreaseCost.Length);
                if(wheelType != WheelType.Always)
                    TurnCost += IncreaseCost[r];
            }
        }

        rewardAppears.SetActive(true);
        rewardImage.sprite = reward.rewardImage;
        rewardTextUI.text = rewardNameUI;

        SaveAndLoadShop.Instance.Save();
        CharacterDatabase.instance.IsDataChanged = true;

    }
    [System.Serializable]
    public struct probabilityChance
    {
        public Rarity rarityType;
        public int chance;
        public List<float> listDegree;
    }

    public enum WheelType
    {
        dailyLogin,daysConsecutive,Buy,Always
    }

    public void TambahCoin(){
        gm.Coins += 9;
        gm.CoinText.text = "Coin : " + gm.Coins;
    }

    public void KurangiCoin(){
        gm.Coins -= 4;
        gm.CoinText.text = "Coin : " + gm.Coins;
    }

    public void TambahDiamond(){
        gm.Diamond += 8;
        gm.DiamondText.text = "Diamond : " + gm.Diamond;
    }

    public void KurangiDiamond(){
        gm.Diamond -= 3;
        gm.DiamondText.text = "Diamond : " + gm.Diamond;
    }

    IEnumerator ButtonAktif(){
        yield return new WaitForSeconds(5f);
        TurnButton.interactable = true;
    	TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1);
    }
}