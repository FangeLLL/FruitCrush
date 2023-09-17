using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject congratText;
    public GameObject finishBackground;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameFinished(true);
        }
    }

    //FUNCTION TO CALL WHEN PLAYER FAILES OR SUCCESS LEVEL
    //FAILING: PLAYER RUNS OUT OF MOVES BEFORE COMPLETING LEVEL MISSION
    //SUCCESS: PLAYER FINISHED ALL MISSIONS
    public void GameFinished(bool status)
    {
        string gameFinishTrigger = "GameFinishTrigger";

        if (status)
        {
            congratText.SetActive(true);
            congratText.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }

        finishBackground.SetActive(true);
        finishBackground.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
    }
}
