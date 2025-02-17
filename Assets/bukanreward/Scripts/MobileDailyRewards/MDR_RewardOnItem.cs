using UnityEngine;
using UnityEngine.UI;

namespace AppsDaddyO.Rewards
{
    // this is attached to the instantiated rewards object so we are able to keep track of the rewards and there states
    public class MDR_RewardOnItem : MonoBehaviour
    {

        public Image background;        // the background of the reward
        public GameObject complete;     // the tick if the object has been claimed
        public MDR_RewardsItem reward;  // the reward from the rewardsItem class
        public State state;             // the state of the object, wheather it is available, claimed of unavailable
        public enum State
        { // the  selectable states
            UNAVAILABLE,
            AVAILABLE,
            CLAIMED
        }
        private void Update()
        {

            switch (state)
            {
                case State.UNAVAILABLE: // if the state is unavailable we change the color to black
                    background.color = MDR_RewardsManager.rewardsManager.UNAVAILABLE_Color; // the color is black
                    break;
                case State.AVAILABLE:   // if the state is available we change the color to green
                    background.color = MDR_RewardsManager.rewardsManager.AVAILABLE_Color; // the color is green
                    break;
                case State.CLAIMED:    // if the state is claimed we change the color to gold
                    background.color = MDR_RewardsManager.rewardsManager.CLAIMED_Color; // the color is black
                    complete.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
