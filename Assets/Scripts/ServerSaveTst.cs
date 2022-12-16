using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSaveTst : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(100);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
