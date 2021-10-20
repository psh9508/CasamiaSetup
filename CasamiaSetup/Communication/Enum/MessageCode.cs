using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication.Enum
{
    /// <summary>
    /// 서버와 클라이언트 간의 에러코드를 정의한다.
    /// </summary>
    public enum MessageCode : int
    {
        #region Basic codes (-9 ~ 999)
        UNKNOWN = -9,
        SUCCESS = 0,
        #endregion

        #region Normal inform or message (1_000 ~ 8_999)
        CHANGE_PASSWORD = 1_000,
        PREVIOUSE_DAY_NO_EXACT_STATE = 1_001,
        TODAY_EXACT_STATE = 1_002,
        #endregion

        #region Statemachine errors (9_000 ~ 9_999)
        // TODO: 스테이트에대한 자세한로그 필요
        // TODO: 스테이트 보정을 위해 PosTransaction 응답에 온 State로 보정한다.
        POS_STATE_ERROR = 9_000,
        #endregion

        #region Server logic errors (10_000 ~ 59_999)
        INTERNAL_ERROR = 10_000,

        INVALID_ARGUMENT = 20_000,
        SALEDATE_ERROR = 21_000,

        IRT_ERROR = 30_000,

        TRAN_ERROR = 40_000,
        TRAN_EXIST = 40_001,
        TRAN_NOT_EXIST = 40_002,

        N0_DATA_FOUND = 50_000,
        EMPTY_DATA_ERROR = 51_000,
        EMPTY_SIGN_DATA_ERROR = 51_001,
        EMPTY_INSMON_DATA_ERROR = 51_002,
        DB_ERROR = 52_000,
        #endregion

        #region External communication errors (60_000 ~ 69_999)
        EXT_CONNECTION_ERROR = 60_000,
        RETRY_ERROR = 60_001,
        OPTION_ERROR = 60_002,
        SPOINT_SAVE_ERROR = 60_003,
        COMPANY_POINT_SAVE_ERROR = 60_004,
        #endregion

        #region Internal communication errors (70_000 ~ 79_999)
        IF_CONNECTION_ERROR = 70_000,
        #endregion

        #region Client internal errors (80_000 ~ 89_999)
        CLIENT_JSONERROR = 80_000,
        CLIENT_CONNECTION_ERROR = 81_000,
        CLIENT_UNKNOWN_ERROR = 89_999,
        #endregion

        #region Server abnormal errors (90_000 ~ 99_999)
        SERVER_ERROR = 90_000,
        SERVER_UNKNOWN_ERROR = 99_999,
        #endregion

        #region Security errors (100_000 ~ )
        SECURITY_ERROR = 100_000,
        #endregion
    }
}
