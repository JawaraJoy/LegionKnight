using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    void Start()
    {
        string version = Application.version;
        versionText.text = "Version : " + version;
    }
}
