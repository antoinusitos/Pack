using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    private static MatchManager myInstance = null;

    private Cell[] myBoard = null;

    private List<Cell> myBottomSpawners = null;
    private List<Cell> myTopSpawners = null;

    [SerializeField]
    private Cell myCellPrefab = null;

    [SerializeField]
    private Transform myWallPrefab = null;

    private Transform myBoardHandle = null;
    private Transform myWallsHandle = null;

    [SerializeField]
    private Unit myUnitPrefab = null;

    [SerializeField]
    private Text myMatchStateText = null;

    private Data.MatchState myMatchState = Data.MatchState.PREPARATION;

    [SerializeField]
    private PlayerMatch myPlayerMatch = null;

    [SerializeField]
    private PlayerMatch myPlayer2Match = null;

    private List<Unit> myPlayer1Units = null;
    private List<Unit> myPlayer2Units = null;

    private int myPlayer1ActiveUnitIndex = 0;

    [SerializeField]
    private Transform myInitiativeObject = null;
    private GameObject myInitiativeCurrentUnit = null;

    private Unit[] myInitiativeOrder = null;

    [SerializeField]
    private Image myTimerImage = null;

    private void Awake()
    {
        myInstance = this;
    }

    public static MatchManager GetInstance()
    {
        return myInstance;
    }

    private void Start()
    {
        CreateBoard();
        LaunchGame();
    }

    private void CreateBoard()
    {
        myBoardHandle = new GameObject("BoardHandle").transform;
        myWallsHandle = new GameObject("WallsHandle").transform;

        int boardX = Data.myboardXSize;
        int boardZ = Data.myboardZSize;

        myBoard = new Cell[boardX * boardZ];

        int indexX = 0;
        int indexZ = 0;

        for (int i = 0; i < myBoard.Length; i++)
        {
            myBoard[i] = Instantiate(myCellPrefab, myBoardHandle);

            myBoard[i].Init(i, indexX, indexZ, Data.CellType.NONE);

            indexX++;
            if (indexX % boardX == 0)
            {
                indexZ++;
                indexX = 0;
            }
        }

        PlaceWalls();

        PlaceSpecialCells();

        PlaceSpawners();

        PlaceUnits();
    }

    private void PlaceWalls()
    {
        int boardX = Data.myboardXSize;
        int boardZ = Data.myboardZSize;

        for (int i = 0; i < boardZ; i++)
        {
            Instantiate(myWallPrefab, new Vector3(-1, 0, i), Quaternion.identity).parent = myWallsHandle;
            Instantiate(myWallPrefab, new Vector3(boardZ, 0, i), Quaternion.identity).parent = myWallsHandle;
        }

        for (int i = -1; i <= boardX; i++)
        {
            Instantiate(myWallPrefab, new Vector3(i, 0, -1), Quaternion.identity).parent = myWallsHandle;
            Instantiate(myWallPrefab, new Vector3(i, 0, boardX), Quaternion.identity).parent = myWallsHandle;
        }
    }

    private void PlaceSpecialCells()
    {
        int bottomIndex = (Data.myboardXSize - 1 ) / 2;

        //Bottom
        myBoard[bottomIndex].SetSpecialGoal().SetTeam(1);
        myBoard[bottomIndex - 1].SetGoal().SetTeam(1);
        myBoard[bottomIndex + 1].SetGoal().SetTeam(1);
        myBoard[bottomIndex + Data.myboardXSize].SetGoal().SetTeam(1);
        myBoard[bottomIndex - 1 + Data.myboardXSize].SetGoal().SetTeam(1);
        myBoard[bottomIndex + 1 + Data.myboardXSize].SetGoal().SetTeam(1);

        int topIndex = (Data.myboardZSize - 1) * Data.myboardXSize + (Data.myboardXSize) / 2;

        //top
        myBoard[topIndex].SetSpecialGoal().SetTeam(2);
        myBoard[topIndex - 1].SetGoal().SetTeam(2);
        myBoard[topIndex + 1].SetGoal().SetTeam(2);
        myBoard[topIndex - Data.myboardXSize].SetGoal().SetTeam(2);
        myBoard[topIndex - 1 - Data.myboardXSize].SetGoal().SetTeam(2);
        myBoard[topIndex + 1 - Data.myboardXSize].SetGoal().SetTeam(2);
    }

    private void PlaceSpawners()
    {
        myBottomSpawners = new List<Cell>();
        //bottom
        for (int i = 0; i < (Data.myboardZSize - 1) / 2 ; i++)
        {
            for (int j = 0; j < Data.myboardXSize; j++)
            {
                if (myBoard[j + i * Data.myboardZSize].GetCellType() == Data.CellType.NONE)
                {
                    myBoard[j + i * Data.myboardZSize].SetSpawner().SetTeam(1);
                    myBottomSpawners.Add(myBoard[j + i * Data.myboardZSize]);
                }
            }
        }

        myTopSpawners = new List<Cell>();
        //top
        for (int i = Data.myboardZSize - 1; i > (Data.myboardZSize - 1) / 2; i--)
        {
            for (int j = 0; j < Data.myboardXSize; j++)
            {
                if (myBoard[j + i * Data.myboardZSize].GetCellType() == Data.CellType.NONE)
                {
                    myBoard[j + i * Data.myboardZSize].SetSpawner().SetTeam(2);
                    myTopSpawners.Add(myBoard[j + i * Data.myboardZSize]);
                }
            }
        }

        myBoard[(Data.myboardZSize - 1) / 2 * Data.myboardXSize + (Data.myboardXSize - 1) / 2].SetDroneSpawner();
    }

    private void PlaceUnits()
    {
        myPlayer1Units = new List<Unit>();

        //bottom spawn
        for (int i = 0; i < Data.myPackStartNumber; i++)
        {
            Cell cell = null;
            while(cell == null || cell.GetIsOccupied())
            {
                cell = myBottomSpawners[Random.Range(0, myBottomSpawners.Count)];
            }
            Unit unit = Instantiate(myUnitPrefab, cell.transform.position, Quaternion.identity);
            cell.SetIsOCcupied(true, unit);
            unit.SetCurrentCell(cell);
            unit.SetTeam(1);
            unit.SetIndex(i);
            myPlayer1Units.Add(unit);
        }

        myPlayer2Units = new List<Unit>();

        //top spawn
        for (int i = 0; i < Data.myPackStartNumber; i++)
        {
            Cell cell = null;
            while (cell == null || cell.GetIsOccupied())
            {
                cell = myTopSpawners[Random.Range(0, myTopSpawners.Count)];
            }
            Unit unit = Instantiate(myUnitPrefab, cell.transform.position, Quaternion.identity);
            cell.SetIsOCcupied(true, unit);
            unit.SetCurrentCell(cell);
            unit.SetTeam(2);
            unit.SetIndex(i);
            myPlayer2Units.Add(unit);
        }
    }

    private void LaunchGame()
    {
        StartCoroutine(GameState());
    }

    private IEnumerator GameState()
    {
        myMatchState = Data.MatchState.PREPARATION;
        myMatchStateText.text = "PREPARATION";

        myInitiativeOrder = new Unit[myPlayer1Units.Count + myPlayer2Units.Count];
        for(int i = 0; i < myPlayer1Units.Count; i++)
        {
            myInitiativeOrder[i] = myPlayer1Units[i];
        }
        for (int i = 0; i < myPlayer2Units.Count; i++)
        {
            myInitiativeOrder[myPlayer1Units.Count + i] = myPlayer2Units[i];
        }

        for(int i = 0; i < myInitiativeOrder.Length; i++)
        {
            Unit temp = myInitiativeOrder[i];
            int rand = Random.Range(0, myInitiativeOrder.Length);
            myInitiativeOrder[i] = myInitiativeOrder[rand];
            myInitiativeOrder[rand] = temp;
        }

        for (int i = 0; i < myInitiativeOrder.Length; i++)
        {
            myInitiativeObject.GetChild(i).GetComponent<RawImage>().color = myInitiativeOrder[i].GetTeam() == 1 ? Color.red : Color.blue;
            myInitiativeObject.GetChild(i).GetComponentInChildren<Text>().text = "Unit-" + myInitiativeOrder[i].GetIndex();
        }

        yield return new WaitForSeconds(1);

        //turn loop
        while (myMatchState != Data.MatchState.ENDING)
        {
            myMatchState = Data.MatchState.PLANIFICATION;
            myMatchStateText.text = "PLANIFICATION PLAYER 1";

            //player 1 turn
            myPlayerMatch.SetCanPlay(true);
            myPlayerMatch.SetActiveUnit(FindNextUnit(1, 0));
            while (myMatchState == Data.MatchState.PLANIFICATION)
            {
                yield return null;
            }
            myPlayerMatch.SetCanPlay(false);

            yield return new WaitForSeconds(1);

            myMatchStateText.text = "PLANIFICATION PLAYER 2";
            myMatchState = Data.MatchState.PLANIFICATION;

            //player 2 turn
            myPlayer2Match.SetCanPlay(true);
            myPlayer2Match.SetActiveUnit(FindNextUnit(2, 0));
            while (myMatchState == Data.MatchState.PLANIFICATION)
            {
                yield return null;
            }
            myPlayer2Match.SetCanPlay(false);

            myMatchStateText.text = "RESOLUTION";
            yield return new WaitForSeconds(1);

            bool resolve = true;
            Unit[] tempUnit = new Unit[myInitiativeOrder.Length];
            tempUnit[0] = myInitiativeOrder[0];
            int unitAdded = 0;

            /*while(resolve)
            {
                resolve = false;
                for(int i = 0; i < unitAdded + 1; i++)
                {
                    if (tempUnit[i] != null)
                    {
                        bool found = tempUnit[i].Resolve();
                        if (found)
                            resolve = true;
                        else
                            tempUnit[i] = null;
                    }
                }
                yield return new WaitForSeconds(0.1f);
                if(unitAdded < myInitiativeOrder.Length - 1)
                {
                    unitAdded++;
                    tempUnit[unitAdded] = myInitiativeOrder[unitAdded];
                    resolve = true;
                }
            }*/

            bool inTurn = true;
            while(inTurn)
            {
                while (resolve)
                {
                    bool allResolved = true;
                    for (int i = 0; i < unitAdded + 1; i++)
                    {
                        if (!tempUnit[i].Resolve())
                        {
                            allResolved = false;
                        }
                    }
                    if(allResolved)
                    {
                        resolve = false;
                    }
                    yield return null;
                }
                for (int i = 0; i < unitAdded + 1; i++)
                {
                    tempUnit[i].NextStep();
                }

                if (unitAdded < myInitiativeOrder.Length - 1)
                {
                    
                    unitAdded++;
                    tempUnit[unitAdded] = myInitiativeOrder[unitAdded];
                    resolve = true;
                }
                else
                {
                    bool allFinished = true;
                    for (int i = 0; i < unitAdded + 1; i++)
                    {
                        if(!tempUnit[i].GetMovementFinished())
                        {
                            resolve = true;
                            allFinished = false;
                        }
                    }

                    if(allFinished)
                    {
                        resolve = false;
                        inTurn = false;
                    }
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    public void EndOfPlanification(int aTeam)
    {
        Unit unit = FindNextUnit(aTeam, myPlayer1ActiveUnitIndex);
        if (unit == null)
        {
            if (aTeam == 1)
                myPlayerMatch.SetActiveUnit(null);
            else
                myPlayer2Match.SetActiveUnit(null);
            myPlayer1ActiveUnitIndex = 0;
            myMatchState = Data.MatchState.RESOLUTION;
        }
        else
        {
            if(aTeam == 1)
                myPlayerMatch.SetActiveUnit(unit);
            else
                myPlayer2Match.SetActiveUnit(unit);
        }
    }

    private Unit FindNextUnit(int aTeam, int aStartIndex)
    {
        if(myInitiativeCurrentUnit != null)
        {
            myInitiativeCurrentUnit.SetActive(false);
        }

        for (int i = aStartIndex; i < myInitiativeOrder.Length; i++)
        {
            if (myInitiativeOrder[i].GetTeam() == aTeam)
            {
                myPlayer1ActiveUnitIndex = i + 1;
                myInitiativeCurrentUnit = myInitiativeObject.GetChild(i).GetChild(1).gameObject;
                myInitiativeCurrentUnit.SetActive(true);
                return myInitiativeOrder[i];
            }
        }
        myInitiativeCurrentUnit = null;
        return null;
    }

    public List<Cell> GetMovementCells(Cell aCell, int aMovementSize)
    {
        List<Cell> cells = new List<Cell>();
        cells.Add(aCell);

        int index = aCell.GetIndex();

        for(int i = 0; i <= aMovementSize; i++)
        {
            if(index + i < myBoard.Length)
            {
                Cell cell = myBoard[index + i];
                if(cell != null && cell.GetZ() == aCell.GetZ())
                    cells.Add(cell);
            }
            if (index - i >= 0)
            {
                Cell cell = myBoard[index - i];
                if (cell != null && cell.GetZ() == aCell.GetZ())
                    cells.Add(cell);
            }
        }

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetColor(Color.magenta);
        }

        return cells;
    }

    public Cell GetCell(int aX, int aZ)
    {
        return myBoard[aZ * Data.myboardXSize + aX];
    }

    public void SetTimerTime(float aRatio)
    {
        myTimerImage.fillAmount = aRatio;
    }
}
