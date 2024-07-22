namespace CWX_SPT_Backend.Models.SPT;

public class ServerProfileClass
{
    public string username { get; set; }
    public string nickname { get; set; }
    public string side { get; set; }
    public int currlvl { get; set; }
    public long currexp { get; set; }
    public long prevexp { get; set; }
    public long nextlvl { get; set; }
    public int maxlvl { get; set; }
    public string profileId { get; set; }
    public string edition { get; set; }
    public SPTDataClass sptData { get; set; }
}