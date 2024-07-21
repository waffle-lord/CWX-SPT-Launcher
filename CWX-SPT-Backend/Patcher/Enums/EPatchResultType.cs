/* License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 * Basuro
 */

namespace CWX_SPT_Backend.Patcher.Enums;

public enum EPatchResultType
{
    Success,
    InputLengthMismatch,
    InputChecksumMismatch,
    AlreadyPatched,
    OutputChecksumMismatch
}