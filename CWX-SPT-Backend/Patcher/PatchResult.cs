/* License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 * Basuro
 */

using CWX_SPT_Backend.Patcher.Enums;

namespace CWX_SPT_Backend.Patcher;

public class PatchResult
{
    public EPatchResultType Result { get; }
    public byte[] PatchedData { get; }

    public PatchResult(EPatchResultType result, byte[] patchedData)
    {
        Result = result;
        PatchedData = patchedData;
    }
}