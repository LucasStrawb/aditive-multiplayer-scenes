using Fusion;
using System.Collections.Generic;

public struct SetInputData : INetworkInput
{
    public int FocusSetId;
    public int CreateSetId;
    public int DeleteSetId;

    public SetInputData(int focusSet, int createSet, int destroySet)
    {
        FocusSetId = focusSet;
        CreateSetId = createSet;
        DeleteSetId = destroySet;
    }
}
