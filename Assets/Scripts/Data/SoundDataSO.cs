using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataSO", menuName = "Create SoundDataSO")]
public class SoundDataSO : ScriptableObject {

    public List<BgmData> bgmDataList = new List<BgmData>();
    public List<SeData> seDataList = new List<SeData>();
}