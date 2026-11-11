using UnityEngine;

namespace LegionKnight
{
    public class GoogleAuthAgent : MonoBehaviour
    {
        public void StartSignInWithGoogle()
        {
            GooglePlayService.Instance.StartSignInWithGoogle();
        }
    }
}
