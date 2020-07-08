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

        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, 5000))
            {
                Cell cell = hit.transform.GetComponent<Cell>();
                if(cell != null)
                {
                    if(!cell.GetIsOccupied() && myMovementCells.Contains(cell))
                    {
                        myActiveUnit.MoveUnitToCell(cell);
                        EndTurn();
                    }
                }
            }
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
        for (int i = 0; i < myMovementCells.Count; i++)
        {
            myMovementCells[i].ResetColor();
        }
        myMatchManager.EndOfPlanification(myTeam);
    }

    private void GetMovementCells(Cell aCell, int aMovementSize)
    {
        myMovementCells = myMatchManager.GetMovementCells(aCell, aMovementSize);
    }
}
