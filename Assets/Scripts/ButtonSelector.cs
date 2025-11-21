using UnityEngine;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    public Button button;
    
    private void OnEnable()
    {
        button.Select();
    }
}
