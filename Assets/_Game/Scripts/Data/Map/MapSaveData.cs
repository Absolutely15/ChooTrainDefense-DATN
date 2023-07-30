using System;
using System.Collections.Generic;

[Serializable]
public class MapSaveData
{
    public List<RoomSaveData> roomSaveDatas;
    public List<GuardSaveData> guardSaveDatas;
    public List<TrapSaveData> trapSaveDatas;
    public List<ChestSaveData> chestSaveDatas;
    public List<int> blockerGoldReceived;
}

[Serializable]
public class RoomSaveData
{
    public bool isUnlocked;
}

[Serializable]
public class TrapSaveData
{
    public bool isUnlocked;
}

[Serializable]
public class GuardSaveData
{
    public int level; //0:not unlock
}

[Serializable]
public class ChestSaveData
{
    public bool isOpened;
}
