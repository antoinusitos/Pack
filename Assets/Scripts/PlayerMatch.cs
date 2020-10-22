using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMatch : MonoBehaviour
{
    [SerializeField]
    private MatchManager myMatchManager = null;

    [SerializeField]
    private GameObject myDecisionPanel = null;

    private bool myCanPlay = false;

    [SerializeField]
    private Camera myCamera = null;

    private Unit myActiveUnit = null;

    [SerializeField]
    private Text myUnitIndexText = null;

    [SerializeField]
    private int myTeam = 1;

    private List<Cell> myMovementCells = null;

    private float myPlanificationTime = 0;

    private Data.NextActionType myACtionType = Data.NextActionType.MOVEMENT;

    [SerializeField]
    private GameObject myPassButton = null;

    public void SetCanPlay(bool aNewState)
    {
        myCanPlay = aNewState;
    }

    private void Update()
    {
        if (!myCanPlay)
            return;

        if (!myActiveUnit)
            return;

        myPlanificationTime += Time.deltaTime;
        MatchManager.GetInstance().SetTimerTime(1 - myPlanificationTime / Data.myPlanificationTimeForUnit);
        if(myPlanificationTime >= Data.myPlanificationTimeForUnit)
        {
            EndTurn();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, 5000))
            {
                Cell cell = hit.transform.GetComponent<Cell>();
                if(cell != null)
                {
                    switch(myACtionType)
                    {
                        case Data.NextActionType.MOVEMENT:
                        {
                            if (myMovementCells.Contains(cell))
                            {
                                myActiveUnit.MoveUnitToCell(cell);
                                EndTurn();
                            }
                            break;
                        }
                        case Data.NextActionType.PASS:
                        {
                            if (myMovementCells.Contains(cell) && myActiveUnit.GetDrone() != null)
                            {
                                myActiveUnit.Pass(cell);
                                EndTurn();
                                myACtionType = Data.NextActionType.MOVEMENT;
                            }
                            break;
                        }
                    }

                    
                }
            }
        }
    }

    public void SetActionType(int anActionType)
    {
        Data.NextActionType action = (Data.NextActionType)anActionType;
        myACtionType = action;
        switch (myACtionType)
        {
            case Data.NextActionType.MOVEMENT:

                break;
            case Data.NextActionType.PASS:
                GetPassCells(myActiveUnit.GetCell(), 3);
                break;
        }
    }

    public void SetActiveUnit(Unit aUnit)
    {
        if(myActiveUnit != null)
            myActiveUnit.SetActive(false);

        myActiveUnit = aUnit;
        if(aUnit != null)
        {
            myDecisionPanel.SetActive(true);
            if(aUnit.GetDrone() != null)
            {
                myPassButton.SetActive(true);
            }
            else
            {
                myPassButton.SetActive(false);
            }
            myUnitIndexText.text = "Unit" + aUnit.GetIndex();
            aUnit.SetActive(true);
            GetMovementCells(aUnit.GetCell(), aUnit.GetAgility());
        }
        else
        {
            myDecisionPanel.SetActive(false);
        }
    }

    public void EndTurn()
    {
        myPlanificationTime = 0;
        for (int i = 0; i < myMovementCells.Count; i++)
        {
            myMovementCells[i].ResetColor();
        }
        myMatchManager.EndOfPlanification(myTeam);
    }

    public void GetMovementCells(Cell aCell, int aMovementSize)
    {
        if (myMovementCells != null)
        {
            for (int i = 0; i < myMovementCells.Count; i++)
            {
                myMovementCells[i].ResetColor();
            }
        }

        myMovementCells = myMatchManager.GetMovementCells(aCell, aMovementSize);
    }

    public void GetPassCells(Cell aCell, int aPassSize)
    {
        if (myMovementCells != null)
        {
            for (int i = 0; i < myMovementCells.Count; i++)
            {
                myMovementCells[i].ResetColor();
            }
        }

        myMovementCells = myMatchManager.GetPassCells(aCell, aPassSize);
    }
}
