using System;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private Unit myOwnerUnit = null;

    private Cell myCell = null;

    private Transform myTransform = null;

    private Cell myDirectionCell = null;

    private float myStep = 0;

    private Cell[] myMovementCells = null;
    private int myMovementCellsIndex = 0;

    private Vector3 myStartPos = Vector3.zero;

    private const float myDroneSpeed = 2.0f;

    private void Start()
    {
        myTransform = transform;
    }

    public void SetCell(Cell aCell)
    {
        myCell = aCell;
    }

    public void SetUnit(Unit aUnit)
    {
        myOwnerUnit = aUnit;
    }

    private void Update()
    {
        if(myOwnerUnit != null)
        {
            myTransform.position = myOwnerUnit.transform.position + Vector3.up * 1.5f;
        }
    }

    public void MoveDroneToCell(Cell aCell)
    {
        myStartPos = transform.position;

        myDirectionCell = aCell;

        if (myOwnerUnit != null)
            myCell = myOwnerUnit.GetCell();

        int distX = myDirectionCell.GetX() - myCell.GetX();
        int distZ = myDirectionCell.GetZ() - myCell.GetZ();

        int dist = Mathf.Abs(distX) + Mathf.Abs(distZ);

        myMovementCells = new Cell[dist / 2];
        for (int i = 1; i <= myMovementCells.Length; i++)
        {
            //left
            if (myCell.GetX() > myDirectionCell.GetX())
            {
                //backward
                if (myCell.GetZ() > myDirectionCell.GetZ())
                {
                    myMovementCells[i - 1] = MatchManager.GetInstance().GetCell(myCell.GetX() - i, myCell.GetZ() - i);
                }
                //forward
                else if (myCell.GetZ() < myDirectionCell.GetZ())
                {
                    myMovementCells[i - 1] = MatchManager.GetInstance().GetCell(myCell.GetX() - i, myCell.GetZ() + i);
                }
            }
            //right
            else if (myCell.GetX() < myDirectionCell.GetX())
            {
                //backward
                if (myCell.GetZ() > myDirectionCell.GetZ())
                {
                    myMovementCells[i - 1] = MatchManager.GetInstance().GetCell(myCell.GetX() + i, myCell.GetZ() - i);
                }
                //forward
                else if (myCell.GetZ() < myDirectionCell.GetZ())
                {
                    myMovementCells[i - 1] = MatchManager.GetInstance().GetCell(myCell.GetX() + i, myCell.GetZ() + i);
                }
            }
        }
    }

    public bool Resolve()
    {
        if (myStep >= 1 || myMovementCells == null)
        {
            return true;
        }

        if (myMovementCellsIndex < myMovementCells.Length)
        {
            myOwnerUnit = null;
            myStep += Time.deltaTime * Data.myResolutionSpeed * myDroneSpeed;
            return MoveToStep(myMovementCells[myMovementCellsIndex].transform.position, myStartPos);
        }

        return true;
    }

    private bool MoveToStep(Vector3 aTarget, Vector3 aStartPos)
    {
        transform.position = Vector3.Lerp(aStartPos, aTarget, myStep);
        if (myStep >= 1)
        {
            myStep = 0;
            SetCell(myMovementCells[myMovementCellsIndex]);
            myStartPos = aTarget;
            if(myMovementCells[myMovementCellsIndex].GetIsOccupied())
            {
                myOwnerUnit = myMovementCells[myMovementCellsIndex].GetUnit();
                myOwnerUnit.SetDrone(this);
                myMovementCells = null;
            }
            return true;
        }
        return false;
    }

    public void NextStep()
    {
        myMovementCellsIndex++;
        myStep = 0;
        if (myMovementCells != null && myMovementCellsIndex >= myMovementCells.Length)
        {
            myMovementCells = null;
        }
    }

    public void ResetBeforeTurn()
    {
        myMovementCells = null;
        myMovementCellsIndex = 0;
        myStep = 0;
    }
}
