using UnityEngine;

namespace LegionKnight
{
    public class OpenWeb : MonoBehaviour
    {
        public void OpenWebsite(string webLink)
        {
            Application.OpenURL(webLink);
        }
    }
}
