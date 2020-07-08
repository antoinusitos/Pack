using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    private int myTeam = -1;

    [SerializeField]
    private Color myTeam1Color = Color.white;
    
    [SerializeField]
    private Color myTeam2Color = Color.white;

    private int myIndex = -1;

    [SerializeField]
    private GameObject mySelectionCylinder = null;

    private Cell myCurrentCell = null;

    [SerializeField]
    private int myInitiative = 2;

    [SerializeField]
    private int myAgility = 3;

    [SerializeField]
    private int myStrength = 1;

    [SerializeField]
    private int myDefense = 0;

    [SerializeField]
    private int myEndurance = 3;

    [SerializeField]
    private int myTech = 0;

    private Cell myTargetCell = null;

    private Cell[] myMovementCells = null;
    private int myMovementCellsIndex = 0;

    [SerializeField]
    private LineRenderer myLineRenderer = null;

    [SerializeField]
    private MeshRenderer myBodyRenderer = null;

    private float myStep = 0;

    private Vector3 myStartPos = Vector3.zero;

    private bool myIsInMovement = false;

    public int GetTeam()
    {
        return myTeam;
    }

    public void SetTeam(int aTeam)
    {
        myTeam = aTeam;
        if (aTeam == 1)
            myBodyRenderer.material.color = myTeam1Color * new Color(Random.Range(0.5f, 5f), 1, 1);
        else
            myBodyRenderer.material.color = myTeam2Color * new Color(1, Random.Range(0.5f, 5f), 1);
    }

    public void SetIndex(int anIndex)
    {
        myIndex = anIndex;
    }

    public int GetIndex()
    {
        return myIndex;
    }

    public void SetActive(bool aState)
    {
        mySelectionCylinder.SetActive(aState);
    }

    public void SetCurrentCell(Cell aCell)
    {
        myCurrentCell = aCell;
    }

    public Cell GetCell()
    {
        return myCurrentCell;
    }

    public int GetAgility()
    {
        return myAgility;
    }

    public void MoveUnitToCell(Cell aCell)
    {
        myStartPos = transform.position;

        //myCurrentCell.SetIsOCcupied(false, null);
        myTargetCell = aCell;

        int distX = myTargetCell.GetX() - myCurrentCell.GetX();
        int distZ = myTargetCell.GetZ() - myCurrentCell.GetZ();

        int dist = Mathf.Abs(distX) + Mathf.Abs(distZ);

        myLineRenderer.SetPosition(0, transform.position + Vector3.up * 0.5f);
        myLineRenderer.SetPosition(1, myTargetCell.transform.position + Vector3.up * 0.5f);

        myMovementCells = new Cell[dist];
        for(int i = 0; i < myMovementCells.Length; i++)
        {
            //left
            if(myCurrentCell.GetX() > myTargetCell.GetX())
            {
                myMovementCells[i] = MatchManager.GetInstance().GetCell(myCurrentCell.GetX() - 1 * (i + 1), myCurrentCell.GetZ());
            }
            //right
            else if(myCurrentCell.GetX() < myTargetCell.GetX())
            {
                myMovementCells[i] = MatchManager.GetInstance().GetCell(myCurrentCell.GetX() + 1 * (i + 1), myCurrentCell.GetZ());
            }
            else
            {
                //backward
                if (myCurrentCell.GetZ() > myTargetCell.GetZ())
                {
                    myMovementCells[i] = MatchManager.GetInstance().GetCell(myCurrentCell.GetX(), myCurrentCell.GetZ() - 1 * (i + 1));
                }
                //forward
                else if (myCurrentCell.GetZ() < myTargetCell.GetZ())
                {
                    myMovementCells[i] = MatchManager.GetInstance().GetCell(myCurrentCell.GetX(), myCurrentCell.GetZ() + 1 * (i + 1));
                }
            }
        }
    }

    public bool GetMovementFinished()
    {
        return !myIsInMovement;
    }

    public bool Resolve()
    {
        if (myStep >= 1 || myMovementCells == null)
        {
            return true;
        }

        if (myMovementCellsIndex < myMovementCells.Length)
        {
            myIsInMovement = true;
            myStep += Time.deltaTime * Data.myResolutionSpeed;
            return MoveToStep(myMovementCells[myMovementCellsIndex].transform.position, myStartPos);
        }

        return true;
    }

    public void NextStep()
    {
        myMovementCellsIndex++;
        myStep = 0;
        if (myMovementCells != null && myMovementCellsIndex >= myMovementCells.Length)
        {
            myIsInMovement = false;
            myMovementCells = null;
        }
    }

    private bool MoveToStep(Vector3 aTarget, Vector3 aStartPos)
    {
        transform.position = Vector3.Lerp(aStartPos, aTarget, myStep);
        if(myStep >= 1)
        {
            /*if(myMovementCells[myMovementCellsIndex].GetIsOccupied())
            {
                Unit meetingUnit = myMovementCells[myMovementCellsIndex].GetUnit();
                if(meetingUnit != null)
                {
                    if(meetingUnit.GetMovementFinished())
                    {
                        //not moving
                        Debug.Log("Je lui défonce sa gueule");
                    }
                    else
                    {
                        //not moving
                        Debug.Log("y a bagarre ?!");
                    }
                }
            }*/

            myStep = 0;
            SetCurrentCell(myMovementCells[myMovementCellsIndex]);
            myStartPos = aTarget;
            return true;
        }
        return false;
    }

    public void ResetBeforeTurn()
    {
        myIsInMovement = false;
        myMovementCells = null;
        myMovementCellsIndex = 0;
        myStep = 0;
    }

    public Cell GetCurrentCell()
    {
        return myCurrentCell;
    }

    public Cell GetNextCell()
    {
        if (myMovementCells == null)
            return null;

        return myMovementCells[myMovementCellsIndex];
    }
}
