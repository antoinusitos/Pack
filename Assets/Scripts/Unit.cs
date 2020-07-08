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

        myCurrentCell.SetIsOCcupied(false, null);
        myTargetCell = aCell;

        int distX = myTargetCell.GetX() - myCurrentCell.GetX();
        int distZ = myTargetCell.GetZ() - myCurrentCell.GetZ();

        int dist = Mathf.Abs(distX) + Mathf.Abs(distZ);

        myLineRenderer.SetPosition(0, transform.position + Vector3.up * 0.5f);
        myLineRenderer.SetPosition(1, myTargetCell.transform.position + Vector3.up * 0.5f);

        myMovementCells = new Cell[dist];
        bool goLeft = myCurrentCell.GetX() > myTargetCell.GetX();
        for(int i = 0; i < myMovementCells.Length; i++)
        {
            if(goLeft)
            {
                myMovementCells[i] = MatchManager.GetInstance().GetCell(myCurrentCell.GetX() - 1 * (i + 1), myCurrentCell.GetZ());
            }
            else
            {
                myMovementCells[i] = MatchManager.GetInstance().GetCell(myCurrentCell.GetX() + 1 * (i + 1), myCurrentCell.GetZ());
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
            //StartCoroutine(MoveTo(myMovementCells[myMovementCellsIndex].transform.position, transform.position));
            
            //return true;
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
            myStep = 0;
            SetCurrentCell(myMovementCells[myMovementCellsIndex]);
            //myMovementCellsIndex++;
            myStartPos = aTarget;
            return true;
        }
        return false;
    }

    /*private IEnumerator MoveTo(Vector3 aTarget, Vector3 aStartPos)
    {
        float timer = 0;
        while(timer < 1)
        {
            transform.position = Vector3.Lerp(aStartPos, aTarget, timer);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = aTarget;
    }*/
}
