using Chessboard.Logic;
using Newtonsoft.Json;

namespace Chessboard.UI
{
    public static class CallHelpers
    {
        public static string ConvertPieceVMtoJSON(PieceViewModel piecevm)
        {
            return JsonConvert.SerializeObject(piecevm);
        }
    }
}
