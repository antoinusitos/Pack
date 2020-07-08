using UnityEngine;

public class Cell : MonoBehaviour
{
    private int myIndex = -1;
    private int myX = -1;
    private int myZ = -1;
    private int myTeam = -1;
    private bool myIsOccupied = false;
    private Data.CellType myCellType = Data.CellType.NONE;

    private Transform myTransform = null;

    [SerializeField]
    private Color mySpecialGoalColor = Color.white;

    [SerializeField]
    private Color myGoalColor = Color.white;

    [SerializeField]
    private Color mySpawnColor = Color.white;

    [SerializeField]
    private Color myDroneSpawnColor = Color.white;

    private Unit myUnit = null;

    private Renderer myRenderer = null;

    private Color myBaseColor = Color.white;

    public void Init(int anIndex, int aX, int aZ, Data.CellType aCellType)
    {
        myIndex = anIndex;
        myX = aX;
        myZ = aZ;
        myIsOccupied = false;
        myCellType = aCellType;

        myTransform = transform;
        myTransform.position = new Vector3(aX, 0, aZ);

        myRenderer = GetComponent<Renderer>();
    }

    public void SetCellType(Data.CellType aCellType)
    {
        myCellType = aCellType;
    }

    public Cell SetSpecialGoal()
    {
        myCellType = Data.CellType.SPECIALGOAL;
        myBaseColor = mySpecialGoalColor;
        myRenderer.material.SetColor("_Color", mySpecialGoalColor);
        return this;
    }

    public Cell SetGoal()
    {
        myCellType = Data.CellType.GOAL;
        myBaseColor = myGoalColor;
        myRenderer.material.SetColor("_Color", myGoalColor);
        return this;
    }

    public Cell SetSpawner()
    {
        myCellType = Data.CellType.SPAWN;
        myBaseColor = mySpawnColor;
        myRenderer.material.SetColor("_Color", mySpawnColor);
        return this;
    }

    public void SetDroneSpawner()
    {
        myCellType = Data.CellType.DRONESPAWN;
        myBaseColor = myDroneSpawnColor;
        myRenderer.material.SetColor("_Color", myDroneSpawnColor);
    }

    public void SetTeam(int aTeam)
    {
        myTeam = aTeam;
    }

    public Data.CellType GetCellType()
    {
        return myCellType;
    }

    public bool GetIsOccupied()
    {
        return myIsOccupied;
    }

    public void SetIsOCcupied(bool aState, Unit aUnit)
    {
        myIsOccupied = aState;
        myUnit = aUnit;
    }

    public void CellToString()
    {
        Debug.Log("index:" + myIndex + " X:" + myX + " Z:" + myZ + " Team:" + myTeam + " Occupied:" + myIsOccupied);
    }     

    public void SetColor(Color aColor)
    {
        myRenderer.material.color = aColor;
    }

    public void ResetColor()
    {
        myRenderer.material.color = myBaseColor;
    }

    public int GetIndex()
    { 
        return myIndex; 
    }

    public Unit GetUnit()
    {
        return myUnit;
    }

    public int GetX()
    {
        return myX;
    }

    public int GetZ()
    {
        return myZ;
    }
}
