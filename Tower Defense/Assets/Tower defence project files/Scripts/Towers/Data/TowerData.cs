using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Towers Data", menuName = "Towers/Towers Data")]
public class TowerData : ScriptableObject
{
    public static int ID = 1; // can be improved later
    public int towerID;
    public string towerName;
    [TextArea(2, 5)] public string towerDescription;
    public Sprite towerSprite;


    void OnEnable()
    {
        if (towerID == 0)
        {
            ID++;
            towerID = ID;
        }
    }
}
