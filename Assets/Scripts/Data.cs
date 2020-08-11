public static class Data
{
    public enum CellType
    {
        NONE,
        GOAL,
        SPECIALGOAL,
        SPAWN,
        DRONESPAWN,
    }

    public enum MatchState
    {
        PREPARATION,
        PLANIFICATION,
        RESOLUTION,
        ENDING
    }

    public enum NextActionType
    {
        NONE,
        MOVEMENT,
        PASS
    }

    public static int myPackMaxNumber = 8;
    public static int myPackStartNumber = 5;

    public static int myboardXSize = 9;
    public static int myboardZSize = 9;
    public static int myCellSize = 1;

    public static float myResolutionSpeed = 1.0f;

    public static float myPlanificationTimeForUnit = 15.0f;
}

//Board = All the cells for the match